using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance;

    private List<ScriptableUnit> _units;
    
    public BaseHero SelectedHero;

    private void Awake()
    {
        Instance = this;

        _units = Resources.LoadAll<ScriptableUnit>("Units").ToList();
    }

    public void SpawnHeroes()
    {
        var heroCount = 1;

        for (int i = 0; i < heroCount; i++)
        {
            var randomPrefab = GetRandomUnit<BaseHero>(Faction.Hero);
            var spawnedHero = Instantiate(randomPrefab);
            var randomSpawnTile = GridManager.Instance.GetHeroSpawnTile();

            randomSpawnTile.SetUnit(spawnedHero);

            // Set the hero's remaining movement points to its max movement points
            spawnedHero.RemainingMovementPoints = spawnedHero.GetMovementPoints();

            // Show hero attributes in the UI when a hero is selected
            if (spawnedHero != null)
            {
                MenuManager.Instance.ShowHeroAttributes(spawnedHero.OccupiedTile);
            }
            else
            {
                MenuManager.Instance.ShowHeroAttributes(null);
            }
        }
        GameManager.Instance.ChangeState(GameState.SpawnEnemies);
    }

    public void SpawnEnemies()
    {
        var ennemyCount = 1;

        for (int i = 0; i < ennemyCount; i++)
        {
            var randomPrefab = GetRandomUnit<BaseEnemy>(Faction.Enemy);
            var spawnedEnemy = Instantiate(randomPrefab);
            var randomSpawnTile = GridManager.Instance.GetEnemySpawnTile();

            randomSpawnTile.SetUnit(spawnedEnemy);
        }

        GameManager.Instance.ChangeState(GameState.HeroesTurn);
    }

    private T GetRandomUnit<T>(Faction faction) where T : BaseUnit
    {
        var filteredUnits = _units.Where(u => u.Faction == faction && u.UnitPrefab is T).ToList();

        T selectedUnit = _units.Where(u => u.Faction == faction && u.UnitPrefab is T)
                     .OrderBy(o => Random.value)
                     .Select(u => u.UnitPrefab)
                     .Cast<T>()
                     .FirstOrDefault();

        return selectedUnit;
    }

    public void SetSelectedHero(BaseHero hero)
    {
        SelectedHero = hero;
        MenuManager.Instance.ShowSelectedHero(hero);
    }


    public void HandleTileClick(Tile tile)
    {
        if (GameManager.Instance.GameState != GameState.HeroesTurn) return;

        // If the tile is occupied by a unit
        if (tile.OccupiedUnit != null)
        {

            // If the unit is a hero
            if (tile.OccupiedUnit.Faction == Faction.Hero) 
            {
                BaseHero selectedHero = SelectedHero;

                // If a hero is already selected
                if (selectedHero != null)
                {
                    SelectedHero.HideMovementRange();
                    SetSelectedHero(null);
                }
                else // If no hero is selected
                {
                    SetSelectedHero((BaseHero) tile.OccupiedUnit);
                    SelectedHero.ShowMovementRange();
                }
            }
            else
            {
                if(SelectedHero != null)
                {
                    var enemy = (BaseEnemy) tile.OccupiedUnit;
                Destroy(enemy.gameObject);
                SetSelectedHero(null);
                }
            }
        }
        else
        {
            if (SelectedHero != null)
            {
                SelectedHero.HideMovementRange();

                List<Tile> availableTiles = RangeFinder.GetMovementRangeTiles(SelectedHero.OccupiedTile, SelectedHero.RemainingMovementPoints);
                
                if (availableTiles.Contains(tile))
                {
                    if (SelectedHero.HighlightedPath != null)
                    {
                        foreach (Tile pathTile in SelectedHero.HighlightedPath)
                        {
                            pathTile.UnhighlightPath();
                        }
                    }  

                    int distanceTravelled = tile.SetUnit(SelectedHero);
                    SelectedHero.RemainingMovementPoints -= distanceTravelled;

                    SetSelectedHero(null);

                    MenuManager.Instance.ShowHeroAttributes(tile);
                
                }
            }
        }
    }   
}
