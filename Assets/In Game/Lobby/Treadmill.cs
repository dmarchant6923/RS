using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treadmill : MonoBehaviour
{
    Action treadmillAction;
    InteractableObject objectScript;
    public GameObject skillsPanel;

    private void Start()
    {
        treadmillAction = GetComponent<Action>();
        objectScript = GetComponent<InteractableObject>();

        treadmillAction.menuTexts[0] = "Use ";
        treadmillAction.UpdateName();
        objectScript.interaction += UseTreadmill;
    }

    void UseTreadmill()
    {
        skillsPanel.SetActive(true);
    }
}
