using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImbuedHeart : MonoBehaviour
{
    Action heartAction;
    Item itemScript;

    private IEnumerator Start()
    {
        heartAction = GetComponent<Action>();
        itemScript = GetComponent<Item>();

        yield return null;

        itemScript.menuTexts[0] = "Invigorate ";
        heartAction.orderLevels[0] = -1;
        heartAction.cancelLevels[1] = 1;
        heartAction.serverAction0 += Invigorate;
        itemScript.UpdateActions();
    }

    void Invigorate()
    {
        if (PlayerStats.imbuedHeartCharged == false)
        {
            int minutes = Mathf.CeilToInt((float)PlayerStats.imbuedHeartTicks / 100);
            GameLog.Log("The heart is still drained of its power. Judging by how it feels, it will be ready in around " + minutes + " minutes.");
            return;
        }
        int boost = 1 + Mathf.FloorToInt((float)PlayerStats.initialMagic / 10);
        int newLevel = Mathf.Min(PlayerStats.currentMagic + boost, PlayerStats.initialMagic + boost);
        PlayerStats.currentMagic = newLevel;
        PlayerStats.InvigorateImbuedHeart();
        string endText = "<color=red>Your imbued heart has regained its magical power.</color>";
        BuffBar.instance.CreateExtraTimer(itemScript.itemTexture, 420, gameObject.name, "", endText);
    }
}
