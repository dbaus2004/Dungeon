using System.Collections.Generic;
using System;

namespace Dungeon.ENUM
{
    public enum GameState
    {
        FreePlay,
        PlayerTurn,
        EnemyTurn
    }
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
    public enum DiceType
    {
        d0,
        d4,
        d6,
        d8,
        d10,
        d12,
        d20,
        d100
    }
}