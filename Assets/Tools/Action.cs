using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action : MonoBehaviour
{
    [System.NonSerialized] public string[] menuTexts = new string[9];
    [System.NonSerialized] public int[] menuPriorities = new int[9]; // priority -1, 0, or 1. highest priority shows at the top of action menu
    [System.NonSerialized] public int[] cancelLevels = new int[10]; //cancel level 0, 1, 2, or 3. level cancels that level and all below it.
    [System.NonSerialized] public int[] orderLevels = new int[10]; //order level -1, 0, 1. before tick, on tick, after tick.
    public string examineText;
    [HideInInspector] public string objectName;
    public bool addObjectName;

    [HideInInspector] public bool itemAction = false;
    public bool ignoreUse = false;
    public bool inGame = false;
    [HideInInspector] public bool redClick = true;


    public delegate void ObjectAction();

    public event ObjectAction clientAction0;
    public event ObjectAction clientAction1;
    public event ObjectAction clientAction2;
    public event ObjectAction clientAction3;
    public event ObjectAction clientAction4;
    public event ObjectAction clientAction5;
    public event ObjectAction clientAction6;
    public event ObjectAction clientAction7;
    public event ObjectAction clientActionExamine;
    public event ObjectAction clientActionUse;

    public event ObjectAction serverAction0;
    public event ObjectAction serverAction1;
    public event ObjectAction serverAction2;
    public event ObjectAction serverAction3;
    public event ObjectAction serverAction4;
    public event ObjectAction serverAction5;
    public event ObjectAction serverAction6;
    public event ObjectAction serverAction7;
    public event ObjectAction serverActionExamine;
    public event ObjectAction serverActionUse;

    public static List<int> beforeTickNum = new List<int>();
    public static List<int> onTickNum = new List<int>();
    public static List<int> afterTickNum = new List<int>();

    public static List<Action> beforeTickActions = new List<Action>();
    public static List<Action> onTickActions = new List<Action>();
    public static List<Action> afterTickActions = new List<Action>();
    static bool staticEventsAddedToTickManager;

    public static event ObjectAction cancel1;
    public static event ObjectAction cancel2;
    public static event ObjectAction cancel3;

    static bool invokeCancel1;
    static bool invokeCancel2;
    static bool invokeCancel3;

    [HideInInspector] public Action actionUsedOnThis;

    public bool actionOnPointerUp;

    bool validAction = false;
    bool topActionPicked = false;

    private IEnumerator Start()
    {
        menuTexts = new string[9];
        if (examineText != null && string.IsNullOrEmpty(examineText) == false)
        {
            menuTexts[8] = "Examine ";
            serverActionExamine += Examine;
        }

        menuPriorities[8] = -1;

        if (staticEventsAddedToTickManager == false)
        {
            TickManager.cancelBeforeTick += CancelActions;
            TickManager.beforeTick += BeforeTick;
            TickManager.onTick += OnTick;
            TickManager.afterTick += AfterTick;
            staticEventsAddedToTickManager = true;
        }

        beforeTickNum = new List<int>();
        beforeTickActions = new List<Action>();
        onTickNum = new List<int>();
        onTickActions = new List<Action>();
        afterTickNum = new List<int>();
        afterTickActions = new List<Action>();

        yield return null;
        for (int i = 0; i < orderLevels.Length; i++)
        {
            orderLevels[i] = Mathf.Clamp(orderLevels[i], -1, 1);
        }
        for (int i = 0; i < menuPriorities.Length; i++)
        {
            menuPriorities[i] = Mathf.Clamp(menuPriorities[i], -1, 1);
        }
        for (int i = 0; i < cancelLevels.Length; i++)
        {
            cancelLevels[i] = Mathf.Clamp(cancelLevels[i], 0, 3);
        }
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


    public void PickTopAction()
    {
        for (int i = 1; i > -2; i--)
        {
            for (int j = 0; j < menuPriorities.Length; j++)
            {
                if (menuPriorities[j] == i && string.IsNullOrEmpty(menuTexts[j]) == false)
                {
                    PickAction(j);
                    if (topActionPicked)
                    {
                        topActionPicked = false;
                        return;
                    }
                }
            }
        }
    }
    public void PickAction(int actionNumber)
    {
        if (actionNumber == 0)
        {
            clientAction0?.Invoke();
            if (serverAction0 != null)
            {
                StartCoroutine(DelayAction(actionNumber));
            }
            if (clientAction0 != null || serverAction0 != null)
            {
                validAction = true;
            }
        }
        else if (actionNumber == 1)
        {
            clientAction1?.Invoke();
            if (serverAction1 != null)
            {
                StartCoroutine(DelayAction(actionNumber));
            }
            if (clientAction1 != null || serverAction1 != null)
            {
                validAction = true;
            }
        }
        else if (actionNumber == 2)
        {
            clientAction2?.Invoke();
            if (serverAction2 != null)
            {
                StartCoroutine(DelayAction(actionNumber));
            }
            if (clientAction2 != null || serverAction2 != null)
            {
                validAction = true;
            }
        }
        else if (actionNumber == 3)
        {
            clientAction3?.Invoke();
            if (serverAction3 != null)
            {
                StartCoroutine(DelayAction(actionNumber));
            }
            if (clientAction3 != null || serverAction3 != null)
            {
                validAction = true;
            }
        }
        else if (actionNumber == 4)
        {
            clientAction4?.Invoke();
            if (serverAction4 != null)
            {
                StartCoroutine(DelayAction(actionNumber));
            }
            if (clientAction4 != null || serverAction4 != null)
            {
                validAction = true;
            }
        }
        else if (actionNumber == 5)
        {
            clientAction5?.Invoke();
            if (serverAction5 != null)
            {
                StartCoroutine(DelayAction(actionNumber));
            }
            if (clientAction5 != null || serverAction5 != null)
            {
                validAction = true;
            }
        }
        else if (actionNumber == 6)
        {
            clientAction6?.Invoke();
            if (serverAction6 != null)
            {
                StartCoroutine(DelayAction(actionNumber));
            }
            if (clientAction6 != null || serverAction6 != null)
            {
                validAction = true;
            }
        }
        else if (actionNumber == 7)
        {
            clientAction7?.Invoke();
            if (serverAction7 != null)
            {
                StartCoroutine(DelayAction(actionNumber));
            }
            if (clientAction7 != null || serverAction7 != null)
            {
                validAction = true;
            }
        }
        else if (actionNumber == 8)
        {
            clientActionExamine?.Invoke();
            if (serverActionExamine != null)
            {
                StartCoroutine(DelayAction(actionNumber));
                validAction = true;
            }
        }
        else if (actionNumber == 9)
        {
            clientActionUse?.Invoke();
            StartCoroutine(DelayAction(actionNumber));
            if (clientActionUse != null || serverActionUse != null)
            {
                validAction = true;
            }
            RightClickMenu.isUsingItem = false;
        }
        else
        {
            Debug.Log("no action selected");
        }

        topActionPicked = true;
        if (validAction == false)
        {
            topActionPicked = false;
            return;
        }
        validAction = false;
        if (inGame)
        {
            UIManager.ClickX(redClick);
        }

        if (cancelLevels[actionNumber] > 0)
        {
            StartCoroutine(DelayCancel(cancelLevels[actionNumber]));
        }
    }
    IEnumerator DelayAction(int num)
    {
        yield return new WaitForSeconds(TickManager.simLatency);
        if (orderLevels[num] == -1)
        {
            beforeTickActions.Add(this);
            beforeTickNum.Add(num);
        }
        else if (orderLevels[num] == 0)
        {
            onTickActions.Add(this);
            onTickNum.Add(num);
        }
        else if (orderLevels[num] == 1)
        {
            afterTickActions.Add(this);
            afterTickNum.Add(num);
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

    public static void CancelActions()
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
    public static void BeforeTick()
    {
        for (int i = 0; i < beforeTickActions.Count; i++)
        {
            if (beforeTickActions[i].gameObject.activeSelf)
            {
                beforeTickActions[i].PickServerAction(beforeTickNum[i]);
            }
        }

        beforeTickActions = new List<Action>();
        beforeTickNum = new List<int>();
    }
    public static void OnTick()
    {
        for (int i = 0; i < onTickActions.Count; i++)
        {
            if (onTickActions[i].gameObject.activeSelf)
            {
                onTickActions[i].PickServerAction(onTickNum[i]);
            }
        }

        onTickActions = new List<Action>();
        onTickNum = new List<int>();
    }
    public static void AfterTick()
    {
        for (int i = 0; i < afterTickActions.Count; i++)
        {
            if (afterTickActions[i].gameObject.activeSelf)
            {
                afterTickActions[i].PickServerAction(afterTickNum[i]);
            }
        }

        afterTickActions = new List<Action>();
        afterTickNum = new List<int>();
    }

    public void PickServerAction(int actionNumber)
    {
        if (actionNumber == 0)
        {
            serverAction0?.Invoke();
        }
        if (actionNumber == 1)
        {
            serverAction1?.Invoke();
        }
        if (actionNumber == 2)
        {
            serverAction2?.Invoke();
        }
        if (actionNumber == 3)
        {
            serverAction3?.Invoke();
        }
        if (actionNumber == 4)
        {
            serverAction4?.Invoke();
        }
        if (actionNumber == 5)
        {
            serverAction5?.Invoke();
        }
        if (actionNumber == 6)
        {
            serverAction6?.Invoke();
        }
        if (actionNumber == 7)
        {
            serverAction7?.Invoke();
        }
        if (actionNumber == 8)
        {
            serverActionExamine?.Invoke();
        }
        if (actionNumber == 9)
        {
            if (serverActionUse != null && actionUsedOnThis != null)
            {
                serverActionUse();
            }
            else
            {
                DefaultUseAction();
            }
            actionUsedOnThis = null;
        }
    }

    public void DefaultUseAction()
    {
        Debug.Log("Nothing interesting happens.");
    }

    void Examine()
    {
        Debug.Log(examineText);
    }
}
