using System;
using System.Text;
using Dungeon.ENUM;

namespace Dungeon.DiceClasses;

public sealed class AttackRoll
{
    public string Source { get; }
    public string Target { get; }
    public int D10One { get; }
    public int D10Two { get; }
    public int AC { get; }
    public (int Modifier, string Source)[] ToHitModifiers { get; }
    public int ToHitModifierTotal { get; }
    public (int Modifier, string Source)[] ToCritModifiers { get; }
    public int ToCritModifierTotal { get; }
    public (int Modifier, string Source)[] ToCritMissModifiers { get; }
    public int ToCritMissModifierTotal { get; }
    public bool IsHit { get; private set; }
    public string Log { get; }
    public bool IsCrit { get; private set; }
    public bool IsCritFail { get; private set; }

    public AttackRoll(
        string source,
        string target,
        int ac,
        (int Modifier, string Source)[] toHitModifiers,
        (int Modifier, string Source)[] toCritModifiers = null,
        (int Modifier, string Source)[] toCritMissModifiers = null,
        IDiceRoller diceRoller = null
    )
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(target);
        ArgumentNullException.ThrowIfNull(toHitModifiers);

        diceRoller ??= Dice.DefaultRoller;
        Source = source;
        Target = target;
        AC = ac;
        ToHitModifiers = (ValueTuple<int, string>[])toHitModifiers.Clone();
        ToCritModifiers = toCritModifiers is null
            ? Array.Empty<(int Modifier, string Source)>()
            : ((int Modifier, string Source)[])toCritModifiers.Clone();
        ToCritMissModifiers = toCritMissModifiers is null
            ? Array.Empty<(int Modifier, string Source)>()
            : ((int Modifier, string Source)[])toCritMissModifiers.Clone();

        ToHitModifierTotal = SumModifiers(ToHitModifiers);
        ToCritModifierTotal = SumModifiers(ToCritModifiers);
        ToCritMissModifierTotal = SumModifiers(ToCritMissModifiers);

        D10One = diceRoller.Roll(DiceType.d10);
        D10Two = diceRoller.Roll(DiceType.d10);

        int naturalRoll = D10One + D10Two;
        IsHit = naturalRoll + ToHitModifierTotal >= AC;
        IsCritFail = naturalRoll <= 2 + ToCritMissModifierTotal;
        IsCrit = naturalRoll >= 20 + ToCritModifierTotal;

        if (IsCritFail)
            IsHit = false;
        if (IsCrit)
            IsHit = true;

        Log = CreateLog(naturalRoll);
    }

    private static int SumModifiers((int Modifier, string Source)[] modifiers)
    {
        int total = 0;
        foreach ((int modifier, _) in modifiers)
            total = checked(total + modifier);
        return total;
    }

    private string CreateLog(int naturalRoll)
    {
        var log = new StringBuilder();
        log.Append(Source);
        log.Append(IsHit ? " hit " : " missed ");
        log.Append(Target);
        log.Append(" with a roll of ");
        log.Append(naturalRoll + ToHitModifierTotal);
        log.Append(" (");
        log.Append(D10One);
        log.Append(" + ");
        log.Append(D10Two);
        log.Append(") vs AC ");
        log.Append(AC);

        if (IsCrit)
            log.Append(" (Critical!)");
        else if (IsCritFail)
            log.Append(" (Critical Miss!)");

        foreach ((int modifier, string name) in ToHitModifiers)
        {
            log.AppendLine();
            log.Append("+ ");
            log.Append(modifier);
            log.Append(" (");
            log.Append(name);
            log.Append(')');
        }

        AppendModifiers(log, "Critical modifiers", ToCritModifiers);
        AppendModifiers(log, "Critical miss modifiers", ToCritMissModifiers);
        return log.ToString();
    }

    private static void AppendModifiers(
        StringBuilder log,
        string label,
        (int Modifier, string Source)[] modifiers
    )
    {
        if (modifiers.Length == 0)
            return;

        log.AppendLine();
        log.Append(label);
        log.Append(':');
        foreach ((int modifier, string source) in modifiers)
        {
            log.Append(" + ");
            log.Append(modifier);
            log.Append(" (");
            log.Append(source);
            log.Append(')');
        }
    }
}
