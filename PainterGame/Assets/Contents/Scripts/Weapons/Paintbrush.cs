using UnityEngine;
using System.Collections;

public class Paintbrush : MonoBehaviour {

    public Transform spellLaunch;

    public void CastSpell(GameObject spell)
    {
        Spell s = spell.GetComponent<Spell>();
        s.isReady = false;
        s.cooldownTimer = s.baseCooldownTime;

        spell.GetComponent<Spell>().Cast();

        GameObject castedSpell = Instantiate(spell, spellLaunch.position, spellLaunch.rotation) as GameObject;
    }
}
