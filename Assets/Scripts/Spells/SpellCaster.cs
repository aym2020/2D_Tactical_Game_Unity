using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpellCaster : MonoBehaviour
{
    public List<BaseSpell> spells;
    private BaseSpell activeSpell;

    // getters and setters
    public BaseSpell GetActiveSpell() => activeSpell;
    public List<BaseSpell> Spells => spells;
       
    public void CastSpell(int spellIndex, Vector3 target)
    {
        if (spellIndex >= 0 && spellIndex < spells.Count)
        {
            spells[spellIndex].Cast(this.GetComponent<BaseUnit>(), target);
        }
    }

    public void SetActiveSpell(BaseSpell spell)
    {
        activeSpell = spell;
    }
}
