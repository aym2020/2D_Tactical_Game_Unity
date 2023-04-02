using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SpellManager : MonoBehaviour
{
    public static SpellManager Instance;
    
    private List<ScriptableSpell> _spells;

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
}
