using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelButtons : MonoBehaviour
{
    public Sprite onSprite;
    public Sprite offSprite;

    public Transform attackStyles;
    public Transform inventory;
    public Transform wornEquipment;
    public Transform prayer;
    public Transform spellbook;
    Transform currentButton;
    Transform[] buttons = new Transform[5];

    public GameObject panel;

    private void Start()
    {
        buttons[0] = attackStyles;
        buttons[1] = inventory;
        buttons[2] = wornEquipment;
        buttons[3] = prayer;
        buttons[4] = spellbook;
    }

    public void OnClick(Transform selectedButton)
    {
        foreach (Transform button in buttons)
        {
            button.GetComponent<Image>().sprite = offSprite;
        }
        if (selectedButton == currentButton)
        {
            selectedButton.GetComponent<Image>().sprite = offSprite;
            currentButton = null;
            panel.SetActive(false);
        }
        else
        {
            selectedButton.GetComponent<Image>().sprite = onSprite;
            currentButton = selectedButton;
            panel.SetActive(true);
        }
    }
}
