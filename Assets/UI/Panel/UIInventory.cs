using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInventory : MonoBehaviour
{
    public RectTransform panel;
    public GameObject itemParent;

    [HideInInspector] public RectTransform[] items;
    Vector2[] itemPositions = new Vector2[28];

    private void Start()
    {
        items = new RectTransform[28];

        foreach (RectTransform rt in itemParent.GetComponentsInChildren<RectTransform>())
        {
            for (int i = 0; i < 28; i++)
            {
                if (items[i] == null)
                {
                    items[i] = rt;
                    break;
                }
            }
        }

        Vector2 panelAnchor = panel.position;
        float panelWidth = panel.rect.width;
        float panelHeight = panel.rect.height;
        float widthIncrement = panelWidth / 5;
        float heightIncrement = panelHeight / 8;

        Vector2 br = panelAnchor + new Vector2(-widthIncrement / 2, heightIncrement / 2);

        for (int j = 0; j < 7; j++)
        {
            for (int i = 0; i < 4; i++)
            {
                int index = (j * 4) + (i % 4);
                float height = br.y + (6 - j) * heightIncrement;
                float width = br.x - (3 - i) * widthIncrement;
                Vector2 position = new Vector2(width, height);

                items[index].position = position;
            }
        }
    }
}
