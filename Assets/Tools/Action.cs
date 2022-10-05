using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action : MonoBehaviour
{
    public List<string> menuTexts = new List<string>();
    public List<int> priorities = new List<int>();

    private void Start()
    {
        if (menuTexts.Count == 0)
        {
            menuTexts = new List<string>(1);
        }
        while (priorities.Count < menuTexts.Count)
        {
            priorities.Add(1);
        }
    }
}
