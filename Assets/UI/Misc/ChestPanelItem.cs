using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ChestPanelItem : MonoBehaviour, IPointerClickHandler
{
    Action itemAction;

    public string examineText = "asdfas";

    private void Start()
    {
        itemAction = GetComponent<Action>();

        itemAction.menuTexts[0] = "Take ";
        itemAction.objectName = "<color=orange>" + gameObject.name + "</color>";
        itemAction.serverAction0 += TakeItem;
        itemAction.examineText = examineText;
        itemAction.UpdateName();
    }

    void TakeItem()
    {
        GameObject newItem = Instantiate(Tools.LoadFromResource(gameObject.name));
        newItem.name = gameObject.name;
        if (newItem.GetComponent<StackableItem>() != null)
        {
            newItem.GetComponent<StackableItem>().quantity = 2000;
        }
        if (newItem.GetComponent<ChargeItem>() != null)
        {
            newItem.GetComponent<ChargeItem>().charges = 2000;
        }
        Inventory.instance.PlaceInInventory(newItem);
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {

    }
}
