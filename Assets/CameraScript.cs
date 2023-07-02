using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    float maxCamSize = 13; //13
    float minCamSize = 3;
    float camSize;

    float minCamSpeed = 0.5f;
    float maxCamSpeed = 10;
    float camSpeed;

    public float rotateSensitivity = 2.5f;

    [System.NonSerialized] float zoomSensitivity = 50;

    Camera cam;
    [HideInInspector] public Player player;

    public static CameraScript instance;

    void Start()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
        camSize = PlayerPrefs.GetFloat("Camera Size", 5);

        cam = GetComponent<Camera>();
        cam.orthographicSize = camSize;

        player = FindObjectOfType<Player>();
        cam.transform.position = player.transform.position + Vector3.back * 10;

        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.mouseScrollDelta.y != 0 && MouseManager.mouseOnScreen)
        {
            if (cam.orthographicSize >= minCamSize && cam.orthographicSize <= maxCamSize)
            {
                cam.orthographicSize -= Input.mouseScrollDelta.y * Time.deltaTime * zoomSensitivity;
            }

            if (cam.orthographicSize <= minCamSize)
            {
                cam.orthographicSize = minCamSize;
            }
            else if (cam.orthographicSize >= maxCamSize)
            {
                cam.orthographicSize = maxCamSize;
            }
        }

        camSpeed = minCamSpeed + Mathf.Max((transform.position - player.transform.position).magnitude / 3 * (maxCamSpeed - minCamSpeed), maxCamSpeed - minCamSpeed);
        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, camSpeed * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, transform.position.y, -1);

        if (Input.GetMouseButton(2))
        {
            transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z - Input.GetAxis("Mouse X") * rotateSensitivity);
        }
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetFloat("Camera Size", cam.orthographicSize);
    }

    public void ResetCameraPosition()
    {
        transform.position = new Vector3(player.playerPosition.x, player.playerPosition.y, -10);
    }

    //private void OnLevelWasLoaded(int level)
    //{
    //    ResetCameraPosition();
    //}
}
