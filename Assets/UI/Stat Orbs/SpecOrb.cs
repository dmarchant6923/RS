using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecOrb : MonoBehaviour
{
    StatOrbManager orbManager;
    void Start()
    {
        orbManager = GetComponent<StatOrbManager>();
        orbManager.actionText = "Use Special Attack";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
