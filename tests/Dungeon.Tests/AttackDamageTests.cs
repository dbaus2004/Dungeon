using Dungeon.DiceClasses;
using Dungeon.ENUM;
using Dungeon.Objects;

namespace Dungeon.Tests;

public class AttackDamageTests
{
    [Fact]
    public void HealingResistanceRestoresHealthWithoutExceedingMaximum()
    {
        var target = new TestEntity(10);
        target.TakeDamage(2);
        target.Heat = DamageResistanceLevel.Heal;

        var damage = new AttackDamage(target, new FixedDiceRoller(5));
        damage.AddDamage(Damage.Heat, DamageType.Magic, DiceType.d10, 0, "Test");
        damage.Apply();

        Assert.Equal(10u, target.HP);
        Assert.Equal(5, damage.HealingTotal);
        Assert.Equal(0, damage.DamageTotal);
    }

    [Fact]
    public void ResistanceReducesDamageBeforeApplyingIt()
    {
        var target = new TestEntity(10)
        {
            MagicalPercent = 50,
            HeatPercent = 20
        };
        var damage = new AttackDamage(target, new FixedDiceRoller(10));

        damage.AddDamage(Damage.Heat, DamageType.Magic, DiceType.d10, 0, "Test");
        damage.Apply();

        Assert.Equal(4, damage.DamageTotal);
        Assert.Equal(6u, target.HP);
    }

    private sealed class TestEntity : Entity
    {
        public TestEntity(uint maxHP)
        {
            SetMaxHP(maxHP);
        }
    }

    private sealed class FixedDiceRoller : IDiceRoller
    {
        private readonly int _roll;

        public FixedDiceRoller(int roll)
        {
            _roll = roll;
        }

        public int Roll(DiceType diceType) => _roll;
    }
}
