using System;
using UnityEngine;
using Utility;
using WorldGeneration.Creatures;
using WorldGeneration.RollTables;
using WorldGeneration.Sites.Purpose;
using Random = System.Random;

namespace WorldGeneration.Sites
{
    public class SiteBuilder
    {
        private WorldHex _currentHex;
        private WorldGenSettings _currentSettings;
        private Random _random;
        private Site _site;

        private IInhabitant _builder;

        public void BuildSite(WorldHex hex, WorldGenSettings settings)
        {
            _currentHex = hex;
            _currentSettings = settings;
            _random = settings.Random;
            _site = new Site();

            var isNatural = _random.Next(1, 3) == 1;
            if (isNatural)
            {
                BuildNatural();
            }
            else
            {
                BuildConstructed();
            }

            BuildHistory();
            //PrintSite(_site, hex);
            hex.Sites.Add(_site);
        }

        private void BuildHistory()
        {
            var siteHistoryGenerator = new SiteHistoryGenerator(_currentSettings);
            siteHistoryGenerator.Generate(_site, _currentHex);
        }

        private void BuildNatural()
        {
            var tableHolder = _currentSettings.RollTables;
            var siteAge = tableHolder.SiteAges.Roll(_random);
            var sitePurpose = tableHolder.GetNaturalSitePurposeTable(_currentHex).Roll(_random);
            _site.Age = siteAge;
            _site.Size = tableHolder.GetSizeTable(TierGroup.High).Roll(_random);
            _site.InitialPurpose = sitePurpose;
        }

        private void BuildConstructed()
        {
            var tableHolder = _currentSettings.RollTables;
            var siteAge = tableHolder.SiteAges.Roll(_random);
            var builder = CreatureBuilder.GenerateCreature(_currentHex, siteAge, _currentSettings, ConditionOption.RequireTrue);
            var tierGroup = RollTableManager.TierToTierGroup(builder.Tier);
            var siteSize = tableHolder.GetSizeTable(tierGroup).Roll(_random);
            var sitePurpose = PurposeBuilder.GeneratePurposeForCreature(builder, _currentHex, siteAge, _currentSettings);

            _site.Age = siteAge;
            _site.Size = siteSize;
            _site.InitialPurpose = sitePurpose;
            _site.CurrentPurpose = sitePurpose;
            _site.Inhabitants.Add(builder);
            _builder = builder;
        }

        public static string InhabitantString(IInhabitant inhabitant)
        {
            var result = " a tier " + inhabitant.Tier + " ";
            var typeString = inhabitant.Type switch
            {
                InhabitantType.PowerfulIndividual => "",
                InhabitantType.CreatureGroup => "group of ",
                _ => throw new ArgumentOutOfRangeException()
            };
            var speciesString = inhabitant.Species.name;
            var classString = inhabitant.Type switch
            {
                InhabitantType.PowerfulIndividual => inhabitant.Class.name,
                InhabitantType.CreatureGroup => inhabitant.Class.name + "s",
                _ => throw new ArgumentOutOfRangeException()
            };

            if (inhabitant.Species.hasUniqueClass)
            {
                classString = "";
                if (inhabitant.Type == InhabitantType.CreatureGroup)
                    speciesString += "s";
            }
            else
            {
                speciesString += " ";
            }

            return result + typeString + speciesString + classString;
        }

        public static string PurposeString(SitePurpose purpose)
        {
            var result = purpose.name + ".";

            switch (purpose)
            {
                case TemplePurpose templePurpose:
                    result += " The temple was dedicated to " + templePurpose.worshippedDeity.name;
                    break;
                case MinePurpose minePurpose:
                    result += " The mine was used to mine " + minePurpose.minedMineral;
                    break;
                case PrisonPurpose prisonPurpose:
                    result += " The prison was used to imprison ";
                    result += InhabitantString(prisonPurpose.Prisoner) + ".";
                    break;
            }

            return result;
        }

        private void PrintSite(Site site, WorldHex hex)
        {
            var result = "In the ";
            
            var ageString = site.Age.ToString();
            if (site.Age == WorldAge.Young)
                ageString = "recent";
            result += ageString + " times,";
            result += " somewhere in the " + hex.Climate + " " + hex.TerrainType + ",";

            if (site.InitialPurpose.isNatural)
            {
                result += " there was a " + site.Size + " " + site.InitialPurpose.name + ".";
            }
            else
            {
                result += InhabitantString(_builder) + " built a " + site.Size + " " + PurposeString(site.InitialPurpose);
            }
            Debug.Log(result);

            foreach (var hEvent in site.History)
            {
                Debug.Log(hEvent.Narration());
            }
        }
    }
}