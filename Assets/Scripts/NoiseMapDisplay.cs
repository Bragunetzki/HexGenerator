using UnityEngine;

public class NoiseMapDisplay : MonoBehaviour
{
    [SerializeField] private Renderer textureRenderer;

    public void DrawTexture(Texture2D texture)
    {
        textureRenderer.sharedMaterial.mainTexture = texture;
        textureRenderer.transform.localScale = new Vector3(texture.width / 16f, 1, texture.height / 16f);
    }
}
