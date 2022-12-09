using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class PresetsPanel : MonoBehaviour
{
    public class PresetEquipment
    {
        public bool filled = false;

        public string[] equipment = new string[11];

        public string blowpipeAmmo;
    }

    public class PresetInventory
    {
        public bool filled = false;

        public string[] items = new string[28];

        public string blowpipeAmmo;
    }

    [System.NonSerialized] public PresetEquipment[] presetEquipment = new PresetEquipment[3];
    [System.NonSerialized] public PresetInventory[] presetInventory = new PresetInventory[3];

    bool IgnoreActions;

    public Text[] presetTexts = new Text[3];

    public ButtonScript[] saveButtons = new ButtonScript[3];
    public ButtonScript[] loadButtons = new ButtonScript[3];
    public ButtonScript[] clearButtons = new ButtonScript[3];

    string folder = "/SaveData/";
    string extension = ".txt";
    string dir;

    //final build should be using persistentDataPath, not dataPath

    private void Start()
    {
        for (int i = 0; i < 3; i++)
        {
            saveButtons[i].buttonAction.menuTexts[0] = "Save preset " + (i + 1);
            loadButtons[i].buttonAction.menuTexts[0] = "Load preset " + (i + 1);
            clearButtons[i].buttonAction.menuTexts[0] = "Clear preset " + (i + 1);
            if (i == 0)
            {
                saveButtons[i].buttonClicked += SavePreset1;
                loadButtons[i].buttonClicked += LoadPreset1;
                clearButtons[i].buttonClicked += ClearPreset1;
            }
            if (i == 1)
            {
                saveButtons[i].buttonClicked += SavePreset2;
                loadButtons[i].buttonClicked += LoadPreset2;
                clearButtons[i].buttonClicked += ClearPreset2;
            }
            if (i == 2)
            {
                saveButtons[i].buttonClicked += SavePreset3;
                loadButtons[i].buttonClicked += LoadPreset3;
                clearButtons[i].buttonClicked += ClearPreset3;
            }
            saveButtons[i].buttonAction.UpdateName();
            loadButtons[i].buttonAction.UpdateName();
            clearButtons[i].buttonAction.UpdateName();
        }

        dir = Application.dataPath + folder;

        InitializePresets();

        TickManager.beforeTick += BeforeTick;
    }
    void InitializePresets()
    {
        for (int i = 0; i < 3; i++)
        {
            string fileName = "Preset" + i + "Inventory";
            string fullPath = dir + fileName + extension;
            if (File.Exists(fullPath))
            {
                string jsonString = File.ReadAllText(fullPath);
                presetInventory[i] = JsonUtility.FromJson<PresetInventory>(jsonString);
            }
            else
            {
                presetInventory[i] = new PresetInventory();
                string jsonString = JsonUtility.ToJson(presetInventory[i]);
                File.WriteAllText(fullPath, jsonString);
            }

            fileName = "Preset" + i + "Equipment";
            fullPath = dir + fileName + extension;
            if (File.Exists(fullPath))
            {
                string jsonString = File.ReadAllText(fullPath);
                presetEquipment[i] = JsonUtility.FromJson<PresetEquipment>(jsonString);
            }
            else
            {
                presetEquipment[i] = new PresetEquipment();
                string jsonString = JsonUtility.ToJson(presetEquipment[i]);
                File.WriteAllText(fullPath, jsonString);
            }

            if (presetEquipment[i].filled == false && presetInventory[i].filled == false)
            {
                presetTexts[i].text = "Preset " + (i + 1) + "\n(empty)";
                loadButtons[i].ActivateButton(false);
                clearButtons[i].ActivateButton(false);
            }
            else
            {
                presetTexts[i].text = "Preset " + (i + 1);
                loadButtons[i].ActivateButton(true);
                clearButtons[i].ActivateButton(true);
            }
        }
    }

    public void SavePreset(int presetNumber)
    {
        if (IgnoreActions)
        {
            return;
        }
        IgnoreActions = true;

        presetEquipment[presetNumber].filled = false;

        presetEquipment[presetNumber].equipment[0] = WornEquipment.head != null ? WornEquipment.head.name : "";
        presetEquipment[presetNumber].equipment[1] = WornEquipment.cape != null ? WornEquipment.cape.name : "";
        presetEquipment[presetNumber].equipment[2] = WornEquipment.neck != null ? WornEquipment.neck.name : "";
        presetEquipment[presetNumber].equipment[3] = WornEquipment.ammo != null ? WornEquipment.ammo.name : "";
        presetEquipment[presetNumber].equipment[4] = WornEquipment.weapon != null ? WornEquipment.weapon.name : "";
        presetEquipment[presetNumber].equipment[5] = WornEquipment.body != null ? WornEquipment.body.name : "";
        presetEquipment[presetNumber].equipment[6] = WornEquipment.shield != null ? WornEquipment.shield.name : "";
        presetEquipment[presetNumber].equipment[7] = WornEquipment.leg != null ? WornEquipment.leg.name : "";
        presetEquipment[presetNumber].equipment[8] = WornEquipment.glove != null ? WornEquipment.glove.name : "";
        presetEquipment[presetNumber].equipment[9] = WornEquipment.boot != null ? WornEquipment.boot.name : "";
        presetEquipment[presetNumber].equipment[10] = WornEquipment.ring != null ? WornEquipment.ring.name : "";

        foreach (string item in presetEquipment[presetNumber].equipment)
        {
            if (string.IsNullOrEmpty(item) == false)
            {
                presetEquipment[presetNumber].filled = true;
                break;
            }
        }

        if (presetEquipment[presetNumber].equipment[4] == "Toxic blowpipe")
        {
            presetEquipment[presetNumber].blowpipeAmmo = WornEquipment.weapon.GetComponent<BlowPipe>().ammoLoaded.name;
        }

        presetInventory[presetNumber].filled = false;
        for (int i = 0; i < 28; i++)
        {
            if (Inventory.inventorySlots[i].GetComponentInChildren<Item>() != null)
            {
                presetInventory[presetNumber].filled = true;
                presetInventory[presetNumber].items[i] = Inventory.inventorySlots[i].GetComponentInChildren<Item>().name;
                if (Inventory.inventorySlots[i].GetComponentInChildren<BlowPipe>() != null)
                {
                    presetInventory[presetNumber].blowpipeAmmo = Inventory.inventorySlots[i].GetComponentInChildren<BlowPipe>().ammoLoaded.name;
                }
            }
            else
            {
                presetInventory[presetNumber].items[i] = "";
            }
        }

        if (presetInventory[presetNumber].filled == false && presetEquipment[presetNumber].filled == false)
        {
            presetTexts[presetNumber].text = "Preset " + (presetNumber + 1) + "\n(empty)";
            loadButtons[presetNumber].ActivateButton(false);
            clearButtons[presetNumber].ActivateButton(false);
        }
        else
        {
            presetTexts[presetNumber].text = "Preset " + (presetNumber + 1);
            loadButtons[presetNumber].ActivateButton(true);
            clearButtons[presetNumber].ActivateButton(true);
        }


        string fileName = "Preset" + presetNumber + "Equipment";
        string fullPath = dir + fileName + extension;
        string jsonString = JsonUtility.ToJson(presetEquipment[presetNumber]);
        File.WriteAllText(fullPath, jsonString);

        fileName = "Preset" + presetNumber + "Inventory";
        fullPath = dir + fileName + extension;
        jsonString = JsonUtility.ToJson(presetInventory[presetNumber]);
        File.WriteAllText(fullPath, jsonString);

        IgnoreActions = true;
    }
    public void LoadPreset(int num)
    {
        if (IgnoreActions)
        {
            return;
        }
        IgnoreActions = true;

        if (gameObject.activeSelf)
        {
            StartCoroutine(LoadPlayerAttributes.LoadPresetEnum(presetEquipment[num].equipment, presetEquipment[num].blowpipeAmmo, presetInventory[num].items, presetInventory[num].blowpipeAmmo));
        }
    }
    public void ClearPreset(int presetNumber)
    {
        if (IgnoreActions)
        {
            return;
        }
        IgnoreActions = true;

        presetEquipment[presetNumber] = new PresetEquipment();
        presetInventory[presetNumber] = new PresetInventory();
        presetTexts[presetNumber].text = "Preset " + (presetNumber + 1) + "\n(empty)";
        loadButtons[presetNumber].ActivateButton(false);
        clearButtons[presetNumber].ActivateButton(false);

        string fileName = "Preset" + presetNumber + "Inventory";
        string fullPath = dir + fileName + extension;
        string jsonString = JsonUtility.ToJson(presetInventory[presetNumber]);
        File.WriteAllText(fullPath, jsonString);

        fileName = "Preset" + presetNumber + "Equipment";
        fullPath = dir + fileName + extension;
        jsonString = JsonUtility.ToJson(presetEquipment[presetNumber]);
        File.WriteAllText(fullPath, jsonString);
    }

    void BeforeTick()
    {
        if (IgnoreActions)
        {
            IgnoreActions = false;
        }
    }


    void SavePreset1()
    {
        SavePreset(0);
    }
    void SavePreset2()
    {
        SavePreset(1);
    }
    void SavePreset3()
    {
        SavePreset(2);
    }
    void LoadPreset1()
    {
        LoadPreset(0);
    }
    void LoadPreset2()
    {
        LoadPreset(1);
    }
    void LoadPreset3()
    {
        LoadPreset(2);
    }
    void ClearPreset1()
    {
        ClearPreset(0);
    }
    void ClearPreset2()
    {
        ClearPreset(1);
    }
    void ClearPreset3()
    {
        ClearPreset(2);
    }

}
