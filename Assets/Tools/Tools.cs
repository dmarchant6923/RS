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

    public static float VectorToAngle(Vector2 vector)
    {
        vector = vector.normalized;
        float angle = -Mathf.Atan2(vector.x, vector.y) * Mathf.Rad2Deg;
        return angle;
    }
}
