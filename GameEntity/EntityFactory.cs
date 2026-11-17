using System;
using System.Collections.Generic;
using System.IO;
using Dungeon.DiceClasses;
using Microsoft.Xna.Framework;

namespace Dungeon.Objects;

public interface IEntityFactory
{
    int PlayerSpawnGid { get; }
    IReadOnlyCollection<int> EnemySpawnGids { get; }
    Entity CreatePlayer(Vector2 position);
    Entity CreateEnemy(int spawnGid, Vector2 position);
}

public sealed class EntityFactory : IEntityFactory
{
    private readonly Dictionary<int, Func<Vector2, Entity>> _enemyFactories = new();

    public int PlayerSpawnGid { get; }
    public IReadOnlyCollection<int> EnemySpawnGids => _enemyFactories.Keys;

    public EntityFactory(IDiceRoller diceRoller, int playerSpawnGid = 101)
    {
        ArgumentNullException.ThrowIfNull(diceRoller);
        if (playerSpawnGid <= 0)
            throw new ArgumentOutOfRangeException(nameof(playerSpawnGid));

        PlayerSpawnGid = playerSpawnGid;
        RegisterEnemy(102, position => new EvilWizard(position, diceRoller));
    }

    public void RegisterEnemy(int spawnGid, Func<Vector2, Entity> factory)
    {
        ArgumentNullException.ThrowIfNull(factory);
        if (spawnGid <= 0 || spawnGid == PlayerSpawnGid)
            throw new ArgumentOutOfRangeException(nameof(spawnGid));
        if (!_enemyFactories.TryAdd(spawnGid, factory))
            throw new InvalidOperationException($"An enemy factory is already registered for spawn GID {spawnGid}.");
    }

    public Entity CreatePlayer(Vector2 position) => new PlayerWizard(position);

    public Entity CreateEnemy(int spawnGid, Vector2 position)
    {
        if (!_enemyFactories.TryGetValue(spawnGid, out Func<Vector2, Entity> factory))
            throw new InvalidDataException($"No enemy factory is registered for spawn GID {spawnGid}.");

        return factory(position)
            ?? throw new InvalidOperationException($"The enemy factory for spawn GID {spawnGid} returned null.");
    }
}
