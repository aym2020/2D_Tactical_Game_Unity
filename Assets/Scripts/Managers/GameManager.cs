using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState GameState;

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
            yield return new WaitUntil(() => GameState == GameState.EnemiesTurn);

            // Enemy Turn
            ChangeState(GameState.EnemiesTurn);
            yield return new WaitUntil(() => GameState == GameState.HeroesTurn);
        }
    }


    public void ChangeState(GameState newState)
    {
        GameState = newState;
        
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
                ChangeState(GameState.GameLoop);
                break;
            case GameState.HeroesTurn:
                UnitManager.Instance.ResetHeroes();
                break;
            case GameState.EnemiesTurn:
                UnitManager.Instance.ResetEnemies();
                StartCoroutine(EnemyAIManager.Instance.PerformEnemyTurn(() => ChangeState(GameState.HeroesTurn)));
                break;
            case GameState.GameLoop:
                StartGameLoop();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
    }

}


public enum GameState
{
    GenerateGrid = 0,
    SpawnHeroes = 1,
    SpawnEnemies = 2,
    HeroesTurn = 3,
    EnemiesTurn = 4,
    GameLoop = 5
}