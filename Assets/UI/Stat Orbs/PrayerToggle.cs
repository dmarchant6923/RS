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
        distance = Mathf.Abs(flickInidcator.localPosition.x) * 2f;
        speed = distance / TickManager.maxTickTime;

        TickManager.onTick += OnTick;
        TickManager.afterTick += ReadPrayer;

        orbManager.orbAction.menuTexts[0] = "Activate Quick-prayers";
        orbManager.orbAction.serverAction0 += ActivateQuickPrayers;
        orbManager.orbAction.orderLevels[0] = -1;

        orbManager.orbAction.menuTexts[1] = "Setup Quick-prayers";
        orbManager.orbAction.serverAction1 += SetupQuickPrayers;
        orbManager.orbAction.orderLevels[1] = -1;

        numberText = GetComponentInChildren<Text>();
        numberText.text = Mathf.Round(PlayerStats.currentPrayer).ToString();
    }

    // Update is called once per frame
    void Update()
    {
        flickInidcator.position = new Vector2(Mathf.MoveTowards(flickInidcator.position.x, flickInidcator.position.x + distance, speed * Time.deltaTime), flickInidcator.position.y);
    }

    void ActivateQuickPrayers()
    {
        orbManager.active = !orbManager.active;
        prayerScript.ActivateQuickPrayers(orbManager.active);
    }
    void SetupQuickPrayers()
    {
        prayerScript.SelectQuickPrayers();
    }


    void OnTick()
    {
        flickInidcator.position = new Vector2(startPosition, flickInidcator.position.y);
    }

    void ReadPrayer()
    {
        numberText.text = Mathf.Round(PlayerStats.currentPrayer).ToString();
        orbManager.UpdateMask();
    }
}
