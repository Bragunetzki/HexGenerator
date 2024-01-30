using System.Linq;
using UnityEngine;
using WorldGeneration.Sites.Purpose;

namespace WorldGeneration.Creatures
{
    [CreateAssetMenu(fileName = "Class", menuName = "Custom/Creatures/CreatureClass")]
    public class CreatureClass : ScriptableObject
    {
        public Rarity baseRarity = Rarity.Common;
        public bool canBeGroup;
        public bool canBeSolitary;
        public int minimumTier = 1;

        public SitePurpose[] favoredPurposes;
        public SitePurpose[] disfavoredPurposes;
        public SitePurpose[] excludedPurposes;
        
        public Deity[] favoredDeities;
        public Deity[] excludedDeities;

        public CreatureTrait[] traits;

        public bool HasTrait(CreatureTrait trait)
        {
            return traits.Contains(trait);
        }
    }
}