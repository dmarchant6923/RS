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
        styleAction.action0 += SelectStyle;

        styleScript = transform.parent.GetComponent<AttackStyles>();
    }

    void SelectStyle()
    {
        if (styleScript.selectedStyle != styleNumber)
        {
            styleScript.ChangeStyleColor(styleNumber);
            Invoke("DelaySelectStyle", TickManager.simLatency);
        }
    }

    void DelaySelectStyle()
    {
        styleScript.selectedStyle = styleNumber;
        styleScript.styleChanged = true;
    }
}
