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

    void Update () 
	{
		// checks if the players are ready and if the start button is useable
		if (GameManager.Instance.GameState == GameState.HeroesTurn && _button.interactable == false) 
		{
			//allows the start button to be used
			_button.interactable = true;
		}
	}

    public void EndTurn()
    {
        if (_button.interactable == true)
        {
            //UnitManager.Instance.HideAllHighlights();
            GameManager.Instance.ChangeState(GameState.EnemiesTurn);
            _button.interactable = false;
        }
    }
}
