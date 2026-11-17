using Dungeon.DiceClasses;
using Dungeon.ENUM;
using Dungeon.Objects;
using Microsoft.Xna.Framework;

namespace Dungeon.Tests;

public class EntityFactoryTests
{
    [Fact]
    public void RegisteredSpawnGidCreatesTheRequestedEnemyType()
    {
        var factory = new EntityFactory(new FixedDiceRoller());
        factory.RegisterEnemy(103, position => new TestEnemy(position));
        Vector2 spawn = new(12, 24);

        Entity enemy = factory.CreateEnemy(103, spawn);

        Assert.IsType<TestEnemy>(enemy);
        Assert.Equal(spawn, enemy.Sprite.Position);
    }

    [Fact]
    public void UnknownSpawnGidThrowsWithTheMissingGid()
    {
        var factory = new EntityFactory(new FixedDiceRoller());

        InvalidDataException exception = Assert.Throws<InvalidDataException>(
            () => factory.CreateEnemy(999, Vector2.Zero)
        );

        Assert.Contains("999", exception.Message);
    }

    private sealed class TestEnemy : Entity
    {
        public TestEnemy(Vector2 position)
        {
            Sprite.Position = position;
            SetMaxHP(10);
        }
    }

    private sealed class FixedDiceRoller : IDiceRoller
    {
        public int Roll(DiceType diceType) => 1;
    }
}
