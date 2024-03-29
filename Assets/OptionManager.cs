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

    public static bool ignorePlayerDeath;
    public static bool ignoreHiscores;

    public static bool showManualSetTimer;

    public static bool firstLogin = false;

    public static float uiScaleFactor = 1;

    private void Awake()
    {
        instance = this;
    }

    IEnumerator Start()
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

        yield return null;
        yield return null;

        if (firstLogin == false)
        {
            firstLogin = true;
            GameLog.Log("Welcome to an in-depth Zuk simulator. Have fun!");
        }
    }

    public static void UpdateGameSettings(bool[] settings, float simLatency, float uiScale, string[] hotkeys)
    {
        CombatInfo.instance.gameObject.SetActive(settings[0]);

        PrayerToggle.instance.flickInidcator.GetComponent<RawImage>().enabled = settings[1];

        InfernoManager.showZukTileMarkers = settings[2];

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
        foreach (NPC npc in FindObjectsOfType<NPC>())
        {
            npc.ShowTrueTile(settings[4]);
        }

        showManualSetTimer = settings[5];

        Zuk.showSetTimer = settings[6];

        ZukShield.showSafeSpot = settings[7];
        
        ignorePlayerDeath = settings[8];

        TickManager.simLatency = simLatency / 1000;

        uiScaleFactor = uiScale / 100;
        //FindObjectOfType<CanvasScaler>().scaleFactor = uiScale / 100;
        //FindObjectOfType<CanvasScaler>().referenceResolution = new Vector2(1920, 1080) * 100 / uiScale;
        UIManager.instance.ResetCanvasScale();
        Canvas.ForceUpdateCanvases();
        Inventory.instance.ResetPanelExtents();
        WornEquipment.instance.ResetStatPanelPosition();
        PanelButtons.instance.ResetPanelPosition();

        PanelButtons.SetHotkeys(hotkeys);
    }


}
