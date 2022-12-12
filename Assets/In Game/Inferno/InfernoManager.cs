using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InfernoManager : MonoBehaviour
{
    public Enemy zukScript;

    int fadeTime = 4;
    int fadeTicks = 0;

    int encounterTicks = 0;
    int damageTaken;
    bool stopTimer = false;

    public Image fadeBox;

    public static bool showZukTileMarkers;
    public GameObject tileMarkerParent;

    public Text timer;

    public GameObject winPanel;
    public Text time;
    public Text pb;
    public Text damage;
    public Text balls;
    public Text chance;
    public Text heals;
    public Text dps;
    public Text nibbler;
    public ButtonScript returnButton;

    float deathChance = 0;
    public int damageDealt = 0;
    public int ballsTanked = 0;

    public static InfernoManager instance;

    void Start()
    {
        instance = this;
        Player.player.playerDeath += PlayerDeath;
        Player.player.tookDamage += DamageCounter;
        zukScript.enemyDied += ZukDeath;
        Combat.enemyDealtDamage += Chanced;
        Player.player.combatScript.playerDealtDamage += PlayerDamage;
        returnButton.buttonClicked += ClosePanel;

        TickManager.afterTick += AfterTick;

        if (showZukTileMarkers == false)
        {
            tileMarkerParent.SetActive(false);
        }

        timer.gameObject.SetActive(true);
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
        if (OptionManager.ignoreHiscores == false && Player.player.dead == false)
        {
            GameManager.UpdateSuccessStats(PlayerStats.totalLevel, damageTaken, encounterTicks);
        }

        yield return new WaitForSeconds(5);
        Action.ignoreAllActions = true;
        Player.player.trueTileScript.StopMovement();
        time.text = Tools.SecondsToMinutes((float)encounterTicks * TickManager.maxTickTime, true);
        pb.text = Tools.SecondsToMinutes(GameManager.scores.fastestTicks * TickManager.maxTickTime, true);
        damage.text = damageTaken.ToString();
        balls.text = ballsTanked.ToString();
        chance.text = (Mathf.Floor(deathChance*10000) / 100).ToString() + "%";
        heals.text = ZukHealer.zukHeals.ToString();
        dps.text = (Mathf.Floor(((float)damageDealt * 1000 / ((float)encounterTicks * TickManager.maxTickTime))) / 1000).ToString();
        float nibblerChance = 1 / 100;
        if (WornEquipment.slayerHelm)
        {
            nibblerChance = 1 / 75;
        }
        if (Random.Range(0f, 1f) < nibblerChance)
        {
            nibbler.text = "YES! gzzzz";
        }
        else
        {
            nibbler.text = "No.";
        }
        winPanel.SetActive(true);
        yield return new WaitForSeconds(1);
        Action.ignoreAllActions = false;

        while (winPanel.activeSelf)
        {
            yield return null;
        }

        ReturnToLobby();
    }

    void ClosePanel()
    {
        winPanel.SetActive(false);
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
            timer.text = Tools.SecondsToMinutes((float)encounterTicks * TickManager.maxTickTime, true);
        }

    }
    public IEnumerator ReturnToLobby(float delay)
    {
        yield return null;
        Player.player.trueTileScript.StopMovement();
        yield return new WaitForSeconds(delay);
        Action.ignoreAllActions = true;
        fadeTicks = fadeTime;
        while (fadeTicks > 0)
        {
            Color color = fadeBox.color;
            color.a += Time.deltaTime;
            fadeBox.color = color;
            yield return null;
        }

        OptionManager.keepRunOn = Player.player.runEnabled;
        SceneManager.LoadScene("Lobby");
    }





    void Chanced(int damage, int maxHit, float hitChance)
    {
        float chanceOfDying = Mathf.Max(0, ((float)maxHit - (float)PlayerStats.currentHitpoints) / (float)maxHit) * hitChance;
        float chanceOfLiving = 1 - chanceOfDying;
        float cumulativeChanceOfLiving = (1 - deathChance) * chanceOfLiving;
        deathChance = 1 - cumulativeChanceOfLiving;
        Debug.Log("max hit: " + maxHit + ". HP: " + PlayerStats.currentHitpoints + ". Chance of dying on this hit: " + chanceOfDying + ". cumulative chance: " + deathChance);
    }

    void PlayerDamage(int damage)
    {
        damageDealt += damage;
    }

    private void OnDestroy()
    {
        Combat.enemyDealtDamage -= Chanced;
    }
}
