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
    public int runEnergy;
    public bool stamina;

    public bool divine;

    public bool boostOverBase = true;

    public float baseBoost;
    public float percentage;

    [HideInInspector] public Potion potionScript;

    private void Start()
    {
        potionScript = GetComponent<Potion>();
        potionScript.potionDrank += Buff;
        potionScript.divine = divine;
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
            if (currentStat < 1)
            {
                currentStat = 1;
            }
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
        if (runEnergy > 0)
        {
            Player.player.runEnergy += (float)runEnergy * 100;
        }
        if (stamina)
        {
            RunToggle.instance.Stamina(true);
            Player.player.stamina = true;
            Player.player.staminaTicks = 100; //200

            Texture texture = potionScript.dose4;
            string name = gameObject.name;

            BuffBar.instance.CreatePotionTimer(texture, (float)Player.player.staminaTicks * TickManager.maxTickTime, name);
        }
    }
}
