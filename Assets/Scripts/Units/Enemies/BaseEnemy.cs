using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : BaseUnit
{
    public EnemyArchetype Archetype;
    public IntelligenceLevel FightIntelligence;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public enum EnemyArchetype
{
    Melee,
    FragileFleeing,
    OffensiveMage,
    HealerMage,
    Summoner,
    LongRange,
    Commander,
    // Add any other archetypes you want
}

public enum IntelligenceLevel
{
    VeryDumb = 0,
    Dumb = 1,
    Average = 2,
    Smart = 3,
    VerySmart = 4
}