using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Minimap : MonoBehaviour, IPointerClickHandler
{
    Image mapArea;
    public Camera minimapCam;
    CameraScript mainCamera;
    Camera newMinimapCam;
    public RawImage flag;
    RawImage newFlag;
    public GameObject inGameFlag;
    GameObject newInGameFlag;

    private void Start()
    {
        mainCamera = Camera.main.GetComponent<CameraScript>();
        newMinimapCam = Instantiate(minimapCam, Vector3.back * -10, Quaternion.identity);

        mapArea = GetComponent<Image>();
        mapArea.alphaHitTestMinimumThreshold = 0.5f;
    }

    private void Update()
    {
        newMinimapCam.transform.position = mainCamera.player.transform.position + Vector3.back * 10;
        newMinimapCam.transform.eulerAngles = mainCamera.transform.eulerAngles;
    }


    public void OnPointerClick(PointerEventData pointerEventData)
    {
        Vector2 relPos = Input.mousePosition - mapArea.rectTransform.position;
        Vector3 camScreenPos = (relPos / (mapArea.rectTransform.rect.width / 2)) * newMinimapCam.orthographicSize;
        Vector3 worldPosition = camScreenPos + minimapCam.transform.position + Vector3.forward * 10;
        Debug.Log(relPos + " " + camScreenPos + " " + worldPosition);

        if (newInGameFlag == null)
        {
            newInGameFlag = Instantiate(inGameFlag, worldPosition, Quaternion.identity);
        }
        newInGameFlag.transform.position = worldPosition +  Vector3.up * 0.8f;

        //if (newFlag == null)
        //{
        //    newFlag = Instantiate(flag, transform.parent);
        //}
        //newFlag.rectTransform.position = Input.mousePosition;
    }
}
