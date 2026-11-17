using System;
using Dungeon.ENUM;

namespace Dungeon.DiceClasses;

public interface IDiceRoller
{
    int Roll(DiceType diceType);
}

public sealed class DiceRoller : IDiceRoller
{
    private readonly Random _random;

    public DiceRoller(Random random = null)
    {
        _random = random ?? Random.Shared;
    }

    public int Roll(DiceType diceType)
    {
        int sides = diceType switch
        {
            DiceType.d0 => 0,
            DiceType.d4 => 4,
            DiceType.d6 => 6,
            DiceType.d8 => 8,
            DiceType.d10 => 10,
            DiceType.d12 => 12,
            DiceType.d20 => 20,
            DiceType.d100 => 100,
            _ => throw new ArgumentOutOfRangeException(nameof(diceType), diceType, "Unsupported dice type.")
        };

        return sides == 0 ? 0 : _random.Next(1, sides + 1);
    }
}

public static class Dice
{
    public static IDiceRoller DefaultRoller { get; } = new DiceRoller();

    public static int D4() => DefaultRoller.Roll(DiceType.d4);
    public static int D6() => DefaultRoller.Roll(DiceType.d6);
    public static int D8() => DefaultRoller.Roll(DiceType.d8);
    public static int D10() => DefaultRoller.Roll(DiceType.d10);
    public static int D12() => DefaultRoller.Roll(DiceType.d12);
    public static int D20() => DefaultRoller.Roll(DiceType.d20);
    public static int D100() => DefaultRoller.Roll(DiceType.d100);
}
