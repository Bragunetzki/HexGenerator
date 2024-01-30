using UnityEngine;
using WorldGeneration.Creatures;

namespace WorldGeneration
{
    [CreateAssetMenu(fileName = "Deity", menuName = "Custom/Deity")]
    public class Deity : ScriptableObject
    {
        public Rarity baseRarity = Rarity.Common;
    }
}