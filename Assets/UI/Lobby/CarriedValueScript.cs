using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarriedValueScript : MonoBehaviour
{
    [HideInInspector] public Text valueText;

    private void Awake()
    {
        valueText = GetComponent<Text>();
    }

    public void UpdateValue()
    {
        StartCoroutine(UpdateValueCR());
    }

    IEnumerator UpdateValueCR()
    {
        if (GameManager.foundPrices == false)
        {
            valueText.text = "-";
            yield break;
        }
        yield return new WaitForSeconds(0.05f);
        valueText.text = "Carried value: " + Tools.DoubleToCashValue(GameManager.instance.TotalCarriedValue());
    }
}
