using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : MonoBehaviour
{
    public class AOE
    {
        public Vector2 position;
        public int damage;
        public int delay;
    }

    List<AOE> activeAttacks = new List<AOE>();

    private void Start()
    {
        TickManager.afterTick += AfterTick;
    }

    void AfterTick()
    {
        for (int i = 0; i < activeAttacks.Count; i++)
        {
            activeAttacks[i].delay--;
            if (activeAttacks[i].delay == 0)
            {
                if (Player.player.trueTile == activeAttacks[i].position)
                {
                    Player.player.AddToDamageQueue(activeAttacks[i].damage, 0, null);
                }

                activeAttacks.RemoveAt(i);
                i--;
            }
        }
    }
}
