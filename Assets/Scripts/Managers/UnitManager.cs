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
        /*Debug.Log($"Nombre d'unités chargées : {_units.Count}");
        foreach (var unit in _units)
        {
            Debug.Log($"Unité chargée : {unit.name}, Faction : {unit.Faction}, Préfab : {unit.UnitPrefab}");
        }*/

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

        /*Debug.Log($"Nombre d'unités filtrées : {filteredUnits.Count}");
        foreach (var unit in filteredUnits)
        {
            Debug.Log($"Unité filtrée : {unit.name}, Faction : {unit.Faction}, Préfab : {unit.UnitPrefab}");
        }*/

        T selectedUnit = _units.Where(u => u.Faction == faction && u.UnitPrefab is T)
                     .OrderBy(o => Random.value)
                     .Select(u => u.UnitPrefab)
                     .Cast<T>()
                     .FirstOrDefault();

        /*Debug.Log($"Unité sélectionnée : {selectedUnit}");*/

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

        if (tile.OccupiedUnit != null)
        {
            if (tile.OccupiedUnit.Faction == Faction.Hero) 
            {
                BaseHero selectedHero = SelectedHero;

                if (selectedHero != null)
                {
                    SelectedHero.HideMovementRange();
                    SetSelectedHero(null);
                }

                SetSelectedHero((BaseHero) tile.OccupiedUnit);
                SelectedHero.ShowMovementRange();
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

                int distanceTravelled = tile.SetUnit(SelectedHero);
                /*Debug.Log($"Distance : {distanceTravelled}");*/
                SelectedHero.RemainingMovementPoints -= distanceTravelled;
                /*Debug.Log($"Remaining movement points : {SelectedHero.RemainingMovementPoints}");*/

                SetSelectedHero(null);
            }
        }
    }
        
}
