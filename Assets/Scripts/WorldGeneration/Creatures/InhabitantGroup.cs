namespace WorldGeneration.Creatures
{
    public class InhabitantGroup : IInhabitant
    {
        public int Tier { get; }
        public CreatureSpecies Species { get; }
        public CreatureClass Class { get; }
        public InhabitantType Type { get; }
        private Inhabitant[] Members { get; }

        public InhabitantGroup(int tier, CreatureSpecies species, CreatureClass @class, InhabitantType type)
        {
            Tier = tier;
            Species = species;
            Class = @class;
            Type = type;
            Members = new Inhabitant[] { };
        }
    }
}