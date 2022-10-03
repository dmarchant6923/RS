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
    Color spriteColor;

    RightClickMenu rightClickMenu;
    string walkHereString = "Walk here";

    void Start()
    {
        cam = FindObjectOfType<Camera>();
        mouseOnScreen = false;

        newMouseTile = Instantiate(mouseTile, TileManager.mouseCoordinate, Quaternion.identity);
        mouseTileSprite = newMouseTile.GetComponent<SpriteRenderer>();
        spriteColor = mouseTileSprite.color;

        rightClickMenu = FindObjectOfType<RightClickMenu>();
    }

    private void Update()
    {
        Vector2 screenPoint = cam.ScreenToViewportPoint(Input.mousePosition);
        if (screenPoint.x < 0 || screenPoint.x > 1 || screenPoint.y < 0 || screenPoint.y > 1)
        {
            mouseOnScreen = false;
            mouseTileSprite.color = Vector4.zero;
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
            mouseTileSprite.color = spriteColor;

            if (rightClickMenu.menuStrings.Contains(walkHereString) == false)
            {
                rightClickMenu.menuStrings.Add(walkHereString);
            }
        }
        else
        {
            mouseTileSprite.color = Vector4.zero;
            if (rightClickMenu.menuStrings.Contains(walkHereString))
            {
                rightClickMenu.menuStrings.Remove(walkHereString);
            }
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
