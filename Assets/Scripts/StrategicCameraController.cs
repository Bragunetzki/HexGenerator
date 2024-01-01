using TreeEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class StrategicCameraControl : MonoBehaviour
{
    [SerializeField] private float horizontalSensitivity = 1f;
    [SerializeField] private float moveSpeed = 5;
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private Transform movementPivot;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Camera controlledCamera;
    private Transform _followedTransform;
    private bool _isFollowing;
    
    private PlayerInputActions _playerInputActions;
    private Vector2 _mouseInput;
    private float _targetCameraDistance = 15;
    private const float MaxCameraDistance = 40;
    private Vector3 _dragStartPosition;
    private Vector3 _dragCurrentPosition;
    private Vector3 _targetVelocity;

    private void Awake()
    {
        _playerInputActions = new PlayerInputActions();
        _playerInputActions.PlayerCamera.Enable();
        _playerInputActions.Player.Movement.Enable();
        _playerInputActions.PlayerCamera.MousePress.performed += OnMousePressed;

        if (movementPivot == null)
            movementPivot = GetComponent<Transform>();
    }

    // Start is called before the first frame update
    // private void Start()
    // {
    //     Cursor.lockState = CursorLockMode.Locked;
    //     Cursor.visible = false;
    // }

    // Update is called once per frame
    private void Update()
    {
        // handle movement input
        HandleMovementInput();
        HandleMovement();

        // handle rotation input
        HandleRotationInput();
        HandleMouseRotationInput();
        
        // handle zoom input
        HandleZoomInput();
    }

    private void HandleMovement()
    {
        var position = movementPivot.position;
        var targetPosition = position + _targetVelocity;
        if (_isFollowing)
        {
            targetPosition = _followedTransform.position;
        }

        movementPivot.position = Vector3.Lerp(position, targetPosition, Time.deltaTime * 10);
    }

    public void StartFollowing(Transform followTransform)
    {
        _followedTransform = followTransform;
        _isFollowing = true;
    }

    public void StopFollowing()
    {
        _followedTransform = null;
        _isFollowing = false;
    }

    private void HandleMovementInput()
    {
        _targetVelocity = Vector3.zero;
        HandleMousePressInput();
        
        var movementInput = _playerInputActions.Player.Movement.ReadValue<Vector2>();
        var horizDirection =  cameraPivot.forward * movementInput.y + cameraPivot.right * movementInput.x;
        horizDirection = Vector3.ProjectOnPlane(horizDirection, movementPivot.up);
        _targetVelocity += Vector3.ClampMagnitude(horizDirection, 1);
        var zoomMod = _targetCameraDistance / MaxCameraDistance * 1.5f;
        _targetVelocity *= moveSpeed * zoomMod;
    }

    private void HandleRotationInput()
    {
        var horizRotationInput = _playerInputActions.PlayerCamera.HorizRotation.ReadValue<float>();
        RotateCameraHorizontalBy(horizRotationInput * horizontalSensitivity);

    }

    private void HandleZoomInput()
    {
        var zoomInput = _playerInputActions.PlayerCamera.Zoom.ReadValue<float>();
        _targetCameraDistance -= zoomInput * Time.deltaTime * moveSpeed;
        _targetCameraDistance = Mathf.Clamp(_targetCameraDistance, 5, 40);
        var localPosition = cameraTransform.localPosition;
        var targetPosition = new Vector3(localPosition.x, localPosition.y,
            -_targetCameraDistance);
        localPosition = Vector3.Lerp(localPosition, targetPosition, Time.deltaTime * 100);
        cameraTransform.localPosition = localPosition;
    }

    private void HandleMousePressInput()
    {
        if (!_playerInputActions.PlayerCamera.MousePress.IsPressed()) return;
        var plane = new Plane(Vector3.up, Vector3.zero);
        var ray = controlledCamera.ScreenPointToRay(Input.mousePosition);
        if (!plane.Raycast(ray, out var entry)) return;
        _dragCurrentPosition = ray.GetPoint(entry);
       _targetVelocity += _dragStartPosition - _dragCurrentPosition;
    }

    private void HandleMouseRotationInput()
    {
        if (!_playerInputActions.PlayerCamera.MousePressMiddle.IsPressed()) return;
        var horizMouseInput = _playerInputActions.PlayerCamera.MousePointerDelta.ReadValue<Vector2>().x;
        RotateCameraHorizontalBy(- horizMouseInput * horizontalSensitivity * 0.5f);
    }

    private void OnMousePressed(InputAction.CallbackContext context)
    {
        if (Cursor.lockState == CursorLockMode.Locked)
            return;
        var plane = new Plane(Vector3.up, Vector3.zero);
        var ray = controlledCamera.ScreenPointToRay(Input.mousePosition);
        if (plane.Raycast(ray, out var entry))
        {
            _dragStartPosition = ray.GetPoint(entry);
        }
    }

    private void RotateCameraHorizontalBy(float angle)
    {
        var localRotation = cameraPivot.localRotation;
        var pivotEuler = localRotation.eulerAngles;
        pivotEuler.y -= angle;
        
        localRotation = Quaternion.Lerp(localRotation, Quaternion.Euler(pivotEuler), Time.deltaTime * 200);
        cameraPivot.localRotation = localRotation;
    }
}
