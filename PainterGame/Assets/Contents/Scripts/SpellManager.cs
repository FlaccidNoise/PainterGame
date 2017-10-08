using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpellManager : MonoBehaviour {
    //static int NUM_COLOURS = 6;
    //static int NUM_SPELLS_PER_COLOUR = 6;
    static int NUM_EQUIPPABLE_SPELLS = 6;

    List<List<Spell>> allSpells = new List<List<Spell>>();

    [SerializeField]
    List<Spell> redSpells = new List<Spell>();

    [SerializeField]
    List<Spell> orangeSpells = new List<Spell>();

    [SerializeField]
    List<Spell> yellowSpells = new List<Spell>();

    [SerializeField]
    List<Spell> greenSpells = new List<Spell>();

    [SerializeField]
    List<Spell> blueSpells = new List<Spell>();

    [SerializeField]
    List<Spell> purpleSpells = new List<Spell>();

    [SerializeField]
    GameObject[] _equippedSpells = new GameObject[NUM_EQUIPPABLE_SPELLS];
    public GameObject[] equippedSpells
    {
        get { return _equippedSpells; }
        private set { _equippedSpells = value; }
    }

    void Start()
    {
        // Initialize Spells
        PopulateAllSpells();
        LockAllSpells();

        // Unlock and equip first spell
        ToggleSpellLock(ColourList.RED, 0);
        EquipSpell(ColourList.RED, 0, SpellInputList.LeftMouseButton);
        ToggleSpellLock(ColourList.RED, 1);
        EquipSpell(ColourList.RED, 1, SpellInputList.RightMouseButton);

        Debug.Log("BaseCooldownTime: " + equippedSpells[(int)SpellInputList.LeftMouseButton].GetComponent<Spell>().baseCooldownTime);
    }

    void PopulateAllSpells()
    {
        // Fill the allSpells List 
        allSpells.Add(redSpells);
        allSpells.Add(orangeSpells);
        allSpells.Add(yellowSpells);
        allSpells.Add(greenSpells);
        allSpells.Add(blueSpells);
        allSpells.Add(purpleSpells);
    }

    void LockAllSpells()
    {
        for (int i = 0; i < allSpells.Count; i++)
        {
            for (int j = 0; j < allSpells[i].Count; j++)
            {
                if (allSpells[i][j] != null)
                {
                    allSpells[i][j].isLocked = true;
                }
            }
        }
    }

    void ToggleSpellLock(ColourList colour, int spellIndex)
    {
        if (allSpells[(int)colour][spellIndex] != null)
        {
            allSpells[(int)colour][spellIndex].isLocked = !allSpells[(int)colour][spellIndex].isLocked;

        }
    }

    void EquipSpell(ColourList colour, int spellIndex, SpellInputList slot)
    {
        if (equippedSpells[(int)slot] != null)
        {
            UnequipSpell(slot);
        }
        int c = (int)colour;
        if (allSpells[c][spellIndex] != null)
        {
            if (allSpells[c][spellIndex].isLocked != true)
            {
                Debug.Log("EquippingSpell: " + allSpells[c][spellIndex].name);
                equippedSpells[(int)slot] = allSpells[c][spellIndex].gameObject;
            }
        }
    }

    void UnequipSpell(SpellInputList slot)
    {
        if (equippedSpells[(int)slot] != null)
        {
            Debug.Log("Unequipped Spell: " + equippedSpells[(int)slot].name);
            equippedSpells[(int)slot] = null;
        }
    }

    void UnequipAllSpells()
    {
        Debug.Log("Unequipping all spells");
        for (int i = 0; i < equippedSpells.Length; i++)
        {
            if (equippedSpells[i] != null)
            {
                equippedSpells[i] = null;
            }
        }
    }

    public bool CanCastSpell(SpellInputList slot)
    {
        // If we have a spell equipped to the given slot
        if (equippedSpells[(int)slot] != null)
        {
            // If that spell is ready
            if (equippedSpells[(int)slot].GetComponent<Spell>().isReady)
            {
                return true;
            }
        }
        return false;
    }

    void Update()
    {
        // TODO: Watch a tutorial on cooldown systems.
        // Every frame, iterate over our 6 equipped spells
        for (int i = 0; i < equippedSpells.Length; i++)
        {
            // If we have an equipped spell there
            if (equippedSpells[i] != null)
            {
                // If that spell is not ready, aka on cooldown
                if (equippedSpells[i].GetComponent<Spell>().isReady == false)
                {
                    // Tick down the cooldown
                    equippedSpells[i].GetComponent<Spell>().cooldownTimer -= Time.deltaTime;
                    if (equippedSpells[i].GetComponent<Spell>().cooldownTimer <= 0.0f)
                    {
                        equippedSpells[i].GetComponent<Spell>().isReady = true;
                        equippedSpells[i].GetComponent<Spell>().cooldownTimer = equippedSpells[i].GetComponent<Spell>().baseCooldownTime;
                    }
                }
            }
        }
    }
}
