using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    public static PlayerAudio instance;

    public AudioSource source;

    public AudioClip equipSound;
    public AudioClip unequipSound;
    public AudioClip takeDamage;

    private void Start()
    {
        instance = this;
        source = GetComponent<AudioSource>();
    }
}
