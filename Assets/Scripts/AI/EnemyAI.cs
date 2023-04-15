using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyAI : MonoBehaviour
{
    protected BaseEnemy enemy;

    private Action onMovementFinished;

    private void Awake()
    {
        enemy = GetComponent<BaseEnemy>();
    }

    public void PerformAI()
    {
        switch (enemy.Archetype)
        {
            case EnemyArchetype.Melee:
                PerformMeleeAI();
                break;
            case EnemyArchetype.FragileFleeing:
                PerformFragileFleeingAI();
                break;
            // Add cases for other archetypes
            default:
                break;
        }
    }

    private void PerformMeleeAI()
    {
        // Perform melee AI based on the enemy's IntelligenceLevel
        switch (enemy.FightIntelligence)
        {
            case IntelligenceLevel.VeryDumb:
                // Perform very dumb melee behavior
                break;
            case IntelligenceLevel.Dumb:
                // Perform dumb melee behavior (Minotaur)
                MoveDumbMeleeTowardHero(() => UseDumbMeleeCloseCombatSpell());
                break;
            // Add cases for other IntelligenceLevels
            default:
                break;
        }
    }

    private void MoveDumbMeleeTowardHero(Action onMovementFinished = null)
    {
        Debug.Log("MoveDumbMeleeTowardHero");

        // Find the most efficient path to the hero
        var enemyOriginTile = enemy.OccupiedTile;
        List<Tile> availableTiles = RangeFinder.GetMovementRangeTiles(enemyOriginTile, enemy.GetMovementPoints());

        // ennemy distance to hero
        int distanceToHero = enemy.OccupiedTile.CalculateDistance(UnitManager.Instance.SpawnedHero.OccupiedTile);

        // if no available tiles, don't move
        if (availableTiles.Count == 0)
        {
            return;
        }

        // If the enemy is already next to the hero, don't move
        if (distanceToHero == 1)
        {
            return;
        }

        Tile nearestTileToHero = FindNearestTileToHero(availableTiles);

        // Find the most efficient path without considering other units (enemies or hero)
        var path = Pathfinder.GetPath(enemyOriginTile, nearestTileToHero);

        // Remove tiles occupied by other units (enemies or hero) from the path
        path.RemoveAll(tile => tile.OccupiedByUnit() && tile != enemyOriginTile);

        // If the path is not empty, move the enemy
        if (path.Count > 0)
            {
                int distanceTravelled = path[0].CalculateDistance(enemyOriginTile);
                enemy.RemainingMovementPoints -= distanceTravelled;

                StartCoroutine(enemy.MoveToTile(UnitManager.Instance.MovementDelay, path, onMovementFinished));
            }
    }

    private void UseDumbMeleeCloseCombatSpell()
    {
        Debug.Log("UseDumbMeleeCloseCombatSpell");

        SpellCaster enemySpellCaster = enemy.GetComponent<SpellCaster>();
        BaseSpell closeCombatSpell = null;

        // Find the close combat spell among the Minotaur's spells
        foreach (BaseSpell spell in enemySpellCaster.Spells)
        {
            if (spell.name == "CaC") // Replace with the actual name of the close combat spell
            {
                closeCombatSpell = spell;
                break;
            }
        }

        if (closeCombatSpell != null)
        {
            Tile heroTile = UnitManager.Instance.SpawnedHero.OccupiedTile;
            int distanceToHero = enemy.OccupiedTile.CalculateDistance(heroTile);

            // Check if the hero is within range of the close combat spell
            if (distanceToHero <= closeCombatSpell.GetSpellRange())
            {
                SpellManager.Instance.Cast(closeCombatSpell, enemy, heroTile);
            }
        }
    }

    private Tile FindNearestTileToHero(List<Tile> availableTiles)
    {
        var heroTile = UnitManager.Instance.SpawnedHero.OccupiedTile;
        Tile nearestTile = null;
        int minDistance = int.MaxValue;

        foreach (var tile in availableTiles)
        {
            int distance = tile.CalculateDistance(heroTile);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestTile = tile;
            }
            else if (distance == minDistance)
            {
                // If there are multiple tiles with the same distance, choose one of them
                if (UnityEngine.Random.value > 0.5f)
                {
                    nearestTile = tile;
                }
            }
        }

        return nearestTile;
    }

    private void PerformFragileFleeingAI()
    {
        // Perform fragile fleeing AI based on the enemy's IntelligenceLevel
        // Similar to the PerformMeleeAI method, implement the behavior for each IntelligenceLevel
    }

    // Add methods for other archetypes
}
