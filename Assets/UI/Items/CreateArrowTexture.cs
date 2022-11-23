using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateArrowTexture : MonoBehaviour
{
    public List<Texture2D> textures = new List<Texture2D>();
    public List<Color> colors = new List<Color>();

    List<Vector3> offsets = new List<Vector3>();
    Texture2D[] newTextures = new Texture2D[5];

    Item itemScript;
    Equipment equipmentScript;

    public bool arrows;
    public bool bolts;

    private IEnumerator Start()
    {
        itemScript = GetComponent<Item>();
        equipmentScript = GetComponent<Equipment>();
        yield return null;
        Texture2D newTex = textures[0];
        if (textures.Count > 1)
        {
            newTex = CreateTexture.CreateLayeredTexture(textures, colors);
        }

        offsets.Add(Vector3.zero);
        newTextures[0] = newTex;
        if (arrows)
        {
            offsets.Add(new Vector3(0.04f, -0.15f, 0));
        }
        if (bolts)
        {
            offsets.Add(new Vector3(-0.12f, 0.04f, 0));
        }
        newTextures[1] = CreateTexture.CreateLayeredTexture(newTex, offsets);
        if (arrows)
        {
            offsets.Add(new Vector3(-0.15f, -0.04f, 0));
        }
        if (bolts)
        {
            offsets.Add(new Vector3(0.12f, -0.08f, 0));
        }
        newTextures[2] = CreateTexture.CreateLayeredTexture(newTex, offsets);
        if (arrows)
        {
            offsets.Add(new Vector3(0.2f, -0.04f, 0));
        }
        if (bolts)
        {
            offsets.Add(new Vector3(-0.25f, 0, 0));
        }
        newTextures[3] = CreateTexture.CreateLayeredTexture(newTex, offsets);
        if (arrows)
        {
            offsets.Add(new Vector3(-0.3f, -0.04f, 0));
        }
        if (bolts)
        {
            offsets.Add(new Vector3(0.2f, -0.2f, 0));
        }
        newTextures[4] = CreateTexture.CreateLayeredTexture(newTex, offsets);

        if (arrows || bolts)
        {
            GetComponent<StackableItem>().SetImages(newTextures);
            itemScript.itemImage.GetComponent<Outline>().effectDistance = new Vector2(0.1f, -0.1f);
            itemScript.itemImage.GetComponent<Outline>().effectColor = new Color(0, 0, 0, 0.5f);
            GetComponent<StackableItem>().projectileColor = colors[0];
        }

        GetComponent<Item>().itemTexture = newTextures[0];

    }
}
