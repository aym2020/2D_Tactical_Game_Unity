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

    // getters and setters
    public bool IsSpellButtonSelected => isSpellButtonSelected;
    public Sprite OriginalSprite => originalSprite;
    public Sprite SelectedSprite => selectedSprite;
    public Button Button => button;

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

        if (isSpellButtonSelected)
        {
            SpellManager.Instance.SetSelectedSpell(null, null);
        }
        else
        {
            SpellManager.Instance.SetSelectedSpell(selectedSpell, this);
        }
        
        // Hide the movement range and highlight path
        spellCaster.GetComponent<BaseUnit>().HideHighlightPath();
        spellCaster.GetComponent<BaseUnit>().HideMovementRange();
        spellCaster.GetComponent<BaseUnit>().HideSpellRange();

        // Show the spell range
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

        // Inform SpellManager that this button has been selected
        if (isSelected)
        {
            SpellManager.Instance.SetSelectedSpellButton(this);
        }
    }

    public BaseSpell GetSpell()
    {
        if (spellCaster != null && spellIndex >= 0 && spellIndex < spellCaster.Spells.Count)
        {
            return spellCaster.Spells[spellIndex];
        }
        return null;
    }


}
