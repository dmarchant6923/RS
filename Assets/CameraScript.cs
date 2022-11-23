using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    float maxCamSize = 13;
    float minCamSize = 3;
    float camSize;

    float minCamSpeed = 0.5f;
    float maxCamSpeed = 10;
    float camSpeed;

    Camera cam;
    [HideInInspector] public Player player;

    void Start()
    {
        camSize = PlayerPrefs.GetFloat("Camera Size", 5);

        cam = GetComponent<Camera>();
        cam.orthographicSize = camSize;

        player = FindObjectOfType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.mouseScrollDelta.y != 0 && MouseManager.mouseOnScreen)
        {
            if (cam.orthographicSize >= minCamSize && cam.orthographicSize <= maxCamSize)
            {
                cam.orthographicSize -= Input.mouseScrollDelta.y * Time.deltaTime * 130;
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
        transform.position = new Vector3(transform.position.x, transform.position.y, -10);

        if (MouseManager.mouseOnScreen && Input.GetMouseButton(2))
        {

        }

        if (Input.GetMouseButton(2))
        {
            transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z - Input.GetAxis("Mouse X") * 4.2f);
        }
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetFloat("Camera Size", cam.orthographicSize);
    }
}
