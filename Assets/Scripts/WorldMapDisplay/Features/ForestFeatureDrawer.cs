using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WorldGeneration;

namespace WorldMapDisplay.Features
{
    public class ForestFeatureDrawer : IHexFeatureDrawer
    {

        public List<FeatureDrawData> GetDrawFeatureList(WorldHex hex, Vector3 origin, Vector2 hexScale,
            Dictionary<string, GameObject> featurePrefabs)
        {
            var treePrefab = featurePrefabs.TryGetValue("tree", out var p) ? p : null;

            const float treeScaleMult = 0.2f;
            var hexMag = hexScale.magnitude;
            var treeScale = new Vector3(hexScale.x, hexMag, hexScale.y);
            treeScale *= treeScaleMult;
            var treeCount = Random.Range(16, 33);
            var minDistance = Mathf.Max(hexScale.x, hexScale.y) * treeScaleMult;

            var treePositions = GetTreePositions(origin, hexScale, treeCount, minDistance);
            return treePositions.Select(pos => new FeatureDrawData(treePrefab, pos, Quaternion.identity, treeScale)).ToList();
        }
        
        private static IEnumerable<Vector3> GetTreePositions(Vector3 origin, Vector2 hexScale, int treeCount, float minDistance)
        {
            var treePositions = new List<Vector3>();
            
            var minDistanceSquared = minDistance * minDistance;

            for (var i = 0; i < treeCount; i++)
            {
                // Generate a random point within the hexagon
                var randomX = Random.Range(-hexScale.x * 0.46f, hexScale.x * 0.46f);
                var randomZ = Random.Range(-hexScale.y * 0.46f, hexScale.y * 0.46f);
                var treePosition = new Vector3(origin.x + randomX, origin.y, origin.z + randomZ);

                // Check if the tree is too close to existing trees
                var isValidPosition = treePositions.All(existingTreePos => !(Vector3.SqrMagnitude(treePosition - existingTreePos) < minDistanceSquared));

                // If the position is valid, add the tree
                if (!isValidPosition) continue;
                treePositions.Add(treePosition);
                yield return treePosition;
            }
        }
    }
}