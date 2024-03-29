using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsCheckmark : MonoBehaviour
{
    Action checkAction;
    public SettingsPanel panelScript;

    public Texture checkOn;
    public Texture checkOff;
    RawImage checkImage;

    public bool check = false;

    public GameObject warning;

    [HideInInspector] public int checkNumber;

    private void Awake()
    {
        checkImage = GetComponent<RawImage>();
        checkAction = GetComponent<Action>();
    }
    private void Start()
    {
        checkAction.menuTexts[0] = check ? "Disable" : "Enable";
        checkAction.clientAction0 += Check;
        if (warning != null)
        {
            panelScript.CheckWarning();
            warning.SetActive(check);
        }

        Check(panelScript.settings.bools[checkNumber]);
    }

    void Check()
    {
        check = !check;
        Check(check);
        PlayerAudio.PlayClip(PlayerAudio.instance.menuClickSound);
    }

    public void Check(bool on)
    {
        check = on;
        if (checkImage != null)
        {
            checkImage.texture = check ? checkOn : checkOff;
            checkAction.menuTexts[0] = check ? "Disable" : "Enable";
        }
        if (warning != null)
        {
            panelScript.CheckWarning();
            warning.SetActive(check);
        }
    }
}
