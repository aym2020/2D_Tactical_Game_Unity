using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseSpell : MonoBehaviour
{
    [SerializeField] private string spellName;
    [SerializeField] private Sprite spellIcon;
    [SerializeField] private int spellMinRange;
    [SerializeField] private int spellRange;
    [SerializeField] private int spellDamageBottom;
    [SerializeField] private int spellDamageTop;
    [SerializeField] private int spellCost;
    [SerializeField] private int spellCooldown;
    [SerializeField] private int spellDuration;
    [SerializeField] private int spellRadius;
    public SpellType spellType;
    public SpellRangeType spellRangeType;
    public SpellTargetType spellTargetType;

    // getter and setter methods
    public string GetSpellName() => spellName;
    public Sprite GetSpellIcon() => spellIcon;
    public int GetSpellMinRange() => spellMinRange;
    public int GetSpellRange() => spellRange;
    public int GetSpellDamageBottom() => spellDamageBottom;
    public int GetSpellDamageTop() => spellDamageTop;
    public int GetSpellCost() => spellCost;
    public int GetSpellCooldown() => spellCooldown;
    public int GetSpellDuration() => spellDuration;
    public int GetSpellRadius() => spellRadius;
    public SpellType GetSpellType() => spellType;
    public SpellRangeType GetSpellRangeType() => spellRangeType;
    public SpellTargetType GetSpellTargetType() => spellTargetType;

    public void SetSpellName(string value) => spellName = value;
    public void SetSpellIcon(Sprite value) => spellIcon = value;
    public void SetSpellMinRange(int value) => spellMinRange = value;
    public void SetSpellRange(int value) => spellRange = value;
    public void SetSpellDamageBottom(int value) => spellDamageBottom = value;
    public void SetSpellDamageUp(int value) => spellDamageTop = value;
    public void SetSpellCost(int value) => spellCost = value;
    public void SetSpellCooldown(int value) => spellCooldown = value;
    public void SetSpellDuration(int value) => spellDuration = value;
    public void SetSpellRadius(int value) => spellRadius = value;
    public void SetSpellType(SpellType value) => spellType = value;
    public void SetSpellRangeType(SpellRangeType value) => spellRangeType = value;
    public void SetSpellTargetType(SpellTargetType value) => spellTargetType = value;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
