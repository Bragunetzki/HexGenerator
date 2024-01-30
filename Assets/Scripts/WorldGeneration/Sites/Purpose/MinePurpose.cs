using UnityEngine;

namespace WorldGeneration.Sites.Purpose
{
    [CreateAssetMenu(fileName = "Mine Purpose", menuName = "Custom/Sites/MinePurpose")]
    public class MinePurpose : SitePurpose
    {
        [HideInInspector] public Mineral minedMineral;
    }
}