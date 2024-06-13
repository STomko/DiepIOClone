using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public GameObject player;
    public Camera cam;

    public int camZPos = -10;
    public int maxXPos = 10;
    public int maxYPos = 10;

    public float defaultCamSize = 7.0f;
    public float currentCamsize;
    public float zoomAmount = 0.25f;
    public float minZoom = 1.0f;
    public float maxZoom = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
        currentCamsize = defaultCamSize;
    }

    // Update is called once per frame
    void Update()
    {
        followPlayer();
        cameraZoom();
    }

    /* Sets the camera position to the position of the player.
     * If the player moves towards an edge or corner (X or Y of 10 or -10), then the camera remains in the game.*/
    void followPlayer()
    {
        if (player == null)
        {
            player = GameObject.Find("Enemy");
        }
        Vector3 camPos = player.transform.position;
        camPos.z = camZPos;

        if (camPos.x > maxXPos)
        {
            camPos.x = maxXPos;
        }
        else if (camPos.x < -maxXPos)
        {
            camPos.x = -maxXPos;
        }

        if (camPos.y > maxYPos)
        {
            camPos.y = maxYPos;
        }
        else if (camPos.y < -maxYPos)
        {
            camPos.y = -maxYPos;
        }
        transform.position = camPos;
    }

    /*This is called when one of the tanks has been destroyed.
     *If the player is destroyed, the camera will follow the enemy.*/
    public void setTargetObject(GameObject targetObject)
    {
        player = targetObject;
    }

    /*Allows the player to zoom in and out with the scroll-wheel.
     *Allows the camera zoom to be reset with middle-click.*/
    void cameraZoom()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0.0f)
        {
            if (cam.orthographicSize > minZoom)
            {
                cam.orthographicSize -= zoomAmount;
            }
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0.0f)
        {
            if (cam.orthographicSize < maxZoom)
            {
                cam.orthographicSize += zoomAmount;
            }
        }
        if (Input.GetMouseButtonDown(2))
        {
            cam.orthographicSize = defaultCamSize;
        }
    }
}
