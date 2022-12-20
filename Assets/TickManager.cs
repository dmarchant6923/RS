using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Profiling;

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

    static readonly ProfilerMarker newMarker = new ProfilerMarker("Tick");

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
            newMarker.Begin();
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
            newMarker.End();

            currentVariance = Random.Range(-maxTickVariance, maxTickVariance);
        }   
    }

    private void OnDestroy()
    {
        if (cancelBeforeTick != null)
        {
            foreach (var d in cancelBeforeTick.GetInvocationList())
            {
                cancelBeforeTick -= d as TickAction;
            }
        }

        if (beforeTick != null)
        {
            foreach (var d in beforeTick.GetInvocationList())
            {
                beforeTick -= d as TickAction;
            }
        }

        if (onTick != null)
        {
            foreach (var d in onTick.GetInvocationList())
            {
                onTick -= d as TickAction;
            }
        }

        if (afterTick != null)
        {
            foreach (var d in afterTick.GetInvocationList())
            {
                afterTick -= d as TickAction;
            }
        }
    }
}
