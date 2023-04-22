using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Spell", menuName = "Spells/Spell")]

public class ScriptableSpell : ScriptableObject
{
    public BaseSpell spellPrefab;
}

public enum SpellRangeType
{
    Self = 0,
    Target = 1,
    SelfTarget = 2,
    NoLineOfSight = 3,
    Line = 4
}

public enum SpellTargetType
{
    OneTile = 0,
    Cross = 1,
    Circle = 2,
    Inline = 3
}

public enum SpellType
{
    Damage = 0,
    Heal = 1,
    Buff = 2,
    Debuff = 3,
    Summon = 4,
    Teleport = 5
}
