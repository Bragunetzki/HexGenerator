using UnityEngine;

namespace WorldGeneration.Sites.Purpose
{
    [CreateAssetMenu(fileName = "Temple Purpose", menuName = "Custom/Sites/TemplePurpose")]
    public class TemplePurpose : SitePurpose
    {
        [HideInInspector] public Deity worshippedDeity;
    }
}