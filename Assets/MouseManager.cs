using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MouseManager : MonoBehaviour
{
    public delegate void MouseAction();
    public static event MouseAction IGClientClick;
    public static event MouseAction IGServerClick;

    public static bool mouseOnScreen;
    public static Vector2 mouseCoordinate;
    GraphicRaycaster raycaster;
    PointerEventData eventData;
    EventSystem eventSystem;

    public static bool showMouseTile;
    public GameObject mouseTile;
    GameObject newMouseTile;
    SpriteRenderer mouseTileSprite;
    Color spriteColor;

    void Start()
    {
        mouseOnScreen = false;

        raycaster = FindObjectOfType<GraphicRaycaster>();
        eventSystem = FindObjectOfType<EventSystem>();

        newMouseTile = Instantiate(mouseTile, mouseCoordinate, Quaternion.identity);
        mouseTileSprite = newMouseTile.GetComponent<SpriteRenderer>();
        if (showMouseTile == false)
        {
            mouseTileSprite.enabled = false;
        }
        spriteColor = mouseTileSprite.color;
    }

    private void Update()
    {
        Vector2 screenPoint = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        RaycastHit2D[] mouseCast = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10)), Vector2.zero, 100);

        eventData = new PointerEventData(eventSystem);
        eventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(eventData, results);
        if (Input.GetMouseButtonDown(0))
        {
            foreach (RaycastResult result in results)
            {
                GameObject item = result.gameObject;
                while (item.GetComponent<Action>() == null)
                {
                    if (item.transform.parent == null || item.transform.parent.GetComponent<Canvas>() != null)
                    {
                        break;
                    }
                    item = item.transform.parent.gameObject;
                }
                if (item.GetComponent<Action>() != null)
                {
                    foreach (Action action in item.GetComponents<Action>())
                    {
                        Debug.Log(action.menuText);
                    }
                }
            }
        }
        if (screenPoint.x < 0 || screenPoint.x > 1 || screenPoint.y < 0 || screenPoint.y > 1)
        {
            mouseOnScreen = false;
            mouseTileSprite.color = Vector4.zero;
            return;
        }
        mouseOnScreen = true;

        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (MouseOverGame.isOverGame)
        {
            mouseCoordinate = TileManager.FindTile(new Vector2(worldPoint.x, worldPoint.y));
            if (Input.GetMouseButtonDown(0))
            {
                GameClientClick();
                StartCoroutine(GameServerClick());
            }
            mouseTileSprite.color = spriteColor;

        }
        else
        {
            mouseTileSprite.color = Vector4.zero;
        }
        newMouseTile.transform.position = mouseCoordinate;
    }

    void GameClientClick()
    {
        IGClientClick?.Invoke();
    }

    IEnumerator GameServerClick()
    {
        yield return new WaitForSeconds(TickManager.simLatency);
        IGServerClick?.Invoke();
    }
}
