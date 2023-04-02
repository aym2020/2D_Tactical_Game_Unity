using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    [SerializeField] private GameObject _selectedHeroObject, _tileObject, _tileUnitObject, _heroAttributesObject, _currentGameStateObject;
    [SerializeField] private Button _endTurnButton;
    [SerializeField] private GameObject _battlefield;

    private void Awake()
    {
        Instance = this;
        _heroAttributesObject.SetActive(true);
        _endTurnButton.interactable = true;
        _battlefield.SetActive(false);
    }

    public void ShowTileInfo(Tile tile)
    {
        if (tile == null)
        {
            _tileObject.SetActive(false);
            _tileUnitObject.SetActive(false);
            return;
        }

        _tileObject.GetComponentInChildren<Text>().text = tile.TileName;
        _tileObject.SetActive(true);

        if (tile.OccupiedUnit)
        {
            _tileUnitObject.GetComponentInChildren<Text>().text = tile.OccupiedUnit.UnitName;
            _tileUnitObject.SetActive(true);
        }
    }

    public void ShowHeroAttributes(Tile tile)
    {
        if (tile != null && tile.OccupiedUnit != null)
        {
            _heroAttributesObject.GetComponentInChildren<Text>().text = tile.OccupiedUnit.RemainingMovementPoints.ToString();
            _heroAttributesObject.SetActive(true);
        }
        else
        {
            _heroAttributesObject.SetActive(false);
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
