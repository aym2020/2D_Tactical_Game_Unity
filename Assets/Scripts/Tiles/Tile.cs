using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tile : MonoBehaviour
{
    public string TileName;
    [SerializeField] protected SpriteRenderer _renderer;
    [SerializeField] private GameObject _highlight;
    [SerializeField] private GameObject _movementHighlight;
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
    }
    void OnMouseExit()
    {
        _highlight.SetActive(false);
        MenuManager.Instance.ShowTileInfo(null);
    }

    private void OnMouseDown()
    {
        UnitManager.Instance.HandleTileClick(this);
    }

    // Set unit on tile
    public void SetUnit(BaseUnit unit)
    {
        if (unit.OccupiedTile != null) 
        {
            unit.OccupiedTile.OccupiedUnit = null;
        }

        unit.transform.position = transform.position;
        OccupiedUnit = unit;
        unit.OccupiedTile = this;
    }

    // Highlight movement range
    public void Highlight()
    {
        _movementHighlight.SetActive(true);
    }

    public void Unhighlight()
    {
        _movementHighlight.SetActive(false);
    }
}

 