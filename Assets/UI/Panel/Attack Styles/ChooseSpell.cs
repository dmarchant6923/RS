using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseSpell : MonoBehaviour
{
    public RectTransform panel;
    public static List<GameObject> combatSpellObjects = new List<GameObject>();
    public static List<Spell> combatSpells = new List<Spell>();

    

    private IEnumerator Start()
    {
        yield return null;

        foreach (Spell spell in Spellbook.spells)
        {
            if (spell.combatSpell)
            {
                GameObject newSpell = Instantiate(spell.gameObject);
                newSpell.transform.SetParent(transform);
                combatSpells.Add(newSpell.GetComponent<Spell>());
                newSpell.GetComponent<Spell>().onAutocastSelectPanel = true;
                newSpell.GetComponent<Spell>().chooseSpellScript = this;
                newSpell.GetComponent<Spell>().attackStyleScript = transform.parent.GetComponent<AttackStyles>();
                newSpell.name = spell.name;
            }
        }

        int spellsPerRow = 4;
        int numberOfSpells = combatSpells.Count;
        float panelScalex = panel.rect.width;
        float panelScaley = panel.rect.height;
        float widthIncrement = panelScalex / (spellsPerRow + 1);
        float heightIncrement = panelScaley / 8;
        float widthScale = 2.68f;
        float heightScale = 2.2f;

        for (int j = 0; j < 7; j++)
        {
            for (int i = 0; i < spellsPerRow; i++)
            {
                int index = (j * spellsPerRow) + (i % spellsPerRow);
                if (index > numberOfSpells - 1)
                {
                    break;
                }
                float width = -(widthIncrement * (float)spellsPerRow / widthScale) + widthIncrement * i;
                float height = (heightIncrement * 7 / heightScale) - heightIncrement * j;

                //float height = br.y + (6 - j) * heightIncrement;
                //float width = br.x - (spellsPerRow - 1 - i) * widthIncrement;
                Vector2 position = new Vector2(width, height);
                combatSpells[index].GetComponent<RectTransform>().localPosition = position;
            }
        }

        Spellbook.runesUpdated += CountRunes;
    }

    void CountRunes()
    {
        foreach (Spell spell in combatSpells)
        {
            spell.CheckRunes();
        }
    }
}
