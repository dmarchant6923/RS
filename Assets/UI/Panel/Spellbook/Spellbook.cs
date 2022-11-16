using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spellbook : MonoBehaviour
{
    public Transform ancientsParent;
    Transform spellsParent;
    int spellsPerRow;
    int numberOfSpells;

    public bool ancients;

    public static List<Spell> spells = new List<Spell>();

    float panelScale;
    public RectTransform panel;
    float panelWidthScale;
    float panelHeightScale;
    float brWidthScale;
    float brHeightScale;

    public Inventory inventory;
    public static List<StackableItem> runesInInventory = new List<StackableItem>();
    public delegate void Runes();
    public static event Runes runesUpdated;

    public static RuneSource runeSource;

    void Start()
    {
        spells = new List<Spell>();
        if (ancients)
        {
            spellsParent = ancientsParent;
            spellsPerRow = 4;
            panelWidthScale = 0.89f;
            panelHeightScale = 0.75f;
            brWidthScale = 1.5f;
            brHeightScale = 2.2f;
        }
        for (int i = 0; i < spellsParent.GetComponentsInChildren<Spell>().Length; i++)
        {
            spells.Add(spellsParent.GetComponentsInChildren<Spell>()[i]);
        }
        numberOfSpells = spells.Count;

        panelScale = panel.transform.localScale.x;
        Vector2 panelAnchor = panel.position;
        float panelWidth = panel.rect.width * panelScale;
        float panelHeight = panel.rect.height * panelScale;
        Vector2 center = panelAnchor + new Vector2(-panelWidth / 2, panelHeight / 2);
        float widthIncrement = panelWidth * panelWidthScale / spellsPerRow;
        float heightIncrement = panelHeight * panelHeightScale / 7;
        Vector2 br = center += new Vector2(widthIncrement * brWidthScale, -heightIncrement * brHeightScale);

        for (int j = 0; j < 7; j++)
        {
            for (int i = 0; i < spellsPerRow; i++)
            {
                int index = (j * spellsPerRow) + (i % spellsPerRow);
                if (index > numberOfSpells - 1)
                {
                    break;
                }
                float height = br.y + (6 - j) * heightIncrement;
                float width = br.x - (spellsPerRow - 1 - i) * widthIncrement;
                Vector2 position = new Vector2(width, height);
                spells[index].GetComponent<RectTransform>().position = position;
            }
        }

        TickManager.onTick += CountRunes;

        runeSource = null;
}

    public static void CountRunes()
    {
        runesInInventory = new List<StackableItem>();
        foreach (GameObject slot in Inventory.inventorySlots)
        {
            if (slot.GetComponentInChildren<StackableItem>() != null)
            {
                StackableItem item = slot.GetComponentInChildren<StackableItem>();
                if (item.gameObject.name.Contains(" rune"))
                {
                    runesInInventory.Add(item);
                }
            }
        }

        runesUpdated();

        foreach (Spell spell in spells)
        {
            spell.CheckRunes();
        }
    }
}
