using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyAIManager : MonoBehaviour
{
    public static EnemyAIManager Instance;

    public List<BaseEnemy> enemyUnits = new List<BaseEnemy>();

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
    }

    public IEnumerator PerformEnemyTurn(Action onTurnFinished)
    {
        foreach (BaseEnemy enemy in enemyUnits)
        {
            // Move the enemy randomly
            MoveEnemyRandomly(enemy);

            // Use the enemy's spells
            UseEnemySpells(enemy);

            // Wait for some time before executing the next enemy's turn
            yield return new WaitForSeconds(2f);
        }

        // End the enemy turn and switch to the hero's turn
        onTurnFinished?.Invoke();
    }

    private void MoveEnemyRandomly(BaseEnemy enemy)
    {
        List<Tile> availableTiles = RangeFinder.GetMovementRangeTiles(enemy.OccupiedTile, enemy.GetMovementPoints());
        Tile randomTile = availableTiles[UnityEngine.Random.Range(0, availableTiles.Count)];
        int distanceTravelled = randomTile.SetUnit(enemy);
        enemy.RemainingMovementPoints -= distanceTravelled;
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
