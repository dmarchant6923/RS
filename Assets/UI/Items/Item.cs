using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Item : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [HideInInspector] public Action itemAction;

    bool useActive = false;

    [HideInInspector] public RawImage itemImage;
    public Texture itemTexture;
    GameObject mask;
    Shadow shadow;
    Outline outline;
    public delegate void ItemClicked();
    public event ItemClicked darkenItem;
    public event ItemClicked undarkenItem;

    [HideInInspector] public bool clickHeld = false;

    [HideInInspector] public Inventory inventory;

    [System.NonSerialized] public string[] menuTexts = new string[9];

    bool dropped;

    [HideInInspector] public GameObject groundItemsParent;
    public GameObject groundPrefab;
    Player player;

    public static bool highlightSelectedItems;

    bool isEquipment = false;
    Equipment equipScript;

    bool isStackable = false;
    StackableItem stackScript;

    bool isChargeable = false;
    ChargeItem chargeScript;

    public static bool shiftClickToDrop;
    public GameObject groundItemTileObject;

    public float groundSizeFactor = 0.8f;

    private void Start()
    {
        itemAction = GetComponent<Action>();
        inventory = UIManager.staticInventory;
        groundItemsParent = UIManager.staticGroundItemsParent;

        itemImage = transform.GetChild(1).GetComponent<RawImage>();
        mask = transform.GetChild(0).gameObject;
        shadow = itemImage.GetComponent<Shadow>();
        outline = itemImage.GetComponent<Outline>();
        itemImage.texture = itemTexture;
        mask.GetComponent<RawImage>().texture = itemTexture;

        itemAction.objectName = "<color=orange>" + gameObject.name + "</color>";
        itemAction.addObjectName = true;

        menuTexts[2] = "Use ";
        itemAction.clientAction2 += Use;

        menuTexts[4] = "Drop ";
        itemAction.cancelLevels[4] = 1;
        itemAction.serverAction4 += Drop;
        itemAction.orderLevels[4] = -1;

        menuTexts[8] = "Examine ";

        UpdateActions();

        player = FindObjectOfType<Player>();

        if (GetComponent<Equipment>() != null)
        {
            isEquipment = true;
            equipScript = GetComponent<Equipment>();
        }
        if (GetComponent<StackableItem>() != null)
        {
            isStackable = true;
            stackScript = GetComponent<StackableItem>();
        }
        if (GetComponent<ChargeItem>() != null)
        {
            isChargeable = true;
            chargeScript = GetComponent<ChargeItem>();
        }
    }

    public void UpdateActions(string name)
    {
        itemAction.objectName = "<color=orange>" + name + "</color>";
        UpdateActions();
    }
    public void UpdateActions()
    {
        for (int i = 0; i < menuTexts.Length; i++)
        {
            itemAction.menuTexts[i] = menuTexts[i];
        }
        itemAction.UpdateName();
    }

    public void UpdateImage()
    {
        itemImage.texture = itemTexture;
    }

    void Use()
    {
        useActive = !useActive;
        if (useActive)
        {
            mask.SetActive(true);
            shadow.enabled = false;
            outline.enabled = false;

            RightClickMenu.isUsingItem = true;
            RightClickMenu.itemBeingUsed = gameObject;
        }
        else
        {
            mask.SetActive(false);
            shadow.enabled = true;
            outline.enabled = true;
        }
    }
    void Drop()
    {
        SpawnGroundItem(player.trueTile, true);
    }

    public void DestroyItem()
    {
        Destroy(gameObject);
    }

    public GameObject SpawnGroundItem(Vector2 tile, bool removeFromInventory)
    {
        GameObject newItem = Instantiate(groundPrefab, tile, Quaternion.identity);
        newItem.gameObject.name = gameObject.name;
        newItem.GetComponent<Action>().examineText = itemAction.examineText;
        GroundItem groundScript = newItem.GetComponent<GroundItem>();
        groundScript.itemTexture = itemTexture;
        groundScript.trueTile = tile;
        if (isEquipment)
        {
            groundScript.equipment = true;
        }
        if (isChargeable)
        {
            groundScript.chargeItem = true;
            groundScript.charges = chargeScript.charges;
        }
        if (isStackable)
        {
            StackableGroundItem groundStackScript = newItem.AddComponent<StackableGroundItem>();
            groundStackScript.quantity = stackScript.quantity;
            groundStackScript.textures = stackScript.textures;
            groundStackScript.thresholds = stackScript.thresholds;
        }
        if (GetComponent<Potion>() != null)
        {
            GetComponent<Potion>().DropPotion(groundScript);
        }
        groundScript.GetComponent<SpriteRenderer>().size = Vector2.one * groundSizeFactor;

        if (removeFromInventory)
        {
            Destroy(gameObject);
        }

        RaycastHit2D castAll = Physics2D.CircleCast(tile, 0.1f, Vector2.zero, 0, LayerMask.GetMask("Ground Items Tile"));
        GroundItemTile itemTileObject;
        if (castAll.collider == null)
        {
            itemTileObject = Instantiate(groundItemTileObject, tile, Quaternion.identity).GetComponent<GroundItemTile>();
        }
        else
        {
            itemTileObject = castAll.collider.GetComponent<GroundItemTile>();
        }

        itemTileObject.AddGroundItem(newItem.GetComponent<SpriteRenderer>());


        return newItem;
    }

    private void Update()
    {
        if (useActive && RightClickMenu.isUsingItem == false)
        {
            Use();
        }


        if (clickHeld && Input.GetMouseButton(0) == false)
        {
            clickHeld = false;
        }
        if (clickHeld && itemImage.color.a == 1)
        {
            Color color = itemImage.color;
            color.a = 0.6f;
            itemImage.color = color;
            darkenItem?.Invoke();
        }
        else if (clickHeld == false && itemImage.color.a != 1)
        {
            Color color = itemImage.color;
            color.a = 1;
            itemImage.color = color;
            undarkenItem?.Invoke();
        }

        if (shiftClickToDrop)
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                if (itemAction.menuPriorities[4] != 1)
                {
                    itemAction.menuPriorities[4] = 1;
                }
            }
            else if (itemAction.menuPriorities[4] != 0)
            {
                itemAction.menuPriorities[4] = 0;
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (useActive == false && RightClickMenu.isCastingSpell == false)
        {
            clickHeld = true;
            if (isEquipment)
            {
                equipScript.equippedWhenClicked = equipScript.isEquipped;
            }
        }
        if (Input.GetMouseButtonDown(0) && RightClickMenu.isUsingItem == false && RightClickMenu.isCastingSpell == false)
        {
            StartCoroutine(ItemDrag(eventData.position));
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {

    }

    IEnumerator ItemDrag(Vector3 clickedPosition)
    {
        bool isDragging = false;
        RectTransform itemRT = GetComponent<RectTransform>();
        Vector3 relativeDist = clickedPosition - itemRT.position;
        //Debug.Log(relativeDist);
        float maxDist = 15;
        float timer = 0;
        float window = 0.15f;

        while (Input.GetMouseButton(0) && gameObject.transform.parent.parent == inventory.inventoryParent.transform)
        {
            timer += Time.deltaTime;
            Vector3 mousePoint = Input.mousePosition;
            //Debug.Log(mousePoint + " " + itemRT.position + " " + relativeDist + " " + (mousePoint - (itemRT.position + relativeDist)).magnitude);
            if ((mousePoint - (itemRT.position + relativeDist)).magnitude > maxDist && timer > window)
            {
                isDragging = true;
                break;
            }
            yield return null;
        }
        if (isDragging == false)
        {
            itemAction.PickTopAction();
        }
        else
        {
            float insertRange = 27;
            bool failed = false;
            while (Input.GetMouseButton(0))
            {
                if (gameObject.transform.parent.parent != inventory.inventoryParent.transform)
                {
                    failed = true;
                    break;
                }
                Vector3 mousePoint = Input.mousePosition;
                itemRT.position = mousePoint - relativeDist;
                yield return null;
            }
            if (failed == false)
            {
                float minDist = 10000;
                foreach (GameObject slot in Inventory.inventorySlots)
                {
                    RectTransform rt = slot.GetComponent<RectTransform>();
                    minDist = Mathf.Min((Input.mousePosition - rt.position).magnitude, minDist);
                    if ((Input.mousePosition - rt.position).magnitude < insertRange)
                    {
                        Transform currentParent = transform.parent;
                        if (rt.GetComponentInChildren<Item>() != null)
                        {
                            rt.GetComponentInChildren<Item>().transform.SetParent(currentParent);
                        }
                        transform.SetParent(rt.transform);
                        break;
                    }
                }
                inventory.SortInventory();
            }
        }
        yield return new WaitForSeconds(0.1f);
        clickHeld = false;
    }

    private void OnDestroy()
    {
        if (inventory != null)
        {
            inventory.SortInventory();
        }
    }
}
