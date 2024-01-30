using UnityEngine;
using WorldGeneration.Creatures;

namespace WorldGeneration.Sites.Purpose
{
    [CreateAssetMenu(fileName = "Site Purpose", menuName = "Custom/Sites/SitePurpose")]
    public class SitePurpose : ScriptableObject
    {
        public Rarity baseRarity = Rarity.Common;
        public bool isNatural;
        public bool canBeAquatic = true;
    }
}