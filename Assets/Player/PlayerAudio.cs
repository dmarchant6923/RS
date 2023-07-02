using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    public static PlayerAudio instance;

    [HideInInspector] public AudioSource source;

    public AudioClip equipSound;
    public AudioClip unequipSound;

    public AudioClip[] takeDamageSounds;

    public AudioClip outOfPrayerSound;
    public AudioClip deactivatePrayerSound;
    public AudioClip prayerUnavailableSound;
    public AudioClip redemptionHealSound;

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

    public static AudioClip PlayerDamageNoise()
    {
        int rand = Random.Range(0, instance.takeDamageSounds.Length);
        for (int i = 0; i < instance.takeDamageSounds.Length; i++)
        {
            if (rand == i)
            {
                return instance.takeDamageSounds[i];
            }
        }

        Debug.LogWarning("fix the rand stuff that picks the damage noise u make");
        return instance.takeDamageSounds[0];
    }
}
