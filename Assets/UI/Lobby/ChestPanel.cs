using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChestPanel : MonoBehaviour
{
    public GameObject image;

    public int itemsPerRow;
    public int itemsPerColumn;

    public RectTransform nwCorner;
    public RectTransform neCorner;
    public RectTransform swCorner;
    public RectTransform seCorner;

    public float itemScale = 0.6f;

    public List<GameObject> items = new List<GameObject>();
    List<GameObject> spawnedItems = new List<GameObject>();

    Vector2 onPosition;

    private IEnumerator Start()
    {
        PanelButtons.instance.ForceOpen("Inventory");

        yield return null;

        foreach (GameObject item in items)
        {
            if (item != null)
            {
                GameObject newItem = Instantiate(item, Vector2.one * -1000, Quaternion.identity);
                newItem.name = item.name;
                spawnedItems.Add(newItem);
            }
        }

        yield return null;
        yield return null;

        int numItems = itemsPerRow * itemsPerColumn;

        float spacingX = ((float)neCorner.position.x - (float)nwCorner.position.x) / (itemsPerRow - 1);
        float spacingY = ((float)neCorner.position.y - (float)seCorner.position.y) / (itemsPerColumn - 1);

        Vector2 position = nwCorner.position;
        int columnNumber = 1;
        int rowNumber = 1;

        for (int i = 0; i < spawnedItems.Count; i++)
        {
            if (i < spawnedItems.Count && spawnedItems[i] != null)
            {
                float positionX = nwCorner.position.x + (columnNumber - 1) * spacingX;
                float positionY = nwCorner.position.y - (rowNumber - 1) * spacingY;
                position = new Vector2(positionX, positionY);

                GameObject newImage = Instantiate(image, position, Quaternion.identity);
                newImage.GetComponent<RawImage>().texture = spawnedItems[i].GetComponent<Item>().itemTexture;
                newImage.GetComponent<ChestPanelItem>().examineText = spawnedItems[i].GetComponent<Action>().examineText;
                newImage.name = items[i].name;
                newImage.transform.localScale = Vector2.one * itemScale * FindObjectOfType<Canvas>().scaleFactor;
                newImage.transform.SetParent(transform);

                columnNumber++;
                if (columnNumber > itemsPerRow)
                {
                    columnNumber = 1;
                    rowNumber++;
                }
            }
            else
            {
                break;
            }
        }

        foreach (GameObject item in spawnedItems)
        {
            Destroy(item);
        }
    }
}
