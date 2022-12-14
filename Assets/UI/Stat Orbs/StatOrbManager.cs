using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StatOrbManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Toggle orbToggle;
    public delegate void OrbEvent();

    public bool active = false;

    public Sprite panelHighlighted;
    public Sprite panelNotHighlighted;

    public Texture onSpriteBackground;
    public Texture offSpriteBackground;
    public Texture onSpriteIcon;
    public Texture offSpriteIcon;
    public Texture disabledSpriteBackground;

    public Image panelSprite;
    public RawImage backgroundSprite;
    public RawImage iconSprite;

    public bool canBeToggled = true;

    public RectTransform maskRT;
    float initialScale;
    float initialPosition;
    float minScale;
    float minPosition;
    float currentScale;
    float currentPosition;

    public Text number;
    public float initialValue;
    [HideInInspector] public float value;

    [HideInInspector] public Action orbAction;


    private void Start()
    {
        orbToggle = GetComponent<Toggle>();
        //orbToggle.onValueChanged.AddListener(delegate { Toggle(); });

        transform.GetChild(0).GetComponent<Image>().alphaHitTestMinimumThreshold = 0.5f;

        if (initialValue == 0)
        {
            initialValue = 100;
        }

        initialScale = maskRT.rect.height;
        initialPosition = maskRT.rect.position.y;

        minScale = 0;
        minPosition = initialPosition - initialScale / 2;

        if (disabledSpriteBackground != null && canBeToggled == false)
        {
            backgroundSprite.texture = disabledSpriteBackground;
        }

        TickManager.afterTick += AfterTick;

        PlayerStats.newStats += ChangeInitialStats;

        orbAction = GetComponent<Action>();
        if (orbAction != null)
        {
            orbAction.clientAction0 += ClientClickedToggle;
        }
    }

    public void ClientClickedToggle()
    {
        if (canBeToggled)
        {
            SwitchSprites();
        }
    }


    public void SwitchSprites(bool isOn)
    {
        if (canBeToggled)
        {
            if (isOn)
            {
                backgroundSprite.texture = onSpriteBackground;
                iconSprite.texture = onSpriteIcon;
            }
            else
            {
                backgroundSprite.texture = offSpriteBackground;
                iconSprite.texture = offSpriteIcon;
            }
        }
        else if (disabledSpriteBackground != null)
        {
            backgroundSprite.texture = disabledSpriteBackground;
        }
    }

    void SwitchSprites()
    {
        if (canBeToggled)
        {
            if (backgroundSprite.texture == offSpriteBackground)
            {
                backgroundSprite.texture = onSpriteBackground;
                iconSprite.texture = onSpriteIcon;
            }
            else
            {
                backgroundSprite.texture = offSpriteBackground;
                iconSprite.texture = offSpriteIcon;
            }
        }
        else if (disabledSpriteBackground != null)
        {
            backgroundSprite.texture = disabledSpriteBackground;
        }
    }


    void AfterTick()
    {
        orbToggle.isOn = active;
        SwitchSprites(active);

        UpdateMask();
    }
    public void UpdateMask()
    {
        value = float.Parse(number.text);

        currentScale = minScale + ((initialScale - minScale) * value / initialValue);
        currentPosition = minPosition + ((initialPosition - minPosition) * value / initialValue) - 7f;
        maskRT.sizeDelta = new Vector2(maskRT.sizeDelta.x, currentScale);
        maskRT.localPosition = new Vector2(maskRT.localPosition.x, currentPosition);


        if (value >= initialValue / 2)
        {
            number.color = new Color(1 - (value - (initialValue / 2)) / initialValue * 2, 1, 0);
        }
        else
        {
            number.color = new Color(1, value / initialValue * 2, 0);
        }
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (canBeToggled)
        {
            panelSprite.sprite = panelHighlighted;
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (canBeToggled)
        {
            panelSprite.sprite = panelNotHighlighted;
        }
    }

    public void ChangeInitialStats()
    {
        if (GetComponent<HealthOrb>() != null)
        {
            initialValue = PlayerStats.initialHitpoints;
        }
        else if (GetComponent<PrayerToggle>() != null)
        {
            initialValue = PlayerStats.initialPrayer;
        }
    }
}
