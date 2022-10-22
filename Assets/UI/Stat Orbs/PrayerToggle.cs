using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrayerToggle : MonoBehaviour
{
    Toggle prayerToggle;
    bool togglePressed;

    StatOrbManager orbManager;

    public Transform flickInidcator;
    float startPosition;
    float distance;
    float speed;

    public Prayer prayerScript;

    Text numberText;

    void Start()
    {
        orbManager = GetComponent<StatOrbManager>();
        orbManager.initialValue = PlayerStats.initialPrayer;

        prayerToggle = GetComponent<Toggle>();

        startPosition = flickInidcator.position.x;
        distance = Mathf.Abs(flickInidcator.localPosition.x) * 1.3f;
        speed = distance / TickManager.maxTickTime;

        TickManager.onTick += OnTick;
        TickManager.afterTick += ReadPrayer;
        orbManager.onToggle += PrayerPressed;
        orbManager.orbAction.menuTexts[0] = "Activate Quick-prayers";
        orbManager.orbAction.menuTexts[1] = "Setup Quick-prayers";
        orbManager.orbAction.action1 += SetupQuickPrayers;

        numberText = GetComponentInChildren<Text>();
        numberText.text = Mathf.Round(PlayerStats.currentPrayer).ToString();
    }

    // Update is called once per frame
    void Update()
    {
        flickInidcator.position = new Vector2(Mathf.MoveTowards(flickInidcator.position.x, flickInidcator.position.x + distance, speed * Time.deltaTime), flickInidcator.position.y);
    }

    void PrayerPressed()
    {
        prayerScript.ActivateQuickPrayers();
    }
    void SetupQuickPrayers()
    {
        Invoke("DelaySetupQuickPrayers", TickManager.simLatency);
    }

    void DelaySetupQuickPrayers()
    {
        prayerScript.selectQuickPrayers = true;
    }

    void OnTick()
    {
        if (togglePressed)
        {
            //prayerScript.ActivateQuickPrayers();
            togglePressed = false;
        }

        flickInidcator.position = new Vector2(startPosition, flickInidcator.position.y);
    }

    void ReadPrayer()
    {
        numberText.text = Mathf.Round(PlayerStats.currentPrayer).ToString();
        orbManager.UpdateMask();
    }
}
