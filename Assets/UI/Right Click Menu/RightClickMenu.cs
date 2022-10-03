using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RightClickMenu : MonoBehaviour
{
    public GameObject menu;
    GameObject newMenu;

    [HideInInspector] public bool menuOpen = false;

    public Text actionText;

    public List<string> menuStrings = new List<string>();

    private void Start()
    {
        menuStrings = new List<string>();
        Debug.Log(actionText);
    }

    private void Update()
    {
        if (MouseManager.mouseOnScreen && Input.GetMouseButtonDown(1) && menuOpen == false)
        {
            menuOpen = true;
            newMenu = Instantiate(menu, Input.mousePosition, Quaternion.identity);
            newMenu.transform.parent = transform;
            newMenu.GetComponent<MenuScript>().parentScript = this;
            newMenu.GetComponent<MenuScript>().menuStrings = menuStrings;
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
