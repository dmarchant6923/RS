using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZukShield : MonoBehaviour
{

    int startDirection = 1;
    int currentDirection;
    bool moving = false;

    NPC npcScript;
    [HideInInspector] public Enemy enemyScript;
    Vector2 absolutePosition = new Vector2(13, 2);
    [HideInInspector] public float[] safeSpotRange = new float[2];

    int ticks = 1;

    public Action shieldAction;

    public static bool showSafeSpot;
    public GameObject safeSpotMarker;
    public Vector3 relativePosition;


    private IEnumerator Start()
    {
        if (Random.Range(0, 2) == 0)
        {
            startDirection = -1;
        }
        currentDirection = startDirection;

        npcScript = GetComponent<NPC>();
        enemyScript = GetComponent<Enemy>();

        shieldAction.objectName = "<color=yellow>Shield</color>";
        shieldAction.UpdateName();

        npcScript.afterMovement += AfterMovement;

        yield return null;

        safeSpotMarker.transform.SetParent(npcScript.newSizeTileMarker.transform);
        if (showSafeSpot)
        {
            safeSpotMarker.SetActive(true);
        }
        else
        {
            safeSpotMarker.SetActive(false);
        }
        relativePosition = safeSpotMarker.transform.position - npcScript.newSizeTileMarker.transform.position;
    }

    void AfterMovement()
    {
        if (moving == false)
        {
            ticks--;
            if (ticks == 0)
            {
                npcScript.ExternalMovement(new Vector2(absolutePosition.x * currentDirection, absolutePosition.y) + Vector2.left);
                moving = true;
            }

        }
        else
        {
            if (npcScript.trueTile == new Vector2(absolutePosition.x * currentDirection, absolutePosition.y) + Vector2.left)
            {
                moving = false;
                ticks = 4;
                currentDirection *= -1;
            }
        }

        safeSpotRange[0] = npcScript.trueTile.x - 1;
        safeSpotRange[1] = npcScript.trueTile.x + 3;
        if (npcScript.trueTile == new Vector2(absolutePosition.x * -1, absolutePosition.y) + Vector2.left)
        {
            safeSpotRange[1]++;
            safeSpotMarker.transform.position = npcScript.newSizeTileMarker.transform.position + relativePosition + Vector3.right;
        }
        else if (npcScript.trueTile == new Vector2(absolutePosition.x, absolutePosition.y) + Vector2.left)
        {
            safeSpotRange[0]--;
            safeSpotMarker.transform.position = npcScript.newSizeTileMarker.transform.position + relativePosition + Vector3.left;
        }
        else
        {
            safeSpotMarker.transform.position = npcScript.newSizeTileMarker.transform.position + relativePosition;
        }
    }
}
