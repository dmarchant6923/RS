using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateTexture : MonoBehaviour
{
    public static CreateTexture instance;
    public GameObject image;
    public GameObject cam;

    Vector3 currentPosition = Vector3.zero;

    private void Awake()
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
        List<Vector3> offsets = new List<Vector3>();
        for (int i = 0; i < textures.Count; i++)
        {
            offsets.Add(Vector3.zero);
        }

        return CreateLayeredTexture(textures, colors, offsets);
    }
    public static Texture2D CreateLayeredTexture(Texture2D texture, List<Vector3> offsets)
    {
        List<Color> colors = new List<Color>();
        List<Texture2D> textures = new List<Texture2D>();
        for (int i = 0; i < offsets.Count; i++)
        {
            textures.Add(texture);
            colors.Add(Color.white);
        }

        return CreateLayeredTexture(textures, colors, offsets);
    }
    public static Texture2D CreateLayeredTexture(List<Texture2D> textures, List<Color> colors, List<Vector3> offsets)
    {
        List<Sprite> sprites = new List<Sprite>();
        for (int i = 0; i < textures.Count; i++)
        {
            sprites.Add(Sprite.Create(textures[i], new Rect(0, 0, textures[i].width, textures[i].height), new Vector2(0.5f, 0.5f)));
            if (offsets.Count < textures.Count)
            {
                offsets.Add(Vector3.zero);
            }
        }

        return CreateLayeredTexture(sprites, colors, offsets);
    }




    public static Texture2D CreateLayeredTexture(List<Sprite> sprites, List<Color> colors, List<Vector3> offsets)
    {
        GameObject cam = Instantiate(instance.cam, Vector3.back * 10 + instance.currentPosition, Quaternion.identity);
        int texSize = 32;
        RenderTexture targetTex = new RenderTexture(texSize, texSize, 24, RenderTextureFormat.ARGB32);
        targetTex.isPowerOfTwo = true;
        cam.GetComponent<Camera>().targetTexture = targetTex;
        targetTex.Create();

        //GameObject image = Instantiate(instance.image);
        //image.transform.position = Vector3.zero;

        //for (int i = 0; i < sprites.Count; i++)
        //{
        //    image.GetComponent<SpriteRenderer>().sprite = sprites[i];
        //    image.GetComponent<SpriteRenderer>().color = colors[i];
        //    image.transform.position = offsets[i];
        //    cam.GetComponent<Camera>().Render();
        //}

        List<GameObject> newImages = new List<GameObject>();
        for (int i = 0; i < sprites.Count; i++)
        {
            GameObject image = Instantiate(instance.image);
            image.GetComponent<SpriteRenderer>().sprite = sprites[i];
            image.GetComponent<SpriteRenderer>().color = colors[i];
            image.GetComponent<SpriteRenderer>().sortingOrder = i;
            image.transform.position = instance.currentPosition + offsets[i];
            newImages.Add(image);
        }
        cam.GetComponent<Camera>().Render();

        Texture2D newTex = new Texture2D(texSize, texSize, TextureFormat.ARGB32, false);
        RenderTexture.active = targetTex;
        newTex.ReadPixels(new Rect(0, 0, texSize, texSize), 0, 0);
        newTex.Apply(true);

        RenderTexture.active = null;
        targetTex.Release();

        Destroy(cam);
        for (int i = 0; i < sprites.Count; i++)
        {
            Destroy(newImages[i]);
        }

        instance.currentPosition += Vector3.right * 10;

        return newTex;
    }
}
