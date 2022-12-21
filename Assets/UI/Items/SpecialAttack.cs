using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialAttack : MonoBehaviour
{
    public int specCost;
    public string specDescription;
    public Sprite customProjectile;
    public Color customProjectileColor;

    public bool blowPipe;
    public bool armadylCrossbow;
    public bool zaryteCrossbow;
    public bool eldritchStaff;

    [System.NonSerialized] public bool guaranteeHit = false;
    [System.NonSerialized] public bool slowProjectile = false;
    [System.NonSerialized] public bool leech = false;
    [System.NonSerialized] public float leechPercent;

    private void Start()
    {
        if (blowPipe)
        {
            slowProjectile = true;
            leech = true;
            leechPercent = 0.5f;
        }
    }

    public float AccuracyRollMult()
    {
        if (blowPipe || armadylCrossbow || zaryteCrossbow)
        {
            return 2;
        }

        return 1;
    }
    public float MaxHitMult()
    {
        if (blowPipe)
        {
            return 1.5f;
        }

        return 1;
    }
    public int Leech(int damage)
    {
        if (blowPipe)
        {
            return Mathf.FloorToInt((float)damage * 0.5f);
        }

        return 0;
    }
    public float EnchantedBoltChance(float chance, bool success)
    {
        if (armadylCrossbow)
        {
            return chance * 2;
        }
        else if (zaryteCrossbow && success)
        {
            return 1;
        }

        return chance;
    }
}
