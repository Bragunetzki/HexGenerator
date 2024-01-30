using System.Collections.Generic;
using System.Linq;
using Hexes;
using UnityEngine;

namespace World
{
    public class World : MonoBehaviour
    {
        public HexGrid Grid { get; }
        private readonly WorldClock _worldClock;
        private readonly List<IWorldComponent> _components;

        public World(HexGrid grid)
        {
            Grid = grid;
            _worldClock = new WorldClock();
            _worldClock.HourElapsed += OnHourElapsed;
            _worldClock.DayElapsed += OnDayElapsed;
            _components = new List<IWorldComponent>();
        }

        private void OnHourElapsed()
        {
            foreach (var component in _components.Where(component =>
                         component.GetSimulationScale() == SimulationScale.Hourly))
            {
                component.Update();
            }
        }

        private void OnDayElapsed()
        {
            foreach (var component in _components.Where(component =>
                         component.GetSimulationScale() == SimulationScale.Daily))
            {
                component.Update();
            }
        }
    }
}