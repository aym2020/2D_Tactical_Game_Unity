using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class EndTurnButton : MonoBehaviour
{
    public Button _button;

    private void Start()
    {
        Button endTurnButton = _button.GetComponent<Button>();
        endTurnButton.onClick.AddListener(EndTurn);
    }

    void Update()
    {
        // Check if it's the player's turn
        if (GameManager.Instance.GameState == GameState.HeroesTurn && !GameManager.Instance.PlayerTurnCompleted)
        {
            _button.interactable = true;
        }
        else
        {
            _button.interactable = false;
        }
    }

    public void EndTurn()
    {
        _button.interactable = false;
        GameManager.Instance.PlayerTurnCompleted = true;
    }
}
