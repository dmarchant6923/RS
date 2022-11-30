using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class PresetsPanel : MonoBehaviour
{
    public OpenCloseButton closeButton;

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

    public bool IgnoreActions = false;
    bool endIgnoreActions;

    public Text[] presetTexts = new Text[3];

    public PresetsPanelButton[] saveButtons = new PresetsPanelButton[3];
    public PresetsPanelButton[] loadButtons = new PresetsPanelButton[3];
    public PresetsPanelButton[] clearButtons = new PresetsPanelButton[3];

    string folder = "/SaveData/";
    string extension = ".txt";
    string dir;

    //final build should be using persistentDataPath, not dataPath

    private void Start()
    {
        dir = Application.dataPath + folder;
        closeButton.buttonClicked += ClosePanel;
        Action.cancel1 += ClosePanel;

        InitializePresets();

        TickManager.afterTick += AfterTick;
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
                loadButtons[i].SetActive(false);
                clearButtons[i].SetActive(false);
            }
            else
            {
                presetTexts[i].text = "Preset " + (i + 1);
                loadButtons[i].SetActive(true);
                clearButtons[i].SetActive(true);
            }
        }
    }

    public void SavePreset(int presetNumber)
    {
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
            loadButtons[presetNumber].SetActive(false);
            clearButtons[presetNumber].SetActive(false);
        }
        else
        {
            presetTexts[presetNumber].text = "Preset " + (presetNumber + 1);
            loadButtons[presetNumber].SetActive(true);
            clearButtons[presetNumber].SetActive(true);
        }


        string fileName = "Preset" + presetNumber + "Equipment";
        string fullPath = dir + fileName + extension;
        string jsonString = JsonUtility.ToJson(presetEquipment[presetNumber]);
        File.WriteAllText(fullPath, jsonString);

        fileName = "Preset" + presetNumber + "Inventory";
        fullPath = dir + fileName + extension;
        jsonString = JsonUtility.ToJson(presetInventory[presetNumber]);
        File.WriteAllText(fullPath, jsonString);

        endIgnoreActions = true;
    }
    public void LoadPreset(int num)
    {
        StartCoroutine(LoadPlayerAttributes.LoadPresetEnum(presetEquipment[num].equipment, presetEquipment[num].blowpipeAmmo, presetInventory[num].items, presetInventory[num].blowpipeAmmo));
        endIgnoreActions = true;
    }
    public IEnumerator LoadPresetEnum(int presetNumber)
    {
        foreach (GameObject slot in Inventory.inventorySlots)
        {
            if (slot.GetComponentInChildren<Item>() != null)
            {
                Destroy(slot.GetComponentInChildren<Item>().gameObject);
            }
        }

        yield return null;

        foreach (Transform slot in WornEquipment.slots)
        {
            if (slot.GetComponentInChildren<Equipment>() != null)
            {
                Equipment item = slot.GetComponentInChildren<Equipment>();
                item.Equip();
                Destroy(item.gameObject);
            }
        }

        List<GameObject> equipments = new List<GameObject>();
        for (int i = 0; i < 11; i++)
        {
            if (presetEquipment[presetNumber].equipment[i] != "")
            {
                GameObject newEquipment = Tools.LoadFromResource(presetEquipment[presetNumber].equipment[i]);
                if (newEquipment != null)
                {
                    equipments.Add(newEquipment);
                    if (newEquipment.GetComponent<StackableItem>() != null)
                    {
                        newEquipment.GetComponent<StackableItem>().quantity = 2000;
                    }
                    if (newEquipment.GetComponent<ChargeItem>() != null)
                    {
                        newEquipment.GetComponent<ChargeItem>().charges = 2000;
                    }
                    if (newEquipment.GetComponent<BlowPipe>() != null && string.IsNullOrEmpty(presetEquipment[presetNumber].blowpipeAmmo) == false)
                    {
                        newEquipment.GetComponent<BlowPipe>().ammoLoaded = Tools.LoadFromResource(presetEquipment[presetNumber].blowpipeAmmo);
                        newEquipment.GetComponent<BlowPipe>().numberLoaded = 2000;
                    }
                }
            }
        }

        List<GameObject> items = new List<GameObject>();
        for (int i = 0; i < 28; i++)
        {
            if (presetInventory[presetNumber].items[i] != "")
            {
                GameObject newItem = Tools.LoadFromResource(presetInventory[presetNumber].items[i]);
                if (newItem != null)
                {
                    items.Add(newItem);
                    if (newItem.GetComponent<StackableItem>() != null)
                    {
                        newItem.GetComponent<StackableItem>().quantity = 2000;
                    }
                    if (newItem.GetComponent<ChargeItem>() != null)
                    {
                        newItem.GetComponent<ChargeItem>().charges = 2000;
                    }
                    if (newItem.GetComponent<BlowPipe>() != null && string.IsNullOrEmpty(presetInventory[presetNumber].blowpipeAmmo) == false)
                    {
                        newItem.GetComponent<BlowPipe>().ammoLoaded = Tools.LoadFromResource(presetInventory[presetNumber].blowpipeAmmo);
                        newItem.GetComponent<BlowPipe>().numberLoaded = 2000;
                    }
                }
            }
        }

        yield return new WaitForSeconds(0.1f);

        foreach (GameObject equipment in equipments)
        {
            equipment.GetComponent<Equipment>().Equip();
        }
        items.Reverse();
        foreach (GameObject item in items)
        {
            Inventory.instance.PlaceInInventory(item);
        }

        endIgnoreActions = true;
    }
    public void ClearPreset(int presetNumber)
    {
        presetEquipment[presetNumber] = new PresetEquipment();
        presetInventory[presetNumber] = new PresetInventory();
        presetTexts[presetNumber].text = "Preset " + (presetNumber + 1) + "\n(empty)";
        loadButtons[presetNumber].SetActive(false);
        clearButtons[presetNumber].SetActive(false);

        string fileName = "Preset" + presetNumber + "Inventory";
        string fullPath = dir + fileName + extension;
        string jsonString = JsonUtility.ToJson(presetInventory[presetNumber]);
        File.WriteAllText(fullPath, jsonString);

        fileName = "Preset" + presetNumber + "Equipment";
        fullPath = dir + fileName + extension;
        jsonString = JsonUtility.ToJson(presetEquipment[presetNumber]);
        File.WriteAllText(fullPath, jsonString);

        endIgnoreActions = true;
    }

    void AfterTick()
    {
        if (endIgnoreActions)
        {
            IgnoreActions = false;
            endIgnoreActions = false;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ClosePanel();
        }
    }

    void ClosePanel()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        Action.cancel1 -= ClosePanel;
    }
}
