using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackableGroundItem : MonoBehaviour
{
    public int quantity;
    public Sprite[] sprites = new Sprite[5];
    public Texture[] textures = new Texture[5];
    public int[] thresholds = new int[4];

    SpriteRenderer sprite;

    private IEnumerator Start()
    {
        sprite = GetComponent<SpriteRenderer>();

        for (int i = 0; i < sprites.Length; i++)
        {
            if (textures[i] != null)
            {
                Texture2D tex = (Texture2D)textures[i];
                Sprite newSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                sprites[i] = newSprite;
            }
        }

        yield return null;
        ChooseSprite();
    }

    void ChooseSprite()
    {
        if (quantity <= 0)
        {
            Destroy(gameObject);
        }

        for (int i = 4; i >= 0; i--)
        {
            if (i == 0)
            {
                sprite.sprite = sprites[i];
            }
            else if (sprites[i] != null && quantity >= thresholds[i - 1])
            {
                sprite.sprite = sprites[i];
                break;
            }
        }
    }

    public void AddToQuantity(int num)
    {
        quantity += num;
        ChooseSprite();
    }
}
