using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    public static PlayerAudio instance;

    [HideInInspector] public AudioSource source;

    public AudioClip equipSound;
    public AudioClip unequipSound;
    public AudioClip takeDamageSound;

    public AudioClip outOfPrayerSound;
    public AudioClip deactivatePrayerSound;
    public AudioClip prayerUnavailableSound;

    public AudioClip diamondBoltProcSound;
    public AudioClip rubyBoltProcSound;

    private void Start()
    {
        instance = this;
        source = GetComponent<AudioSource>();
    }

    public static void PlayClip(AudioClip clip)
    {
        instance.source.PlayOneShot(clip);
    }
}
