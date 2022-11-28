using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InfernoPortal : MonoBehaviour
{
    Action portalAction;
    Vector2 swTile;
    int size = 4;

    bool willEnterPortal = false;
    void Start()
    {
        portalAction = GetComponent<Action>();
        portalAction.menuTexts[0] = "Enter ";
        portalAction.menuPriorities[0] = 1;
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
        if (Tools.PlayerIsAdjacentToLargeObject(swTile, size, false) == false)
        {
            Player.player.trueTileScript.ExternalMovement(Tools.NearestTileToPlayer(swTile, size));
            return;
        }
        BeforeTick();
    }

    void BeforeTick()
    {
        if (willEnterPortal && Tools.PlayerIsAdjacentToLargeObject(swTile, size, false))
        {
            willEnterPortal = false;

            SceneManager.LoadScene("Zuk");
        }
    }

    void Cancel()
    {
        willEnterPortal = false;
    }
}
