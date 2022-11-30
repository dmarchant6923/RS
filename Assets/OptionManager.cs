using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public static OptionManager instance;


    void Start()
    {
        instance = this;

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

    public static void UpdateGameSettings(bool[] settings, float simLatency)
    {
        CombatInfo.instance.gameObject.SetActive(settings[0]);

        PrayerToggle.instance.flickInidcator.GetComponent<RawImage>().enabled = settings[1];

        Zuk.showZukTileMarkers = settings[2];

        instance.showTrueTile = settings[3];
        instance.showClickedTile = settings[3];
        instance.showMouseTile = settings[3];
        Player.player.showTrueTile = settings[3];
        Player.player.showClickedTile = settings[3];
        Player.player.trueTileScript.newTileMarker.GetComponent<SpriteRenderer>().enabled = settings[3];
        Player.player.trueTileScript.showClickedTile = settings[3];
        MouseManager.showMouseTile = settings[3];
        MouseManager.instance.newMouseTile.GetComponent<SpriteRenderer>().enabled = settings[3];

        NPC.showTrueTile = settings[4];

        Zuk.showSetTimer = settings[5];

        ZukShield.showSafeSpot = settings[6];


        TickManager.simLatency = simLatency / 1000;
    }


}
