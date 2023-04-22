using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Tile : MonoBehaviour
{
    public string TileName;
    public bool isObstacle;
    [SerializeField] protected SpriteRenderer _renderer;
    [SerializeField] private GameObject _highlight;
    [SerializeField] private GameObject _highlightMovement;
    [SerializeField] private GameObject _highlightPath;
    [SerializeField] private GameObject _highlightSpellRange;
    [SerializeField] private GameObject _highlightSpellTargetable;
    [SerializeField] private bool _isWalkable;
    [SerializeField] private BoxCollider2D tileCollider;

    public Sprite _spriteHighlight;
    public Sprite _spriteTargetHighlight;
    public Sprite _spriteMovementHighlight;
    public BaseUnit OccupiedUnit;
    public List<Tile> TargetTile = new List<Tile>();

    // getters and setters
    public int X { get; private set; }
    public int Y { get; private set; }
    public GameObject Highlight => _highlight;
    public bool Walkable => _isWalkable && OccupiedUnit == null;

    public virtual void Init(int x, int y)
    {
        X = x;
        Y = y;
        
        _highlight.GetComponent<SpriteRenderer>().sprite = _spriteHighlight;
    }

    void OnMouseEnter()
    {
        _highlight.SetActive(true);
        MenuManager.Instance.ShowTileInfo(this);

        if (UnitManager.Instance.SelectedHero != null && SpellManager.Instance.SelectedSpell == null)
        {
            List<Tile> AvailableTiles = RangeFinder.GetMovementRangeTiles(UnitManager.Instance.SelectedHero.OccupiedTile, UnitManager.Instance.SelectedHero.RemainingMovementPoints);
            if (AvailableTiles.Contains(this))
            {
                UnitManager.Instance.SelectedHero.ShowHighlightPath(this);
                ChangeHighlightSpriteToMovementSprite();
            }
        }
        else if (SpellManager.Instance.SelectedSpell != null)
        {
            List<Tile> targetableTiles = UnitManager.Instance.SpawnedHero.SpellTargetableTiles;
            SpellTargetType spellTargetType = SpellManager.Instance.SelectedSpell.GetSpellTargetType();
            int targetSize = SpellManager.Instance.SelectedSpell.GetSpellRadius();

            if (targetableTiles.Contains(this))
            {
                HighlightTargetTiles(this, targetSize, spellTargetType);

                foreach (Tile tile in TargetTile)
                {
                    tile.ChangeHighlightSpriteToTargetSprite();
                }
               
            }

            
        }
    }
    void OnMouseExit()
    {
        _highlight.SetActive(false);

        UnhighlightTargetTiles();

        MenuManager.Instance.ShowTileInfo(null);

        if (UnitManager.Instance.SelectedHero != null)
        {
            List<Tile> availableTiles = RangeFinder.GetMovementRangeTiles(UnitManager.Instance.SelectedHero.OccupiedTile, UnitManager.Instance.SelectedHero.RemainingMovementPoints);
            if (availableTiles.Contains(this))
            {
                UnitManager.Instance.SelectedHero.HideHighlightPath();
            }
        }
        ResetHighlightSpriteToDefault();
    }

    private void OnMouseDown()
    {
        UnitManager.Instance.HandleTileClick(this);
        ResetHighlightSpriteToDefault();
    }

    // Calculate the distance between two tiles
    public int CalculateDistance(Tile otherTile)
    {
        int distance = Mathf.Abs(X - otherTile.X) + Mathf.Abs(Y - otherTile.Y);
        return distance;
    }

    // Remove enemy from tile when killed
    public void RemoveEnemyFromTile()
    {
        OccupiedUnit = null;
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

    public void ClearUnit(BaseUnit unit)
    {
        unit.OccupiedTile.OccupiedUnit = null;
    }

    public bool OccupiedByUnit()
    {
        return UnitManager.Instance.AllBaseUnits.Any(unit => unit.OccupiedTile == this);
    }

    // Highlight cross tiles
    public void HighlightTargetTiles(Tile centerTile, int crossSize, SpellTargetType spellTargetType)
    {
        var gridManager = GridManager.Instance;
        int centerX = centerTile.X;
        int centerY = centerTile.Y;

        if (spellTargetType == SpellTargetType.OneTile)
        {
            centerTile._highlight.SetActive(true);
            TargetTile.Add(centerTile);
        }

        else if (spellTargetType == SpellTargetType.Cross)
        {
            for (int i = 1; i <= crossSize; i++)
            {
                // Up
                var upTile = gridManager.GetTileAtPosition(new Vector2(centerX, centerY + i));
                upTile._highlight.SetActive(true);
                TargetTile.Add(upTile);
                
                // Down
                var downTile = gridManager.GetTileAtPosition(new Vector2(centerX, centerY - i));
                downTile._highlight.SetActive(true);
                TargetTile.Add(downTile);

                // Left
                var leftTile = gridManager.GetTileAtPosition(new Vector2(centerX - i, centerY));
                leftTile._highlight.SetActive(true);
                TargetTile.Add(leftTile);

                // Right
                var rightTile = gridManager.GetTileAtPosition(new Vector2(centerX + i, centerY));
                rightTile._highlight.SetActive(true);
                TargetTile.Add(rightTile);
            }
        }

        else if (spellTargetType == SpellTargetType.Circle)
        {
            for (int i = 1; i <= crossSize; i++)
            {
                // Up
                var upTile = gridManager.GetTileAtPosition(new Vector2(centerX, centerY + i));
                upTile._highlight.SetActive(true);
                TargetTile.Add(upTile);
                
                // Down
                var downTile = gridManager.GetTileAtPosition(new Vector2(centerX, centerY - i));
                downTile._highlight.SetActive(true);
                TargetTile.Add(downTile);

                // Left
                var leftTile = gridManager.GetTileAtPosition(new Vector2(centerX - i, centerY));
                leftTile._highlight.SetActive(true);
                TargetTile.Add(leftTile);

                // Right
                var rightTile = gridManager.GetTileAtPosition(new Vector2(centerX + i, centerY));
                rightTile._highlight.SetActive(true);
                TargetTile.Add(rightTile);

                // Up Left
                var upLeftTile = gridManager.GetTileAtPosition(new Vector2(centerX - i, centerY + i));
                upLeftTile._highlight.SetActive(true);
                TargetTile.Add(upLeftTile);

                // Up Right
                var upRightTile = gridManager.GetTileAtPosition(new Vector2(centerX + i, centerY + i));
                upRightTile._highlight.SetActive(true);
                TargetTile.Add(upRightTile);

                // Down Left
                var downLeftTile = gridManager.GetTileAtPosition(new Vector2(centerX - i, centerY - i));
                downLeftTile._highlight.SetActive(true);
                TargetTile.Add(downLeftTile);

                // Down Right
                var downRightTile = gridManager.GetTileAtPosition(new Vector2(centerX + i, centerY - i));
                downRightTile._highlight.SetActive(true);
                TargetTile.Add(downRightTile);
            }
        }
    }

    public void UnhighlightTargetTiles()
    {
        List<Tile> tilesToRemove = new List<Tile>();

        foreach (Tile tile in TargetTile)
        {
            tile._highlight.SetActive(false);
            tilesToRemove.Add(tile);
        }

        // Remove the tiles after iterating through the list
        foreach (Tile tile in tilesToRemove)
        {
            TargetTile.Remove(tile);
        }
    }


    // Highlight movement range
    public virtual void HighlightMovementRange()
    {
        _highlightMovement.SetActive(true);
    }

    public virtual void UnhighlightMovementRange()
    {
        _highlightMovement.SetActive(false);
    }

    // Highlight movement range
    public virtual void HighlightPath()
    {
        _highlightPath.SetActive(true);
    }

    public virtual void UnhighlightPath()
    {
        _highlightPath.SetActive(false);
    }

    // Highlight spell range
    public virtual void HighlightSpellRange()
    {
        _highlightSpellRange.SetActive(true);
    }

    public virtual void UnhighlightSpellRange()
    {
        _highlightSpellRange.SetActive(false);
    }

    // Highlight targetable tile
    public virtual void HighlightTargetableTile()
    {
        _highlightSpellTargetable.SetActive(true);
    }

    public virtual void UnhighlightTargetableTile()
    {
        _highlightSpellTargetable.SetActive(false);
    }
    
    // Change the sprite of the game object _highlight
    public void ChangeHighlightSpriteToTargetSprite()
    {
        Highlight.GetComponent<SpriteRenderer>().sprite = _spriteTargetHighlight;
    }

    // Reset the sprite of the game object _highlight
    public void ResetHighlightSpriteToDefault()
    {
        Highlight.GetComponent<SpriteRenderer>().sprite = _spriteHighlight;
    }

    // Change the sprite of the game object _highlight
    public void ChangeHighlightSpriteToMovementSprite()
    {
        Highlight.GetComponent<SpriteRenderer>().sprite = _spriteMovementHighlight;
    }

} 


 