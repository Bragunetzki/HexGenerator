using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace WorldGeneration.RollTables
{
    public class RollTable<T>
    {
        public readonly List<RollTableEntry<T>> Entries = new();
        private readonly List<Func<T, bool>> _filters = new();

        public List<RollTableEntry<T>> GetEntries()
        {
            return Entries;
        }

        public void AddEntry(int weight, T value)
        {
            Entries.Add(new RollTableEntry<T>(weight, value));
        }

        public void AddFilter(Func<T, bool> filter)
        {
            _filters.Add(filter);
        }

        public void ClearFilters()
        {
            _filters.Clear();
        }

        public RollTable<T> Copy()
        {
            var copy = new RollTable<T>();
            foreach (var entry in Entries)
            {
                copy.AddEntry(entry.Weight, entry.Value);
            }

            copy._filters.AddRange(_filters);
            return copy;
        }

        public T Roll(Random random)
        {
            var filteredEntries = Entries.Where(entry => _filters.All(filter => filter(entry.Value))).ToList();

            var totalWeight = filteredEntries.Sum(entry => entry.Weight);
            var roll = random.Next(totalWeight + 1);
            foreach (var entry in filteredEntries)
            {
                roll -= entry.Weight;
                if (roll <= 0)
                    return entry.Value;
            }

            throw new InvalidOperationException("Invalid roll table configuration");
        }
    }

    public class RollTableEntry<T>
    {
        public RollTableEntry(int weight, T value)
        {
            Weight = weight;
            Value = value;
        }

        public int Weight;
        public readonly T Value;
    }
}