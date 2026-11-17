using Dungeon.DiceClasses;
using Dungeon.ENUM;

namespace Dungeon.Tests;

public class AttackRollTests
{
    [Fact]
    public void CriticalHitOverridesArmorClass()
    {
        var attack = new AttackRoll(
            "Player",
            "Target",
            ac: 100,
            Array.Empty<(int, string)>(),
            diceRoller: new SequenceDiceRoller(10, 10));

        Assert.True(attack.IsCrit);
        Assert.True(attack.IsHit);
    }

    [Fact]
    public void CriticalMissOverridesToHitModifiers()
    {
        var attack = new AttackRoll(
            "Player",
            "Target",
            ac: 1,
            new[] { (100, "Accuracy") },
            diceRoller: new SequenceDiceRoller(1, 1));

        Assert.True(attack.IsCritFail);
        Assert.False(attack.IsHit);
    }

    private sealed class SequenceDiceRoller : IDiceRoller
    {
        private readonly Queue<int> _rolls;

        public SequenceDiceRoller(params int[] rolls)
        {
            _rolls = new Queue<int>(rolls);
        }

        public int Roll(DiceType diceType) => _rolls.Dequeue();
    }
}
