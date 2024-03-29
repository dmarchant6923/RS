using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MouseManager : MonoBehaviour
{
    public static bool mouseOnScreen;
    public static Vector2 mouseCoordinate;
    public static bool isOverGame;

    public static bool screenLeftClick = false;

    public static bool showMouseTile;
    public GameObject newMouseTile;
    SpriteRenderer mouseTileSprite;
    Color spriteColor;

    public static MouseManager instance;

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        mouseOnScreen = false;

        DontDestroyOnLoad(newMouseTile);
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
        RightClickMenu.gameActions = gameActions;

        if (isOverGame)
        {
            RaycastHit2D[] castAll = Physics2D.CircleCastAll(worldPoint, 0.1f, Vector2.zero, 0);
            for (int i = -20; i <=0; i++)
            //for (int i = 0; i >= -20; i--)
            {
                foreach (RaycastHit2D cast in castAll)
                {
                    if (cast.collider.GetComponent<Action>() != null &&// gameActions.Contains(cast.collider.GetComponent<Action>()) == false && 
                        cast.collider.GetComponent<SpriteRenderer>() != null && cast.collider.GetComponent<SpriteRenderer>().sortingOrder == i)
                    {
                        //Debug.Log(cast.collider.GetComponent<SpriteRenderer>().sortingOrder + " " + cast.collider.gameObject + " " + i);
                        gameActions.Add(cast.collider.GetComponent<Action>());
                    }
                }
            }

            mouseCoordinate = TileManager.FindTile(new Vector2(worldPoint.x, worldPoint.y));
            mouseTileSprite.color = spriteColor;

        }
        else
        {
            mouseTileSprite.color = Vector4.zero;
        }

        newMouseTile.transform.position = mouseCoordinate;
    }
}
