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

    PresetEquipment initialPreset1Equipment = new PresetEquipment();
    PresetInventory initialPreset1Inventory = new PresetInventory();
    PresetEquipment initialPreset2Equipment = new PresetEquipment();
    PresetInventory initialPreset2Inventory = new PresetInventory();

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
    public ButtonScript restorePresetsButton;

    public GameObject warning;
    public CarriedValueScript valueScript;

    [HideInInspector] public bool panelOpen = false;

    //final build should be using persistentDataPath, not dataPath

    private void Awake()
    {
        initialPreset1Equipment.filled = true;
        initialPreset1Equipment.equipmentNames[0] = "Slayer helmet (i)";
        initialPreset1Equipment.equipmentNames[1] = "Masori assembler";
        initialPreset1Equipment.equipmentNames[2] = "Necklace of anguish";
        initialPreset1Equipment.equipmentNames[3] = "Ruby dragon bolts (e)";
        initialPreset1Equipment.equipmentQuantities[3] = 500;
        initialPreset1Equipment.equipmentNames[4] = "Zaryte crossbow";
        initialPreset1Equipment.equipmentNames[5] = "Masori body (f)";
        initialPreset1Equipment.equipmentNames[6] = "Twisted buckler";
        initialPreset1Equipment.equipmentNames[7] = "Masori chaps (f)";
        initialPreset1Equipment.equipmentNames[8] = "Zaryte vambraces";
        initialPreset1Equipment.equipmentNames[9] = "Pegasian boots";
        initialPreset1Equipment.equipmentNames[10] = "Lightbearer";

        initialPreset1Inventory.filled = true;
        initialPreset1Inventory.itemNames[0] = "Toxic blowpipe";
        initialPreset1Inventory.blowpipeAmmo = "Dragon dart";
        initialPreset1Inventory.itemNames[1] = "Twisted bow";
        initialPreset1Inventory.itemNames[2] = "Dragon arrow";
        initialPreset1Inventory.itemQuantities[2] = 500;
        initialPreset1Inventory.itemNames[4] = "Bastion potion";
        initialPreset1Inventory.itemNames[5] = "Stamina potion";
        for (int i = 6; i < 11; i++)
        {
            initialPreset1Inventory.itemNames[i] = "Saradomin brew";
        }
        for (int i = 11; i < 16; i++)
        {
            initialPreset1Inventory.itemNames[i] = "Super restore";
        }
        initialPreset1Inventory.itemNames[16] = "Black chinchompa";
        initialPreset1Inventory.itemQuantities[16] = 50;
        System.Array.Reverse(initialPreset1Inventory.itemNames);
        System.Array.Reverse(initialPreset1Inventory.itemQuantities);


        initialPreset2Equipment.filled = true;
        initialPreset2Equipment.equipmentNames[0] = "Crystal helm";
        initialPreset2Equipment.equipmentNames[1] = "Masori assembler";
        initialPreset2Equipment.equipmentNames[2] = "Necklace of anguish";
        initialPreset2Equipment.equipmentNames[3] = "Rada's blessing 4";
        initialPreset2Equipment.equipmentNames[4] = "Bow of faerdhinen";
        initialPreset2Equipment.equipmentNames[5] = "Crystal body";
        initialPreset2Equipment.equipmentNames[7] = "Crystal legs";
        initialPreset2Equipment.equipmentNames[8] = "Barrows gloves";
        initialPreset2Equipment.equipmentNames[9] = "Zamorak d'hide boots";
        initialPreset2Equipment.equipmentNames[10] = "Ring of suffering (i)";

        initialPreset2Inventory.filled = true;
        initialPreset2Inventory.itemNames[0] = "Toxic blowpipe";
        initialPreset2Inventory.blowpipeAmmo = "Dragon dart";
        initialPreset2Inventory.itemNames[1] = "Ranging potion";
        initialPreset2Inventory.itemNames[2] = "Stamina potion";
        for (int i = 3; i < 8; i++)
        {
            initialPreset2Inventory.itemNames[i] = "Saradomin brew";
        }
        for (int i = 8; i < 13; i++)
        {
            initialPreset2Inventory.itemNames[i] = "Super restore";
        }
        initialPreset2Inventory.itemNames[13] = "Blood rune";
        initialPreset2Inventory.itemQuantities[13] = 200;
        initialPreset2Inventory.itemNames[14] = "Death rune";
        initialPreset2Inventory.itemQuantities[14] = 200;
        initialPreset2Inventory.itemNames[15] = "Water rune";
        initialPreset2Inventory.itemQuantities[15] = 200;
        System.Array.Reverse(initialPreset2Inventory.itemNames);
        System.Array.Reverse(initialPreset2Inventory.itemQuantities);





        PlayerStats.reinitialize += FindPanel;
        panelScript = GetComponent<BasePanelScript>();
        panelScript.panelClosed += CloseWardrobe;
        TickManager.afterTick += CheckWarningTick;

        depositInventoryButton.buttonClicked += DepositInventory;
        depositEquipmentButton.buttonClicked += DepositEquipment;
        restorePresetsButton.buttonClicked += RestorePresets;
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.1f);
        GameManager.ItemAndQuantity[] equipment = new GameManager.ItemAndQuantity[11];
        GameManager.ItemAndQuantity[] items = new GameManager.ItemAndQuantity[28];
        for (int i = 0; i < initialPreset1Equipment.equipmentNames.Length; i++)
        {
            GameManager.ItemAndQuantity newEquipment = new GameManager.ItemAndQuantity();
            newEquipment.name = initialPreset1Equipment.equipmentNames[i];
            newEquipment.quantity = initialPreset1Equipment.equipmentQuantities[i];
            equipment[i] = newEquipment;
        }
        for (int i = 0; i < initialPreset1Inventory.itemNames.Length; i++)
        {
            GameManager.ItemAndQuantity newItem = new GameManager.ItemAndQuantity();
            newItem.name = initialPreset1Inventory.itemNames[i];
            newItem.quantity = initialPreset1Inventory.itemQuantities[i];
            items[i] = newItem;
        }
        StartCoroutine(LoadPlayerAttributes.LoadPresetEnum(equipment, initialPreset1Equipment.blowpipeAmmo, items, initialPreset1Inventory.blowpipeAmmo));
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

            InitializePresets(false);

            TickManager.beforeTick += BeforeTick;
        }
    }
    void InitializePresets(bool RestoreInitialPresets)
    {
        for (int i = 0; i < 3; i++)
        {
            string fileName = "Preset" + i + "Inventory";
            string fullPath = dir + fileName + extension;
            //if (Application.platform == RuntimePlatform.WebGLPlayer)
            if (true)
            {
                presetInventory[i] = new PresetInventory();
                if (RestoreInitialPresets)
                {
                    if (i == 0)
                    {
                        presetInventory[0] = initialPreset1Inventory;
                    }
                    if (i == 1)
                    {
                        presetInventory[1] = initialPreset2Inventory;
                    }
                    SavePresetWriteToData(i);
                }
                else
                {
                    if (i == 0)
                    {
                        presetInventory[i].filled = PlayerPrefs.GetInt("Preset Inventory " + (i + 1) + " filled", initialPreset1Inventory.filled ? 1 : 0) == 1;
                        for (int j = 0; j < presetInventory[i].itemNames.Length; j++)
                        {
                            presetInventory[i].itemNames[j] = PlayerPrefs.GetString("Preset Inventory " + (i + 1) + " item name " + j, initialPreset1Inventory.itemNames[j]);
                        }
                        for (int j = 0; j < presetInventory[i].itemQuantities.Length; j++)
                        {
                            presetInventory[i].itemQuantities[j] = PlayerPrefs.GetInt("Preset Inventory " + (i + 1) + " item quantity " + j, initialPreset1Inventory.itemQuantities[j]);
                        }
                        presetInventory[i].blowpipeAmmo = PlayerPrefs.GetString("Preset Inventory " + (i + 1) + " blowpipe ammo", initialPreset1Inventory.blowpipeAmmo);
                    }
                    else if (i == 1)
                    {
                        presetInventory[i].filled = PlayerPrefs.GetInt("Preset Inventory " + (i + 1) + " filled", initialPreset2Inventory.filled ? 1 : 0) == 1;
                        for (int j = 0; j < presetInventory[i].itemNames.Length; j++)
                        {
                            presetInventory[i].itemNames[j] = PlayerPrefs.GetString("Preset Inventory " + (i + 1) + " item name " + j, initialPreset2Inventory.itemNames[j]);
                        }
                        for (int j = 0; j < presetInventory[i].itemQuantities.Length; j++)
                        {
                            presetInventory[i].itemQuantities[j] = PlayerPrefs.GetInt("Preset Inventory " + (i + 1) + " item quantity " + j, initialPreset2Inventory.itemQuantities[j]);
                        }
                        presetInventory[i].blowpipeAmmo = PlayerPrefs.GetString("Preset Inventory " + (i + 1) + " blowpipe ammo", initialPreset2Inventory.blowpipeAmmo);
                    }
                    else
                    {
                        presetInventory[i].filled = PlayerPrefs.GetInt("Preset Inventory " + (i + 1) + " filled", 0) == 1;
                        for (int j = 0; j < presetInventory[i].itemNames.Length; j++)
                        {
                            presetInventory[i].itemNames[j] = PlayerPrefs.GetString("Preset Inventory " + (i + 1) + " item name " + j, "");
                        }
                        for (int j = 0; j < presetInventory[i].itemQuantities.Length; j++)
                        {
                            presetInventory[i].itemQuantities[j] = PlayerPrefs.GetInt("Preset Inventory " + (i + 1) + " item quantity " + j, 0);
                        }
                        presetInventory[i].blowpipeAmmo = PlayerPrefs.GetString("Preset Inventory " + (i + 1) + " blowpipe ammo", "");
                    }
                }
            }
            else if (File.Exists(fullPath))
            {
                string jsonString = File.ReadAllText(fullPath);
                presetInventory[i] = JsonUtility.FromJson<PresetInventory>(jsonString);
            }
            else
            {
                presetInventory[i] = new PresetInventory();
                if (i == 0)
                {
                    presetInventory[0] = initialPreset1Inventory;
                }
                if (i == 1)
                {
                    presetInventory[1] = initialPreset2Inventory;
                }
                string jsonString = JsonUtility.ToJson(presetInventory[i]);
                File.WriteAllText(fullPath, jsonString);
            }




            fileName = "Preset" + i + "Equipment";
            fullPath = dir + fileName + extension;
            //if (Application.platform == RuntimePlatform.WebGLPlayer)
            if (true)
            {
                presetEquipment[i] = new PresetEquipment();
                if (RestoreInitialPresets)
                {
                    if (i == 0)
                    {
                        presetEquipment[0] = initialPreset1Equipment;
                    }
                    if (i == 1)
                    {
                        presetEquipment[1] = initialPreset2Equipment;
                    }
                    SavePresetWriteToData(i);
                }
                else
                {
                    if (i == 0)
                    {
                        presetEquipment[i].filled = PlayerPrefs.GetInt("Preset Equipment " + (i + 1) + " filled", initialPreset1Equipment.filled ? 1 : 0) == 1;
                        for (int j = 0; j < presetEquipment[i].equipmentNames.Length; j++)
                        {
                            presetEquipment[i].equipmentNames[j] = PlayerPrefs.GetString("Preset Equipment " + (i + 1) + " item name " + j, initialPreset1Equipment.equipmentNames[j]);
                        }
                        for (int j = 0; j < presetEquipment[i].equipmentQuantities.Length; j++)
                        {
                            presetEquipment[i].equipmentQuantities[j] = PlayerPrefs.GetInt("Preset Equipment " + (i + 1) + " item quantity " + j, initialPreset1Equipment.equipmentQuantities[j]);
                        }
                        presetEquipment[i].blowpipeAmmo = PlayerPrefs.GetString("Preset Equipment " + (i + 1) + " blowpipe ammo", initialPreset1Equipment.blowpipeAmmo);
                    }
                    else if (i == 1)
                    {
                        presetEquipment[i].filled = PlayerPrefs.GetInt("Preset Equipment " + (i + 1) + " filled", initialPreset2Equipment.filled ? 1 : 0) == 1;
                        for (int j = 0; j < presetEquipment[i].equipmentNames.Length; j++)
                        {
                            presetEquipment[i].equipmentNames[j] = PlayerPrefs.GetString("Preset Equipment " + (i + 1) + " item name " + j, initialPreset2Equipment.equipmentNames[j]);
                        }
                        for (int j = 0; j < presetEquipment[i].equipmentQuantities.Length; j++)
                        {
                            presetEquipment[i].equipmentQuantities[j] = PlayerPrefs.GetInt("Preset Equipment " + (i + 1) + " item quantity " + j, initialPreset2Equipment.equipmentQuantities[j]);
                        }
                        presetEquipment[i].blowpipeAmmo = PlayerPrefs.GetString("Preset Equipment " + (i + 1) + " blowpipe ammo", initialPreset2Equipment.blowpipeAmmo);
                    }
                    else
                    {
                        presetEquipment[i].filled = PlayerPrefs.GetInt("Preset Equipment " + (i + 1) + " filled", 0) == 1;
                        for (int j = 0; j < presetEquipment[i].equipmentNames.Length; j++)
                        {
                            presetEquipment[i].equipmentNames[j] = PlayerPrefs.GetString("Preset Equipment " + (i + 1) + " item name " + j, "");
                        }
                        for (int j = 0; j < presetEquipment[i].equipmentQuantities.Length; j++)
                        {
                            presetEquipment[i].equipmentQuantities[j] = PlayerPrefs.GetInt("Preset Equipment " + (i + 1) + " item quantity " + j, 0);
                        }
                        presetEquipment[i].blowpipeAmmo = PlayerPrefs.GetString("Preset Equipment " + (i + 1) + " blowpipe ammo", "");
                    }
                }
            }
            else if (File.Exists(fullPath))
            {
                string jsonString = File.ReadAllText(fullPath);
                presetEquipment[i] = JsonUtility.FromJson<PresetEquipment>(jsonString);
            }
            else
            {
                presetEquipment[i] = new PresetEquipment();
                if (i == 0)
                {
                    presetEquipment[0] = initialPreset1Equipment;
                }
                if (i == 1)
                {
                    presetEquipment[1] = initialPreset2Equipment;
                }
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

        SavePresetWriteToData(presetNumber);

        GameLog.Log("Preset " + (presetNumber + 1) + " saved.");

        IgnoreActions = true;
    }
    void SavePresetWriteToData(int presetNumber)
    {
        //if (Application.platform == RuntimePlatform.WebGLPlayer)
        if (true)
        {
            PlayerPrefs.SetInt("Preset Inventory " + (presetNumber + 1) + " filled", presetInventory[presetNumber].filled ? 1 : 0);
            for (int j = 0; j < presetInventory[presetNumber].itemNames.Length; j++)
            {
                PlayerPrefs.SetString("Preset Inventory " + (presetNumber + 1) + " item name " + j, presetInventory[presetNumber].itemNames[j]);
            }
            for (int j = 0; j < presetInventory[presetNumber].itemQuantities.Length; j++)
            {
                PlayerPrefs.SetInt("Preset Inventory " + (presetNumber + 1) + " item quantity " + j, presetInventory[presetNumber].itemQuantities[j]);
            }
            PlayerPrefs.GetString("Preset Inventory " + (presetNumber + 1) + " blowpipe ammo", presetInventory[presetNumber].blowpipeAmmo);

            PlayerPrefs.SetInt("Preset Equipment " + (presetNumber + 1) + " filled", presetEquipment[presetNumber].filled ? 1 : 0);
            for (int j = 0; j < presetEquipment[presetNumber].equipmentNames.Length; j++)
            {
                PlayerPrefs.SetString("Preset Equipment " + (presetNumber + 1) + " item name " + j, presetEquipment[presetNumber].equipmentNames[j]);
            }
            for (int j = 0; j < presetEquipment[presetNumber].equipmentQuantities.Length; j++)
            {
                PlayerPrefs.SetInt("Preset Equipment " + (presetNumber + 1) + " item quantity " + j, presetEquipment[presetNumber].equipmentQuantities[j]);
            }
            PlayerPrefs.SetString("Preset Equipment " + (presetNumber + 1) + " blowpipe ammo", presetEquipment[presetNumber].blowpipeAmmo);
        }
        else
        {
            string fileName = "Preset" + presetNumber + "Equipment";
            string fullPath = dir + fileName + extension;
            string jsonString = JsonUtility.ToJson(presetEquipment[presetNumber]);
            File.WriteAllText(fullPath, jsonString);

            fileName = "Preset" + presetNumber + "Inventory";
            fullPath = dir + fileName + extension;
            jsonString = JsonUtility.ToJson(presetInventory[presetNumber]);
            File.WriteAllText(fullPath, jsonString);
        }
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
        GameLog.Log("Preset " + (num + 1) + " loaded.");
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

        GameLog.Log("Preset " + (presetNumber + 1) + " cleared.");

        //if (Application.platform == RuntimePlatform.WebGLPlayer)
        if (true)
        {
            PlayerPrefs.SetInt("Preset Inventory " + (presetNumber + 1) + " filled", 0);
            for (int j = 0; j < presetInventory[presetNumber].itemNames.Length; j++)
            {
                PlayerPrefs.SetString("Preset Inventory " + (presetNumber + 1) + " item name " + j, "");
            }
            for (int j = 0; j < presetInventory[presetNumber].itemQuantities.Length; j++)
            {
                PlayerPrefs.SetInt("Preset Inventory " + (presetNumber + 1) + " item quantity " + j, 0);
            }
            PlayerPrefs.GetString("Preset Inventory " + (presetNumber + 1) + " blowpipe ammo", "");

            PlayerPrefs.SetInt("Preset Equipment " + (presetNumber + 1) + " filled", 0);
            for (int j = 0; j < presetInventory[presetNumber].itemNames.Length; j++)
            {
                PlayerPrefs.SetString("Preset Equipment " + (presetNumber + 1) + " item name " + j, "");
            }
            for (int j = 0; j < presetInventory[presetNumber].itemQuantities.Length; j++)
            {
                PlayerPrefs.SetInt("Preset Equipment " + (presetNumber + 1) + " item quantity " + j, 0);
            }
            PlayerPrefs.SetString("Preset Equipment " + (presetNumber + 1) + " blowpipe ammo", "");

            return;
        }

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
    private void RestorePresets()
    {
        InitializePresets(true);
        GameLog.Log("Initial presets restored.");
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
