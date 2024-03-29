using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DialogueBox : MonoBehaviour
{
    public static RectTransform rt;
    public static InputField textInput;
    public static Text prompt;
    public static Vector3 onPosition;
    public static Vector2 offPosition;

    public static bool dialogueActive = false;

    public delegate void DialogueAction();
    //public static event DialogueAction DialogueCanceled;
    public static event DialogueAction InputSubmitted;
    public static string InputString;

    private void Awake()
    {
        gameObject.SetActive(true);
    }
    private void Start()
    {
        rt = GetComponent<RectTransform>();
        textInput = GetComponentInChildren<InputField>();
        prompt = transform.GetChild(0).GetComponent<Text>();

        onPosition = rt.position;
        offPosition = onPosition + Vector3.left * 1500;
        rt.position = offPosition;

        dialogueActive = false;
        textInput.text = "";
        InputString = "";

        Action.cancel1 += EndEdit;
    }

    private void Update()
    {
        if (dialogueActive && EventSystem.current.currentSelectedGameObject != textInput.gameObject)
        {
            EventSystem.current.SetSelectedGameObject(textInput.gameObject);
            textInput.interactable = true;
            textInput.ActivateInputField();
            textInput.interactable = false;
            StartCoroutine(EndFocus());
        }
        if (dialogueActive && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
        {
            EndEdit();
        }
    }

    IEnumerator EndFocus()
    {
        yield return null;
        textInput.caretPosition = textInput.text.Length;
    }

    public static void PlayerInput(string textPrompt)
    {
        if (dialogueActive)
        {
            CloseDialogue();
        }

        dialogueActive = true;
        rt.position = onPosition;
        prompt.text = textPrompt;
        textInput.ActivateInputField();
        textInput.interactable = false;
    }
    public static void CancelEdit()
    {
        InputString = "";
        EndEdit();
    }

    public static void EndEdit()
    {
        textInput.interactable = true;
        if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
        {
            InputString = textInput.text;
        }
        else
        {
            InputString = "";
        }
        InputSubmitted?.Invoke();
        CloseDialogue();
    }

    public static void CloseDialogue()
    {
        InputString = "";
        textInput.text = "";
        textInput.DeactivateInputField();
        rt.position = offPosition;
        dialogueActive = false;
    }

    private void OnDestroy()
    {
        Action.cancel1 -= EndEdit;
    }
}
