using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUnit : MonoBehaviour
{
    public string UnitName;
    public Tile OccupiedTile;
    public Faction Faction;
    public int RemainingMovementPoints;
    public int RemainingActionPoints;


    [SerializeField] private int MovementPoints;
    [SerializeField] private int ActionPoints;
    [SerializeField] private int HealthPoints;

    private List<Tile> availableTiles;
    private List<Tile> highlightedPath;
    private List<Tile> targetableTiles;

    public List<Tile> AvailableTiles
    {
        get { return availableTiles; }
    }
    public List<Tile> HighlightedPath
    {
        get { return highlightedPath; }
    }
    public List<Tile> TargetableTiles
    {
        get { return targetableTiles; }
    }

    // Getter and setter methods
    public int GetMovementPoints() => MovementPoints;
    public int GetActionPoints() => ActionPoints;
    public int GetHealthPoints() => HealthPoints;
    public int GetRemainingMovementPoints() => RemainingMovementPoints;
    public int GetRemainingActionPoints() => RemainingActionPoints;

    public void SetMovementPoints(int value) => MovementPoints = value;
    public void SetActionPoints(int value) => ActionPoints = value;
    public void SetHealthPoints(int value) => HealthPoints = value;
    public void SetRemainingMovementPoints(int value) => RemainingMovementPoints = value;
    public void SetRemainingActionPoints(int value) => RemainingActionPoints = value;
    
    private void Awake() 
    {
        SetRemainingMovementPoints(MovementPoints);
        SetRemainingActionPoints(ActionPoints);

        OccupiedTile = GridManager.Instance.GetHeroSpawnTile();
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

    //Show and hide spell range
    public void ShowSpellRange(Tile OccupiedTile, int spellRange, SpellRangeType spellRangeType)
    {
        targetableTiles = RangeFinder.GetSpellRangeTiles(OccupiedTile, spellRange);

        if (!targetableTiles.Contains(OccupiedTile) && spellRangeType == SpellRangeType.SelfTarget)
        {
            targetableTiles.Add(OccupiedTile);
        }

        HighlightTargetableTiles();
    }

    public void HideSpellRange()
    {
        UnhighlightTargetableTiles();
    }

    public void HighlightTargetableTiles()
    {
        foreach (Tile tile in targetableTiles)
        {
            tile.HighlightSpellRange();
        }
    }

    public void UnhighlightTargetableTiles()
    {
        if (targetableTiles != null)
        {
            foreach (Tile tile in targetableTiles)
            {
                tile.UnhighlightSpellRange();
            }
        }
    }


}
