using UnityEngine;
using WorldGeneration;

namespace WorldMapDisplay
{
    public class HexMaterialProvider : MonoBehaviour
    {
        [SerializeField] private Material defaultHexMaterial;
        [SerializeField] private Material plainsMaterial;
        [SerializeField] private Material seaMaterial;
        [SerializeField] private Material freshWaterMaterial;
        [SerializeField] private Material sandMaterial;
        [SerializeField] private Material mountainMaterial;
        [SerializeField] private Material tallMountainMaterial;
        [SerializeField] private Material hillsMaterial;
        [SerializeField] private Material snowMaterial;

        public Material GetMaterialOfHex(WorldHex hex)
        {
            return hex.TerrainType switch
            {
                WorldTerrainType.Plains => plainsMaterial,
                WorldTerrainType.Sea => seaMaterial,
                WorldTerrainType.Beach => sandMaterial,
                WorldTerrainType.Freshwater => freshWaterMaterial,
                WorldTerrainType.Mountain => mountainMaterial,
                WorldTerrainType.TallMountains => tallMountainMaterial,
                WorldTerrainType.Hills => hillsMaterial,
                WorldTerrainType.SnowyPeaks => snowMaterial,
                _ => defaultHexMaterial
            };
        }
    }
}
