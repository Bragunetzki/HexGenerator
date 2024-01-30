namespace WorldGeneration.Creatures
{
    public interface IInhabitant
    {
        int Tier { get; }
        CreatureSpecies Species { get; }
        CreatureClass Class { get; }
        InhabitantType Type { get; }
    }
}