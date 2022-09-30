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

    Camera cam;
    public static bool mouseOnScreen;

    public GameObject mouseTile;
    GameObject newMouseTile;
    SpriteRenderer mouseTileSprite;

    public class Action
    {

    }
    [HideInInspector] public static List<string> actions = new List<string>();

    void Start()
    {
        cam = FindObjectOfType<Camera>();
        mouseOnScreen = false;

        newMouseTile = Instantiate(mouseTile, TileManager.mouseCoordinate, Quaternion.identity);
        mouseTileSprite = newMouseTile.GetComponent<SpriteRenderer>();
        mouseTileSprite.enabled = false;
    }

    private void Update()
    {
        Vector2 screenPoint = cam.ScreenToViewportPoint(Input.mousePosition);
        if (screenPoint.x < 0 || screenPoint.x > 1 || screenPoint.y < 0 || screenPoint.y > 1)
        {
            mouseOnScreen = false;
            mouseTileSprite.enabled = false;
            return;
        }
        mouseOnScreen = true;

        if (MouseOverGame.isOverGame)
        {
            if (Input.GetMouseButtonDown(0))
            {
                GameClientClick();
                StartCoroutine(GameServerClick());
            }
            mouseTileSprite.enabled = true;
        }

        else
        {
            mouseTileSprite.enabled = false;
        }
        newMouseTile.transform.position = TileManager.mouseCoordinate;
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
