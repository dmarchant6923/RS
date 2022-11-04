using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionManager : MonoBehaviour
{
    Player player;
    public bool showTrueTile;
    public bool showClickedTile;
    public bool showMouseTile;

    public bool shiftClickToDrop;

    public bool enablePanelHotkeys;
    public KeyCode attackStylesHotKey;
    public KeyCode inventoryHotKey;
    public KeyCode wornEquipmentHotKey;
    public KeyCode prayerHotKey;
    public KeyCode spellbookHotKey;


    void Start()
    {
        player = FindObjectOfType<Player>();
        player.showTrueTile = showTrueTile;
        player.showClickedTile = showClickedTile;

        MouseManager.showMouseTile = showMouseTile;
        Item.shiftClickToDrop = shiftClickToDrop;

        PanelButtons.enablePanelHotkeys = enablePanelHotkeys;
        PanelButtons.attackStylesHotKey = attackStylesHotKey;
        PanelButtons.inventoryHotKey = inventoryHotKey;
        PanelButtons.wornEquipmentHotKey = wornEquipmentHotKey;
        PanelButtons.prayerHotKey = prayerHotKey;
        PanelButtons.spellbookHotKey = spellbookHotKey;
    }
}
