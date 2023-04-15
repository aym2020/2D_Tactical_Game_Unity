using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState GameState;
    public bool PlayerTurnCompleted { get; set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        MenuManager.Instance.ShowCurrentGameState(GameState);
    }

    void Start()
    {
        ChangeState(GameState.GenerateGrid);
    }

    public void StartGameLoop()
    {
        StartCoroutine(GameLoop());
    }

    private IEnumerator GameLoop()
    {
        while (true)
        {
            // Hero Turn
            ChangeState(GameState.HeroesTurn);
            yield return new WaitUntil(() => GameManager.Instance.PlayerTurnCompleted);
            CheckGameState();

            // Enemy Turn
            ChangeState(GameState.EnemiesTurn);
            yield return new WaitUntil(() => EnemyAIManager.Instance.EnemiesTurnCompleted);
            GameManager.Instance.PlayerTurnCompleted = false; // Add this line
            EnemyAIManager.Instance.EnemiesTurnCompleted = false;
            CheckGameState();
        }
    }

    public void ChangeState(GameState newState)
    {
        GameState = newState;
        
        Debug.Log("Game State: " + GameState);

        UnitManager.Instance.HideAllHighlights();
        
        switch (newState)
        {
            case GameState.GenerateGrid:
                GridManager.Instance.GenerateGrid();
                break;
            case GameState.SpawnHeroes:
                UnitManager.Instance.SpawnHeroes();
                break;
            case GameState.SpawnEnemies:
                UnitManager.Instance.SpawnEnemies();
                break;
            case GameState.HeroesTurn:
                UnitManager.Instance.ResetHeroes();
                UnitManager.Instance.ResetEnemies();
                break;
            case GameState.EnemiesTurn:
                StartCoroutine(EnemyAIManager.Instance.PerformEnemyTurn(() => EnemyAIManager.Instance.EnemiesTurnCompleted = true));
                break;
            case GameState.GameLoop:
                StartGameLoop();
                break;
            case GameState.Victory:
                Debug.Log("Victory!");
                // Add your custom victory logic here
                break;
            case GameState.GameOver:
                Debug.Log("Game Over!");
                // Add your custom game over logic here
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
    }

    public bool CheckGameState()
    {
        // if hero has health points less than or equal to 0, then hero is dead
        if (UnitManager.Instance.SpawnedHero == null)
        {
            ChangeState(GameState.GameOver);
            return false;
        }

        // if all enemies are dead, then player wins
        if (EnemyAIManager.Instance.enemyUnits.Count == 0)
        {
            ChangeState(GameState.Victory);
            return false;
        }
        return true;
    }
}

public enum GameState
{
    GenerateGrid = 0,
    SpawnHeroes = 1,
    SpawnEnemies = 2,
    HeroesTurn = 3,
    EnemiesTurn = 4,
    GameLoop = 5,
    Victory = 6,
    GameOver = 7
}