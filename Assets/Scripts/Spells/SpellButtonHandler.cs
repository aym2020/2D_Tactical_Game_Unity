using UnityEngine;
using UnityEngine.UI;

public class SpellButtonHandler : MonoBehaviour
{
    public SpellCaster spellCaster;
    public int spellIndex;
    private Button button;
    public bool isSpellButtonSelected;
    public Sprite originalSprite;
    public Sprite selectedSprite;

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

        // select the button
        SetSelectedButton(true);
        
        // Hide the movement range and highlight path
        spellCaster.GetComponent<BaseUnit>().HideHighlightPath();
        spellCaster.GetComponent<BaseUnit>().HideMovementRange();
        spellCaster.GetComponent<BaseUnit>().HideSpellRange();

        // Show the spell range
        BaseSpell selectedSpell = spellCaster.Spells[spellIndex];
        spellCaster.SetActiveSpell(selectedSpell);
        spellCaster.GetComponent<BaseUnit>().ShowSpellRange(spellCaster.GetComponent<BaseUnit>().OccupiedTile, selectedSpell.GetSpellRange(), selectedSpell.GetSpellRangeType());

        // Set the selected spell
        SpellManager.Instance.SelectedSpell = selectedSpell;
        UnitManager.Instance.SetSelectedHero(null);
    }

    public void SetSpellCaster(SpellCaster newSpellCaster)
    {
        spellCaster = newSpellCaster;
    }

    public void SetSelectedButton(bool isSelected)
    {
        isSpellButtonSelected = isSelected;
        button.image.sprite = isSelected ? selectedSprite : originalSprite;
    }
}
