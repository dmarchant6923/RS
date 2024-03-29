using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StackableItem : MonoBehaviour
{
    public bool useRangeAmmoScript = false;

    public RawImage maskImage;
    public RawImage itemImage;

    public Texture image1;
    public Texture image2;
    public Texture image3;
    public Texture image4;
    public Texture image5;
    [System.NonSerialized] public Texture[] textures = new Texture[5];

    public int threshold2;
    public int threshold3;
    public int threshold4;
    public int threshold5;
    [System.NonSerialized] public int[] thresholds = new int[4];

    public int quantity = 1;
    public Text quantityText;

    public Color projectileColor;
    public Sprite customProjectile;

    [System.NonSerialized] public Item itemScript;
    [HideInInspector] public bool groundItem = false;

    public class DelaySpawn
    {
        public int ticks;
        public Vector2 targetTile;
    }
    List<DelaySpawn> spawnList = new List<DelaySpawn>();

    private IEnumerator Start()
    {
        if (useRangeAmmoScript == false)
        {
            textures[0] = image1;
            textures[1] = image2;
            textures[2] = image3;
            textures[3] = image4;
            textures[4] = image5;
        }

        thresholds[0] = threshold2;
        thresholds[1] = threshold3;
        thresholds[2] = threshold4;
        thresholds[3] = threshold5;

        itemScript = GetComponent<Item>();

        ChooseImage();

        TickManager.beforeTick += BeforeTick;

        yield return null;

        itemImage.enabled = true;
    }

    public void SetImages(Texture2D[] newTextures)
    {
        for (int i = 0; i < newTextures.Length; i++)
        {
            textures[i] = newTextures[i];
        }
        ChooseImage();
    }
    void ChooseImage()
    {
        if (quantity <= 0)
        {
            if (groundItem == false && GetComponent<Equipment>() != null && GetComponent<Equipment>().isEquipped)
            {
                GetComponent<Equipment>().equipSlot.GetComponent<RawImage>().enabled = true;
            }
            Destroy(gameObject);
        }

        for (int i = 4; i >= 0; i--)
        {
            if (i == 0)
            {
                maskImage.texture = textures[i];
                itemImage.texture = textures[i];
            }
            else if (textures[i] != null && quantity >= thresholds[i - 1])
            {
                maskImage.texture = textures[i];
                itemImage.texture = textures[i];
                break;
            }
        }

        quantityText.text = quantity.ToString();
    }

    public void NewQuantity(int newQuantity)
    {
        quantity = newQuantity;
        ChooseImage();
    }

    public void AddToQuantity(int increment)
    {
        quantity += increment;
        ChooseImage();
    }

    public void UseRangedAmmo(Vector2 targetTile, int delay, bool spawnOnGround)
    {
        float rand = Random.Range(0f, 1f);
        if (WornEquipment.assembler)
        {
            if (rand < 0.2f)
            {
                AddToQuantity(-1);
            }
            return;
        }

        if (WornEquipment.accumulator)
        {
            if (rand < 0.2f)
            {
                AddToQuantity(-1);
            }
            else if (rand < 0.28f)
            {
                AddToQuantity(-1);
                if (spawnOnGround)
                {
                    DelaySpawn newSpawn = new DelaySpawn();
                    newSpawn.ticks = delay;
                    newSpawn.targetTile = targetTile;
                    spawnList.Add(newSpawn);
                }
            }
            return;
        }

        AddToQuantity(-1);
        if (spawnOnGround)
        {
            if (rand > 0.07f)
            {
                DelaySpawn newSpawn = new DelaySpawn();
                newSpawn.ticks = delay;
                newSpawn.targetTile = targetTile;
                spawnList.Add(newSpawn);
            }
        }

    }
    public void UseRangedAmmo(Vector2 targetTile, int delay)
    {
        UseRangedAmmo(targetTile, delay, true);
    }

    void BeforeTick()
    {
        for (int i = 0; i < spawnList.Count; i++)
        {
            spawnList[i].ticks--;
            if (spawnList[i].ticks <= 0)
            {
                Spawn(spawnList[i].targetTile);
                spawnList.RemoveAt(i);
                i--;
            }
        }
    }

    public void Spawn(Vector2 tile)
    {
        RaycastHit2D[] castAll = Physics2D.CircleCastAll(tile, 0.1f, Vector2.zero, 0);
        StackableGroundItem existingItem = null;
        foreach (RaycastHit2D cast in castAll)
        {
            if (cast.collider.name == gameObject.name)
            {
                existingItem = cast.collider.GetComponent<StackableGroundItem>();
            }
        }

        if (existingItem != null)
        {
            existingItem.AddToQuantity(1);
        }
        else
        {
            GameObject newItem = itemScript.SpawnGroundItem(tile, false);
            newItem.GetComponent<StackableGroundItem>().quantity = 1;
        }
    }

    private void OnDestroy()
    {
        TickManager.beforeTick -= BeforeTick;
    }
}
