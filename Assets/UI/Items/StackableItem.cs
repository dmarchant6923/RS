using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StackableItem : MonoBehaviour
{
    public RawImage maskImage;
    public RawImage itemImage;

    public Texture image1;
    public Texture image2;
    public Texture image3;
    public Texture image4;
    public Texture image5;

    public int threshold2;
    public int threshold3;
    public int threshold4;
    public int threshold5;

    public int quantity = 1;
    public Text quantityText;

    public Item itemScript;

    [HideInInspector] public bool groundItem = false;

    private void Start()
    {
        itemScript = GetComponent<Item>();

        ChooseImage();
    }

    void ChooseImage()
    {
        if (quantity <= 0)
        {
            Destroy(gameObject);
        }

        if (image5 != null && quantity >= threshold5)
        {
            maskImage.texture = image5;
            itemImage.texture = image5;
        }
        else if (image4 != null && quantity >= threshold4)
        {
            maskImage.texture = image4;
            itemImage.texture = image4;
        }
        else if (image3 != null && quantity >= threshold3)
        {
            maskImage.texture = image3;
            itemImage.texture = image3;
        }
        else if (image2 != null && quantity >= threshold2)
        {
            maskImage.texture = image2;
            itemImage.texture = image2;
        }
        else
        {
            maskImage.texture = image1;
            itemImage.texture = image1;
        }

        quantityText.text = quantity.ToString();
    }

    public void NewQuantity(int newQuantity)
    {
        quantity = newQuantity;
        ChooseImage();
    }

    public void AddToQuantity(int increment)
    {
        quantity += increment;
        ChooseImage();
    }

    public void UseRangedAmmo(Vector2 targetTile)
    {
        float rand = Random.Range(0f, 1f);
        if (WornEquipment.assembler)
        {
            if (rand < 0.2f)
            {
                AddToQuantity(-1);
            }
        }
        else if (WornEquipment.accumulator)
        {
            if (rand < 0.2f)
            {
                AddToQuantity(-1);
            }
            else if (rand < 0.28f)
            {
                AddToQuantity(-1);
                Spawn(targetTile);
            }
        }
        else
        {
            AddToQuantity(-1);
            if (rand > 0.07f)
            {
                Spawn(targetTile);
            }
        }
    }

    public void Spawn(Vector2 tile)
    {
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(tile);
        RaycastHit2D[] castAll = Physics2D.CircleCastAll(worldPoint, 0.1f, Vector2.zero, 0);
        StackableItem existingItem = null;
        foreach (RaycastHit2D cast in castAll)
        {
            if (cast.collider.name == gameObject.name)
            {
                existingItem = cast.collider.GetComponent<StackableItem>();
            }
        }

        if (existingItem != null)
        {
            existingItem.AddToQuantity(1);
        }
        else
        {
            GameObject newItem = Instantiate(itemScript.groundPrefab, tile, Quaternion.identity);
            newItem.gameObject.name = itemScript.itemAction.objectName;
            newItem.GetComponent<GroundItem>().item = gameObject;
            newItem.GetComponent<GroundItem>().trueTile = tile;
            newItem.GetComponent<Action>().examineText = itemScript.itemAction.examineText;
            UnityEditorInternal.ComponentUtility.CopyComponent(this);
            UnityEditorInternal.ComponentUtility.PasteComponentAsNew(newItem);
            newItem.GetComponent<StackableItem>().groundItem = true;
            newItem.GetComponent<StackableItem>().quantity = 1;
        }
    }
}
