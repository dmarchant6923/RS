using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TickManager : MonoBehaviour
{
    public delegate void TickAction();
    public static event TickAction cancelBeforeTick;
    public static event TickAction beforeTick;
    public static event TickAction onTick;
    public static event TickAction afterTick;

    public const float maxTickTime = 0.6f;
    public static float maxTickVariance = 0.07f;
    float currentVariance = 0;
    public static int tickCount;
    float tickTimer;
    public static float simLatency = 0.2f;

    private void Awake()
    {
        tickCount = 0;
        tickTimer = 0;
    }

    private void Update()
    {
        tickTimer += Time.deltaTime;
        if (tickTimer >= maxTickTime + currentVariance)
        {
            tickTimer -= maxTickTime + currentVariance;
            tickCount++;
            if (cancelBeforeTick != null)
            {
                cancelBeforeTick();
            }
            if (beforeTick != null)
            {
                beforeTick();
            }
            if (onTick != null)
            {
                onTick();
            }
            if (afterTick != null)
            {
                afterTick();
            }

            currentVariance = Random.Range(-maxTickVariance, maxTickVariance);
        }
    }
}
