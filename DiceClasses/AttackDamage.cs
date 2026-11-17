using System;
using System.Text;
using Dungeon.ENUM;
using Dungeon.Objects;

namespace Dungeon.DiceClasses;

public sealed class AttackDamage
{
    private readonly Entity _target;
    private readonly IDiceRoller _diceRoller;
    private readonly StringBuilder _log = new();

    public int DamageTotal { get; private set; }
    public int HealingTotal { get; private set; }
    public string Log => _log.ToString();

    public AttackDamage(Entity target, IDiceRoller diceRoller = null)
    {
        _target = target ?? throw new ArgumentNullException(nameof(target));
        _diceRoller = diceRoller ?? Dice.DefaultRoller;
    }

    public void AddDamage(
        Damage damage,
        DamageType damageType,
        DiceType diceType,
        int modifier,
        string source
    )
    {
        ArgumentNullException.ThrowIfNull(source);

        int roll = _diceRoller.Roll(diceType);
        DamageResistanceLevel typeResistance = GetTypeResistance(damageType);
        DamageResistanceLevel damageResistance = GetDamageResistance(damage);
        int categoryPercent = GetCategoryPercent(damageType);
        int damagePercent = GetDamagePercent(damage);
        long rolledDamage = Math.Max(0L, (long)roll + modifier);
        int adjustedDamage = 0;
        bool healsTarget = false;

        if (typeResistance != DamageResistanceLevel.Immune &&
            damageResistance != DamageResistanceLevel.Immune)
        {
            double typeMultiplier = Math.Max(0d, (100d - categoryPercent) / 100d);
            double damageMultiplier = Math.Max(0d, (100d - damagePercent) / 100d);
            double totalMultiplier = typeMultiplier * damageMultiplier;

            adjustedDamage = (int)Math.Min(
                int.MaxValue,
                Math.Ceiling(rolledDamage * totalMultiplier));

            healsTarget =
                typeResistance == DamageResistanceLevel.Heal ||
                damageResistance == DamageResistanceLevel.Heal;
        }

        if (healsTarget)
            HealingTotal = AddSaturating(HealingTotal, adjustedDamage);
        else
            DamageTotal = AddSaturating(DamageTotal, adjustedDamage);

        _log.Append(source)
            .Append(healsTarget ? " healed " : " dealt ")
            .Append(adjustedDamage)
            .Append(' ')
            .Append(damageType)
            .Append(' ')
            .Append(damage)
            .Append(" damage to ")
            .Append(_target.Name)
            .AppendLine(".");
        _log.Append("Rolled: ")
            .Append(roll)
            .Append(" (")
            .Append(diceType)
            .Append(") + ")
            .Append(modifier)
            .Append(" = ")
            .Append(rolledDamage)
            .Append("; resistance: ")
            .Append(categoryPercent)
            .Append("% x ")
            .Append(damagePercent)
            .AppendLine("%");
    }

    public void Apply()
    {
        _target.TakeDamage((uint)DamageTotal);
        _target.RestoreHealth((uint)HealingTotal);
    }

    private DamageResistanceLevel GetTypeResistance(DamageType damageType) =>
        damageType switch
        {
            DamageType.Normal => _target.Normal,
            DamageType.Magic => _target.Magical,
            DamageType.True => _target.True,
            _ => throw new ArgumentOutOfRangeException(nameof(damageType), damageType, "Unsupported damage type.")
        };

    private DamageResistanceLevel GetDamageResistance(Damage damage) =>
        damage switch
        {
            Damage.Bludgeoning => _target.Bludgeoning,
            Damage.Piercing => _target.Piercing,
            Damage.Slashing => _target.Slashing,
            Damage.Heat => _target.Heat,
            Damage.Cold => _target.Cold,
            Damage.Energy => _target.Energy,
            Damage.Necrotic => _target.Necrotic,
            Damage.Psychic => _target.Psychic,
            _ => throw new ArgumentOutOfRangeException(nameof(damage), damage, "Unsupported damage.")
        };

    private int GetCategoryPercent(DamageType damageType) =>
        damageType switch
        {
            DamageType.Normal => _target.PhysicalPercent,
            DamageType.Magic => _target.MagicalPercent,
            DamageType.True => 0,
            _ => throw new ArgumentOutOfRangeException(nameof(damageType), damageType, "Unsupported damage type.")
        };

    private int GetDamagePercent(Damage damage) =>
        damage switch
        {
            Damage.Bludgeoning => _target.BludgeoningPercent,
            Damage.Piercing => _target.PiercingPercent,
            Damage.Slashing => _target.SlashingPercent,
            Damage.Heat => _target.HeatPercent,
            Damage.Cold => _target.ColdPercent,
            Damage.Energy => _target.EnergyPercent,
            Damage.Necrotic => _target.NecroticPercent,
            Damage.Psychic => _target.PsychicPercent,
            _ => throw new ArgumentOutOfRangeException(nameof(damage), damage, "Unsupported damage.")
        };

    private static int AddSaturating(int current, int value) =>
        (int)Math.Min(int.MaxValue, (long)current + value);
}
