using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InfernoManager : MonoBehaviour
{
    public Enemy zukScript;

    int fadeTime = 3;
    int fadeTicks = 0;

    int encounterTicks = 0;
    int damageTaken;

    public Image fadeBox;

    public static bool showZukTileMarkers;
    public GameObject tileMarkerParent;

    void Start()
    {
        Player.player.playerDeath += PlayerDeath;
        Player.player.tookDamage += DamageCounter;
        zukScript.enemyDied += ZukDeath;

        TickManager.afterTick += AfterTick;

        if (showZukTileMarkers == false)
        {
            tileMarkerParent.SetActive(false);
        }
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
            HiscoresPanel.UpdateDeaths();
        }
        StartCoroutine(ReturnToLobby(2));
    }

    void DamageCounter(int damage)
    {
        damageTaken += damage;
    }

    void ZukDeath()
    {
        if (OptionManager.ignoreHiscores == false && Player.player.dead == false)
        {
            HiscoresPanel.UpdateSuccessStats(PlayerStats.totalLevel, damageTaken, encounterTicks);
        }
        StartCoroutine(ReturnToLobby(5));
    }

    void AfterTick()
    {
        if (fadeTicks > 0)
        {
            fadeTicks--;
        }
        encounterTicks++;
    }
    IEnumerator ReturnToLobby(float delay)
    {
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
}
