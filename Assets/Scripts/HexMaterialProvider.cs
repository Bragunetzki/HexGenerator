using UnityEngine;

public class HexMaterialProvider : MonoBehaviour
{
    [SerializeField] private Material defaultHexMaterial;
    [SerializeField] private Material plainsMaterial;
    [SerializeField] private Material seaMaterial;
    [SerializeField] private Material freshWaterMaterial;
    [SerializeField] private Material desertMaterial;
    [SerializeField] private Material forestMaterial;
    [SerializeField] private Material mountainMaterial;
    
    public Material GetMaterialOfHex(WorldHex hex)
    {
        return hex.TerrainType switch
        {
            WorldTerrainType.Plains => plainsMaterial,
            WorldTerrainType.Sea => seaMaterial,
            WorldTerrainType.Desert => desertMaterial,
            WorldTerrainType.Freshwater => freshWaterMaterial,
            WorldTerrainType.Forest => forestMaterial,
            WorldTerrainType.Mountain => mountainMaterial,
            _ => defaultHexMaterial
        };
    }
}
