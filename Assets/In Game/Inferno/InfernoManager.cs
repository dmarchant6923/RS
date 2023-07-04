using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InfernoManager : MonoBehaviour
{
    public Enemy zukScript;
    public ZukShield shieldScript;

    int fadeTime = 4;
    int fadeTicks = 0;

    bool stopTimer = false;

    public static bool showZukTileMarkers;
    public GameObject tileMarkerParent;

    public Text timer;

    public CompletionPanel winPanel;

    [HideInInspector] public int encounterTicks = 0;
    [HideInInspector] public int damageTaken;
    [HideInInspector] public float deathChance = 0;
    [HideInInspector] public int damageDealt = 0;
    [HideInInspector] public int ballsTanked = 0;
    [HideInInspector] public int shieldHealthValue;
    [HideInInspector] public int lowestGearValue = -1;

    public static InfernoManager instance;

    GameObject infernoUI;
    Canvas canvas;

    void Start()
    {
        instance = this;
        Player.player.playerDeath += PlayerDeath;
        Player.player.tookDamage += DamageCounter;
        zukScript.enemyDied += ZukDeath;
        Player.player.damageInfo += Chanced;
        Player.player.combatScript.playerDealtDamage += PlayerDamage;

        Player.player.SetNewPosition(Vector2.zero);
        Action.ignoreAllActions = false;

        TickManager.afterTick += AfterTick;

        if (showZukTileMarkers == false)
        {
            tileMarkerParent.SetActive(false);
        }

        canvas = FindObjectOfType<Canvas>();
        infernoUI = UIManager.instance.infernoUI;
        infernoUI.SetActive(true);
        //newUI = Instantiate(infernoUI, canvas.transform);
        //newUI.transform.position = canvas.transform.position;
        //newUI.GetComponent<RectTransform>().sizeDelta = new Vector2(newUI.GetComponent<RectTransform>().rect.width / canvas.scaleFactor, newUI.GetComponent<RectTransform>().rect.height / canvas.scaleFactor);
        winPanel = infernoUI.GetComponentInChildren<CompletionPanel>();
        timer = infernoUI.transform.GetChild(0).GetComponent<Text>();

        winPanel.gameObject.SetActive(false);
        winPanel.manager = this;
        GameLog.Log("Good luck!");
        if (OptionManager.ignoreHiscores)
        {
            GameLog.Log("<color=red>You are not eligible for high scores this run.</color>");
        }

        StartCoroutine(Unfade());
    }

    IEnumerator Unfade()
    {
        yield return null;
        Music.PlayInfernoTrack();
        CameraScript.instance.ResetCameraPosition();
        Color color = UIManager.instance.fadeBox.color;
        while (color.a > 0)
        {
            color = UIManager.instance.fadeBox.color;
            color.a -= Time.deltaTime;
            UIManager.instance.fadeBox.color = color;
            yield return null;
        }
    }

    private void OnDestroy()
    {
        Player.player.playerDeath -= PlayerDeath;
        Player.player.tookDamage -= DamageCounter;
        Player.player.damageInfo -= Chanced;
        TickManager.afterTick -= AfterTick;
    }

    void PlayerDeath()
    {
        if (OptionManager.ignorePlayerDeath)
        {
            return;
        }
        Player.player.StandardDeath();
        zukScript.ClearDamageQueue();
        if (OptionManager.ignoreHiscores == false)
        {
            GameManager.UpdateDeaths();
            HiscoresPanel.instance.WriteScores();
        }
        StartCoroutine(ReturnToLobby(1.5f));
    }

    public void ReturnToLobby()
    {
        StartCoroutine(ReturnToLobby(0));
    }

    void ZukDeath()
    {
        StartCoroutine(ZukDeathCR());
    }

    IEnumerator ZukDeathCR()
    {
        stopTimer = true;
        float score = winPanel.CalculateScore();
        Player.player.ClearDamageQueue();
        shieldHealthValue = shieldScript != null ? shieldScript.GetComponent<Enemy>().hitpoints : 0;

        yield return new WaitForSeconds(4);
        if (OptionManager.ignoreHiscores == false && Player.player.standardDeath == false)
        {
            GameManager.UpdateSuccessStats(PlayerStats.totalLevel, damageTaken, encounterTicks, score, InfernoPortal.GearValueAtPortalEntrance);
            Action.ignoreAllActions = true;
            Player.player.trueTileScript.StopMovement();
            winPanel.UpdateText();
            winPanel.gameObject.SetActive(true);
            HiscoresPanel.instance.WriteScores();

            yield return new WaitForSeconds(1);
            Action.ignoreAllActions = false;
        }

        while (winPanel.gameObject.activeSelf)
        {
            yield return null;
        }

        ReturnToLobby();
    }

    void DamageCounter(int damage)
    {
        damageTaken += damage;
    }

    void AfterTick()
    {
        if (fadeTicks > 0)
        {
            fadeTicks--;
        }
        if (stopTimer == false)
        {
            encounterTicks++;
            timer.text = Tools.SecondsToMinutes((float)encounterTicks * TickManager.maxTickTime, true, true);
        }

    }
    public IEnumerator ReturnToLobby(float delay)
    {
        Player.player.ClearDamageQueue();
        Music.FadeTrack();
        yield return null;
        Player.player.trueTileScript.StopMovement();
        yield return new WaitForSeconds(delay);
        Action.ignoreAllActions = true;
        fadeTicks = fadeTime;
        while (fadeTicks > 0)
        {
            Color color = UIManager.instance.fadeBox.color;
            color.a += Time.deltaTime;
            UIManager.instance.fadeBox.color = color;
            yield return null;
        }
        CustomHUD.instance.Deactivate();
        ManualSetTimer.instance.gameObject.SetActive(false);

        Prayer.DeactivatePrayers();
        //foreach (GameObject element in newUIElements)
        //{
        //    Destroy(element);
        //}
        //Destroy(infernoUI);
        timer.text = Tools.SecondsToMinutes(0, true, true);
        infernoUI.SetActive(false);
        winPanel.gameObject.SetActive(true);
        if (Player.player.standardDeath)
        {
            GameLog.Log("Oh dear, you are dead!");
        }
        else if (OptionManager.ignoreHiscores == false)
        {
            GameObject newCape = Tools.LoadFromResource("Infernal cape");
            Inventory.instance.PlaceInInventory(newCape);
        }

        SceneManager.LoadScene("Lobby");
    }

    void Chanced(Player.IncomingDamage damage)
    {
        if (damage.maxHit == 0)
        {
            return;
        }

        float adjustedHP = PlayerStats.currentHitpoints - damage.minHit;
        float adjustedMaxHit = damage.maxHit - damage.minHit;
        float chanceOfDying = Mathf.Max(0, (adjustedMaxHit - adjustedHP) / adjustedMaxHit * damage.hitChance);
        if (damage.prayedAgainst)
        {
            chanceOfDying = 0;
        }
        float chanceOfLiving = 1 - chanceOfDying;
        float cumulativeChanceOfLiving = (1 - deathChance) * chanceOfLiving;
        deathChance = 1 - cumulativeChanceOfLiving;
        //Debug.Log("max hit: " + damage.maxHit + ". HP: " + PlayerStats.currentHitpoints + ". hitchance: " + damage.hitChance + ". Chance of dying on this hit: " + chanceOfDying + ". cumulative chance: " + deathChance);
    }

    void PlayerDamage(int damage)
    {
        damageDealt += damage;
    }
}
