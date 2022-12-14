using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompletionPanel : MonoBehaviour
{
    public Text time;
    public Text pb;
    public Text damage;
    public Text balls;
    public Text chance;
    public Text heals;
    public Text shieldHealth;
    public Text dps;
    public Text nibbler;
    public ButtonScript returnButton;
    public InfernoManager manager;

    void Start()
    {
        returnButton.buttonClicked += ClosePanel;
    }

    void ClosePanel()
    {
        gameObject.SetActive(false);
    }

    public void UpdateText()
    {
        time.text = Tools.SecondsToMinutes((float)manager.encounterTicks * TickManager.maxTickTime, true);
        pb.text = Tools.SecondsToMinutes(GameManager.scores.fastestTicks * TickManager.maxTickTime, true);
        damage.text = manager.damageTaken.ToString();
        balls.text = manager.ballsTanked.ToString();
        chance.text = (Mathf.Floor(manager.deathChance * 10000) / 100).ToString() + "%";
        heals.text = ZukHealer.zukHeals.ToString();
        shieldHealth.text = manager.shieldHealthValue + "/" + 600;
        dps.text = (Mathf.Floor(((float)manager.damageDealt * 1000 / ((float)manager.encounterTicks * TickManager.maxTickTime))) / 1000).ToString();
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
    }
}
