using System.Collections.Generic;
using UnityEngine;
using WorldGeneration;

namespace WorldMapDisplay.Features
{
    public interface IHexFeatureDrawer
    {
        public List<FeatureDrawData> GetDrawFeatureList(WorldHex hex, Vector3 origin,
            Vector2 hexScale, Dictionary<string, GameObject> featurePrefabs);
    }

    public struct FeatureDrawData
    {
        public FeatureDrawData(GameObject prefab, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            Prefab = prefab;
            Position = position;
            Rotation = rotation;
            Scale = scale;
        }

        public readonly GameObject Prefab;
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Scale;
    }
}