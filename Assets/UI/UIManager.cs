using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    GraphicRaycaster raycaster;
    PointerEventData eventData;
    EventSystem eventSystem;
    public static Canvas canvas;

    Action UIAction;

    public GameObject panel;
    public RectTransform statPanel;
    float panelOpenPos;

    public GameObject xClick;
    public static GameObject staticXClick;
    public static GameObject newXClick;

    public GameObject hitSplat;
    public static GameObject staticHitSplat;

    public GameObject overhead;
    public static GameObject staticOverhead;
    public static GameObject newOverhead;

    public GameObject healthBar;
    public static GameObject staticHealthBar;

    public GameObject groundItemsParent;
    public static GameObject staticGroundItemsParent;

    public Inventory inventory;
    public static Inventory staticInventory;

    public Image fadeBox;
    public RawImage frontBlackScreen;
    public static UIManager instance;

    public GameObject infernoUI;
    public GameObject entryInfoBox;

    public List<GameObject> bannedItems = new List<GameObject>();

    float screenWidth = 0;

    private IEnumerator Start()
    {
        DontDestroyOnLoad(gameObject);

        instance = this;

        canvas = FindObjectOfType<Canvas>();
        raycaster = FindObjectOfType<GraphicRaycaster>();
        eventSystem = FindObjectOfType<EventSystem>();

        panelOpenPos = statPanel.position.x;
        staticXClick = xClick;
        staticHitSplat = hitSplat;
        staticOverhead = overhead;
        staticHealthBar = healthBar;
        staticGroundItemsParent = groundItemsParent;
        staticInventory = inventory;

        yield return new WaitForSeconds(0.1f);

        infernoUI.SetActive(false);
    }

    void Update()
    {
        Vector2 screenPoint = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        RaycastHit2D[] mouseCast = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10)), Vector2.zero, 100);

        eventData = new PointerEventData(eventSystem);
        eventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(eventData, results);

        UIAction = null;
        foreach (RaycastResult result in results)
        {
            GameObject item = result.gameObject;
            while (item.GetComponent<Action>() == null)
            {
                if (item.transform.parent == null || item.transform.parent.tag == "Action Parent")
                {
                    break;
                }
                item = item.transform.parent.gameObject;
            }
            if (item.GetComponent<Action>() != null)
            {
                UIAction = item.GetComponent<Action>();
                break;
            }
        }

        RightClickMenu.UIAction = UIAction;

        if (Screen.width != screenWidth)
        {
            ResetCanvasScale();
            screenWidth = Screen.width;
        }
    }

    public void ResetCanvasScale()
    {
        //canvas.scaleFactor = Mathf.Clamp(100 * canvas.pixelRect.width / 1920, 50, 100) * OptionManager.uiScaleFactor / 100;
        canvas.scaleFactor = canvas.pixelRect.width / 1920 * OptionManager.uiScaleFactor;
    }

    public static void ClickX(bool redClick)
    {
        if (newXClick != null)
        {
            Destroy(newXClick);
        }
        //newXClick = Instantiate(staticXClick, Camera.main.ScreenToViewportPoint(Input.mousePosition), Quaternion.identity);
        newXClick = Instantiate(staticXClick, Input.mousePosition, Quaternion.identity);
        newXClick.GetComponent<ClickX>().redClick = redClick;
    }

    public bool CheckWarning()
    {
        foreach (GameObject slot in Inventory.inventorySlots)
        {
            Item item = slot.GetComponentInChildren<Item>();
            if (item == null)
            {
                continue;
            }

            foreach (GameObject bannedItem in bannedItems)
            {
                string name = item.name.ToLower();
                if (item.GetComponent<Potion>() != null)
                {
                    name = item.GetComponent<Potion>().initialName.ToLower();
                }

                if (name == bannedItem.name.ToLower())
                {
                    return true;
                }
            }
        }

        foreach (Transform slot in WornEquipment.slots)
        {
            Equipment item = slot.GetComponentInChildren<Equipment>();
            if (item == null)
            {
                continue;
            }

            foreach (GameObject bannedItem in bannedItems)
            {
                string name = item.name.ToLower();
                if (item.GetComponent<Potion>() != null)
                {
                    name = item.GetComponent<Potion>().initialName.ToLower();
                }

                if (name == bannedItem.name.ToLower())
                {
                    return true;
                }
            }
        }

        return false;
    }
}
