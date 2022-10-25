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
    public static bool isOverGame;

    public static bool screenLeftClick = false;

    public static bool showMouseTile;
    public GameObject mouseTile;
    GameObject newMouseTile;
    SpriteRenderer mouseTileSprite;
    Color spriteColor;

    void Start()
    {
        mouseOnScreen = false;

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

        if (screenPoint.x < 0 || screenPoint.x > 1 || screenPoint.y < 0 || screenPoint.y > 1)
        {
            mouseOnScreen = false;
            mouseTileSprite.color = Vector4.zero;
            return;
        }
        mouseOnScreen = true;
        if (screenLeftClick)
        {
            screenLeftClick = false;
        }
        if (Input.GetMouseButtonDown(0))
        {
            screenLeftClick = true;
        }

        List<Action> gameActions = new List<Action>();
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D[] castAll = Physics2D.CircleCastAll(worldPoint, 0.1f, Vector2.zero, 0);
        for (int i = 0; i >= -20; i--) 
        {
            foreach (RaycastHit2D cast in castAll)
            {
                if (cast.collider.GetComponent<Action>() != null && gameActions.Contains(cast.collider.GetComponent<Action>()) == false && cast.collider.GetComponent<SpriteRenderer>().sortingOrder == i)
                {
                    //Debug.Log(cast.collider.GetComponent<SpriteRenderer>().sortingOrder + " " + cast.collider.gameObject + " " + i);
                    gameActions.Add(cast.collider.GetComponent<Action>());
                }
            }
        }
        RightClickMenu.gameActions = gameActions;

        if (isOverGame)
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
