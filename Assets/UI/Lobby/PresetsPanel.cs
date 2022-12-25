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

        public string[] equipmentNames = new string[11];
        public int[] equipmentQuantities = new int[11];

        public string blowpipeAmmo;
    }

    public class PresetInventory
    {
        public bool filled = false;

        public string[] itemNames = new string[28];
        public int[] itemQuantities = new int[28];

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

    public GameObject parent;
    Wardrobe wardrobe;
    BasePanelScript panelScript;

    public ButtonScript depositInventoryButton;
    public ButtonScript depositEquipmentButton;

    public GameObject warning;
    public CarriedValueScript valueScript;

    [HideInInspector] public bool panelOpen = false;

    //final build should be using persistentDataPath, not dataPath

    private void Awake()
    {
        PlayerStats.reinitialize += FindPanel;
        panelScript = GetComponent<BasePanelScript>();
        panelScript.panelClosed += CloseWardrobe;
        TickManager.afterTick += CheckWarningTick;

        depositInventoryButton.buttonClicked += DepositInventory;
        depositEquipmentButton.buttonClicked += DepositEquipment;
    }

    void FindPanel()
    {
        wardrobe = FindObjectOfType<Wardrobe>();
        if (wardrobe != null)
        {
            wardrobe.presetPanel = parent;
        }
        StartCoroutine(StartPanel());
    }

    private IEnumerator StartPanel()
    {
        if (wardrobe != null)
        {
            while (saveButtons[0].buttonAction == null)
            {
                yield return null;
            }

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

            dir = Application.persistentDataPath + folder;

            InitializePresets();

            TickManager.beforeTick += BeforeTick;
        }
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


        presetEquipment[presetNumber].equipmentNames[0] = WornEquipment.head != null ? WornEquipment.head.name : "";
        presetEquipment[presetNumber].equipmentNames[1] = WornEquipment.cape != null ? WornEquipment.cape.name : "";
        presetEquipment[presetNumber].equipmentNames[2] = WornEquipment.neck != null ? WornEquipment.neck.name : "";
        presetEquipment[presetNumber].equipmentNames[3] = WornEquipment.ammo != null ? WornEquipment.ammo.name : "";
        if (WornEquipment.ammo != null && WornEquipment.ammo.stackable)
        {
            presetEquipment[presetNumber].equipmentQuantities[3] = WornEquipment.ammo.itemScript.stackScript.quantity;
        }
        presetEquipment[presetNumber].equipmentNames[4] = WornEquipment.weapon != null ? WornEquipment.weapon.name : "";
        if (WornEquipment.weapon != null && WornEquipment.weapon.stackable)
        {
            presetEquipment[presetNumber].equipmentQuantities[4] = WornEquipment.weapon.itemScript.stackScript.quantity;
        }
        presetEquipment[presetNumber].equipmentNames[5] = WornEquipment.body != null ? WornEquipment.body.name : "";
        presetEquipment[presetNumber].equipmentNames[6] = WornEquipment.shield != null ? WornEquipment.shield.name : "";
        presetEquipment[presetNumber].equipmentNames[7] = WornEquipment.leg != null ? WornEquipment.leg.name : "";
        presetEquipment[presetNumber].equipmentNames[8] = WornEquipment.glove != null ? WornEquipment.glove.name : "";
        presetEquipment[presetNumber].equipmentNames[9] = WornEquipment.boot != null ? WornEquipment.boot.name : "";
        presetEquipment[presetNumber].equipmentNames[10] = WornEquipment.ring != null ? WornEquipment.ring.name : "";

        foreach (string item in presetEquipment[presetNumber].equipmentNames)
        {
            if (string.IsNullOrEmpty(item) == false)
            {
                presetEquipment[presetNumber].filled = true;
                break;
            }
        }

        if (presetEquipment[presetNumber].equipmentNames[4] == "Toxic blowpipe")
        {
            presetEquipment[presetNumber].blowpipeAmmo = WornEquipment.weapon.GetComponent<BlowPipe>().ammoLoaded.name;
        }

        presetInventory[presetNumber].filled = false;
        for (int i = 0; i < 28; i++)
        {
            if (Inventory.inventorySlots[i].GetComponentInChildren<Item>() != null)
            {
                Item item = Inventory.inventorySlots[i].GetComponentInChildren<Item>();
                presetInventory[presetNumber].filled = true;
                presetInventory[presetNumber].itemNames[i] = item.name;
                if (Inventory.inventorySlots[i].GetComponentInChildren<BlowPipe>() != null)
                {
                    presetInventory[presetNumber].blowpipeAmmo = Inventory.inventorySlots[i].GetComponentInChildren<BlowPipe>().ammoLoaded.name;
                }
                if (item.isStackable)
                {
                    presetInventory[presetNumber].itemQuantities[i] = item.stackScript.quantity;
                }
            }
            else
            {
                presetInventory[presetNumber].itemNames[i] = "";
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

        GameLog.Log("Preset " + (presetNumber + 1) + " saved!");

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
            StartCoroutine(LoadPresetEnum(num));
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




    public void OpenWardrobe()
    {
        panelOpen = true;
        PanelButtons.instance.ForceOpen("Inventory");
        parent.SetActive(true);
        foreach (GameObject slot in Inventory.inventorySlots)
        {
            if (slot.GetComponentInChildren<Item>() != null)
            {
                Item item = slot.GetComponentInChildren<Item>();
                item.menuTexts[7] = "Deposit ";
                item.itemAction.serverAction7 += item.DestroyItem;
                item.itemAction.menuPriorities[7] = 1;
                item.UpdateActions();
            }
        }

        UpdatePrice();
    }

    void CloseWardrobe()
    {
        panelOpen = false;
        foreach (GameObject slot in Inventory.inventorySlots)
        {
            if (slot.GetComponentInChildren<Item>() != null)
            {
                Item item = slot.GetComponentInChildren<Item>();
                item.menuTexts[7] = null;
                item.itemAction.serverAction7 -= item.DestroyItem;
                item.itemAction.menuPriorities[7] = 0;
                item.UpdateActions();
            }
        }
    }

    IEnumerator LoadPresetEnum(int num)
    {
        GameManager.ItemAndQuantity[] equipment = new GameManager.ItemAndQuantity[11];
        GameManager.ItemAndQuantity[] items = new GameManager.ItemAndQuantity[28];
        for (int i = 0; i < presetEquipment[num].equipmentNames.Length; i++)
        {
            GameManager.ItemAndQuantity newEquipment = new GameManager.ItemAndQuantity();
            newEquipment.name = presetEquipment[num].equipmentNames[i];
            newEquipment.quantity = presetEquipment[num].equipmentQuantities[i];
            equipment[i] = newEquipment;
        }
        for (int i = 0; i < presetInventory[num].itemNames.Length; i++)
        {
            GameManager.ItemAndQuantity newItem = new GameManager.ItemAndQuantity();
            newItem.name = presetInventory[num].itemNames[i];
            newItem.quantity = presetInventory[num].itemQuantities[i];
            items[i] = newItem;
        }

        yield return StartCoroutine(LoadPlayerAttributes.LoadPresetEnum(equipment, presetEquipment[num].blowpipeAmmo, items, presetInventory[num].blowpipeAmmo));
        yield return null;
        foreach (GameObject slot in Inventory.inventorySlots)
        {
            if (slot.GetComponentInChildren<Item>() != null)
            {
                Item item = slot.GetComponentInChildren<Item>();
                item.menuTexts[7] = "Deposit ";
                item.itemAction.serverAction7 += item.DestroyItem;
                item.itemAction.menuPriorities[7] = 1;
                item.UpdateActions();
            }
        }

        UpdatePrice();
    }

    void DepositInventory()
    {
        foreach (GameObject slot in Inventory.inventorySlots)
        {
            if (slot.GetComponentInChildren<Item>() != null)
            {
                Destroy(slot.GetComponentInChildren<Item>().gameObject);
            }
        }

        UpdatePrice();
    }

    void DepositEquipment()
    {
        foreach (Transform slot in WornEquipment.slots)
        {
            if (slot.GetComponentInChildren<Equipment>() != null)
            {
                slot.GetComponentInChildren<Equipment>().DestroyEquippedItem();
            }
        }

        UpdatePrice();
    }

    void CheckWarningTick()
    {
        if (panelOpen == false)
        {
            return;
        }

        Invoke(nameof(CheckWarning), 0.1f);
        Invoke(nameof(UpdatePrice), 0.1f);
    }

    void CheckWarning()
    {
        warning.SetActive(UIManager.instance.CheckWarning());
    }

    public void UpdatePrice()
    {
        valueScript.UpdateValue();
    }
}
