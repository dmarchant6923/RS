using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PanelButtonClick : MonoBehaviour
{
    PanelButtons script;
    Action buttonAction;

    void Start()
    {
        script = transform.parent.GetComponent<PanelButtons>();
        buttonAction = GetComponent<Action>();
        buttonAction.menuTexts[0] = gameObject.name;
        buttonAction.clientAction0 += ClickButton;
    }

    public void ClickButton()
    {
        script.OnClick(transform, false);
    }
}
