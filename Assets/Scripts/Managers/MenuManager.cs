using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    [SerializeField] private GameObject _selectedHeroObject, _tileObject, _tileUnitObject, _tileCoordinate, _currentGameStateObject;
    [SerializeField] private GameObject _heroRemainingMovementPointObject, _heroRemainingActionPointObject;
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

    public void ShowRemainingMovementPoint(Tile tile)
    {
        if (tile != null && tile.OccupiedUnit != null)
        {
            _heroRemainingMovementPointObject.GetComponentInChildren<Text>().text = tile.OccupiedUnit.RemainingMovementPoints.ToString();
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
            _heroRemainingActionPointObject.GetComponentInChildren<Text>().text = tile.OccupiedUnit.RemainingActionPoints.ToString();
            _heroRemainingActionPointObject.SetActive(true);
        }
        else
        {
            _heroRemainingActionPointObject.SetActive(false);
        }
    }

    public void ShowSelectedHero(BaseHero hero)
    {
        if (hero == null)
        {
            _selectedHeroObject.SetActive(false);
            return;
        }

        _selectedHeroObject.GetComponentInChildren<Text>().text = hero.UnitName;
        _selectedHeroObject.SetActive(true);
    }

    public void ShowCurrentGameState(GameState gameState)
    {
        _currentGameStateObject.GetComponentInChildren<Text>().text = gameState.ToString();
    }


}
