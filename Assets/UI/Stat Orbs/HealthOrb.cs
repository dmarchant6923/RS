using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthOrb : MonoBehaviour
{
    StatOrbManager orbManager;
    Text numberText;

    public RectTransform regenIndicator;
    float startPosition;
    float distance;

    int tick;
    int regenTicks = 100;

    bool rapidHeal = false;

    IEnumerator Start()
    {
        startPosition = regenIndicator.position.x;
        distance = Mathf.Abs(regenIndicator.localPosition.x) * 2;
        tick = regenTicks;
        regenIndicator.gameObject.SetActive(false);

        yield return null;
        orbManager = GetComponent<StatOrbManager>();
        orbManager.initialValue = PlayerStats.initialHitpoints;
        numberText = GetComponentInChildren<Text>();
        numberText.text = Mathf.Round(PlayerStats.currentHitpoints).ToString();

        TickManager.afterTick += AfterTick;
    }

    void AfterTick()
    {
        if (rapidHeal == false && Prayer.rapidHeal)
        {
            rapidHeal = true;
            regenTicks = 50;
            tick = regenTicks;
            regenIndicator.position = new Vector2(startPosition, regenIndicator.position.y);
        }
        if (rapidHeal && Prayer.rapidHeal == false)
        {
            rapidHeal = false;
            regenTicks = 100;
            tick = regenTicks;
            regenIndicator.position = new Vector2(startPosition, regenIndicator.position.y);
        }

        if (PlayerStats.currentHitpoints == PlayerStats.initialHitpoints)
        {
            tick = regenTicks;
            if (regenIndicator.gameObject.activeSelf)
            {
                regenIndicator.position = new Vector2(startPosition, regenIndicator.position.y);
                regenIndicator.gameObject.SetActive(false);
            }
        }
        else
        {
            if (regenIndicator.gameObject.activeSelf == false)
            {
                regenIndicator.gameObject.SetActive(true);
            }
            float percent = 1 - ((float)tick / (float)regenTicks);
            regenIndicator.position = new Vector2(startPosition + distance * percent, regenIndicator.position.y);

            if (tick == 0)
            {
                PlayerStats.currentHitpoints += (int)Mathf.Sign(PlayerStats.initialHitpoints - PlayerStats.currentHitpoints);
                if (WornEquipment.regenBrace && PlayerStats.currentHitpoints < PlayerStats.initialHitpoints)
                {
                    PlayerStats.currentHitpoints++;
                }
                tick = regenTicks;
            }
            tick--;
        }

        numberText.text = Mathf.Round(PlayerStats.currentHitpoints).ToString();
        orbManager.value = Mathf.Round(PlayerStats.currentHitpoints);
        orbManager.UpdateMask();
    }
}
