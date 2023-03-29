using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUnit : MonoBehaviour
{
    public string UnitName;
    public Tile OccupiedTile;
    public Faction Faction;
    public int RemainingMovementPoints;

    [SerializeField] private int MovementPoints;
    [SerializeField] private int ActionPoints;
    [SerializeField] private int HealthPoints;

    private List<Tile> availableTiles;

    // Getter and setter methods
    public int GetMovementPoints() => MovementPoints;
    public int GetActionPoints() => ActionPoints;
    public int GetHealthPoints() => HealthPoints;

    public void SetMovementPoints(int value) => MovementPoints = value;
    public void SetActionPoints(int value) => ActionPoints = value;
    public void SetHealthPoints(int value) => HealthPoints = value;

    public void ShowMovementRange()
    {
        availableTiles = RangeFinder.GetMovementRangeTiles(OccupiedTile, RemainingMovementPoints);

        // Include the current tile (OccupiedTile) if it's not already in the list
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
            tile.Highlight();
        }
    }

    private void UnhighlightAvailableTiles()
    {
        foreach (Tile tile in availableTiles)
        {
            tile.Unhighlight();
        }
    }
}
