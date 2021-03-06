﻿using Bearded.Utilities.SpaceTime;

namespace Bearded.TD.Game.Units
{
    struct UnitBlueprint
    {
        public int Health { get; }
        public int Damage { get; }
        public Speed Speed { get; }
        public float Value { get; }

        public UnitBlueprint(int health, int damage, Speed speed, float value)
        {
            Health = health;
            Damage = damage;
            Speed = speed;
            Value = value;
        }
    }
}
