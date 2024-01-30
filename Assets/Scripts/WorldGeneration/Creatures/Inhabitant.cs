namespace WorldGeneration.Creatures
{
    public class Inhabitant : IInhabitant
    {
        public Inhabitant(int tier, CreatureSpecies species, CreatureClass @class, InhabitantType type)
        {
            Tier = tier;
            Species = species;
            Class = @class;
            Type = type;
        }

        public int Tier { get; }
        public CreatureSpecies Species { get; }
        public CreatureClass Class { get; }
        public InhabitantType Type { get; }
    }
}