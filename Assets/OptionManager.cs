using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionManager : MonoBehaviour
{
    Player player;
    public bool showTrueTile;
    public bool showClickedTile;
    public bool showMouseTile;

    void Start()
    {
        player = FindObjectOfType<Player>();
        player.showTrueTile = showTrueTile;
        player.showClickedTile = showClickedTile;

        MouseManager.showMouseTile = showMouseTile;
    }
}
