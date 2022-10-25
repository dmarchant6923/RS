using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackStyle : MonoBehaviour
{
    Action styleAction;
    AttackStyles styleScript;

    [HideInInspector] public string styleName;
    public int styleNumber;

    public bool effectiveActive = false;

    private void Start()
    {
        styleAction = GetComponent<Action>();
        styleAction.menuTexts[0] = styleName;
        styleAction.clientAction0 += SelectStyle;
        styleAction.serverAction0 += ServerSelectStyle;
        styleAction.orderLevels[0] = -1;

        //TickManager.beforeTick += BeforeTick;

        styleScript = transform.parent.GetComponent<AttackStyles>();
    }

    void SelectStyle()
    {
        if (styleScript.selectedStyle != styleNumber)
        {
            styleScript.ChangeStyleColor(styleNumber);
        }
    }

    void ServerSelectStyle()
    {
        styleScript.selectedStyle = styleNumber;
        styleScript.styleChanged = true;
    }
}
