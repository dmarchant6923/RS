using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class XPDrop : MonoBehaviour
{
    public GameObject xpDrop;
    public Texture attack;
    public Texture strength;
    public Texture defense;
    public Texture range;
    public Texture mage;
    public Texture hitpoints;
    public static XPDrop instance;
    public static List<GameObject> activeXPDrops = new List<GameObject>();
    [System.NonSerialized] public float speed = 150;

    public class LoadedDrop
    {
        public Texture tex;
        public int number;
        public bool offensivePrayer = false;
    }
    public static List<LoadedDrop> loadedXPDrops = new List<LoadedDrop>();

    private void Start()
    {
        activeXPDrops = new List<GameObject>();
        instance = this;

        TickManager.afterTick += AfterTick;
    }

    private void Update()
    {
        for (int i = 0; i < activeXPDrops.Count; i++)
        {
            activeXPDrops[i].transform.localPosition += Vector3.up * speed * Time.deltaTime;
            if (activeXPDrops[i].GetComponent<RectTransform>().position.y > Screen.height + 100)
            {
                Destroy(activeXPDrops[i]);
                activeXPDrops.RemoveAt(i);
                i--;
            }
        }
    }

    void AfterTick()
    {
        for (int i = 0; i < loadedXPDrops.Count; i++)
        {
            GameObject newDrop = Instantiate(instance.xpDrop, instance.transform);
            newDrop.transform.localPosition = Vector2.up * i * 30;
            newDrop.GetComponentInChildren<Text>().text = loadedXPDrops[i].number.ToString();
            if (loadedXPDrops[i].offensivePrayer)
            {
                newDrop.GetComponentInChildren<Text>().color = Color.cyan;
            }
            newDrop.GetComponent<RawImage>().texture = loadedXPDrops[i].tex;
            activeXPDrops.Add(newDrop);
        }

        loadedXPDrops = new List<LoadedDrop>();
    }

    public static void CombatXPDrop(string style, string type, int damage, bool offensivePrayer)
    {
        CombatXPDrop(style, type, damage, 1, offensivePrayer);
    }
    public static void CombatXPDrop(string style, string type, float damage, float mult, bool offensivePrayer)
    {
        SpawnXPDrop(instance.hitpoints, damage * 1.33f * mult, offensivePrayer);
        if (type == AttackStyles.controlledType)
        {
            SpawnXPDrop(instance.attack, damage * 1.33f * mult, offensivePrayer);
            SpawnXPDrop(instance.strength, damage * 1.33f * mult, offensivePrayer);
            SpawnXPDrop(instance.defense, damage * 1.33f * mult, offensivePrayer);
        }
        else if (style == AttackStyles.rangedStyle && type == AttackStyles.longrangeType)
        {
            SpawnXPDrop(instance.range, damage * 2f * mult, offensivePrayer);
            SpawnXPDrop(instance.defense, damage * 2f * mult, offensivePrayer);
        }
        else if (style == AttackStyles.magicStyle && (type == AttackStyles.defensiveType || type == AttackStyles.longrangeType))
        {
            SpawnXPDrop(instance.mage, damage * 1.33f * mult, offensivePrayer);
            SpawnXPDrop(instance.defense, damage * 1f * mult, offensivePrayer);
        }

        else if (style == AttackStyles.magicStyle)
        {
            SpawnXPDrop(instance.mage, damage * 2f * mult, offensivePrayer);
        }
        else if (style == AttackStyles.rangedStyle)
        {
            SpawnXPDrop(instance.range, damage * 4f * mult, offensivePrayer);
        }
        else if (type == AttackStyles.accurateType)
        {
            SpawnXPDrop(instance.attack, damage * 4f * mult, offensivePrayer);
        }
        else if (type == AttackStyles.aggressiveType)
        {
            SpawnXPDrop(instance.strength, damage * 4f * mult, offensivePrayer);
        }
        else if (type == AttackStyles.defensiveType)
        {
            SpawnXPDrop(instance.defense, damage * 4f * mult, offensivePrayer);
        }
    }
    public static void SkillXPDrop(string skill, float number)
    {
        if (skill == "Attack")
        {
            SpawnXPDrop(instance.attack, number);
        }
        if (skill == "Strength")
        {
            SpawnXPDrop(instance.strength, number);
        }
        if (skill == "Defence" || skill == "Defense")
        {
            SpawnXPDrop(instance.defense, number);
        }
        if (skill == "Ranged")
        {
            SpawnXPDrop(instance.range, number);
        }
        if (skill == "Magic")
        {
            SpawnXPDrop(instance.mage, number);
        }
        if (skill == "Hitpoints")
        {
            SpawnXPDrop(instance.hitpoints, number);
        }
    }

    public static void SpawnXPDrop(Texture tex, float number)
    {
        SpawnXPDrop(tex, number, false);
    }

    public static void SpawnXPDrop(Texture tex, float number, bool offensivePrayer)
    {
        foreach (LoadedDrop drop in loadedXPDrops)
        {
            if (drop.tex == tex)
            {
                drop.number += Mathf.FloorToInt(number);
                if (drop.offensivePrayer == false)
                {
                    drop.offensivePrayer = offensivePrayer;
                }
                return;
            }
        }

        LoadedDrop newDrop = new LoadedDrop();
        newDrop.tex = tex;
        newDrop.number = Mathf.FloorToInt(number);
        newDrop.offensivePrayer = offensivePrayer;
        loadedXPDrops.Add(newDrop);
    }
}
