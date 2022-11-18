using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Overhead : MonoBehaviour
{
    public GameObject objectWithOverhead;
    public float worldSpaceOffset;
    public string initialOverhead;

    RawImage image;
    public Texture prayMageTexture;
    public Texture prayMeleeTexture;
    public Texture prayRangeTexture;
    public Texture retributionTexture;
    public Texture redemptionTexture;
    public Texture smiteTexture;

    private void Start()
    {
        image = GetComponent<RawImage>();
        transform.SetParent(FindObjectOfType<Canvas>().transform);
        SwitchOverhead(initialOverhead);
        image.enabled = true;
    }

    private void Update()
    {
        //transform.position = Camera.main.WorldToScreenPoint(objectWithOverhead.transform.position + Tools.AngleToVector3(Camera.main.transform.eulerAngles.z) * worldSpaceOffset);
        transform.position = Camera.main.WorldToScreenPoint(objectWithOverhead.transform.position + Tools.AngleToVector3(Camera.main.transform.eulerAngles.z) * worldSpaceOffset) + Vector3.up * 60;

    }

    public void SwitchOverhead(string overhead)
    {
        if (overhead == "Protect from Magic")
        {
            image.texture = prayMageTexture;
        }
        else if (overhead == "Protect from Missiles")
        {
            image.texture = prayRangeTexture;
        }
        else if (overhead == "Protect from Melee")
        {
            image.texture = prayMeleeTexture;
        }
        else if (overhead == "Retribution")
        {
            image.texture = retributionTexture;
        }
        else if (overhead == "Redemption")
        {
            image.texture = redemptionTexture;
        }
        else if (overhead == "Smite")
        {
            image.texture = smiteTexture;
        }
    }
}
