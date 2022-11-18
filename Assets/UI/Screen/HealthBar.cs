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

    int ticksToDelete = 11;
    int ticks;
    void Start()
    {
        greenBar = transform.GetChild(0).GetComponent<RectTransform>();
        greenBarFullXWidth = greenBar.rect.width;
        transform.SetParent(FindObjectOfType<Canvas>().transform);
        UpdateHealth(currentHealth);

        TickManager.afterTick += AfterTick;
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position = Camera.main.WorldToScreenPoint(objectWithHealth.transform.position + Tools.AngleToVector3(Camera.main.transform.eulerAngles.z) * worldSpaceOffset);
        transform.position = Camera.main.WorldToScreenPoint(objectWithHealth.transform.position + Tools.AngleToVector3(Camera.main.transform.eulerAngles.z) * worldSpaceOffset) + Vector3.up * 20;

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
        currentHealth = number;
        ticks = 0;
        float percent = Mathf.Clamp(1 - (maxHealth - currentHealth) / maxHealth, 0, 1);
        greenBar.sizeDelta = new Vector2(percent * greenBarFullXWidth, greenBar.rect.height);
        greenBar.rect.Set(greenBar.position.x, greenBar.position.y, percent * greenBarFullXWidth, greenBar.rect.height);
    }

    private void OnDestroy()
    {
        TickManager.afterTick -= AfterTick;
    }
}
