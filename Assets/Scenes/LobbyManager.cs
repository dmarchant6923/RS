using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    public static bool firstLoad = true;
    public GameObject nibbler;

    private IEnumerator Start()
    {
        if (FindObjectOfType<UIManager>() == null)
        {
            Destroy(FindObjectOfType<Canvas>().gameObject);
            SceneManager.LoadScene("UI Scene", LoadSceneMode.Additive);
        }
        Canvas.ForceUpdateCanvases();
        yield return null;
        PlayerStats.ResetAllAttributes();
        Player.player.SetNewPosition(Vector2.zero);
        Action.ignoreAllActions = false;
        if (firstLoad)
        {
            UIManager.instance.frontBlackScreen.color = Color.black;
        }
        UIManager.instance.fadeBox.color = Color.black;
        Player.player.ClearDamageQueue();
        StartCoroutine(Unfade());
        if (PlayerPrefs.GetInt("nibbler", 0) == 1)
        {
            GameObject newNibbler = Instantiate(nibbler, new Vector2(0, 3), Quaternion.identity);
            newNibbler.name = nibbler.name;
        }

        yield return new WaitForSeconds(0.1f);

        Music.PlayLobbyTrack();
    }

    IEnumerator Unfade()
    {
        yield return null;
        UIManager.instance.frontBlackScreen.color = new Color(0, 0, 0, 0);
        CameraScript.instance.ResetCameraPosition();
        yield return new WaitForSeconds(0.5f);
        Color color = UIManager.instance.fadeBox.color;
        while (color.a > 0)
        {
            color = UIManager.instance.fadeBox.color;
            color.a -= Time.deltaTime * 0.8f;
            UIManager.instance.fadeBox.color = color;
            yield return null;
        }
    }
}
