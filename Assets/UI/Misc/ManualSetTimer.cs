using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManualSetTimer : MonoBehaviour
{
    public Text timerText;

    public ButtonScript button;

    int step;

    float timer = 75;
    bool timerPaused = false;

    private IEnumerator Start()
    {
        if (OptionManager.showManualSetTimer == false)
        {
            Debug.Log("you are here");
            gameObject.SetActive(false);
        }
        step = -1;
        yield return null;
        button.buttonClicked += NextStep;
        NextStep();
    }

    private void Update()
    {
        if (timerPaused)
        {
            return;
        }

        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            timer += 210;
        }

        timerText.text = Tools.SecondsToMinutes(timer, true);
    }

    public void NextStep()
    {
        step++;
        if (step >= 4)
        {
            step = 0;
        }

        if (step == 0)
        {
            timer = 75;
            timerPaused = true;
            button.buttonAction.menuTexts[0] = "Start timer";
            timerText.text = Tools.SecondsToMinutes(timer, true);
            button.buttonText.text = "Start";
        }
        else if (step == 1)
        {
            timerPaused = false;
            button.buttonAction.menuTexts[0] = "Pause timer";
            button.buttonText.text = "Pause";
        }
        else if (step == 2)
        {
            timerPaused = true;
            button.buttonAction.menuTexts[0] = "Resume timer";
            timer += 105;
            timerText.text = Tools.SecondsToMinutes(timer, true);
            button.buttonText.text = "Resume";
        }
        else if (step == 3)
        {
            timerPaused = false;
            button.buttonAction.menuTexts[0] = "Reset timer";
            button.buttonText.text = "Reset";
        }
    }
}
