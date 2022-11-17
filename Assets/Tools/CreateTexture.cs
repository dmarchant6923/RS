using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateTexture : MonoBehaviour
{
    public static CreateTexture instance;
    public GameObject image;
    public GameObject cam;
    public Sprite texture1;
    public Sprite texture2;
    private void Start()
    {
        instance = this;
        StartCoroutine(CreateLayeredTexture(texture1, texture2));
    }

    public IEnumerator CreateLayeredTexture(Sprite texture1, Sprite texture2)
    {
        GameObject cam = Instantiate(instance.cam, Vector3.back * 10, Quaternion.identity);
        int texSize = 512;
        RenderTexture targetTex = new RenderTexture(texSize, texSize, 24, RenderTextureFormat.ARGB32);
        targetTex.isPowerOfTwo = true;
        cam.GetComponent<Camera>().targetTexture = targetTex;
        targetTex.Create();

        Debug.Log("created camera and target RenderTexture");
        yield return new WaitForSeconds(1);

        GameObject image = Instantiate(instance.image);
        image.transform.localScale = Vector3.one;
        image.transform.position = Vector3.zero;
        image.GetComponent<RectTransform>().sizeDelta = Vector2.one * texSize;

        Debug.Log("Instantiated image and set it's position to 0");
        yield return new WaitForSeconds(1);

        image.GetComponent<SpriteRenderer>().sprite = texture1;
        cam.GetComponent<Camera>().Render();

        Debug.Log("Set image to texture 1 and rendered");
        yield return new WaitForSeconds(1);

        image.GetComponent<SpriteRenderer>().sprite = texture2;
        cam.GetComponent<Camera>().Render();

        Debug.Log("Set image to texture 2 and rendered");
        yield return new WaitForSeconds(1);

        Texture2D newTex = new Texture2D(texSize, texSize, TextureFormat.RGB24, false);
        RenderTexture.active = targetTex;
        newTex.ReadPixels(new Rect(0, 0, texSize, texSize), 0, 0);
        newTex.Apply(true);

        Debug.Log("Created new texture, read pixels of targetTex");
        yield return new WaitForSeconds(1);

        RenderTexture.active = null;
        targetTex.Release();

        Destroy(cam);
        Destroy(image);

        GetComponent<RawImage>().texture = newTex;

        Debug.Log("Destroyed image and camera, set image texture to newTex");
        yield return null;

        //GameObject cam = (GameObject)Resources.Load("TextureCam");

        //int texSize = 512;
        //RenderTexture targetTex = new RenderTexture(texSize, texSize, 24, RenderTextureFormat.ARGB32);
        //targetTex.isPowerOfTwo = true;
        //cam.GetComponent<Camera>().targetTexture = targetTex;
        //targetTex.Create();

        //GameObject GUITexCombiner = (GameObject)Resources.Load("TextureRawImage");
        //GUITexCombiner.transform.localScale = new Vector3(0f, 0f, 0f);
        //GUITexCombiner.transform.position = new Vector3(0f, 0f, 0f);
        //GUITexCombiner.GetComponent<RectTransform>().sizeDelta = new Vector2(texSize, texSize);

        //GUITexCombiner.GetComponent<RawImage>().texture = (Texture2D)Resources.Load(texture1);
        //cam.GetComponent<Camera>().Render();
        //Resources.UnloadAsset(GUITexCombiner.GetComponent<RawImage>().texture);

        //GUITexCombiner.GetComponent<RawImage>().texture = (Texture2D)Resources.Load(texture2);
        //cam.GetComponent<Camera>().Render();
        //Resources.UnloadAsset(GUITexCombiner.GetComponent<RawImage>().texture);

        //Texture2D newTex = new Texture2D(texSize, texSize, TextureFormat.RGB24, false);
        //RenderTexture.active = targetTex;
        //newTex.ReadPixels(new Rect(0, 0, texSize, texSize), 0, 0);
        //newTex.Apply(true);

        //RenderTexture.active = null;
        //targetTex.Release();
    }
}
