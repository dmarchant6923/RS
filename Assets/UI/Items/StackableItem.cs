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
}
