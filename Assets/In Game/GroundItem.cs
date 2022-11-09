using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GroundItem : MonoBehaviour
{
    public GameObject item;
    public Vector2 trueTile;

    Action itemAction;
    Inventory inventory;
    SpriteRenderer sprite;
    Player playerScript;

    bool willTake = false;

    public static GameObject itemToTake;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        sprite.color = new Color(1, 1, 1, 1);
        Texture2D tex = (Texture2D) item.transform.GetChild(0).GetComponent<RawImage>().texture;
        Sprite newSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        sprite.sprite = newSprite;

        if (item.GetComponent<ChargeItem>() != null && item.GetComponent<ChargeItem>().charges <= 0)
        {
            sprite.color = new Color(0.7f, 0.7f, 0.7f, 1);
        }

        itemAction = GetComponent<Action>();
        itemAction.addObjectName = true;
        itemAction.objectName = "<color=orange>" + gameObject.name + "</color>";

        itemAction.menuTexts[0] = "Take ";
        itemAction.menuPriorities[0] = 1;
        itemAction.clientAction0 += TrueTileIndicator;
        itemAction.serverAction0 += Take;
        itemAction.orderLevels[0] = -1;
        itemAction.UpdateName();

        TickManager.beforeTick += BeforeTick;
        TickManager.afterTick += RenderSprite;
        Action.cancel1 += CancelTake;

        itemAction.examineText = item.GetComponent<Action>().examineText;

        inventory = FindObjectOfType<Inventory>();
        playerScript = FindObjectOfType<Player>();

        RaycastHit2D[] castAll = Physics2D.CircleCastAll(trueTile, 0.1f, Vector2.zero, 0);
        foreach (RaycastHit2D cast in castAll)
        {
            if (cast.collider.GetComponent<GroundItem>() != null)
            {
                sprite.sortingOrder--;
                if (sprite.sortingOrder < -2)
                {
                    sprite.enabled = false;
                }
            }
        }
    }

    void TrueTileIndicator()
    {
        playerScript.trueTileScript.ClientClick(trueTile);
    }

    void Take()
    {
        itemToTake = gameObject;
        willTake = true;

        if (playerScript.trueTile != trueTile)
        {
            playerScript.trueTileScript.ExternalMovement(trueTile);
        }
    }

    void BeforeTick()
    {
        if (willTake && itemToTake == gameObject && trueTile == playerScript.trueTile)
        {
            willTake = false;

            item.SetActive(true);
            inventory.PlaceInInventory(item);
            Destroy(gameObject);
        }
    }

    void CancelTake()
    {
        willTake = false;
        itemToTake = null;
    }

    void RenderSprite()
    {
        if (sprite.enabled == false)
        {
            RaycastHit2D[] castAll = Physics2D.CircleCastAll(trueTile, 0.1f, Vector2.zero, 0);
            if (castAll.Length < 4)
            {
                sprite.enabled = true;
            }
        }
    }

    private void OnDestroy()
    {
        Action.cancel1 -= CancelTake;
        TickManager.beforeTick -= BeforeTick;
        TickManager.afterTick -= RenderSprite;
    }
}
