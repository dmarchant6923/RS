using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RightClickMenu : MonoBehaviour
{
    public GameObject menu;
    public static GameObject newMenu;

    public static bool menuOpen = false;

    public Text actionText;

    public static List<string> menuStrings = new List<string>();
    public static List<string> openMenuStrings = new List<string>();

    public static List<GameObject> menuObjects = new List<GameObject>();
    public static List<GameObject> openMenuObjects = new List<GameObject>();

    public static List<Action> actions = new List<Action>();
    public static List<Action> openActions = new List<Action>();

    private void Start()
    {
        menuStrings = new List<string>();

        actions = new List<Action>();
    }

    public static void AddItem(string String, GameObject gameObject)
    {
        
    }

    private void Update()
    {
        if (MouseManager.mouseOnScreen && Input.GetMouseButtonDown(1) && menuOpen == false)
        {
            newMenu = Instantiate(menu, Input.mousePosition, Quaternion.identity);
            newMenu.transform.SetParent(transform);
            openMenuStrings = menuStrings;
            openMenuObjects = menuObjects;
        }

        if (menuStrings.Count > 0)
        {
            actionText.text = menuStrings[menuStrings.Count - 1];
        }
        if (menuStrings.Count > 1)
        {
            actionText.text += " / " + (menuStrings.Count - 1) + " more options";
        }
        if (menuStrings.Count == 0)
        {
            actionText.text = "";
        }
    }

    
}
