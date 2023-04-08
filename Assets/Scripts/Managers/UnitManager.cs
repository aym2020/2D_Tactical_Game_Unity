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
    public BaseHero SpawnedHero;

    // Setters
    public void SetSpawnedHero(BaseHero hero)
    {
        SpawnedHero = hero;
    }

    
    [SerializeField] private List<GameObject> spellButtons;

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

            SetSpawnedHero(Instantiate(randomPrefab));

            var randomSpawnTile = GridManager.Instance.GetHeroSpawnTile();

            randomSpawnTile.SetUnit(SpawnedHero);

            // Show hero attributes in the UI when a hero is selected
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

        // If selected spell is not null, hide the spell range and try casting the spell
        if (SpellManager.Instance.SelectedSpell != null)
        {
            if (SpawnedHero != null)
            {
                SpawnedHero.HideSpellRange();
            }

            TryCastSpell(tile);

            SpellManager.Instance.SelectedSpell = null;
        }

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

                    MenuManager.Instance.ShowRemainingMovementPoint(tile);
                
                }
                else
                {
                    SetSelectedHero(null);
                }
            }
        }
    }  

    private void TryCastSpell(Tile targetTile)
    {
        if (GameManager.Instance.GameState != GameState.HeroesTurn) return;

        // If selected spell is not null, hide the spell range and try casting the spell
        if (SpellManager.Instance.SelectedSpell != null && SpawnedHero != null)
        {
            SpawnedHero.HideSpellRange();

            // Check if the target tile is within the spell range
            if (SpawnedHero.TargetableTiles.Contains(targetTile))
            {
                // Cast the selected spell on the target tile
                SpellManager.Instance.Cast(SpellManager.Instance.SelectedSpell, SpawnedHero, targetTile);

                // Update the remaining action points in the UI
                MenuManager.Instance.ShowRemainingActionPoint(SpawnedHero.OccupiedTile);
            }

            SpellManager.Instance.SelectedSpell = null;
        }
    }

}
