using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundItemTile : MonoBehaviour
{
    public List<SpriteRenderer> groundItems = new List<SpriteRenderer>();
    public int maxVisible = 4;

    public void AddGroundItem(SpriteRenderer item)
    {
        item.sortingOrder = -groundItems.Count;
        groundItems.Add(item);
        if (groundItems.Count > maxVisible)
        {
            item.GetComponent<SpriteRenderer>().enabled = false;
        }
        item.GetComponent<GroundItem>().itemTileObject = this;
    }

    public void RemoveGroundItem(SpriteRenderer item)
    {
        for (int i = 0; i < groundItems.Count; i++)
        {
            if (groundItems[i] == item)
            {
                groundItems.RemoveAt(i);
                i--;
            }
        }

        if (groundItems.Count == 0)
        {
            Destroy(gameObject);
            return;
        }

        for (int i = 0; i < groundItems.Count; i++)
        {
            groundItems[i].sortingOrder = -i;
            if (i >= maxVisible)
            {
                groundItems[i].enabled = false;
            }
            else
            {
                groundItems[i].enabled = true;
            }
            Debug.Log(groundItems[i].name + " " + groundItems[i].sortingOrder);
        }
    }
}
