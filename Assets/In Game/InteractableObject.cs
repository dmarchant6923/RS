using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [HideInInspector] public Action objectAction;

    Vector2 swTile;
    public int sizeX = 1;
    public int sizeY = 1;

    bool willInteract = false;

    public delegate void ObjectInteract();
    public event ObjectInteract interaction;

    private IEnumerator Start()
    {
        objectAction = GetComponent<Action>();
        objectAction.objectName = "<color=cyan>" + gameObject.name + "</color>";
        objectAction.serverAction0 += Interact;
        objectAction.orderLevels[0] = -1;
        objectAction.menuPriorities[0] = 1;
        objectAction.cancelLevels[0] = 1;
        objectAction.staticPlayerActions[0] = true;
        objectAction.inGame = true;
        //objectAction.UpdateName();

        Action.cancel1 += Cancel;

        Vector2 position = transform.position;
        Vector2 offset = new Vector2(((float)sizeX - 1) / 2, ((float)sizeY - 1) / 2);
        swTile = TileManager.FindTile(position - offset);
        position = swTile + offset;
        transform.position = position;

        TickManager.beforeTick += BeforeTick;

        yield return null;

        objectAction.UpdateName();
    }


    void Interact()
    {
        willInteract = true;
        if (Tools.PlayerIsAdjacentToLargeObject(swTile, sizeX, sizeY, false) == false)
        {
            Player.player.trueTileScript.ExternalMovement(Tools.NearestTileToPlayer(swTile, sizeX, sizeY));
            return;
        }
        BeforeTick();
    }
    void BeforeTick()
    {
        if (willInteract && Tools.PlayerIsAdjacentToLargeObject(swTile, sizeX, sizeY, false))
        {
            interaction?.Invoke();
            willInteract = false;
        }
    }



    void Cancel()
    {
        willInteract = false;
    }
}
