using UnityEngine;
using System.Collections;
using System;

public class Weapon : MonoBehaviour
{
    public Transform spellLaunch
    {
        get
        {
            throw new NotImplementedException();
        }

        set
        {
            throw new NotImplementedException();
        }
    }

    void Start()
    {
        // muzzleFlash = GetComponent<MuzzleFlash>();
    }

    public void CastSpell(ISpell spell)
    {
        Debug.Log("Casted Spell" + spell.name);
        //Spell castedSpell = Instantiate(spell, muzzle.position, muzzle.rotation) as Spell;
    }
}
