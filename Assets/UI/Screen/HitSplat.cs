using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitSplat : MonoBehaviour
{
    public bool showMaxHitSplat = false;

    public GameObject hitSplat;
    GameObject[] activeHitSplats = new GameObject[4];
    int numActive = 0;
    float[] timers = new float[4];
    [HideInInspector] public GameObject objectGettingHit;
    public Texture blueHitSplat;
    public Texture maxHitSplat;

    float timer = 1;
    void Start()
    {
        transform.SetParent(FindObjectOfType<Canvas>().transform);
    }

    // Update is called once per frame
    void Update()
    {
        bool destroy = true;
        for (int i = 0; i < activeHitSplats.Length; i++)
        {
            if (activeHitSplats[i] != null)
            {
                destroy = false;
                if (timers[i] > 0)
                {
                    timers[i] -= Time.deltaTime;
                }
                if (timers[i] <= 0)
                {
                    Destroy(activeHitSplats[i]);
                    activeHitSplats[i] = null;
                    timers[i] = 0;
                }
            }
        }

        if (destroy)
        {
            Destroy(gameObject);
        }


        if (numActive == 1 && activeHitSplats[0] != null)
        {
            activeHitSplats[0].transform.localPosition = Vector2.zero;
        }
        else if (numActive > 1)
        {
            for (int i = 0; i < numActive; i++)
            {
                if (activeHitSplats[i] != null)
                {
                    Vector2 offset = Vector2.up * 20;
                    if (i == 1) { offset = Vector2.down * 20; }
                    if (i == 2) { offset = Vector2.right * 20; }
                    if (i == 3) { offset = Vector2.left * 20; }

                    activeHitSplats[i].transform.localPosition = offset;
                }
            }
        }

        transform.position = Camera.main.WorldToScreenPoint(objectGettingHit.transform.position);
    }

    public void NewHitSplat(int damage, int maxHit)
    {
        for (int i = 0; i < activeHitSplats.Length; i++)
        {
            if (activeHitSplats[i] == null)
            {
                GameObject newHitSplat = Instantiate(hitSplat, transform);
                if (damage == 0)
                {
                    newHitSplat.GetComponent<RawImage>().texture = blueHitSplat;
                }
                if (showMaxHitSplat && damage == maxHit && damage > 1)
                {
                    newHitSplat.GetComponent<RawImage>().texture = maxHitSplat;
                }
                newHitSplat.GetComponentInChildren<Text>().text = damage.ToString();

                activeHitSplats[i] = newHitSplat;
                timers[i] = timer;
                numActive++;
                break;
            }
        }
    }
}
