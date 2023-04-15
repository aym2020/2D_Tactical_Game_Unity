using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyAIManager : MonoBehaviour
{
    public static EnemyAIManager Instance;

    public List<BaseEnemy> enemyUnits = new List<BaseEnemy>();
    public bool EnemiesTurnCompleted { get; set; }

    private void Awake()
    {
        Instance = this;
    }

    public void RegisterEnemy(BaseEnemy enemy)
    {
        enemyUnits.Add(enemy);
    }

    public void UnregisterEnemy(BaseEnemy enemy)
    {
        enemyUnits.Remove(enemy);
        UnitManager.Instance.AllBaseUnits.Remove(enemy);
    }

    public IEnumerator PerformEnemyTurn(Action onTurnFinished)
    {
        Debug.Log("PerformEnemyTurn");

        foreach (BaseEnemy enemy in enemyUnits)
        {
            Debug.Log("PerformEnemyTurn of " + enemy.name + "");

            // Move the enemy randomly
            MoveEnemyRandomly(enemy);

            // Use the enemy's spells
            UseEnemySpells(enemy);

            // Wait for some time before executing the next enemy's turn
            yield return new WaitForSeconds(1f);
        }

        // End the enemy turn and switch to the hero's turn
        onTurnFinished?.Invoke();
        EnemiesTurnCompleted = true;

        Debug.Log("Enemy turn finished");
    }

    private void MoveEnemyRandomly(BaseEnemy enemy)
    {
        Debug.Log("MoveEnemyRandomly");

        var enemyOriginTile = enemy.OccupiedTile;
        
        List<Tile> availableTiles = RangeFinder.GetMovementRangeTiles(enemyOriginTile, enemy.GetMovementPoints());

        if (availableTiles.Count == 0)
        {
            Debug.Log("No available tiles to move to");
            return;
        }
        
        Tile randomDestinationTile = availableTiles[UnityEngine.Random.Range(0, availableTiles.Count)];
        
        var path = Pathfinder.GetPath(enemyOriginTile, randomDestinationTile);
        int distanceTravelled = randomDestinationTile.CalculateDistance(enemyOriginTile);
        enemy.RemainingMovementPoints -= distanceTravelled;

        // MenuManager.Instance.ShowRemainingMovementPoint(enemy.OccupiedTile);

        StartCoroutine(enemy.MoveToTile(UnitManager.Instance.MovementDelay, path));
    }

    private void UseEnemySpells(BaseEnemy enemy)
    {
        SpellCaster enemySpellCaster = enemy.GetComponent<SpellCaster>();

        if (enemySpellCaster.Spells.Count > 0)
        {
            // Choose a random spell
            BaseSpell randomSpell = enemySpellCaster.Spells[UnityEngine.Random.Range(0, enemySpellCaster.Spells.Count)];

            // Get all targetable tiles within the spell's range and line of sight
            (List<Tile> allRangeTiles, List<Tile> lineOfSightTiles) = RangeFinder.GetSpellRangeTilesWithLineOfSight(enemy.OccupiedTile, randomSpell.GetSpellRange());

            // Merge the two lists of tiles (allRangeTiles and lineOfSightTiles)
            List<Tile> spellTargetableTiles = new List<Tile>(allRangeTiles);
            spellTargetableTiles.AddRange(lineOfSightTiles);

            if (spellTargetableTiles.Count > 0)
            {
                Tile randomTargetTile = spellTargetableTiles[UnityEngine.Random.Range(0, spellTargetableTiles.Count)];

                // Cast the spell on the selected target tile
                SpellManager.Instance.Cast(randomSpell, enemy, randomTargetTile); 
            }
        }
    }
}
