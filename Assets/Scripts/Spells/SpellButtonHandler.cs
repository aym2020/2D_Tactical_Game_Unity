using UnityEngine;
using UnityEngine.UI;

public class SpellButtonHandler : MonoBehaviour
{
    public SpellCaster spellCaster;
    public int spellIndex;
    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);
    }

    public void OnButtonClick()
    {
        
        if (spellCaster == null)
        {
            return;
        }
        
        BaseSpell selectedSpell = spellCaster.Spells[spellIndex];
        spellCaster.SetActiveSpell(selectedSpell);
        spellCaster.GetComponent<BaseUnit>().ShowSpellRange(spellCaster.GetComponent<BaseUnit>().OccupiedTile, selectedSpell.GetSpellRange());
    }

    public void SetSpellCaster(SpellCaster newSpellCaster)
    {
        spellCaster = newSpellCaster;
    }
}
