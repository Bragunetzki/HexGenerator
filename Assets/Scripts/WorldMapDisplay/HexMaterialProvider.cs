using HexMapGeneration;
using UnityEngine;

namespace WorldMapDisplay
{
    public class HexMaterialProvider : MonoBehaviour
    {
        [SerializeField] private Material defaultHexMaterial;
        [SerializeField] private Material plainsMaterial;
        [SerializeField] private Material seaMaterial;
        [SerializeField] private Material freshWaterMaterial;
        [SerializeField] private Material sandMaterial;
        [SerializeField] private Material forestMaterial;
        [SerializeField] private Material mountainMaterial;
        [SerializeField] private Material hillsMaterial;
        [SerializeField] private Material snowMaterial;
    
        public Material GetMaterialOfHex(WorldHex hex)
        {
            return hex.TerrainType switch
            {
                WorldTerrainType.Plains => plainsMaterial,
                WorldTerrainType.Sea => seaMaterial,
                WorldTerrainType.Sand => sandMaterial,
                WorldTerrainType.Freshwater => freshWaterMaterial,
                WorldTerrainType.Mountain => mountainMaterial,
                WorldTerrainType.Hills => hillsMaterial,
                WorldTerrainType.Snow => snowMaterial,
                _ => defaultHexMaterial
            };
        }
    }
}
