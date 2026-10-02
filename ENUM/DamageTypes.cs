using System.Collections.Generic;
using System;

namespace Dungeon.ENUM
{
    public enum Damage
    {
        Bludgeoning,
        Piercing,
        Slashing,
        Heat,
        Cold,
        Energy,
        Necrotic,
        Psychic 
    }
    public enum DamageType
    {
        Normal,
        Magic,
        True
    }
    public enum DamageResistanceLevel
    {
        Normal,
        Immune,
        Heal
    }
}