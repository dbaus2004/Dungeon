using Dungeon.ENUM;

namespace Dungeon.DiceClasses;

public sealed record AttackDefinition(
    string Name,
    int AccuracyModifier,
    Damage Damage,
    DamageType DamageType,
    DiceType DamageDice,
    int DamageModifier
);
