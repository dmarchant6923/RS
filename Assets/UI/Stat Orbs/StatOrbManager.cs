using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StatOrbManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    Toggle orbToggle;
    public delegate void OrbEvent();
    public event OrbEvent onToggle;

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
    float value;

    Action orbAction;
    MenuEntryClick menuEntry;


    private void Start()
    {
        orbToggle = GetComponent<Toggle>();
        //orbToggle.onValueChanged.AddListener(delegate { Toggle(); });

        transform.GetChild(0).GetComponent<Image>().alphaHitTestMinimumThreshold = 0.5f;

        initialScale = maskRT.rect.height;
        initialPosition = maskRT.rect.position.y;

        minScale = 0;
        minPosition = initialPosition - initialScale / 2;

        if (disabledSpriteBackground != null && canBeToggled == false)
        {
            backgroundSprite.texture = disabledSpriteBackground;
        }

        TickManager.afterTick += OnTick;

        orbAction = GetComponent<Action>();
    }

    private void Update()
    {
        value = float.Parse(number.text);

        currentScale = minScale + ((initialScale - minScale) * value / 100);
        currentPosition = minPosition + ((initialPosition - minPosition) * value / 100) - 12.5f;

        maskRT.sizeDelta = new Vector2(maskRT.sizeDelta.x, currentScale);
        maskRT.localPosition = new Vector2(maskRT.localPosition.x, currentPosition);


        if (value >= 50)
        {
            number.color = new Color(1 - (value - 50) / 100 * 2, 1, 0);
        }
        else
        {
            number.color = new Color(1, (value) / 100 * 2, 0);
        }

        if (RightClickMenu.menuOpen == false && menuEntry != null)
        {
            menuEntry = null;
        }
        if (RightClickMenu.menuOpen && RightClickMenu.openActions.Contains(orbAction))
        {
            if (menuEntry == null)
            {
                foreach (MenuEntryClick entry in RightClickMenu.newMenu.GetComponentsInChildren<MenuEntryClick>())
                {
                    if (entry.action == orbAction)
                    {
                        menuEntry = entry;
                        break;
                    }
                }
            }
            if (menuEntry != null)
            {
                menuEntry.clickMethod = ClickedToggle;
            }
        }
    }

    void OnTick()
    {
        orbToggle.isOn = active;
        SwitchSprites(active);
    }

    public void ClickedToggle()
    {
        if (canBeToggled)
        {
            active = !active;
            StartCoroutine(Toggle(active));
        }
    }

    public IEnumerator Toggle(bool active)
    {
        SwitchSprites(active);
        yield return new WaitForSeconds(TickManager.simLatency);
        if (canBeToggled)
        {
            if (onToggle != null)
            {
                onToggle();
            }
        }
        yield return null;
    }

    void SwitchSprites(bool isOn)
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (canBeToggled)
        {
            panelSprite.sprite = panelHighlighted;
            //RightClickMenu.menuStrings.Add(actionText);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (canBeToggled)
        {
            panelSprite.sprite = panelNotHighlighted;
            //RightClickMenu.menuStrings.Remove(actionText);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ClickedToggle();
    }
}
