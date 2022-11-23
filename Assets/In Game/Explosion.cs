using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    SpriteRenderer sprite;

    public GameObject followObject;
    public float decaySpeed = 2;
    public bool rotate;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        Color color = sprite.color;
        color.a -= decaySpeed * Time.deltaTime;
        sprite.color = color;
        if (color.a <= 0)
        {
            Destroy(gameObject);
        }

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
