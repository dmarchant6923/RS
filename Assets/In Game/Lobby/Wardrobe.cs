using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wardrobe : MonoBehaviour
{
    Action wardrobeAction;
    InteractableObject objectScript;
    public GameObject presetPanel;

    private void Start()
    {
        wardrobeAction = GetComponent<Action>();
        objectScript = GetComponent<InteractableObject>();

        wardrobeAction.menuTexts[0] = "Check-presets ";
        wardrobeAction.UpdateName();
        objectScript.interaction += OpenWardrobe;
    }

    void OpenWardrobe()
    {
        PanelButtons.instance.ForceOpen("Inventory");
        presetPanel.SetActive(true);
    }
}
