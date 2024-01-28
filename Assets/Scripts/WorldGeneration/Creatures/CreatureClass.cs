using UnityEngine;
using WorldGeneration.Sites;

namespace WorldGeneration.Creatures
{
    [CreateAssetMenu(fileName = "Class", menuName = "Custom/Creatures/CreatureClass")]
    public class CreatureClass : ScriptableObject
    {
        public Rarity baseRarity;
        public bool canBeGroup;
        public bool canBeSolitary;
        public int minimumTier;

        public SitePurpose[] favoredPurposes;
        public SitePurpose[] disfavoredPurposes;
        public SitePurpose[] excludedPurposes;
    }
}