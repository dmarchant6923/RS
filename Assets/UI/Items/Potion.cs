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
    public RawImage doseImage;
    public Color potionColor;

    public int startingDose = 4;
    int currentDose;
    string initialName;

    public delegate void PotionEffects();
    public event PotionEffects potionDrank;

    Action potionAction;
    Item itemScript;

    private IEnumerator Start()
    {
        potionAction = GetComponent<Action>();
        itemScript = GetComponent<Item>();
        doseImage.color = potionColor;
        initialName = gameObject.name;

        itemScript.darkenItem += DarkenItem;
        itemScript.undarkenItem += UndarkenItem;

        yield return null;
        currentDose = startingDose;

        itemScript.menuTexts[0] = "Drink ";
        potionAction.orderLevels[0] = -1;
        potionAction.serverAction0 += Drink;
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
        potionAction.examineText = currentDose + word + initialName.ToLower() + " potion.";
        if (currentDose == 4)
        {
            doseImage.texture = dose4;
        }
        else if (currentDose == 3)
        {
            doseImage.texture = dose3;
        }
        else if (currentDose == 2)
        {
            doseImage.texture = dose2;
        }
        else if (currentDose == 1)
        {
            doseImage.texture = dose1;
        }
    }

    void DarkenItem()
    {
        Color color = doseImage.color;
        color.a = 0.6f;
        doseImage.color = color;
    }
    void UndarkenItem()
    {
        Color color = doseImage.color;
        color.a = 1f;
        doseImage.color = color;
    }
}
