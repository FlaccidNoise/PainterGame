using UnityEngine;
using System.Collections;

public enum SpellInputList
{
    LeftMouseButton = 0,
    RightMouseButton,
    Q,
    E,
    R, 
    F
}

public class Player : Entity {

    private static int DEFAULT_COLOUR = 100;
    private static int DEFAULT_PAINT = 100;
    private static int DEFAULT_THANKS = 0;
    private static float DEFAULT_MOVE_SPEED = 7.67f;

    [SerializeField]
    private float _colour; // The lifeforce of everything
    [SerializeField]
    private float _paint; // Mana
    [SerializeField]
    private int _thanks; // Given to the player for his good deeds
    // TODO: OMG puzzle solving can be figuring out puzzles, that are thus good deeds, thus the puzzles give thanks (and special rewards)

    PlayerController _controller;
    WeaponController _weaponController;
    SpellManager _spellManager;
    Camera viewCamera;

    protected override void Start()
    {
        base.Start();
        ConstructPlayer();
    }

    void ConstructPlayer()
    {
        // Assign default values
        this._colour = DEFAULT_COLOUR;
        this._paint = DEFAULT_PAINT;
        this._thanks = DEFAULT_THANKS;

        // Get necessary components
        _controller = GetComponent<PlayerController>();
        _weaponController = GetComponent<WeaponController>();
        _spellManager = GetComponent<SpellManager>();
        viewCamera = Camera.main;
    }

    void Update()
    {
        // Inputs
        this._MovementInput();
        this._LookInput();
        this._AttackInput();
    }

    void _MovementInput()
    {
        // Movement Input
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 moveVelocity = moveInput.normalized * DEFAULT_MOVE_SPEED;
        _controller.Move(moveVelocity);
    }

    void _LookInput()
    {
        // Look Input
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayDistance;

        if (groundPlane.Raycast(ray, out rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);
            _controller.LookAt(point);
        }
    }

    void _AttackInput()
    {
        // TODO: Change to http://gameprogrammingpatterns.com/command.html
        // Weapon Input
        if (Input.GetMouseButtonDown(0))
        {
            CastSpell(SpellInputList.LeftMouseButton);
        }
        if (Input.GetMouseButtonDown(1))
        {
            CastSpell(SpellInputList.RightMouseButton);
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            CastSpell(SpellInputList.Q);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            CastSpell(SpellInputList.E);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            CastSpell(SpellInputList.F);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            CastSpell(SpellInputList.R);  
        }
    }

    public override void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        Debug.Log("Player was hit");
        TakeDamage(damage);
    }
    public override void TakeDamage(float damage)
    {
        _controller.Knockback(damage * 28);
       if (_paint > 0.0f)
        {
            if (damage > _paint)
            {
                float remainingDamage = TakePaintDamageWithRemainder(damage);
                TakeColourDamage(remainingDamage);
            } else
            {
                TakePaintDamage(damage);
            }
        } else
        {
            TakeColourDamage(damage);
        }
    }

    void TakeColourDamage(float damage)
    {
        if (damage >= _colour)
        {
            Debug.Log("GAME OVER MAN");
            _colour = 0.0f;
        } else
        {
            _colour -= damage;
        }
    }

    float TakePaintDamageWithRemainder(float damage)
    {
        float remainder = damage - _paint;
        TakePaintDamage(damage);
        return remainder;
    }

    void TakePaintDamage(float damage)
    {
        if (damage >= _paint)
        {
            _paint = 0.0f;
        } else
        {
            _paint -= damage;
        }
    }
    
    void CastSpell(SpellInputList slot)
    {
        // If we can cast a spell equipped in the slot
        if (_weaponController.CanCastSpell() && _spellManager.CanCastSpell(slot))
        {
            float paintCost = _spellManager.equippedSpells[(int)slot].GetComponent<Spell>().paintCost;
            if (_paint >= paintCost)
            {
                TakePaintDamage(_spellManager.equippedSpells[(int)slot].GetComponent<Spell>().paintCost);
                _weaponController.CastSpell(_spellManager.equippedSpells[(int)slot]);
            } else
            {
                // TODO: Indicate that the player cannot cast a spell
                Debug.Log("NO PAINT! CAN'T CAST SPELL");
            }
        }
    }

    public void AddColour(float amount)
    {
        // If we have some colour to be healed, always prioritize
        if (_colour < DEFAULT_COLOUR)
        {
            // If we are trying to add to much colour
            if (_colour + amount > DEFAULT_COLOUR)
            {
                // Set it to max
                _colour = DEFAULT_COLOUR;
            }
            else
            {
                _colour += amount;
            }
        } else if (_colour >= DEFAULT_COLOUR)
        {
            AddPaint(amount);
        }
    }

    void AddPaint(float amount)
    {
        // If we have some paint to be healed
        if (_paint < DEFAULT_PAINT)
        {
            // If we are trying to add too much paint
            if (_paint + amount > DEFAULT_PAINT)
            {
                // Set it to max
                _paint = DEFAULT_PAINT;
            }
            else
            {
                _paint += amount;
            }
        }
    }

    public void AddThanks(int amount)
    {
        _thanks += amount;
    }

    // Getters and Setters
    public float getColour()
    {
        return this._colour;
    }
    public void setColour(float colour)
    {
        this._colour = colour;
    }

    public float getPaint()
    {
        return this._paint;
    }
    public void setPaint(float paint)
    {
        this._paint = paint;
    }

    public int getThanks()
    {
        return this._thanks;
    }
    public void setThanks(int thanks)
    {
        this._colour = thanks;
    }
}
