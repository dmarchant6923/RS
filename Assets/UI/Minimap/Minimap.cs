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

    Vector2 serverClickedPosition;

    public static Minimap instance;
    Action mapAction;

    private void Start()
    {
        instance = this;
        mapAction = GetComponent<Action>();
        mapAction.serverAction0 += ServerClick;

        mainCamera = Camera.main.GetComponent<CameraScript>();
        newMinimapCam = Instantiate(minimapCam, Vector3.back * -10, Quaternion.identity);
        minimapCam.backgroundColor = Camera.main.backgroundColor;

        mapArea = GetComponent<Image>();
        mapArea.alphaHitTestMinimumThreshold = 0.5f;

        TickManager.beforeTick += BeforeTick;
    }

    private void Update()
    {
        newMinimapCam.transform.position = mainCamera.player.transform.position + Vector3.back * 10;
        newMinimapCam.transform.eulerAngles = mainCamera.transform.eulerAngles;
        if (newInGameFlag != null)
        {
            newInGameFlag.transform.eulerAngles = mainCamera.transform.eulerAngles;
        }
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        Vector2 relPos = Input.mousePosition - mapArea.rectTransform.position;
        float angleToImageCenter = Tools.VectorToAngle(relPos);
        float worldSpaceAngle = angleToImageCenter + newMinimapCam.transform.eulerAngles.z;
        Vector2 newPos = Tools.AngleToVector(worldSpaceAngle) * relPos.magnitude;

        Vector3 camScreenPos = (newPos / (mapArea.rectTransform.rect.width / 2)) * newMinimapCam.orthographicSize;
        Vector3 worldPosition = camScreenPos + newMinimapCam.transform.position + Vector3.forward * 10;

        Player.player.trueTileScript.ClientClick(worldPosition);
        mapAction.PickAction(0);
        StartCoroutine(ServerPosition(worldPosition));
    }

    IEnumerator ServerPosition(Vector2 position)
    {
        yield return new WaitForSeconds(TickManager.simLatency);
        serverClickedPosition = position;
    }

    void ServerClick()
    {
        Player.player.trueTileScript.ExternalMovement(serverClickedPosition);
    }

    void BeforeTick()
    {
        if (newInGameFlag != null)
        {
            if (Player.player.trueTile == TileManager.FindTile(newInGameFlag.transform.position))
            {
                Destroy(newInGameFlag);
            }
        }
    }

    public static void PlaceFlag(Vector2 worldPosition)
    {
        if (instance.newInGameFlag == null)
        {
            instance.newInGameFlag = Instantiate(instance.inGameFlag, worldPosition, Quaternion.identity);
        }
        instance.newInGameFlag.transform.position = worldPosition;
    }
}
