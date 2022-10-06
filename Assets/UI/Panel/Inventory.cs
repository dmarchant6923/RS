using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public RectTransform panel;
    public GameObject itemParent;
    float panelScale;

    Item[] items;
    RectTransform[] itemRTs;
    Vector2[] itemPositions = new Vector2[28];

    private void Start()
    {
        items = new Item[28];
        itemRTs = new RectTransform[28];

        foreach (Item item in itemParent.GetComponentsInChildren<Item>())
        {
            for (int i = 0; i < 28; i++)
            {
                if (items[i] == null)
                {
                    items[i] = item;
                    itemRTs[i] = item.GetComponent<RectTransform>();
                    break;
                }
            }
        }

        panelScale = panel.transform.localScale.x;

        Vector2 panelAnchor = panel.position;
        float panelWidth = panel.rect.width * panelScale;
        float panelHeight = panel.rect.height * panelScale;
        Vector2 center = panelAnchor + new Vector2(-panelWidth / 2, panelHeight / 2);
        float widthIncrement = panelWidth * 0.9f / 4;
        float heightIncrement = panelHeight * 0.9f / 7;
        Vector2 br = center += new Vector2(widthIncrement * 1.5f, -heightIncrement * 3);

        for (int j = 0; j < 7; j++)
        {
            for (int i = 0; i < 4; i++)
            {
                int index = (j * 4) + (i % 4);
                float height = br.y + (6 - j) * heightIncrement;
                float width = br.x - (3 - i) * widthIncrement;
                Vector2 position = new Vector2(width, height);
                itemPositions[index] = position;

                if (items[index] != null)
                {
                    itemRTs[index].position = position;
                    items[index].gameObject.SetActive(true);
                }
            }
        }

        //items[0].position = center;
        //items[1].position = br;
        //items[2].position = panelAnchor;
    }
}
