using UnityEngine;
using System.Collections;
using System;

public abstract class Spell : MonoBehaviour, ISpell
{
    [SerializeField]
    protected string _name = "DefaultSpell";
    [SerializeField]
    protected ColourList _colour = ColourList.RED;
    [SerializeField]
    protected int _spellIndex = 0;
    [SerializeField]
    protected string _description = "DEFAULT_SPELL_DESCRIPTION";

    [SerializeField]
    protected GameObject _effect;

    [SerializeField]
    protected float _baseCooldownTime = 2.0f;
    [SerializeField]
    protected float _cooldownTimer = 0.0f;
    [SerializeField]
    protected float _paintCost = 2.0f;
    [SerializeField]
    protected bool _isReady = true;
    [SerializeField]
    protected bool _isLocked = true;
    [SerializeField]
    protected bool _isEquipped = false;

    public ColourList colour
    {
        get
        {
            return _colour;
        }

        set
        {
            _colour = value;
        }
    }

    public int spellIndex
    {
        get
        {
            return _spellIndex;
        }

        set
        {
            _spellIndex = value;
        }
    }

    public string description
    {
        get
        {
            return _description;
        }

        set
        {
            _description = value;
        }
    }

    public GameObject effect
    {
        get
        {
            return _effect;
        }

        set
        {
            _effect = value;
        }
    }

    public float baseCooldownTime
    {
        get
        {
            return _baseCooldownTime;
        }

        set
        {
            _baseCooldownTime = value;
        }
    }


    public float cooldownTimer
    {
        get
        {
            return _cooldownTimer;
        }
        set
        {
            _cooldownTimer = value;
        }
    }

    public bool isReady
    {
        get
        {
            return _isReady;
        }
        set
        {
            _isReady = value;
        }
    }

    public bool isLocked
    {
        get
        {
            return _isLocked;
        }

        set
        {
            _isLocked = value;
        }
    }

    public bool isEquipped
    {
        get
        {
            return _isEquipped;
        }
        set
        {
            _isEquipped = value;
        }
    }

    public float paintCost
    {
        get
        {
            return _paintCost;
        }
    }

    virtual public void Cast()
    {
        Debug.Log("CAST DEFAULT SPELL");
    }
}
