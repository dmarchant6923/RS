using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public bool arrow;
    public bool dart;
    public GameObject target;
    public GameObject source;
    public float airborneTicks;
    Vector3 initialPosition;
    float airborneTime;
    float timer = 0;
    public Color color;

    SpriteRenderer sprite;
    public Sprite magicSprite;
    public Sprite arrowSprite;
    public Sprite dartSprite;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        sprite.color = color;
        airborneTime = airborneTicks * 0.6f;
        airborneTime -= 0.5f;
        sprite.enabled = false;

        if (arrow)
        {
            sprite.sprite = arrowSprite;
        }
        else if (dart)
        {
            sprite.sprite = dartSprite;
        }

        TickManager.beforeTick += BeforeTick;
    }

    void BeforeTick()
    {
        if (sprite.enabled == false)
        {
            initialPosition = transform.position + (target.transform.position - transform.position).normalized * 0.4f;
            sprite.enabled = true;
        }
    }

    private void Update()
    {
        if (sprite.enabled == false)
        {
            transform.position = source.transform.position;
            return;
        }

        float percent = timer / airborneTime;
        Vector3 targetPosition = target.transform.position + (initialPosition - target.transform.position).normalized * 0.3f;
        Vector3 totalDistance = targetPosition - initialPosition;
        transform.position = initialPosition + totalDistance.normalized * totalDistance.magnitude * percent;
        transform.eulerAngles = new Vector3(0, 0, Tools.VectorToAngle(totalDistance));

        timer += Time.deltaTime;
        if (timer > airborneTime)
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        TickManager.beforeTick -= BeforeTick;
    }
}
