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
    public List<Tile> TargetedTiles = new List<Tile>();

    // getters and setters
    public int X { get; private set; }
    public int Y { get; private set; }
    public GameObject Highlight => _highlight;
    public bool Walkable => _isWalkable && OccupiedUnit == null;
    public List<Tile> GetTargetedTilesList() => TargetedTiles;

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
                SetTargetTiles(this, targetSize, spellTargetType);

                foreach (Tile tile in TargetedTiles)
                {
                    tile.ChangeHighlightSpriteToTargetSprite();
                }
               
            }
        }
    }

    void OnMouseExit()
    {
        UnhighlightTargetTiles();

        this.UnhighlightTile();

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
    public void SetTargetTiles(Tile centerTile, int crossSize, SpellTargetType spellTargetType)
    {
        var gridManager = GridManager.Instance;
        int centerX = centerTile.X;
        int centerY = centerTile.Y;

        TargetedTiles.Add(centerTile);

        if (spellTargetType == SpellTargetType.OneTile)
        {
            centerTile._highlight.SetActive(true);
        }

        else if (spellTargetType == SpellTargetType.Cross)
        {
            for (int i = 1; i <= crossSize; i++)
            {
                // Directions to check
                Vector2[] directions = new Vector2[]
                {
                    new Vector2(centerX, centerY + i),
                    new Vector2(centerX, centerY - i),
                    new Vector2(centerX - i, centerY),
                    new Vector2(centerX + i, centerY),
                };

                // Iterate through directions and highlight valid tiles
                foreach (Vector2 direction in directions)
                {
                    var tile = gridManager.GetTileAtPosition(direction);
                    if (tile != null)
                    {
                        tile._highlight.SetActive(true);
                        TargetedTiles.Add(tile);
                    }
                }
            }
        }

        else if (spellTargetType == SpellTargetType.Circle)
        {
            for (int i = 1; i <= crossSize; i++)
            {
                // Directions to check
                Vector2[] directions = new Vector2[]
                {
                    new Vector2(centerX, centerY + i),
                    new Vector2(centerX, centerY - i),
                    new Vector2(centerX - i, centerY),
                    new Vector2(centerX + i, centerY),
                    new Vector2(centerX - (i - 1), centerY + (i - 1)),
                    new Vector2(centerX + (i - 1), centerY + (i - 1)),
                    new Vector2(centerX - (i - 1), centerY - (i - 1)),
                    new Vector2(centerX + (i - 1), centerY - (i - 1)),
                };

                // Iterate through directions and highlight valid tiles
                foreach (Vector2 direction in directions)
                {
                    var tile = gridManager.GetTileAtPosition(direction);
                    if (tile != null)
                    {
                        tile._highlight.SetActive(true);
                        TargetedTiles.Add(tile);
                    }
                }
            }
        }
    }

    public void UnhighlightTargetTiles()
    {
        List<Tile> tilesToRemove = new List<Tile>();

        foreach (Tile tile in TargetedTiles)
        {
            tile.UnhighlightTile();

            tile.ResetHighlightSpriteToDefault();
            
            tilesToRemove.Add(tile);
        }

        // Remove the tiles after iterating through the list
        foreach (Tile tile in tilesToRemove)
        {
            TargetedTiles.Remove(tile);
        }
    }

    // Unhighlight highlighted tile
    public void UnhighlightTile()
    {
        _highlight.SetActive(false);
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
