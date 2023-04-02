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
    Area = 2,
    Line = 3
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
