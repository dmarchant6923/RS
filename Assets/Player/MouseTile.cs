using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseTile : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        transform.position = TickManager.mouseCoordinate;
    }
}
