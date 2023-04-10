using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SpellManager : MonoBehaviour
{
    public static SpellManager Instance;
    
    private List<ScriptableSpell> _spells;
    private SpellButtonHandler selectedSpellButton;

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

            BaseUnit targetUnit = targetTile.OccupiedUnit;

            if (targetUnit != null && targetUnit.Faction != caster.Faction)
            {
                FightManager.Instance.SetAttackerAndTarget(caster, targetUnit);

                Debug.Log(caster + " casting " + selectedSpell.GetSpellName() + " on " + targetUnit.UnitName);

                FightManager.Instance.ApplyDamage(caster, targetUnit, selectedSpell.GetSpellDamageBottom(), selectedSpell.GetSpellDamageUp());
            }

            // If the target tile is not occupied by an enemy unit
            else if (targetUnit == null)
            {
                FightManager.Instance.SetAttackerAndTarget(caster, null);

                Debug.Log(caster + " casting " + selectedSpell.GetSpellName() + " on " + targetTile);
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



    


}
