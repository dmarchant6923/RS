using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action : MonoBehaviour
{
    [System.NonSerialized] public string[] menuTexts = new string[9];
    [System.NonSerialized] public int[] menuPriorities = new int[9]; // priority -1, 0, or 1
    [System.NonSerialized] public int[] cancelLevel = new int[10]; //cancel level 0, 1, 2, or 3. level cancels that level and all below it.
    public string examineText;
    [HideInInspector] public string objectName;
    public bool addObjectName;

    [HideInInspector] public bool itemAction = false;
    public bool ignoreUse = false;

    public delegate void MenuAction();
    public event MenuAction action0;
    public event MenuAction action1;
    public event MenuAction action2;
    public event MenuAction action3;
    public event MenuAction action4;
    public event MenuAction action5;
    public event MenuAction action6;
    public event MenuAction action7;
    public event MenuAction actionExamine;
    public event MenuAction actionUse;

    public static event MenuAction cancel1;
    public static event MenuAction cancel2;
    public static event MenuAction cancel3;

    bool invokeCancel1;
    bool invokeCancel2;
    bool invokeCancel3;

    [HideInInspector] public Action actionUsedOnThis;

    public bool actionOnPointerUp;

    bool validAction = false;

    private void Start()
    {
        menuTexts = new string[9];
        if (examineText != null && string.IsNullOrEmpty(examineText) == false)
        {
            menuTexts[8] = "Examine ";
        }

        menuPriorities[8] = -1;

        TickManager.cancelBeforeTick += CancelActions;
    }

    public void UpdateName()
    {
        if (addObjectName)
        {
            for (int i = 0; i < menuTexts.Length; i++)
            {
                if (string.IsNullOrEmpty(menuTexts[i]) == false)
                {
                    menuTexts[i] += objectName;
                }
            }
        }
    }

    public void PickAction(int actionNumber)
    {
        //Debug.Log(gameObject.name + " action number: " + actionNumber + " cancel level: " + cancelLevel[actionNumber]);
        if (actionNumber == 0)
        {
            Action0();
        }
        else if (actionNumber == 1)
        {
            Action1();
        }
        else if (actionNumber == 2)
        {
            Action2();
        }
        else if (actionNumber == 3)
        {
            Action3();
        }
        else if (actionNumber == 4)
        {
            Action4();
        }
        else if (actionNumber == 5)
        {
            Action5();
        }
        else if (actionNumber == 6)
        {
            Action6();
        }
        else if (actionNumber == 7)
        {
            Action7();
        }
        else if (actionNumber == 8)
        {
            ActionExamine();
        }
        else if (actionNumber == 9)
        {
            UseActionOnThis();
        }
        else
        {
            Debug.Log("no action selected");
        }

        if (validAction == false)
        {
            return;
        }
        validAction = false;

        if (cancelLevel[actionNumber] == 1)
        {
            StartCoroutine(DelayCancel(1));
        }
        else if (cancelLevel[actionNumber] == 2)
        {
            StartCoroutine(DelayCancel(2));
        }
        else if (cancelLevel[actionNumber] == 3)
        {
            StartCoroutine(DelayCancel(3));
        }
    }

    IEnumerator DelayCancel(int num)
    {
        yield return new WaitForSeconds(TickManager.simLatency);
        if (num == 1)
        {
            invokeCancel1 = true;
        }
        else if (num == 2)
        {
            invokeCancel1 = true;
            invokeCancel2 = true;
        }
        else if (num == 3)
        {
            invokeCancel1 = true;
            invokeCancel2 = true;
            invokeCancel3 = true;
        }
    }

    public void CancelActions()
    {
        if (invokeCancel1)
        {
            cancel1?.Invoke();
            invokeCancel1 = false;
        }
        if (invokeCancel2)
        {
            cancel2?.Invoke();
            invokeCancel2 = false;
        }
        if (invokeCancel3)
        {
            cancel3?.Invoke();
            invokeCancel3 = false;
        }
    }

    public void PickTopAction()
    {
        if (action0 != null)
        {
            PickAction(0);
        }
        else if (action1 != null)
        {
            PickAction(1);
        }
        else if (action2 != null)
        {
            PickAction(2);
        }
        else if (action3 != null)
        {
            PickAction(3);
        }
        else if (action4 != null)
        {
            PickAction(4);
        }
        else if (action5 != null)
        {
            PickAction(5);
        }
        else if (action6 != null)
        {
            PickAction(6);
        }
        else if (action7 != null)
        {
            PickAction(7);
        }
    }

    public void Action0()
    {
        if (action0 != null)
        {
            action0();
            validAction = true;
        }
    }
    public void Action1()
    {
        if (action1 != null)
        {
            action1();
            validAction = true;
        }
    }
    public void Action2()
    {
        if (action2 != null)
        {
            action2();
            validAction = true;
        }
    }
    public void Action3()
    {
        if (action3 != null)
        {
            action3();
            validAction = true;
        }
    }
    public void Action4()
    {
        if (action4 != null)
        {
            action4();
            validAction = true;
        }
    }
    public void Action5()
    {
        if (action5 != null)
        {
            action5();
            validAction = true;
        }
    }
    public void Action6()
    {
        if (action6 != null)
        {
            action6();
            validAction = true;
        }
    }
    public void Action7()
    {
        if (action7 != null)
        {
            action7();
            validAction = true;
        }
    }
    public void ActionExamine()
    {
        Debug.Log(examineText);
        validAction = true;
    }
    public void UseActionOnThis()
    {
        if (actionUse != null && actionUsedOnThis != null)
        {
            actionUse();
        }
        else
        {
            actionUsedOnThis = null;
            DefaultUseAction();
        }
        RightClickMenu.isUsingItem = false;
        validAction = true;
    }

    public void DefaultUseAction()
    {
        Debug.Log("Nothing interesting happens.");
    }
}
