using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialEffects : MonoBehaviour
{
    public bool twistedBow;
    public bool crystalBow;
    public bool rerollAttMult;
    public bool rerollDefMult;
    public bool TumekensShadow;

    public float GearBonusMult()
    {
        float mult = 1;
        return mult;
    }
    public float AttackRollMult()
    {
        float mult = 1;

        if (twistedBow)
        {
            float magicLvl = Mathf.Max(Player.targetedNPC.GetComponent<Enemy>().magic, Player.targetedNPC.GetComponent<Enemy>().attackMagic);
            magicLvl = Mathf.Clamp(magicLvl, 0, 250);
            mult = 140 + (10 * 3 * magicLvl / 10 - 10) / 100 - Mathf.Pow(3 * magicLvl / 10 - 100, 2) / 100;
            mult = Mathf.Clamp(mult, 0, 140);
            mult /= 100;
        }

        return mult;
    }
    public float MaxHitMult()
    {
        float mult = 1;

        if (twistedBow)
        {
            float magicLvl = Mathf.Max(Player.targetedNPC.GetComponent<Enemy>().magic, Player.targetedNPC.GetComponent<Enemy>().attackMagic);
            magicLvl = Mathf.Clamp(magicLvl, 0, 250);
            mult = 250 + ( 10 * 3 * magicLvl / 10 - 14) / 100 - Mathf.Pow(3 * magicLvl / 10 - 140, 2) / 100;
            mult = Mathf.Clamp(mult, 0, 250);
            mult /= 100;
        }

        return mult;
    }
}
