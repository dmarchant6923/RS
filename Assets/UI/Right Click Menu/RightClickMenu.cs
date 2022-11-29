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

    public static Action UIAction;
    public static List<Action> tileActions = new List<Action>();
    public static List<Action> gameActions = new List<Action>();

    public static List<Action> actions = new List<Action>();
    public static List<Action> openActions = new List<Action>();

    List<string> menuStrings = new List<string>();

    public static bool isUsingItem = false;
    public static GameObject itemBeingUsed;
    public static string usingItemString;

    public static bool isCastingSpell = false;
    public static GameObject spellBeingCast;
    public static string castingSpellString;

    [HideInInspector] public Action leftClickAction;

    private void Start()
    {
        actions = new List<Action>();
    }

    private void Update()
    {
        actions = new List<Action>();
        foreach(Action action in tileActions)
        {
            actions.Add(action);
        }
        foreach (Action action in gameActions)
        {
            actions.Add(action);
        }
        if (UIAction != null)
        {
            actions.Add(UIAction);
        }

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
                if (menuString != null)
                {
                    menuStrings.Add(menuString);
                }
            }
        }

        leftClickAction = null;
        if (isUsingItem == false && isCastingSpell == false)
        {
            if (actions.Count == 0 || menuOpen)
            {
                actionText.text = "";
            }
            else if (menuOpen == false)
            {
                foreach (Action action in actions)
                {
                    for (int priority = 1; priority > -2; priority--)
                    {
                        for (int i = 0; i < action.menuTexts.Length; i++)
                        {
                            if (string.IsNullOrEmpty(action.menuTexts[i]) == false && action.menuPriorities[i] == priority)
                            {
                                leftClickAction = action;
                                actionText.text = action.menuTexts[i];
                                break;
                            }
                        }
                        if (leftClickAction != null)
                        {
                            break;
                        }
                    }
                }
            }
            if (menuStrings.Count > 1 && menuOpen == false)
            {
                actionText.text += " / " + (menuStrings.Count - 1) + " more options";
            }

            if (MouseManager.screenLeftClick && leftClickAction != null && menuOpen == false && leftClickAction.actionOnPointerUp == false)
            {
                leftClickAction.PickTopAction();
            }
        }
        else if (isUsingItem)
        {
            usingItemString = "Use <color=orange>" + itemBeingUsed.name + "</color> -> ";
            actionText.text = usingItemString;
            int actionCounter = 0;
            foreach (Action action in actions)
            {
                if (action.ignoreUse == false && action != itemBeingUsed.GetComponent<Action>())
                {
                    if (leftClickAction == null)
                    {
                        leftClickAction = action;
                        actionText.text += action.objectName;
                    }
                    else
                    {
                        actionCounter++;
                        actionText.text = "Use <color=orange>" + itemBeingUsed.name + "</color> -> " + action.objectName + " / " + actionCounter + " more options";
                    }
                }
            }

            if (MouseManager.screenLeftClick && menuOpen == false)
            {
                if (leftClickAction != null)
                {
                    leftClickAction.actionUsedOnThis = itemBeingUsed.GetComponent<Action>();
                    leftClickAction.PickAction(9);
                }
                else
                {
                    isUsingItem = false;
                }
            }
        }
        else if (isCastingSpell)
        {
            castingSpellString = "Cast <color=lime>" + spellBeingCast.name + "</color> -> ";
            actionText.text = castingSpellString;
            foreach (Action action in actions)
            {
                if (action.ignoreUse == false && action != spellBeingCast.GetComponent<Action>() && action.GetComponent<NPC>() != null)
                {
                    leftClickAction = action;
                    actionText.text += action.objectName;
                    break;
                }
            }

            if (MouseManager.screenLeftClick && menuOpen == false)
            {
                if (leftClickAction != null)
                {
                    spellBeingCast.GetComponent<Spell>().CastSpell(leftClickAction.gameObject);
                }
                isCastingSpell = false;
            }
        }
    }

    private void OnDestroy()
    {
        newMenu = null;
        menuOpen = false;

        UIAction = null;
        tileActions = new List<Action>();
        gameActions = new List<Action>();

        actions = new List<Action>();
        openActions = new List<Action>();

        isUsingItem = false;
        itemBeingUsed = null;
        usingItemString = null;

        isCastingSpell = false;
        spellBeingCast = null;
        castingSpellString = null;
    }
}
