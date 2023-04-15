using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BaseUnit : MonoBehaviour
{
    public string UnitName;
    public Tile OccupiedTile;
    public Faction Faction;
    public float moveSpeed = 50f;

    [SerializeField] private int MovementPoints;
    [SerializeField] private int ActionPoints;
    [SerializeField] private int HealthPoints;

    public List<Tile> availableTiles;
    private List<Tile> highlightedPath;
    private List<Tile> spellRangeTiles;
    private List<Tile> spellTargetableTiles;

    public List<Tile> AvailableTiles
    {
        get { return availableTiles; }
    }
    public List<Tile> HighlightedPath
    {
        get { return highlightedPath; }
    }
    public List<Tile> SpellRangeTiles
    {
        get { return spellRangeTiles; }
    }
    public List<Tile> SpellTargetableTiles
    {
        get { return spellTargetableTiles; }
    }

    // Getter and setter methods
    public int GetMovementPoints() => MovementPoints;
    public int GetActionPoints() => ActionPoints;
    public int GetHealthPoints() => HealthPoints;
    public void SetHealthPoints(int value) => HealthPoints = value;

    // Events
    public event EventHandler OnMovementPointsReset;
    public event EventHandler OnActionPointsReset;
    public event EventHandler OnRemainingMovementPointsChanged;
    public event EventHandler OnRemainingActionPointsChanged;
    
    // Remaining points
    public int remainingMovementPoints;
    public int remainingActionPoints;

    public int RemainingMovementPoints
    {
        get => remainingMovementPoints;
        set
        {
            remainingMovementPoints = value;
            OnRemainingMovementPointsChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public int RemainingActionPoints
    {
        get => remainingActionPoints;
        set
        {
            remainingActionPoints = value;
            OnRemainingActionPointsChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private void Awake() 
    {
        RemainingMovementPoints = MovementPoints;
        RemainingActionPoints = ActionPoints;

        OccupiedTile = GridManager.Instance.GetHeroSpawnTile();
    }

    // Reset remaining points
    public void ResetMovementPoints()
    {
        RemainingMovementPoints = MovementPoints;
        OnMovementPointsReset?.Invoke(this, EventArgs.Empty);
    }

    public void ResetActionPoints()
    {
        RemainingActionPoints = ActionPoints;
        OnActionPointsReset?.Invoke(this, EventArgs.Empty);
    }

    // Movement
    public IEnumerator MoveToTile(float delay)
    {
        if (HighlightedPath != null)
        {
            foreach (Tile pathTile in HighlightedPath)
            {
                pathTile.UnhighlightPath();

                int distance = pathTile.SetUnit(this);

                float moveTime = distance * (1 / moveSpeed);
                float startTime = Time.time;

                Vector3 startPosition = transform.position;
                Vector3 endPosition = pathTile.transform.position;

                while (Time.time < startTime + moveTime)
                {
                    transform.position = Vector3.Lerp(startPosition, endPosition, (Time.time - startTime) / moveTime);
                    yield return null;
                }

                transform.position = endPosition;

                if (delay > 0)
                {
                    yield return new WaitForSeconds(delay);
                }
            }
        }
    }

    // Show and hide movement range
    public void ShowMovementRange()
    {
        availableTiles = RangeFinder.GetMovementRangeTiles(OccupiedTile, RemainingMovementPoints);

        if (!availableTiles.Contains(OccupiedTile))
        {
            availableTiles.Add(OccupiedTile);
        }

        HighlightAvailableTiles();
    }

    public void HideMovementRange()
    {
        UnhighlightAvailableTiles();
    }

    private void HighlightAvailableTiles()
    {
        foreach (Tile tile in availableTiles)
        {
            tile.HighlightMovementRange();
        }
    }

    private void UnhighlightAvailableTiles()
    {
        if (availableTiles != null)
        {
            foreach (Tile tile in availableTiles)
            {
                tile.UnhighlightMovementRange();
            }
        }
    }

    // Show and hide path
    public void ShowHighlightPath(Tile targetTile)
    {
        
        highlightedPath = Pathfinder.GetPath(OccupiedTile, targetTile);

        HighlightPathTiles();
    }

    public void HideHighlightPath()
    {
        UnhighlightPathTiles();
    }

    public void HighlightPathTiles()
    {
        foreach (Tile tile in highlightedPath)
            {
                tile.HighlightPath();
            }   
    }

    public void UnhighlightPathTiles()
    {
        if (highlightedPath != null)
        {
            foreach (Tile tile in highlightedPath)
            {
                tile.UnhighlightPath();
            }
        }
    }

    // Show and hide spell range
    public void ShowSpellRange(Tile OccupiedTile, int spellRange, SpellRangeType spellRangeType)
    {
        (spellRangeTiles, spellTargetableTiles) = RangeFinder.GetSpellRangeTilesWithLineOfSight(OccupiedTile, spellRange);

        if (!spellRangeTiles.Contains(OccupiedTile) && spellRangeType == SpellRangeType.SelfTarget)
        {
            spellRangeTiles.Add(OccupiedTile);
        }

        HighlightSpellRangeTiles();
        HighlightSpellTargetableTiles();
    }

    public void HideSpellRange()
    {
        UnhighlightSpellRangeTiles();
        UnhighlightSpellTargetableTiles();
    }

    public void HighlightSpellRangeTiles()
    {
        foreach (Tile tile in spellRangeTiles)
        {
            tile.HighlightSpellRange();
        }
    }

    public void UnhighlightSpellRangeTiles()
    {
        if (spellRangeTiles != null)
        {
            foreach (Tile tile in spellRangeTiles)
            {
                tile.UnhighlightSpellRange();
            }
        }
    }

    // Show and hide spell targetable tiles
    public void HighlightSpellTargetableTiles()
    {
        foreach (Tile tile in spellTargetableTiles)
        {
            tile.HighlightTargetableTile();
        }
    }

    public void UnhighlightSpellTargetableTiles()
    {
        if (spellTargetableTiles != null)
        {
            foreach (Tile tile in spellTargetableTiles)
            {
                tile.UnhighlightTargetableTile();
            }
        }
    }
    


}
