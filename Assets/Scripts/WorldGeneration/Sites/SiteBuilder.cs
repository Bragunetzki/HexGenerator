using System;
using UnityEngine;
using WorldGeneration.Creatures;
using Random = System.Random;

namespace WorldGeneration.Sites
{
    public class SiteBuilder
    {
        private WorldHex _currentHex;
        private WorldGenSettings _currentSettings;
        private Random _random;

        private Site _site;

        public void BuildSite(WorldHex hex, WorldGenSettings settings)
        {
            _currentHex = hex;
            _currentSettings = settings;
            _random = settings.Random;
            _site = new Site();

            var isNatural = _random.Next(1, 3) == 1;
            if (isNatural)
            {
                //BuildNatural();
            }
            else
            {
                BuildConstructed();
            }
        }

        private void BuildConstructed()
        {
            var tableHolder = _currentSettings.rollTables;
            
            var siteAge = tableHolder.SiteAges.Roll(_random);

            var builderTable = tableHolder.GetBuilderTable(siteAge, _currentHex.TerrainType, _currentHex.Feature, _currentHex.Climate);
            var builderSpecies = builderTable.Roll(_random);
            
            var builderType = builderSpecies.isSolitary ? InhabitantType.PowerfulIndividual : tableHolder.BuilderTypes.Roll(_random);

            var builderClass = tableHolder.GetClassTable(builderType, builderSpecies).Roll(_random);

            var tier = tableHolder.TierTable.Roll(_random);
            var minTier = Mathf.Max(builderSpecies.minimumTier, builderClass.minimumTier);
            tier += minTier;
            if (tier > 20) tier = 20;

            var tierGroup = tier switch
            {
                <= 5 => TierGroup.Low,
                <= 10 => TierGroup.Medium,
                <= 15 => TierGroup.High,
                <= 20 => TierGroup.Epic,
                _ => throw new ArgumentOutOfRangeException()
            };

            var siteSize = tableHolder.GetSizeTable(tierGroup).Roll(_random);

            var sitePurpose = tableHolder.GetSitePurposeTable(builderClass).Roll(_random);

            // printing
            var ageString = siteAge.ToString();
            if (siteAge == WorldAge.Young)
                ageString = "recent";

            var typeString = builderType switch
            {
                InhabitantType.PowerfulIndividual => "",
                InhabitantType.CreatureGroup => "group of ",
                _ => throw new ArgumentOutOfRangeException()
            };
            
            Debug.Log("In the " + ageString + " times, a tier " + tier + " " + typeString + builderSpecies + " " + builderClass + " built a " + siteAge + " " + sitePurpose);
        }
    }
}