using Dungeon.DiceClasses;
using Dungeon.ENUM;
using Dungeon.Objects;

namespace Dungeon.Tests;

public class CombatResolverTests
{
    [Fact]
    public void SuccessfulAttackRollAppliesDamage()
    {
        var attacker = new TestEntity(10) { Name = "Player" };
        var target = new TestEntity(10) { Name = "Target", AC = 20 };
        var resolver = new CombatResolver(new SequenceDiceRoller(10, 10, 5));
        var attack = new AttackDefinition(
            "Fireball",
            10,
            Damage.Heat,
            DamageType.Magic,
            DiceType.d10,
            0
        );

        CombatResult result = resolver.Resolve(attacker, target, attack);

        Assert.True(result.IsHit);
        Assert.Equal(5u, target.HP);
        Assert.NotNull(result.Damage);
    }

    [Fact]
    public void MissedAttackDoesNotRollOrApplyDamage()
    {
        var attacker = new TestEntity(10) { Name = "Player" };
        var target = new TestEntity(10) { Name = "Target", AC = 20 };
        var resolver = new CombatResolver(new SequenceDiceRoller(1, 1));
        var attack = new AttackDefinition(
            "Fireball",
            10,
            Damage.Heat,
            DamageType.Magic,
            DiceType.d10,
            0
        );

        CombatResult result = resolver.Resolve(attacker, target, attack);

        Assert.False(result.IsHit);
        Assert.Equal(10u, target.HP);
        Assert.Null(result.Damage);
    }

    private sealed class TestEntity : Entity
    {
        public TestEntity(uint maxHP)
        {
            SetMaxHP(maxHP);
        }
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
