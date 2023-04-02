using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SpellManager : MonoBehaviour
{
    public static SpellManager Instance;
    
    private List<ScriptableSpell> _spells;

    public BaseSpell SelectedSpell;

    private void Awake()
    {
        Instance = this;

        _spells = Resources.LoadAll<ScriptableSpell>("Spells").ToList();
    }
}
