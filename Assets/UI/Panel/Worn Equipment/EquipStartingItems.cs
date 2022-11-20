using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipStartingItems : MonoBehaviour
{
    public GameObject head;
    public GameObject cape;
    public GameObject neck;
    public GameObject ammo;
    public GameObject weapon;
    public GameObject body;
    public GameObject shield;
    public GameObject leg;
    public GameObject glove;
    public GameObject boot;
    public GameObject ring;
    GameObject[] equipment = new GameObject[11];

    public int startingAmmo = 1;

    private IEnumerator Start()
    {
        equipment[0] = head;
        equipment[1] = cape;
        equipment[2] = neck;
        equipment[3] = ammo;
        equipment[4] = weapon;
        equipment[5] = body;
        equipment[6] = shield;
        equipment[7] = leg;
        equipment[8] = glove;
        equipment[9] = boot;
        equipment[10] = ring;

        List<GameObject> newItems = new List<GameObject>();

        for (int i = 0; i < equipment.Length; i++)
        {
            if (equipment[i] != null)
            {
                GameObject newItem = Instantiate(equipment[i]);
                newItem.name = equipment[i].name;
                newItems.Add(newItem);
                if (i == 3)
                {
                    newItem.GetComponent<StackableItem>().quantity = startingAmmo;
                }
            }
        }

        yield return null;

        foreach(GameObject item in newItems)
        {
            item.GetComponent<Equipment>().Equip();
        }
    }
}
