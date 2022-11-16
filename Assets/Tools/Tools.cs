using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tools : MonoBehaviour
{
    public static string IntToString(int number)
    {
        string str = number.ToString();

        if (number >= 10000000)
        {
            str = Mathf.Floor(number / 1000).ToString() + "M";
        }
        else if (number >= 100000)
        {
            str = Mathf.Floor(number / 1000).ToString() + "K";
        }

        return str;
    }
    public static string IntToString(int number, bool changeColor)
    {
        string str = IntToString(number);
        if (changeColor)
        {
            if (number >= 10000000)
            {
                str = "<color=green>" + str + "</color>";
            }
            else if (number >= 100000)
            {
                str = "<color=white>" + str + "</color>";
            }
        }

        return str;
    }
    public static string IntToShortString(int number)
    {
        string str = number.ToString();

        if (number >= 10000000)
        {
            str = Mathf.Floor(number / 1000).ToString() + "M";
        }
        else if (number >= 10000)
        {
            str = Mathf.Floor(number / 1000).ToString() + "K";
        }

        return str;
    }

    public static Vector2 AngleToVector(float angle)
    {
        Vector2 vector = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad + Mathf.PI / 2),
                Mathf.Sin(angle * Mathf.Deg2Rad + Mathf.PI / 2)).normalized;
        return vector;
    }
    public static Vector3 AngleToVector3(float angle)
    {
        Vector3 vector = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad + Mathf.PI / 2),
                Mathf.Sin(angle * Mathf.Deg2Rad + Mathf.PI / 2)).normalized;
        return vector;
    }
    public static float VectorToAngle(Vector2 vector)
    {
        vector = vector.normalized;
        float angle = -Mathf.Atan2(vector.x, vector.y) * Mathf.Rad2Deg;
        return angle;
    }

    public static GameObject LoadFromResource(string name)
    {
        GameObject item = null;
        if (name.EndsWith("(uncharged)"))
        {
            name = name.Remove(name.Length - 12);
        }

        item = Resources.Load<GameObject>("Items/" + name);
        if (item == null)
        {
            item = Resources.Load<GameObject>("Equipment/" + name);
        }
        if (item == null)
        {
            item = Resources.Load<GameObject>("Equipment/Weapons/" + name);
        }

        if (item != null)
        {
            item = Instantiate(item);
            item.name = name;
        }

        return item;
    }

    public static bool BlockNPCMovement(NPC npcScript, Vector2 SWTile, int NPCSize)
    {
        for (int i = 0; i < NPCSize; i++)
        {
            for (int j = 0; j < NPCSize; j++)
            {
                Vector2 tile = SWTile + Vector2.right * i + Vector2.up * j;
                if (TileDataManager.GetTileData(tile).obstacle)
                {
                    return true;
                }
                RaycastHit2D[] cast = Physics2D.CircleCastAll(tile, 0.1f, Vector2.zero, 0, LayerMask.GetMask("NPC True Tile"));
                foreach (RaycastHit2D col in cast)
                {
                    if (col.collider.gameObject != npcScript.newSizeTileMarker)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public static string SecondsToMinutes(float seconds)
    {
        float minutes = Mathf.Floor(seconds / 60);
        seconds = seconds % (minutes * 60);
        if (minutes == 0)
        {
            return Mathf.Floor(seconds).ToString();
        }
        else
        {
            return minutes.ToString() + ":" + Mathf.Floor(seconds).ToString();
        }
    }
}
