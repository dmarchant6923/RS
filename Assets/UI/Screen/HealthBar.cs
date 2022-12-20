using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public float maxHealth;
    public float currentHealth;

    RectTransform greenBar;
    float greenBarFullXWidth;

    public GameObject objectWithHealth;
    public float worldSpaceOffset;

    int ticksToDelete = 20;
    int ticks;
    void Start()
    {
        transform.localScale *= UIManager.canvas.scaleFactor;
        greenBar = transform.GetChild(0).GetComponent<RectTransform>();
        greenBarFullXWidth = greenBar.rect.width;
        transform.SetParent(FindObjectOfType<Canvas>().transform);
        UpdateHealth(currentHealth);
        transform.SetAsFirstSibling();

        TickManager.afterTick += AfterTick;
    }

    // Update is called once per frame
    void Update()
    {
        if (objectWithHealth == null)
        {
            Destroy(gameObject);
            return;
        }
        transform.position = Camera.main.WorldToScreenPoint(objectWithHealth.transform.position + Tools.AngleToVector3(Camera.main.transform.eulerAngles.z) * worldSpaceOffset) +
                                                            Vector3.up * 20 * UIManager.canvas.scaleFactor;

    }

    void AfterTick()
    {
        ticks++;
        if (ticks >= ticksToDelete)
        {
            Destroy(gameObject);
        }
    }

    public void UpdateHealth(float number)
    {
        if (greenBar == null)
        {
            greenBar = transform.GetChild(0).GetComponent<RectTransform>();
            greenBarFullXWidth = greenBar.rect.width;
            transform.SetParent(FindObjectOfType<Canvas>().transform);
        }
        currentHealth = number;
        ticks = 0;
        float percent = Mathf.Clamp(1 - (maxHealth - currentHealth) / maxHealth, 0.05f, 0.95f);
        if (currentHealth >= maxHealth)
        {
            percent = 1;
        }
        if (currentHealth <= 0)
        {
            percent = 0;
        }
        greenBar.sizeDelta = new Vector2(percent * greenBarFullXWidth, greenBar.rect.height);
        greenBar.rect.Set(greenBar.position.x, greenBar.position.y, percent * greenBarFullXWidth, greenBar.rect.height);
    }

    public static void DeleteAll()
    {
        foreach (HealthBar bar in FindObjectsOfType<HealthBar>())
        {
            Destroy(bar.gameObject);
        }
    }

    private void OnDestroy()
    {
        TickManager.afterTick -= AfterTick;
    }
}
