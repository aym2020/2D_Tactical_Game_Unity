using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightManager : MonoBehaviour
{
    public static FightManager Instance;
    public List<BaseUnit> Attackers { get; private set; } = new List<BaseUnit>();
    public List<BaseUnit> Targets { get; private set; } = new List<BaseUnit>();


    private void Awake()
    {
        Instance = this;
    }

    public void ApplyDamage(BaseUnit attacker, BaseUnit target, int minDamage, int maxDamage)
    {
        int damage = Random.Range(minDamage, maxDamage);
        int newHealthPoints = target.GetHealthPoints() - damage;
        target.SetHealthPoints(newHealthPoints);

        Debug.Log($"{attacker.UnitName} dealt {damage} damage to {target.UnitName}.");
        Debug.Log($"{target.UnitName} has {newHealthPoints} health points left.");

        if (newHealthPoints <= 0)
        {
            // Handle unit death (remove from the game, play animation, etc.)
            Debug.Log($"{target.UnitName} has died.");
            
            UnitManager.Instance.RemoveEnemy(target);
        }
    }

    public void SetAttackerAndTarget(BaseUnit attacker, BaseUnit target)
    {
        Attackers.Add(attacker);
        Targets.Add(target);
    }

}
