using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthHUD : MonoBehaviour
{
    public RectTransform greenBar;
    float greenBarFullXWidth;

    public Text nameText;
    public Text healthText;

    Enemy currentEnemy;

    float maxHealth;
    float currentHealth;

    int ticksToDelete = 11;
    int ticks;

    RectTransform rt;
    bool active = false;
    Vector2 onPosition;

    public static HealthHUD instance;

    private void Start()
    {
        instance = this;

        greenBarFullXWidth = greenBar.rect.width;
        ticks = ticksToDelete;

        rt = GetComponent<RectTransform>();
        onPosition = rt.anchoredPosition;
        rt.anchoredPosition = onPosition + Vector2.up * 1000;

        TickManager.afterTick += AfterTick;
    }

    void AfterTick()
    {
        if (active && currentEnemy != null)
        {
            int max = currentEnemy.initialHitpoints;
            int current = currentEnemy.hitpoints;
            healthText.text = current + " / " + max;
            float percent = (float)current / (float)max;
            greenBar.sizeDelta = new Vector2(percent * greenBarFullXWidth, greenBar.rect.height);
            greenBar.rect.Set(greenBar.position.x, greenBar.position.y, percent * greenBarFullXWidth, greenBar.rect.height);

            ticks--;
            if (ticks <= 0)
            {
                Deactivate();
            }
        }
    }

    public static void Activate(Enemy enemy)
    {
        instance.ticks = instance.ticksToDelete;
        instance.active = true;
        instance.currentEnemy = enemy;
        instance.rt.anchoredPosition = instance.onPosition;
        instance.nameText.text = enemy.name;
    }

    public static void Deactivate()
    {
        instance.ticks = 0;
        instance.active = false;
        instance.currentEnemy = null;
        instance.rt.anchoredPosition = instance.onPosition + Vector2.up * 1000;
    }
}
