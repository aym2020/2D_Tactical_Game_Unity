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

    public void ApplyDamage(BaseUnit attacker, BaseUnit target, int damage)
    {
        // Show spell animation
        AnimationManager.Instance.CreateSpellAnimation(target.transform.position);

        // Apply damage
        int newHealthPoints = target.GetHealthPoints() - damage;
        target.SetHealthPoints(newHealthPoints);

        Debug.Log($"{attacker.UnitName} dealt {damage} damage to {target.UnitName}.");
        Debug.Log($"{target.UnitName} has {newHealthPoints} health points left.");

        // Show damage popup
        AnimationManager.Instance.CreateDamagePopup(target.transform.position, damage, false);

        if (newHealthPoints <= 0 && target.Faction == Faction.Enemy)
        {
            // Handle unit death (remove from the game, play animation, etc.)
            Debug.Log($"{target.UnitName} has died.");
            
            UnitManager.Instance.RemoveEnemy(target);
        }
        else if (newHealthPoints <= 0 && target.Faction == Faction.Hero)
        {
            // Handle unit death (remove from the game, play animation, etc.)
            Debug.Log($"{target.UnitName} has died.");
            
            UnitManager.Instance.SpawnedHero = null;
        }
    }

    public void SetAttackerAndTarget(BaseUnit attacker, BaseUnit target)
    {
        Attackers.Add(attacker);
        Targets.Add(target);
    }

    // Calculate damage
    public int CalculateDamage(BaseUnit caster, BaseUnit targetUnit, int damages)
    {
        int damageCalculated = damages * 1;
        
        return damageCalculated;
    }

    public int RandomizeDamage(int minDamage, int maxDamage)
    {
        int spellPower = Random.Range(minDamage, maxDamage);
        return spellPower;
    }

}
