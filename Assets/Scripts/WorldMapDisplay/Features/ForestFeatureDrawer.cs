using System.Collections.Generic;
using System.Linq;
using HexMapGeneration;
using UnityEngine;

namespace WorldMapDisplay.Features
{
    public class ForestFeatureDrawer : IHexFeatureDrawer
    {
        public List<FeatureDrawData> GetDrawFeatureList(WorldHex hex, Vector3 origin, Vector2 hexScale,
            Dictionary<string, GameObject> featurePrefabs)
        {
            var treePrefab = featurePrefabs.TryGetValue("tree", out var p) ? p : null;
            var treeHeight = 1f;
            if (treePrefab is not null)
            {
                var renderer = treePrefab.GetComponent<Renderer>();
                treeHeight = renderer.bounds.size.y * hexScale.magnitude * 0.1f;
            }
            var treeScale = new Vector3(hexScale.x * 0.1f, hexScale.magnitude * 0.1f, hexScale.y * 0.1f);
            return GetTreePositions(origin, hexScale, treeHeight)
                .Select(treePos => new FeatureDrawData(treePrefab, treePos, Quaternion.identity, treeScale)).ToList();
        }

        private static IEnumerable<Vector3> GetTreePositions(Vector3 origin, Vector2 hexScale, float treeHeight)
        {
            const int numberOfTrees = 5;
            const float angleIncrement = 360f / numberOfTrees;

            for (var i = 0; i < numberOfTrees; i++)
            {
                var angle = i * angleIncrement;
                var radius = hexScale.x * 0.6f;
                var radius2 = hexScale.y * 0.6f;
                var treePosition =
                    origin + new Vector3(radius * Mathf.Cos(angle), treeHeight / 2, radius2 * Mathf.Sin(angle));
                yield return treePosition;
            }
        }
    }
}