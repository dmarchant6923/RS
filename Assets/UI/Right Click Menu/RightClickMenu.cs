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

    public static List<Action> actions = new List<Action>();
    public static List<Action> openActions = new List<Action>();

    List<string> menuStrings = new List<string>();

    private void Start()
    {
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
            openActions = actions;
        }

        menuStrings = new List<string>();
        foreach (Action action in actions)
        {
            foreach (string menuString in action.menuTexts)
            {
                menuStrings.Add(menuString);
            }
        }

        if (menuStrings.Count > 0)
        {
            actionText.text = menuStrings[0];
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
