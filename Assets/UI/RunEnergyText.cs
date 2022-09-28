using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RunEnergyText : MonoBehaviour
{
    Player player;
    Text text;
    void Start()
    {
        player = FindObjectOfType<Player>();
        text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        text.text = Mathf.Round(player.runEnergy / 100).ToString();
    }
}
