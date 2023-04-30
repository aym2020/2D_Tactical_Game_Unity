using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    [SerializeField] private GameObject _selectedUnitObject, _tileObject, _tileUnitObject, _tileCoordinate, _currentGameStateObject;
    [SerializeField] private GameObject _heroRemainingMovementPointObject, _heroRemainingActionPointObject, _heroRemainingHealthPointObject;
    [SerializeField] private Button _endTurnButton;
    [SerializeField] private GameObject _battlefield;

    private void Awake()
    {
        Instance = this;
        _tileObject.SetActive(true);
        _tileUnitObject.SetActive(true);
        _tileCoordinate.SetActive(true);
        _heroRemainingMovementPointObject.SetActive(true);
        _heroRemainingActionPointObject.SetActive(true);
        _heroRemainingHealthPointObject.SetActive(true);
        _endTurnButton.interactable = true;
        _battlefield.SetActive(false);
    }

    public void ShowTileInfo(Tile tile)
    {
        if (tile == null)
        {
            _tileObject.SetActive(false);
            _tileUnitObject.SetActive(false);
            _tileCoordinate.SetActive(false);
            return;
        }

        _tileObject.GetComponentInChildren<Text>().text = tile.TileName;
        _tileObject.SetActive(true);

        _tileCoordinate.GetComponentInChildren<Text>().text = "(" + tile.X + "," + tile.Y + ")";
        _tileCoordinate.SetActive(true);

        if (tile.OccupiedUnit)
        {
            _tileUnitObject.GetComponentInChildren<Text>().text = tile.OccupiedUnit.UnitName;
            _tileUnitObject.SetActive(true);
        }
    }

    private void UpdateRemainingMovementPoints(object sender, EventArgs e)
    {
        BaseUnit unit = (BaseUnit)sender;
        _heroRemainingMovementPointObject.GetComponentInChildren<Text>().text = unit.RemainingMovementPoints.ToString();
    }

    private void UpdateRemainingActionPoints(object sender, EventArgs e)
    {
        BaseUnit unit = (BaseUnit)sender;
        _heroRemainingActionPointObject.GetComponentInChildren<Text>().text = unit.RemainingActionPoints.ToString();
        SpellManager.Instance.UpdateSpellButtons();
    }

    private void UpdateRemainingHealthPoints(object sender, EventArgs e)
    {
        BaseUnit unit = (BaseUnit)sender;
        _heroRemainingHealthPointObject.GetComponentInChildren<Text>().text = unit.RemainingHealthPoints.ToString();
    }

    public void ShowRemainingMovementPoint(Tile tile)
    {
        if (tile != null && tile.OccupiedUnit != null)
        {
            tile.OccupiedUnit.OnRemainingMovementPointsChanged -= UpdateRemainingMovementPoints;
            tile.OccupiedUnit.OnRemainingMovementPointsChanged += UpdateRemainingMovementPoints;

            UpdateRemainingMovementPoints(tile.OccupiedUnit, EventArgs.Empty);
            _heroRemainingMovementPointObject.SetActive(true);
        }
        else
        {
            _heroRemainingMovementPointObject.SetActive(false);
        }
    }

    public void ShowRemainingActionPoint(Tile tile)
    {
        if (tile != null && tile.OccupiedUnit != null)
        {
            tile.OccupiedUnit.OnRemainingActionPointsChanged -= UpdateRemainingActionPoints;
            tile.OccupiedUnit.OnRemainingActionPointsChanged += UpdateRemainingActionPoints;

            UpdateRemainingActionPoints(tile.OccupiedUnit, EventArgs.Empty);
            _heroRemainingActionPointObject.SetActive(true);
        }
        else
        {
            _heroRemainingActionPointObject.SetActive(false);
        }
    }

    public void ShowRemainingHealthPoint(Tile tile)
    {
        if (tile != null && tile.OccupiedUnit != null)
        {
            tile.OccupiedUnit.OnRemainingHealthPointsChanged -= UpdateRemainingHealthPoints;
            tile.OccupiedUnit.OnRemainingHealthPointsChanged += UpdateRemainingHealthPoints;

            UpdateRemainingHealthPoints(tile.OccupiedUnit, EventArgs.Empty);
            _heroRemainingHealthPointObject.SetActive(true);
        }
    }

    public void ShowUnitName(BaseUnit unit)
    {
        // Display the name of the selected unit, by default, it's the name of the spawned hero
        if (unit != null)
        {
            _selectedUnitObject.GetComponentInChildren<Text>().text = unit.UnitName;
            _selectedUnitObject.SetActive(true);
        }
        else
        {
            _selectedUnitObject.GetComponentInChildren<Text>().text = UnitManager.Instance.SpawnedHero.UnitName;
        }
    }

    public void ShowCurrentGameState(GameState gameState)
    {
        _currentGameStateObject.GetComponentInChildren<Text>().text = gameState.ToString();
    }


}
