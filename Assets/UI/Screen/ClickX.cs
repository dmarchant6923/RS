using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClickX : MonoBehaviour
{
    [HideInInspector] public bool redClick = false;
    RawImage image;

    public Texture yellowClick1;
    public Texture yellowClick2;
    public Texture yellowClick3;
    public Texture yellowClick4;

    public Texture redClick1;
    public Texture redClick2;
    public Texture redClick3;
    public Texture redClick4;

    Texture[] textures = new Texture[3];


    void Start()
    {
        transform.localScale *= UIManager.canvas.scaleFactor;
        image = GetComponent<RawImage>();
        transform.SetParent(FindObjectOfType<Canvas>().transform);

        if (redClick)
        {
            image.texture = redClick1;
            textures[0] = redClick2;
            textures[1] = redClick3;
            textures[2] = redClick4;
        }
        else
        {
            image.texture = yellowClick1;
            textures[0] = yellowClick2;
            textures[1] = yellowClick3;
            textures[2] = yellowClick4;
        }
        image.color = Color.white;

        StartCoroutine(Click());
    }

    IEnumerator Click()
    {
        float waitTime = 0.1f;
        yield return new WaitForSeconds(waitTime);
        for (int i = 0; i < textures.Length; i++)
        {
            image.texture = textures[i];
            yield return new WaitForSeconds(waitTime);
        }
        Destroy(gameObject);
    }
}
