using System.Collections;
using System.Collections.Generic;
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
            if (targetableTiles.Contains(this))
            {
                ChangeHighlightSpriteToTargetSprite();
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
                ResetHighlightSpriteToDefault();
            }
        }

        if (SpellManager.Instance.SelectedSpell != null)
        {
            List<Tile> targetableTiles = UnitManager.Instance.SpawnedHero.SpellTargetableTiles;
            if (targetableTiles.Contains(this))
            {
                ResetHighlightSpriteToDefault();
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
        return Mathf.Abs(X - otherTile.X) + Mathf.Abs(Y - otherTile.Y);
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

    // Highlight  targetable tile
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


 