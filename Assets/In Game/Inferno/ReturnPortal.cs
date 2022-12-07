using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnPortal : MonoBehaviour
{
    public InfernoManager manager;
    InteractableObject objectScript;
    Action objectAction;

    private IEnumerator Start()
    {
        if (OptionManager.ignorePlayerDeath == false)
        {
            Destroy(gameObject);
        }

        objectScript = GetComponent<InteractableObject>();
        objectScript.interaction += Teleport;

        objectAction = GetComponent<Action>();

        yield return null;

        objectAction.menuTexts[0] = "Return ";
        objectAction.UpdateName();
    }

    void Teleport()
    {
        manager.ReturnToLobby();
    }

    private void Update()
    {
        transform.eulerAngles += Vector3.forward * 200 * Time.deltaTime;
    }
}
