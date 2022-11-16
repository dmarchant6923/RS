using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoweredStaff : MonoBehaviour
{
    public int baseLevel;
    public int maxHitAtBase;
    public float scalePerLevel = 1 / 3f;
    public Color projectileColor;

    public float BaseMaxHit(int level)
    {
        float maxHit = Mathf.Floor(maxHitAtBase + (level - baseLevel) * scalePerLevel);
        return maxHit;
    }
}
