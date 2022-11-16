using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartWithItems : MonoBehaviour
{
    public List<GameObject> items = new List<GameObject>();
    public List<int> quantities = new List<int>();

    public Inventory inventory;

    private IEnumerator Start()
    {
        yield return null;

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] != null)
            {
                GameObject newItem = Instantiate(items[i], transform);
                if (newItem.GetComponent<StackableItem>() != null)
                {
                    if (quantities[i] != 0)
                    {
                        newItem.GetComponent<StackableItem>().quantity = quantities[i];
                    }
                    else
                    {
                        newItem.GetComponent<StackableItem>().quantity = 1;
                    }
                }
                newItem.name = items[i].name;
            }
        }

        inventory.SortInventory();
    }
}
