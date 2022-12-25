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
        objectScript.interaction += OpenWardrobe;
    }

    void OpenWardrobe()
    {
        FindObjectOfType<PresetsPanel>().OpenWardrobe();
    }
}
