using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InfernoPortal : MonoBehaviour
{
    Action portalAction;
    Vector2 swTile;
    int size = 4;
    int fadeTime = 3;
    int fadeTicks;

    bool willEnterPortal = false;
    void Start()
    {
        portalAction = GetComponent<Action>();
        portalAction.menuTexts[0] = "Enter ";
        portalAction.menuPriorities[0] = 1;
        portalAction.cancelLevels[0] = 1;
        portalAction.staticPlayerActions[0] = true;
        portalAction.objectName = "<color=cyan>" + gameObject.name + "</color>";
        portalAction.UpdateName();
        portalAction.serverAction0 += EnterPortal;

        Vector2 position = transform.position;
        swTile = TileManager.FindTile(position - Vector2.one * 1.5f);

        TickManager.beforeTick += BeforeTick;
        Action.cancel1 += Cancel;
    }

    void EnterPortal()
    {
        willEnterPortal = true;
        if (Tools.PlayerIsAdjacentToLargeObject(swTile, size, size, false) == false)
        {
            Player.player.trueTileScript.ExternalMovement(Tools.NearestTileToPlayer(swTile, size, size));
            return;
        }
        BeforeTick();
    }

    void BeforeTick()
    {
        if (fadeTicks > 0)
        {
            fadeTicks--;
        }

        if (willEnterPortal && Tools.PlayerIsAdjacentToLargeObject(swTile, size, size, false))
        {
            willEnterPortal = false;
            StartCoroutine(PortalFade());
        }
    }

    IEnumerator PortalFade()
    {
        Action.ignoreAllActions = true;
        fadeTicks = fadeTime;
        while (fadeTicks > 0)
        {
            Color color = UIManager.instance.fadeBox.color;
            color.a += Time.deltaTime;
            UIManager.instance.fadeBox.color = color;
            yield return null;
        }
        UIManager.instance.fadeBox.color = Color.black;

        Player.player.runEnergy = 10000;
        OptionManager.ignoreHiscores = false;
        if (SettingsPanel.instance.settings.bools[6] || SettingsPanel.instance.settings.bools[7] || SettingsPanel.instance.settings.bools[8] || UIManager.instance.CheckWarning())
        {
            OptionManager.ignoreHiscores = true;
        }

        SceneManager.LoadScene("Zuk");
    }

    void Cancel()
    {
        willEnterPortal = false;
    }

    private void OnDestroy()
    {
        Action.cancel1 -= Cancel;
    }
}
