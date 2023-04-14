using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance;
    private List<ScriptableUnit> _scriptableUnits;
    public List<BaseUnit> allBaseUnits = new List<BaseUnit>();
    public BaseHero SelectedHero;
    public BaseHero SpawnedHero;
    public float movementDelay;

    // getters and setters
    public List<ScriptableUnit> ScriptableUnits => _scriptableUnits;
    public List<BaseUnit> AllBaseUnits => allBaseUnits;
    public float MovementDelay => movementDelay;

    public void SetSpawnedHero(BaseHero hero)
    {
        SpawnedHero = hero;
    }

    [SerializeField] private List<GameObject> spellButtons;

    private void Awake()
    {
        Instance = this;
        _scriptableUnits = Resources.LoadAll<ScriptableUnit>("Units").ToList();
    }

    public void SpawnHeroes()
    {
        var heroCount = 1;

        for (int i = 0; i < heroCount; i++)
        {
            var randomPrefab = GetRandomUnit<BaseHero>(Faction.Hero);

            SetSpawnedHero(Instantiate(randomPrefab));
            
            allBaseUnits.Add(SpawnedHero);

            var randomSpawnTile = GridManager.Instance.GetHeroSpawnTile();

            randomSpawnTile.SetUnit(SpawnedHero);

            SpellManager.Instance.UpdateSpellButtons();

            // Show hero attributes in the UI when a hero is spawned
            if (SpawnedHero != null)
            {
                MenuManager.Instance.ShowRemainingMovementPoint(SpawnedHero.OccupiedTile);
                MenuManager.Instance.ShowRemainingActionPoint(SpawnedHero.OccupiedTile);
            }
            else
            {
                MenuManager.Instance.ShowRemainingMovementPoint(null);
                MenuManager.Instance.ShowRemainingActionPoint(null);
            }

            // Update the SpellButtonHandler with the instantiated hero's SpellCaster component
            SpellCaster heroSpellCaster = SpawnedHero.GetComponent<SpellCaster>();

            // Assuming you have a reference to the spell buttons in an array or list
            foreach (GameObject spellButton in spellButtons)
            {
                SpellButtonHandler spellButtonHandler = spellButton.GetComponent<SpellButtonHandler>();
                spellButtonHandler.SetSpellCaster(heroSpellCaster);
                SpellManager.Instance.spellButtonHandlers.Add(spellButtonHandler);

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

            // Register the spawned enemy with the EnemyAIManager
            EnemyAIManager.Instance.RegisterEnemy(spawnedEnemy);

            allBaseUnits.Add(spawnedEnemy);
        }

        GameManager.Instance.ChangeState(GameState.HeroesTurn);
    }

    public void RemoveEnemy(BaseUnit unit)
    {
        if (unit is BaseEnemy enemy)
        {
            // Remove the enemy from the tile it occupies
            enemy.OccupiedTile.RemoveEnemyFromTile();

            // Unregister the enemy from the EnemyAIManager
            EnemyAIManager.Instance.UnregisterEnemy(enemy);

            // Destroy the enemy game object
            Destroy(enemy.gameObject);

            // Check for game state changes (victory or defeat)
            GameManager.Instance.CheckGameState();
        }
    }


    private T GetRandomUnit<T>(Faction faction) where T : BaseUnit
    {
        var filteredUnits = _scriptableUnits.Where(u => u.Faction == faction && u.UnitPrefab is T).ToList();

        T selectedUnit = _scriptableUnits.Where(u => u.Faction == faction && u.UnitPrefab is T)
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

    public void ResetHeroes()
    {
        foreach (BaseHero hero in AllBaseUnits.Where(u => u.Faction == Faction.Hero).Cast<BaseHero>())
        {
            hero.ResetMovementPoints();
            hero.ResetActionPoints();
        }
    }

    public void ResetEnemies()
    {
        foreach (BaseEnemy enemy in AllBaseUnits.Where(u => u.Faction == Faction.Enemy).Cast<BaseEnemy>())
        {
            enemy.ResetMovementPoints();
            enemy.ResetActionPoints();
        }
    }


    public void HandleTileClick(Tile tile)
    {
        if (GameManager.Instance.GameState != GameState.HeroesTurn) return;

        if (SpellManager.Instance.SelectedSpell != null)
        {
            HideAllHighlights();
            HandleSpellCast(tile);
        }
        else if (tile.OccupiedUnit != null)
        {
            HideAllHighlights();
            HandleUnitInteraction(tile);
        }
        else
        {
            HideAllHighlights();
            HandleEmptyTileInteraction(tile);
        }
    }


    private void HandleSpellCast(Tile tile)
    {
        SpellRangeType spellRangeType = SpellManager.Instance.SelectedSpell.GetSpellRangeType();

        if (spellRangeType != SpellRangeType.Self)
        {
            if (tile.OccupiedUnit != SpawnedHero)
            {
                if (SpawnedHero.SpellTargetableTiles.Contains(tile))
                {
                    SpellManager.Instance.Cast(SpellManager.Instance.SelectedSpell, SpawnedHero, tile);
                    MenuManager.Instance.ShowRemainingActionPoint(SpawnedHero.OccupiedTile);

                    SpawnedHero.HideSpellRange();
                    SpellManager.Instance.SetSelectedSpell(null, null);
                }
                else if (SpawnedHero.SpellRangeTiles.Contains(tile))
                {
                    
                }
                else
                {
                    SpawnedHero.HideSpellRange();
                    SpellManager.Instance.SetSelectedSpell(null, null);
                }
            }
            else if (tile.OccupiedUnit == SpawnedHero)
            {
                SetSelectedHero(SpawnedHero);

                SpellManager.Instance.SetSelectedSpell(null, null);
                SelectedHero.ShowMovementRange();
                SpawnedHero.HideSpellRange();
            }
        }
    }

    private void HandleUnitInteraction(Tile tile)
    {
        if (tile.OccupiedUnit.Faction == Faction.Hero)
        {
            BaseHero selectedHero = SelectedHero;

            if (selectedHero != null)
            {
                SelectedHero.HideMovementRange();
                SetSelectedHero(null);
            }
            else
            {
                if (SpellManager.Instance.SelectedSpell != null)
                {
                    SpawnedHero.HideSpellRange();
                    SpellManager.Instance.SetSelectedSpell(null, null);
                }
                else
                {
                    SetSelectedHero((BaseHero)tile.OccupiedUnit);
                    SelectedHero.ShowMovementRange();
                }
            }
        }
        else
        {
            tile.OccupiedUnit.ShowMovementRange();

            if (SelectedHero != null)
            {
                var enemy = (BaseEnemy)tile.OccupiedUnit;
                SetSelectedHero(null);      
            }
            
        }
    }

    private void HandleEmptyTileInteraction(Tile tile)
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

                SelectedHero.HideHighlightPath();

                int distanceTravelled = tile.CalculateDistance(SelectedHero.OccupiedTile);
                SelectedHero.RemainingMovementPoints -= distanceTravelled;
                MenuManager.Instance.ShowRemainingMovementPoint(SelectedHero.OccupiedTile);

                var path = SelectedHero.HighlightedPath;
                StartCoroutine(SelectedHero.MoveToTile(MovementDelay));

                SetSelectedHero(null);
            }
            else
            {
                SetSelectedHero(null);
            }
        }
        else
        {
            SpawnedHero.HideSpellRange();
            SpellManager.Instance.SetSelectedSpell(null, null);
        }
    }

    // Hide all highlights on all units
    public void HideAllHighlights()
    {
        foreach (BaseUnit unit in AllBaseUnits)
        {
            unit.HideMovementRange();
            unit.HideSpellRange();
            unit.HideHighlightPath();
        }
    }


}
