using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public bool arrow;
    public bool bolt;
    public bool dart;
    public GameObject target;
    [HideInInspector] public Vector3 targetPosition;
    public GameObject source;
    public float airborneTicks;
    Vector3 initialPosition;
    float airborneTime;
    float timer = 0;
    public Color color;

    SpriteRenderer sprite;
    public Sprite magicSprite;
    public Sprite arrowSprite;
    public Sprite boltSprite;
    public Sprite dartSprite;

    public Sprite customSprite;

    public int spawnDelay = 1;

    public AudioClip onDestroySound;

    private IEnumerator Start()
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
            else if (bolt)
            {
                sprite.sprite = boltSprite;
            }
            else if (dart)
            {
                sprite.sprite = dartSprite;
            }
        }

        if (spawnDelay > 0)
        {
            airborneTime -= spawnDelay * 0.6f - 0.1f;
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

        yield return null;

        if (source == null)
        {
            Destroy(gameObject);
        }
    }

    void BeforeTick()
    {
        spawnDelay--;

        if (sprite.enabled == false && spawnDelay == 0)
        {
            float mult = 0.4f;
            if (source != null && source.GetComponent<NPC>() != null)
            {
                mult *= (float)source.GetComponent<NPC>().tileSize;
            }
            if (target != null)
            {
                targetPosition = target.transform.position;
            }
            initialPosition = transform.position + (targetPosition - transform.position).normalized * mult;
            sprite.enabled = true;
        }
    }

    private void Update()
    {
        if (sprite.enabled == false && source != null)
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

        if (onDestroySound == null) { return; }
        if (target != null && target.GetComponent<AudioSource>() != null)
        {
            target.GetComponent<AudioSource>().PlayOneShot(onDestroySound);
        }
        else
        {
            Debug.LogWarning("projectile had no audio source to play clip from");
        }
    }
}
