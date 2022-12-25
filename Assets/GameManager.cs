using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;
using System.Text;
using SimpleJSON;

public class GameManager : MonoBehaviour
{
    public static string folder = "/SaveData/";
    public static string fileName = "Hiscores";
    public static string extension = ".txt";

    public class Hiscores
    {
        public int deaths;
        public int completions;
        public int lowestLevel;
        public int leastDamage = -1;
        public double lowestValue = -1;
        public int fastestTicks;
        public float highestScore;
    }

    public class ItemAndQuantity
    {
        public string name;
        public int quantity = 1;
    }

    Dictionary<string, int> priceDictionary = new Dictionary<string, int>();

    public List<string> spawnedItems = new List<string>();

    public static Hiscores scores = new Hiscores();

    public static GameManager instance;

    private void Awake()
    {
        instance = this;
        if (Directory.Exists(Application.persistentDataPath + folder) == false)
        {
            Directory.CreateDirectory(Application.persistentDataPath + folder);
        }
        WriteScores();
        DontDestroyOnLoad(gameObject);
    }

    void WriteScores()
    {
        scores = new Hiscores();
        string dir = Application.persistentDataPath + folder + fileName + extension;
        if (File.Exists(dir))
        {
            string jsonString = File.ReadAllText(dir);
            scores = JsonUtility.FromJson<Hiscores>(jsonString);
        }
        else
        {
            string jsonString = JsonUtility.ToJson(scores);
            File.WriteAllText(dir, jsonString);
        }
    }

    public void ResetScores()
    {
        scores = new Hiscores();
        SaveScores();
        WriteScores();
    }

    public static void UpdateDeaths()
    {
        scores.deaths++;
        SaveScores();
    }


    public static void UpdateSuccessStats(int level, int damage, int ticks, float score, double lowestValue)
    {
        scores.completions++;
        if (level < scores.lowestLevel || scores.lowestLevel <= 0)
        {
            scores.lowestLevel = level;
        }
        if (damage < scores.leastDamage || scores.leastDamage < 0)
        {
            scores.leastDamage = damage;
        }
        if (lowestValue < scores.lowestValue || scores.lowestValue < 0)
        {
            scores.lowestValue = lowestValue;
        }
        if (ticks < scores.fastestTicks || scores.fastestTicks == 0)
        {
            scores.fastestTicks = ticks;
        }
        if (score > scores.highestScore)
        {
            scores.highestScore = score;
        }

        SaveScores();
    }

    public static void SaveScores()
    {
        string dir = Application.persistentDataPath + folder + fileName + extension;
        string jsonString = JsonUtility.ToJson(scores);
        File.WriteAllText(dir, jsonString);
    }

    public void StartGetPrices()
    {
        StartCoroutine(GetPrices());
    }
    IEnumerator GetPrices()
    {
        UnityWebRequest priceReq = new UnityWebRequest();
        priceReq.downloadHandler = new DownloadHandlerBuffer();

        string names = "";
        List<ItemAndQuantity> spawnedItemList = new List<ItemAndQuantity>();
        for (int i = 0; i < spawnedItems.Count; i++)
        {
            ItemAndQuantity newItem = new ItemAndQuantity();
            newItem.name = spawnedItems[i];
            newItem.quantity = 1;
            spawnedItemList.Add(newItem);
        }
        spawnedItemList = ParseItemNames(spawnedItemList);
        foreach (ItemAndQuantity item in spawnedItemList)
        {
            names += item.name + "|";
        }
        names += "Crystal armour seed|Eldritch orb|Nightmare staff";
        

        priceReq.url = "https://api.weirdgloop.org/exchange/history/osrs/latest?name=" + names;
        yield return priceReq.SendWebRequest();
        string jsonString = Encoding.Default.GetString(priceReq.downloadHandler.data);
        JSONNode node = JSON.Parse(jsonString);
        foreach (KeyValuePair<string, JSONNode> kvp in node)
        {
            priceDictionary.Add(kvp.Key, int.Parse(kvp.Value["price"].Value));
        }

        priceDictionary.Add("Crystal helm", priceDictionary["Crystal armour seed"]);
        priceDictionary.Add("Crystal legs", priceDictionary["Crystal armour seed"] * 2);
        priceDictionary.Add("Crystal body", priceDictionary["Crystal armour seed"] * 3);
        priceDictionary.Add("Eldritch nightmare staff", priceDictionary["Eldritch orb"] + priceDictionary["Nightmare staff"]);
    }

    public double TotalCarriedValue()
    {
        double total = 0;
        List<ItemAndQuantity> items = new List<ItemAndQuantity>();
        foreach (GameObject slot in Inventory.inventorySlots)
        {
            Item item = slot.GetComponentInChildren<Item>();
            if (item != null)
            {
                ItemAndQuantity newItem = new ItemAndQuantity();
                newItem.name = item.name;
                if (item.isStackable)
                {
                    newItem.quantity = item.stackScript.quantity;
                }
                else
                {
                    newItem.quantity = 1;
                }
                items.Add(newItem);
            }
        }

        foreach (Transform slot in WornEquipment.slots)
        {
            Item item = slot.GetComponentInChildren<Item>();
            if (item != null)
            {
                ItemAndQuantity newItem = new ItemAndQuantity();
                newItem.name = item.name;
                if (item.isStackable)
                {
                    newItem.quantity = item.stackScript.quantity;
                }
                else
                {
                    newItem.quantity = 1;
                }
                items.Add(newItem);
            }
        }

        List<ItemAndQuantity> newItems = ParseItemNames(items);

        for (int i = 0; i < newItems.Count; i++)
        {
            total += priceDictionary[newItems[i].name] * newItems[i].quantity;
        }

        return total;
    }

    public double GetItemPrice(string name)
    {
        string realName = ParseItemStrings(name);

        if (string.IsNullOrEmpty(realName))
        {
            return -1;
        }

        if (priceDictionary.ContainsKey(realName))
        {
            return priceDictionary[realName];
        }

        return -1;
    }

    List<ItemAndQuantity> ParseItemNames(List<ItemAndQuantity> items)
    {
        List<ItemAndQuantity> newItems = new List<ItemAndQuantity>();
        foreach (ItemAndQuantity item in items)
        {
            string realItem = ParseItemStrings(item.name);

            if (string.IsNullOrEmpty(realItem))
            {
                continue;
            }

            ItemAndQuantity newItem = new ItemAndQuantity();
            newItem.name = realItem;
            newItem.quantity = item.quantity;

            newItems.Add(newItem);
        }

        return newItems;
    }

    string ParseItemStrings(string itemName)
    {
        string realItem = itemName;
        if (itemName == "Bow of faerdhinen")
        {
            realItem = "Enhanced crystal weapon seed";
        }
        else if (itemName == "Toxic blowpipe")
        {
            realItem = "Toxic blowpipe (empty)";
        }
        else if (itemName.Contains("Ring of suffering"))
        {
            realItem = "Ring of suffering";
        }
        else if (itemName.Contains("Slayer helmet"))
        {
            realItem = "Black mask";
        }
        else if (itemName == "Crystal shield")
        {
            realItem = "Crystal weapon seed";
        }
        else if (itemName.Contains("potion") || itemName == "Saradomin brew" || itemName == "Super restore")
        {
            if (itemName.EndsWith(")"))
            {
                realItem = realItem.Remove(realItem.Length - 3);
            }
            realItem += "(4)";
        }
        else if (itemName == "Tumeken's shadow" || itemName == "Sanguinesti staff")
        {
            realItem = itemName + " (uncharged)";
        }
        else if (itemName.Contains("Trident of the swamp"))
        {
            realItem = "Uncharged toxic trident";
        }
        else if (itemName.Contains("(i)"))
        {
            realItem = itemName.Remove(itemName.Length - 4);
        }

        else if (itemName.Contains("assembler") || itemName.Contains("Ranging cape") || itemName == "Rada's blessing 4" || itemName == "Book of law" ||
                 itemName == "Imbued zamorak cape" || itemName == "Rotten potato bow" || itemName == "Barrows gloves")
        {
            realItem = null;
        }

        return realItem;
    }
}
