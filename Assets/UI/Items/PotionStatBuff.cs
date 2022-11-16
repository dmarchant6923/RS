using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionStatBuff : MonoBehaviour
{
    public bool attack;
    public bool strength;
    public bool defense;
    public bool range;
    public bool mage;
    public bool prayer;
    public bool hitpoints;

    public bool divine;
    int divineTimer = 500;

    public bool boostOverBase = true;

    public float baseBoost;
    public float percentage;

    Potion potionScript;

    [HideInInspector] public Texture dose4;

    private void Start()
    {
        potionScript = GetComponent<Potion>();
        potionScript.potionDrank += Buff;
        dose4 = potionScript.dose4;
    }

    public int CalcBoost(int initialStat, int currentStat)
    {
        int boost = Mathf.FloorToInt(baseBoost + ((float)initialStat * percentage/100));
        bool reduce = false;
        if (Mathf.Sign(boost) == -1)
        {
            reduce = true;
        }
        if (reduce == false)
        {
            if (boostOverBase)
            {
                if (currentStat < initialStat + boost)
                {
                    currentStat += boost;
                    if (currentStat > initialStat + boost)
                    {
                        currentStat = initialStat + boost;
                    }
                }
            }
            else
            {
                if (currentStat < initialStat)
                {
                    currentStat += boost;
                    if (currentStat > initialStat)
                    {
                        currentStat = initialStat;
                    }
                }
            }
        }
        else
        {
            currentStat += boost;
        }

        return currentStat;
    }

    void Buff()
    {
        PlayerStats.PotionStatBoost(this);

        if (divine)
        {
            Player.player.AddToDamageQueue(10, 1, null);
        }
    }
}
