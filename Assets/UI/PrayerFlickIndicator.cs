using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrayerFlickIndicator : MonoBehaviour
{
    float startPosition;
    float distance;
    float speed;


    void Start()
    {
        startPosition = transform.position.x;
        distance = Mathf.Abs(transform.localPosition.x) * 2;
        speed = distance / TickManager.maxTickTime;

        TickManager.onTick += resetPosition;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector2(Mathf.MoveTowards(transform.position.x, transform.position.x + distance, speed * Time.deltaTime), transform.position.y);
    }

    void resetPosition()
    {
        transform.position = new Vector2(startPosition, transform.position.y);
    }
}
