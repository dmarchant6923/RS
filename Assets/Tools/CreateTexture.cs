using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateTexture : MonoBehaviour
{
    public static CreateTexture instance;
    public GameObject image;
    public GameObject cam;

    private void Start()
    {
        instance = this;
    }

    public static Texture2D CreateLayeredTexture(List<Texture2D> textures)
    {
        List<Color> colors = new List<Color>();
        for (int i = 0; i < textures.Count; i++)
        {
            colors[i] = Color.white;
        }

        return CreateLayeredTexture(textures, colors);
    }

    public static Texture2D CreateLayeredTexture(List<Texture2D> textures, List<Color> colors)
    {
        List<Sprite> sprites = new List<Sprite>();
        for (int i = 0; i < textures.Count; i++)
        {
            sprites.Add(Sprite.Create(textures[i], new Rect(0, 0, textures[i].width, textures[i].height), new Vector2(0.5f, 0.5f)));
        }

        return CreateLayeredTexture(sprites, colors);
    }

    public static Texture2D CreateLayeredTexture(List<Sprite> sprites, List<Color> colors)
    {
        GameObject cam = Instantiate(instance.cam, Vector3.back * 10, Quaternion.identity);
        int texSize = 32;
        RenderTexture targetTex = new RenderTexture(texSize, texSize, 24, RenderTextureFormat.ARGB32);
        targetTex.isPowerOfTwo = true;
        cam.GetComponent<Camera>().targetTexture = targetTex;
        targetTex.Create();

        GameObject image = Instantiate(instance.image);
        image.transform.position = Vector3.zero;

        for (int i = 0; i < sprites.Count; i++)
        {
            image.GetComponent<SpriteRenderer>().sprite = sprites[i];
            image.GetComponent<SpriteRenderer>().color = colors[i];
            cam.GetComponent<Camera>().Render();
        }

        Texture2D newTex = new Texture2D(texSize, texSize, TextureFormat.ARGB32, false);
        RenderTexture.active = targetTex;
        newTex.ReadPixels(new Rect(0, 0, texSize, texSize), 0, 0);
        newTex.Apply(true);

        RenderTexture.active = null;
        targetTex.Release();

        Destroy(cam);
        Destroy(image);

        return newTex;
    }
}
