using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SpellManager : MonoBehaviour
{
    public static SpellManager Instance;
    
    private List<ScriptableSpell> _spells;
    private SpellButtonHandler selectedSpellButton;
    public List<SpellButtonHandler> spellButtonHandlers;
    public BaseSpell _selectedSpell;

    public BaseSpell SelectedSpell
    {
        get { return _selectedSpell; }
        set { _selectedSpell = value; }
    }

    private void Awake()
    {
        Instance = this;

        _spells = Resources.LoadAll<ScriptableSpell>("Spells").ToList();
    }

    public void Cast(BaseSpell selectedSpell, BaseUnit caster, Tile targetTile)
    {
        // Check if caster has enough action points to cast the spell
        if (caster.RemainingActionPoints >= selectedSpell.GetSpellCost())
        {
            // Deduct the spell cost from caster's remaining action points
            caster.RemainingActionPoints -= selectedSpell.GetSpellCost();
            UpdateSpellButtons();

            // Get the list of target tiles
            List<BaseUnit> targets = GetUnitsFromTargetTiles(targetTile.GetTargetedTilesList());
            
            // Get the spell power
            int spellPower = FightManager.Instance.RandomizeDamage(selectedSpell.GetSpellDamageBottom(), selectedSpell.GetSpellDamageTop());

            foreach (BaseUnit targetUnit in targets)
            {
                // Calculate the damage
                int damageCalculated = FightManager.Instance.CalculateDamage(caster, targetUnit, spellPower);
        
                // If the target tile is occupied by an enemy unit
                if (targetUnit != null && targetUnit.Faction != caster.Faction)
                {
                    
                    FightManager.Instance.SetAttackerAndTarget(caster, targetUnit);
                    FightManager.Instance.ApplyDamage(caster, targetUnit, damageCalculated);
                }

                // If the target tile is not occupied by an enemy unit
                else if (targetUnit == null)
                {
                    FightManager.Instance.SetAttackerAndTarget(caster, null);
                }
            }
        }
        else
        {
            Debug.Log("Not enough action points to cast the spell.");
        }
    }

    public void SetSelectedSpell(BaseSpell newSpell, SpellButtonHandler newButton)
    {
        // If there is a previously selected button, deselect it
        if (selectedSpellButton != null)
        {
            selectedSpellButton.SetSelectedButton(false);
        }

        // Set the new selected spell and button
        SelectedSpell = newSpell;
        selectedSpellButton = newButton;

        // Update the new button's state
        if (selectedSpellButton != null)
        {
            selectedSpellButton.SetSelectedButton(true);
        }
    }

    public void SetSelectedSpellButton(SpellButtonHandler button)
    {
        if (selectedSpellButton != null && selectedSpellButton != button)
        {
            selectedSpellButton.SetSelectedButton(false);
        }
        selectedSpellButton = button;
    }
    
    public void UpdateSpellButtons()
    {
        if (UnitManager.Instance.SpawnedHero == null) return;

        int heroActionPoints = UnitManager.Instance.SpawnedHero.RemainingActionPoints;

        foreach (SpellButtonHandler spellButtonHandler in spellButtonHandlers)
        {
            BaseSpell spell = spellButtonHandler.GetSpell();
            if (spell != null)
            {
                bool canCastSpell = heroActionPoints >= spell.GetSpellCost();
                spellButtonHandler.Button.interactable = canCastSpell;
            }
        }
    }

    public List<BaseUnit> GetUnitsFromTargetTiles(List<Tile> tiles)
    {
        List<BaseUnit> targets = new List<BaseUnit>();

        foreach (Tile tile in tiles)
        {
            if (tile.OccupiedUnit != null)
            {
                targets.Add(tile.OccupiedUnit);
            }
        }
        Debug.Log("Targets: " + targets.Count);
        
        return targets;

        
    }

}
