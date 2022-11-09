using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitSplat : MonoBehaviour
{
    public int damage;
    public int maxHit;
    public bool showMaxHitSplat = false;

    public GameObject objectGettingHit;
    RawImage hitsplat;
    public Texture blueHitSplat;
    public Texture maxHitSplat;

    float timer = 1;
    void Start()
    {
        hitsplat = GetComponent<RawImage>();

        if (damage == 0)
        {
            hitsplat.texture = blueHitSplat;
        }
        if (showMaxHitSplat && damage == maxHit && damage > 1)
        {
            hitsplat.texture = maxHitSplat;
        }

        GetComponentInChildren<Text>().text = damage.ToString();

        transform.SetParent(FindObjectOfType<Canvas>().transform);
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            Destroy(gameObject);
        }

        transform.position = Camera.main.WorldToScreenPoint(objectGettingHit.transform.position);
    }
}
