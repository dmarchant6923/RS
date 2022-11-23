using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PresetsPanel : MonoBehaviour
{
    public OpenCloseButton closeButton;

    string preset1Inventory;
    string preset1Equipment;
    string preset2Inventory;
    string preset2Equipment;
    string preset3Inventory;
    string preset3Equipment;

    string saveFile;

    private void Start()
    {
        closeButton.buttonClicked += ClosePanel;
        Action.cancel1 += ClosePanel;

        preset1Inventory = PlayerPrefs.GetString("preset1Inventory", "");

        saveFile = Application.persistentDataPath + "/presets.json";
        if (File.Exists(saveFile))
        {

        }
    }

    void ClosePanel()
    {
        gameObject.SetActive(false);
    }
}
