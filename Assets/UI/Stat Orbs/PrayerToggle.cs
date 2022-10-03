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

    void Start()
    {
        orbManager = GetComponent<StatOrbManager>();

        prayerToggle = GetComponent<Toggle>();
        prayerToggle.onValueChanged.AddListener(delegate { PrayerPressed(); });

        orbManager.actionText = "Activate Quick-Prayers";

        startPosition = flickInidcator.position.x;
        distance = Mathf.Abs(flickInidcator.localPosition.x) * 2;
        speed = distance / TickManager.maxTickTime;

        TickManager.onTick += OnTick;
        orbManager.onToggle += PrayerPressed;
    }

    // Update is called once per frame
    void Update()
    {
        flickInidcator.position = new Vector2(Mathf.MoveTowards(flickInidcator.position.x, flickInidcator.position.x + distance, speed * Time.deltaTime), flickInidcator.position.y);
    }

    void PrayerPressed()
    {
        togglePressed = true;
    }

    void OnTick()
    {
        if (togglePressed)
        {

            togglePressed = false;
        }


        flickInidcator.position = new Vector2(startPosition, flickInidcator.position.y);
    }
}
