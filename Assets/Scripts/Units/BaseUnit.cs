using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUnit : MonoBehaviour
{
    public string UnitName;
    public Tile OccupiedTile;
    public Faction Faction;
    public List <Tile> availableTiles;
    public int RemainingMovementPoints;

    [SerializeField] private int MovementPoints;
    [SerializeField] private int ActionPoints;
    [SerializeField] private int HealthPoints;

    

    // getters and setters
    public int GetMovementPoints() { return MovementPoints; }
    public int GetActionPoints() { return ActionPoints; }
    public int GetHealthPoints() { return HealthPoints; }

    public void SetMovementPoints(int value) { MovementPoints = value; }
    public void SetActionPoints(int value) { ActionPoints = value; }
    public void SetHealthPoints(int value) { HealthPoints = value; }

    public void ShowMovementRange()
    {
        availableTiles = RangeFinder.GetMovementRangeTiles(OccupiedTile, RemainingMovementPoints);

        // Include the current tile (OccupiedTile) if it's not already in the list
        if (!availableTiles.Contains(OccupiedTile))
        {
            availableTiles.Add(OccupiedTile);
        }

        foreach (Tile tile in availableTiles)
        {
            tile.Highlight();
        }
    }


    public void HideMovementRange()
    {
        foreach (Tile tile in availableTiles)
        {
            tile.Unhighlight();
        }
    }        
}
