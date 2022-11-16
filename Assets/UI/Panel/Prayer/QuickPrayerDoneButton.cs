using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class QuickPrayerDoneButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Texture buttonOff;
    public Texture buttonOn;
    RawImage button;

    Action doneAction;
    public Prayer prayerScript;
    Text buttonText;
    Color textColor;

    private void Start()
    {
        doneAction = GetComponent<Action>();
        doneAction.menuTexts[0] = "Done";
        doneAction.clientAction0 += ClientClickDone;
        doneAction.serverAction0 += ServerClickDone;

        button = GetComponent<RawImage>();
        buttonText = button.GetComponentInChildren<Text>();
        textColor = buttonText.color;
    }

    void ClientClickDone()
    {
        button.texture = buttonOn;
    }
    void ServerClickDone()
    {
        prayerScript.DeselectQuickPrayers();
    }

    private void OnDisable()
    {
        button.texture = buttonOff;
        buttonText.color = textColor;

        foreach (GameObject prayer in Prayer.prayers)
        {
            PlayerPrefs.SetInt(prayer.name, prayer.GetComponent<ActivatePrayer>().QPeffectiveActive ? 1 : 0);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonText.color = Color.white;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        buttonText.color = textColor;
    }
}
