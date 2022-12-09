using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Potion : MonoBehaviour
{
    public Texture dose4;
    public Texture dose3;
    public Texture dose2;
    public Texture dose1;
    public Color potionColor;

    public int startingDose = 4;
    int currentDose;
    string initialName;
    [HideInInspector] public bool divine;

    public delegate void PotionEffects();
    public event PotionEffects potionDrank;

    Action potionAction;
    Item itemScript;


    private IEnumerator Start()
    {
        potionAction = GetComponent<Action>();
        itemScript = GetComponent<Item>();
        initialName = gameObject.name;

        dose4 = CreatePotionTexture(4);
        dose3 = CreatePotionTexture(3);
        dose2 = CreatePotionTexture(2);
        dose1 = CreatePotionTexture(1);

        yield return null;
        currentDose = startingDose;

        itemScript.menuTexts[0] = "Drink ";
        potionAction.orderLevels[0] = -1;
        potionAction.serverAction0 += Drink;
        potionAction.cancelLevels[0] = 1;
        UpdateDose();
    }

    void Drink()
    {
        if (PlayerStats.drinkDelay != 0)
        {
            return;
        }

        currentDose--;
        potionDrank?.Invoke();
        if (currentDose == 0)
        {
            Destroy(gameObject);
        }
        UpdateDose();
    }

    void UpdateDose()
    {
        itemScript.UpdateActions(initialName + "(" + currentDose + ")");
        string word = " doses of ";
        if (currentDose == 1)
        {
            word = " dose of ";
        }
        potionAction.examineText = currentDose + word + initialName.ToLower() + ".";
        if (currentDose == 4)
        {
            itemScript.itemTexture = dose4;
        }
        else if (currentDose == 3)
        {
            itemScript.itemTexture = dose3;
        }
        else if (currentDose == 2)
        {
            itemScript.itemTexture = dose2;
        }
        else if (currentDose == 1)
        {
            itemScript.itemTexture = dose1;
        }
        itemScript.UpdateImage();
    }

    public void DropPotion(GroundItem newItem)
    {
        newItem.doses = currentDose;
        newItem.name += "(" + currentDose + ")";
    }

    public Texture2D CreatePotionTexture(int dose)
    {
        List<Texture2D> textures = new List<Texture2D>();
        List<Color> colors = new List<Color>();
        Texture texture = dose4;
        if (dose == 3)
        {
            texture = dose3;
        }
        else if (dose == 2)
        {
            texture = dose2;
        }
        else if (dose == 1)
        {
            texture = dose1;
        }

        textures.Add((Texture2D)itemScript.itemTexture); colors.Add(Color.white);
        textures.Add((Texture2D) texture); colors.Add(potionColor);
        if (divine)
        {
            textures.Add((Texture2D)transform.GetChild(3).GetComponent<RawImage>().texture); 
            colors.Add(Color.white);
        }

        return CreateTexture.CreateLayeredTexture(textures, colors);
    }
}
