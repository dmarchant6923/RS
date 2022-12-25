using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarriedValueScript : MonoBehaviour
{
    Text valueText;

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
        yield return new WaitForSeconds(0.05f);
        valueText.text = "Carried value: " + Tools.DoubleToCashValue(GameManager.instance.TotalCarriedValue());
    }
}
