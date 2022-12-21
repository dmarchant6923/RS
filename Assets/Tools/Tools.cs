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
        else if (number >= 1000)
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
            item.transform.localScale *= FindObjectOfType<Canvas>().scaleFactor;
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
        return SecondsToMinutes(seconds, false);
    }
    public static string SecondsToMinutes(float seconds, bool keepMinutesPlace)
    {
        return SecondsToMinutes(seconds, keepMinutesPlace, false);
    }
    public static string SecondsToMinutes(float seconds, bool keepMinutesPlace, bool decimalSeconds)
    {
        float minutes = Mathf.Floor(seconds / 60);
        seconds = (minutes == 0) ? seconds : seconds % (minutes * 60);
        string secondsText;
        if (decimalSeconds)
        {
            seconds = Mathf.Floor(seconds * 10) / 10;
            secondsText = seconds.ToString("00.0");

        }
        else
        {
            seconds = Mathf.Floor(seconds);
            secondsText = seconds.ToString("00");
        }

        if (minutes == 0)
        {
            if (keepMinutesPlace)
            {
                return "0:" + secondsText;
            }
            return secondsText;
        }
        else
        {
            return minutes.ToString() + ":" + secondsText;
        }
    }

    public static bool PlayerIsAdjacentToLargeObject(Vector2 swTile, int sizeX, int sizeY, bool allowCorners)
    {
        Vector2 nearestTile = NearestTileToPlayer(swTile, sizeX, sizeY);

        if (TileManager.TileDistance(Player.player.trueTile, nearestTile) <= 1)
        {
            if (allowCorners)
            {
                return true;
            }
            else if ((Player.player.trueTile - nearestTile).magnitude < 1.1f)
            {
                return true;
            }
        }

        return false;
    }

    public static Vector2 NearestTileToPlayer(Vector2 swTile, int sizeX, int sizeY)
    {
        int nearestX = (int)Mathf.Clamp(Player.player.trueTile.x, swTile.x, swTile.x + sizeX - 1);
        int nearestY = (int)Mathf.Clamp(Player.player.trueTile.y, swTile.y, swTile.y + sizeY - 1);
        Vector2 nearestTile = new Vector2(nearestX, nearestY);

        return nearestTile;
    }
}
