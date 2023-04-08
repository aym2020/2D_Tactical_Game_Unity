using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightManager : MonoBehaviour
{
    public static FightManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void ApplyDamage(BaseUnit attacker, BaseUnit target, int minDamage, int maxDamage)
    {
        int damage = Random.Range(minDamage, maxDamage);
        int newHealthPoints = target.GetHealthPoints() - damage;
        target.SetHealthPoints(newHealthPoints);

        if (newHealthPoints <= 0)
        {
            // Handle unit death (remove from the game, play animation, etc.)
        }
    }
}
