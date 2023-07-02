using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    SpriteRenderer sprite;

    public GameObject followObject;
    public float decaySpeed = 2;
    public bool rotate;

    AudioSource audioSource;
    public AudioClip explosionSound;

    private IEnumerator Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(explosionSound);

        yield return new WaitForSeconds((1f / decaySpeed) * 4);

        Destroy(gameObject);
    }
    void Update()
    {
        Color color = sprite.color;
        color.a -= decaySpeed * Time.deltaTime;
        sprite.color = color;

        if (followObject != null)
        {
            transform.position = followObject.transform.position;
        }
        if (rotate)
        {
            transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z + 100 * Time.deltaTime);
        }
    }
}
