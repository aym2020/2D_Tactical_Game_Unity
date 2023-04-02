using UnityEngine;
using UnityEngine.UI;

public class SpellButtonHandler : MonoBehaviour
{
    [SerializeField] private BaseSpell spell;
    [SerializeField] private Button button;
    private SpellCaster spellCaster;

    private void Start()
    {
        // Assuming the SpellCaster component is attached to the same object as the button
        spellCaster = GetComponentInParent<SpellCaster>();

        if (button == null)
        {
            button = GetComponent<Button>();
        }

        if (button != null)
        {
            button.onClick.AddListener(OnButtonClick);
        }
    }

    private void OnButtonClick()
    {
        // You may want to implement a method in your SpellCaster class to set the active spell
        spellCaster.SetActiveSpell(spell);
    }
}
