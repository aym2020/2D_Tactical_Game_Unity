using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyAIManager : MonoBehaviour
{
    public static EnemyAIManager Instance;

    public List<BaseEnemy> enemyUnits = new List<BaseEnemy>();
    public bool EnemiesTurnCompleted { get; set; }

    private void Awake()
    {
        Instance = this;
    }

    public void RegisterEnemy(BaseEnemy enemy)
    {
        enemyUnits.Add(enemy);
    }

    public void UnregisterEnemy(BaseEnemy enemy)
    {
        enemyUnits.Remove(enemy);
        UnitManager.Instance.AllBaseUnits.Remove(enemy);
    }

    public IEnumerator PerformEnemyTurn(Action onTurnFinished)
    {
        foreach (BaseEnemy enemy in enemyUnits)
            {
                // Call the PerformAI method for each enemy unit
                enemy.GetComponent<EnemyAI>().PerformAI();

                // Wait for some time before executing the next enemy's turn
                yield return new WaitForSeconds(1f);
            }

            // End the enemy turn and switch to the hero's turn
            onTurnFinished?.Invoke();
            EnemiesTurnCompleted = true;
        }
}