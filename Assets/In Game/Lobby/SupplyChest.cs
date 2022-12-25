using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupplyChest : MonoBehaviour
{
    Action chestAction;
    public GameObject chestPanel;
    InteractableObject objectScript;

    private void Start()
    {
        chestAction = GetComponent<Action>();
        objectScript = GetComponent<InteractableObject>();

        chestAction.menuTexts[0] = "Open ";
        objectScript.interaction += OpenChest;
    }

    void OpenChest()
    {
        FindObjectOfType<ChestPanel>().OpenChest();
    }
}