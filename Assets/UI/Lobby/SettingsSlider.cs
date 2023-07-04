using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Audio;

public class SettingsSlider : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    RectTransform slider;
    public RectTransform marker;

    public bool music;
    public bool sfx;

    float maxX;
    [HideInInspector] public float value;

    bool held;

    public SettingsDisableButton soundButton;

    public AudioMixer mixer;

    void Start()
    {
        slider = GetComponent<RectTransform>();
        maxX = (slider.rect.width / 2) * 1.1f;
    }

    // Update is called once per frame
    void Update()
    {
        if (held)
        {
            marker.position = new Vector2(Mathf.Clamp(Input.mousePosition.x, slider.position.x - maxX, slider.position.x + maxX), marker.position.y);
            value = Mathf.Clamp(100 * (marker.position.x - slider.position.x + maxX) / (2f * maxX), 0f, 100f);
            value /= 100f;
        }
    }

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        held = true;
        if (soundButton.active == false)
        {
            soundButton.Toggle();
        }
    }

    public void OnPointerUp(PointerEventData pointerEventData)
    {
        held = false;
        SetVolume();
    }

    public void Toggle(bool _active)
    {
        if (_active)
        {
            marker.position = new Vector2(slider.position.x - maxX + value * maxX * 2, marker.position.y);
        }
        else
        {
            marker.position = new Vector2(slider.position.x - maxX, marker.position.y);
        }
        SetVolume();
    }

    public void SetVolume()
    {
        string str = "Music Volume";
        if (sfx)
        {
            str = "SFX Volume";
        }

        float _value = value;
        if (soundButton.active == false)
        {
            _value = 0;
        }

        PlayerPrefs.SetFloat(str, value);
        float db = -80 + Mathf.Pow(_value, 0.25f) * 80;
        mixer.SetFloat(str, db);
    }

    public void SetValue(float _value)
    {
        value = _value;
        Toggle(soundButton.active);

    }
}
