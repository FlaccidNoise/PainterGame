using UnityEngine;
using System.Collections;

public class WeaponController : MonoBehaviour {

    public Transform brushHold;
    public Paintbrush startingPaintBrush;
    Paintbrush equippedBrush;

    void Start()
    {
        if (startingPaintBrush != null)
        {
            EquipBrush(startingPaintBrush);
        }
    }

    public void EquipBrush(Paintbrush brushToEquip)
    {
        if (equippedBrush != null)
        {
            Destroy(equippedBrush.gameObject);
        }

        equippedBrush = Instantiate(brushToEquip, brushHold.position, brushHold.rotation) as Paintbrush;
        equippedBrush.transform.parent = brushHold;
    }

	public void CastSpell(GameObject spell)
    {
        equippedBrush.CastSpell(spell);
    }

    public bool CanCastSpell()
    {
        // If we have a paintbrush equipped
        if (equippedBrush != null)
        {
            return true;
        }
        return false;
    }
}
