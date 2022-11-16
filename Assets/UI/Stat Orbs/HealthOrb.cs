using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthOrb : MonoBehaviour
{
    StatOrbManager orbManager;
    Text numberText;
    IEnumerator Start()
    {
        yield return null;
        orbManager = GetComponent<StatOrbManager>();
        orbManager.initialValue = PlayerStats.initialHitpoints;
        numberText = GetComponentInChildren<Text>();
        numberText.text = Mathf.Round(PlayerStats.currentHitpoints).ToString();

        TickManager.afterTick += AfterTick;
    }

    void AfterTick()
    {
        numberText.text = Mathf.Round(PlayerStats.currentHitpoints).ToString();
        orbManager.value = Mathf.Round(PlayerStats.currentHitpoints);
        orbManager.UpdateMask();
    }
}
