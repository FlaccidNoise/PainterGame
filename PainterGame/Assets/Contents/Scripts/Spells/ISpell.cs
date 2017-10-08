using UnityEngine;

public interface ISpell
{
    // Spell Properties
    string name { get; set; }
    ColourList colour { get; set; }
    int spellIndex { get; set; }
    string description { get; set; }

    // Physical Spell
    GameObject effect { get; set; }

    // Unique Spell Effect properties
    float baseCooldownTime { get; set; }
    float cooldownTimer { get; set; }
    float paintCost { get; }
    bool isReady { get; set; }

    // Spell Mananger Info
    bool isLocked { get; set; }
    bool isEquipped { get; set; }

    void Cast();

    // TODO
    // Add to spellbook
}