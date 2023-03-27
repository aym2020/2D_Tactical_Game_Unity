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
        // Check if it's the heroes' turn; if not, return without processing the click
        if (GameManager.Instance.GameState != GameState.HeroesTurn) return;

        // Check if the clicked tile has a unit on it
        if (tile.OccupiedUnit != null)
        {
            // Check if the unit on the clicked tile is a hero
            if (tile.OccupiedUnit.Faction == Faction.Hero) 
            {
                // Get the currently selected hero, if any
                BaseHero selectedHero = SelectedHero;

                // If there is a selected hero, hide its movement range
                if (selectedHero != null)
                {
                    selectedHero.HideMovementRange();
                }

                // Set the new selected hero to the hero on the clicked tile and show its movement range
                SetSelectedHero((BaseHero) tile.OccupiedUnit);
                SelectedHero.ShowMovementRange();
            }
            // If the unit on the clicked tile is not a hero (i.e., it's an enemy)
            else
            {
                // If there is a selected hero, kill the enemy on the clicked tile and deselect the hero
                if(SelectedHero != null)
                {
                    var enemy = (BaseEnemy) tile.OccupiedUnit;
                // Destroy the enemy object on the clicked tile
                Destroy(enemy.gameObject);
                // Deselect the current selected hero
                SetSelectedHero(null);
                }
            }
        }
        // If the clicked tile has no unit on it
        else
        {
            // If there is a selected hero
            if (SelectedHero != null)
            {
                // Hide the movement range of the selected hero
                SelectedHero.HideMovementRange();
                // Move the selected hero to the clicked tile
                tile.SetUnit(SelectedHero);
                // Deselect the hero after moving
                SetSelectedHero(null);
            }
        }
    }
        
}
