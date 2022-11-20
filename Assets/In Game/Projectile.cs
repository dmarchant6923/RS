using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public bool arrow;
    public bool dart;
    public GameObject target;
    Vector3 targetPosition;
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

    public Sprite customSprite;

    public bool appearInstantly = false;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        sprite.color = color;
        airborneTime = airborneTicks * 0.6f;

        if (customSprite != null)
        {
            sprite.sprite = customSprite;
        }
        else
        {
            if (arrow)
            {
                sprite.sprite = arrowSprite;
            }
            else if (dart)
            {
                sprite.sprite = dartSprite;
            }
        }

        if (appearInstantly == false)
        {
            airborneTime -= 0.5f;
            sprite.enabled = false;
        }
        else
        {
            float mult = 0.4f;
            if (source.GetComponent<NPC>() != null)
            {
                mult *= (float)source.GetComponent<NPC>().tileSize;
            }
            initialPosition = transform.position + (target.transform.position - transform.position).normalized * mult;
            sprite.enabled = true;
        }

        TickManager.beforeTick += BeforeTick;
    }

    void BeforeTick()
    {
        if (sprite.enabled == false)
        {
            float mult = 0.4f;
            if (source.GetComponent<NPC>() != null)
            {
                mult *= (float)source.GetComponent<NPC>().tileSize;
            }
            initialPosition = transform.position + (target.transform.position - transform.position).normalized * mult;
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
        if (target != null)
        {
            targetPosition = target.transform.position + (initialPosition - target.transform.position).normalized * 0.3f;
        }
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
