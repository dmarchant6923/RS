using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{
    public AudioClip InfernoTrack;
    public List<AudioClip> lobbyTracks = new List<AudioClip>();

    public static Music instance;

    private void Start()
    {
        instance = this;
    }

    public static void PlayLobbyTrack()
    {
        int rand = Random.Range(0, instance.lobbyTracks.Count);
        UIManager.instance.musicSource.clip = instance.lobbyTracks[rand];
        UIManager.instance.musicSource.Play();
    }

    public static void PlayInfernoTrack()
    {
        UIManager.instance.musicSource.clip = instance.InfernoTrack;
        UIManager.instance.musicSource.Play();
    }

    public static void FadeTrack()
    {
        instance.StartCoroutine(instance.FadeTrackEnum());
    }

    public IEnumerator FadeTrackEnum()
    {
        float fadeWindow = 1.5f;

        float fadeTime = 0;
        if (UIManager.instance.musicSource.isPlaying)
        {
            while (fadeTime < fadeWindow)
            {
                UIManager.instance.musicSource.volume -= 1 / fadeWindow * Time.deltaTime;
                fadeTime += Time.deltaTime;
                yield return null;
            }
        }

        UIManager.instance.musicSource.Stop();
        UIManager.instance.musicSource.volume = 1;
    }
}
