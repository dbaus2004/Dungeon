using System;
using Dungeon.Objects;

namespace Dungeon.DiceClasses;

public sealed class CombatResult
{
    public AttackRoll AttackRoll { get; }
    public AttackDamage Damage { get; }
    public bool IsHit => AttackRoll.IsHit;

    public string Log =>
        Damage is null ? AttackRoll.Log : $"{AttackRoll.Log}{Environment.NewLine}{Damage.Log}";

    internal CombatResult(AttackRoll attackRoll, AttackDamage damage)
    {
        AttackRoll = attackRoll;
        Damage = damage;
    }
}

public sealed class CombatResolver
{
    private readonly IDiceRoller _diceRoller;

    public CombatResolver(IDiceRoller diceRoller)
    {
        _diceRoller = diceRoller ?? throw new ArgumentNullException(nameof(diceRoller));
    }

    public CombatResult Resolve(Entity attacker, Entity target, AttackDefinition attack)
    {
        ArgumentNullException.ThrowIfNull(attacker);
        ArgumentNullException.ThrowIfNull(target);
        ArgumentNullException.ThrowIfNull(attack);

        var attackRoll = new AttackRoll(
            attacker.Name,
            target.Name,
            target.AC,
            new[] { (attack.AccuracyModifier, $"{attack.Name} accuracy") },
            diceRoller: _diceRoller
        );

        if (!attackRoll.IsHit)
            return new CombatResult(attackRoll, null);

        var damage = new AttackDamage(target, _diceRoller);
        damage.AddDamage(
            attack.Damage,
            attack.DamageType,
            attack.DamageDice,
            attack.DamageModifier,
            attack.Name
        );
        damage.Apply();

        return new CombatResult(attackRoll, damage);
    }
}
