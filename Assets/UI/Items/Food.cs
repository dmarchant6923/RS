using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    public int heal = 3;
    public bool karambwan = false;

    Action foodAction;
    Item itemScript;

    IEnumerator Start()
    {
        foodAction = GetComponent<Action>();
        itemScript = GetComponent<Item>();

        yield return null;

        itemScript.menuTexts[0] = "Eat ";
        foodAction.orderLevels[0] = -1;
        foodAction.serverAction0 += Eat;
        foodAction.cancelLevels[0] = 1;
        itemScript.UpdateActions();
    }

    void Eat()
    {
        if (karambwan && PlayerStats.karambwanDelayLol > 0)
        {
            return;
        }
        if (karambwan == false && PlayerStats.foodDelay > 0)
        {
            return;
        }

        PlayerStats.PlayerHeal(heal);

        if (karambwan == false)
        {
            PlayerStats.foodDelay = 3;
            if (Player.player.combatScript.attackCooldown > Combat.minCoolDown)
            {
                Player.player.combatScript.attackCooldown += 3;
            }
            Destroy(gameObject);
            return;
        }


        PlayerStats.karambwanDelayLol = 3;
        PlayerStats.foodDelay = 3;
        PlayerStats.drinkDelay = 3;
        if (Player.player.combatScript.attackCooldown > Combat.minCoolDown)
        {
            Player.player.combatScript.attackCooldown += 2;
        }
        Destroy(gameObject);
    }
}
