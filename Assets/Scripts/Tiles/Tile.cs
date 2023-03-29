using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tile : MonoBehaviour
{
    public string TileName;
    [SerializeField] protected SpriteRenderer _renderer;
    [SerializeField] private GameObject _highlight;
    [SerializeField] private GameObject _highlightMovement;
    [SerializeField] private GameObject _highlightPath;
    [SerializeField] private bool _isWalkable;

    public int X { get; private set; }
    public int Y { get; private set; }

    public BaseUnit OccupiedUnit;
    public bool Walkable => _isWalkable && OccupiedUnit == null;

    public virtual void Init(int x, int y)
    {
        X = x;
        Y = y;
    }

    void OnMouseEnter()
    {
        _highlight.SetActive(true);
        MenuManager.Instance.ShowTileInfo(this);

        if (UnitManager.Instance.SelectedHero != null)
        {
            List<Tile> availableTiles = RangeFinder.GetMovementRangeTiles(UnitManager.Instance.SelectedHero.OccupiedTile, UnitManager.Instance.SelectedHero.RemainingMovementPoints);
            if (availableTiles.Contains(this))
            {
                UnitManager.Instance.SelectedHero.ShowHighlightPath(this);
            }
        }
    }
    void OnMouseExit()
    {
        _highlight.SetActive(false);
        MenuManager.Instance.ShowTileInfo(null);

        if (UnitManager.Instance.SelectedHero != null)
        {
            List<Tile> availableTiles = RangeFinder.GetMovementRangeTiles(UnitManager.Instance.SelectedHero.OccupiedTile, UnitManager.Instance.SelectedHero.RemainingMovementPoints);
            if (availableTiles.Contains(this))
            {
                UnitManager.Instance.SelectedHero.HideHighlightPath();
            }
        }
    }

    private void OnMouseDown()
    {
        UnitManager.Instance.HandleTileClick(this);
    }

    // Calculate the distance between two tiles
    public int CalculateDistance(Tile otherTile)
    {
        return Mathf.RoundToInt(Vector2Int.Distance(new Vector2Int(X, Y), new Vector2Int(otherTile.X, otherTile.Y)));
    }

    // Set unit on tile
    public int SetUnit(BaseUnit unit)
    {
        int distance = 0;
        
        if (unit.OccupiedTile != null) 
        {
            unit.OccupiedTile.OccupiedUnit = null;
            distance = CalculateDistance(unit.OccupiedTile);
        }

        unit.transform.position = transform.position;
        OccupiedUnit = unit;
        unit.OccupiedTile = this;
        
        return distance;
    }


    // Highlight movement range
    public void HighlightMovementRange()
    {
        _highlightMovement.SetActive(true);
    }

    public void UnhighlightMovementRange()
    {
        _highlightMovement.SetActive(false);
    }

    // Highlight movement range
    public void HighlightPath()
    {
        _highlightPath.SetActive(true);
    }

    public void UnhighlightPath()
    {
        _highlightPath.SetActive(false);
    }

    

} 


 