using UnityEngine;
using WorldGeneration.Creatures;

namespace WorldGeneration.Sites
{
    [CreateAssetMenu(fileName = "Site Purpose", menuName = "Custom/Sites/SitePurpose")]
    public class SitePurpose : ScriptableObject
    {
        public Rarity baseRarity;
    }
}