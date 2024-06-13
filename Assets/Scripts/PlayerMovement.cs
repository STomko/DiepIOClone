using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;

    float moveSpeed;
    float maxSpeed;

    Vector3 mousePos;
    Vector3 objectPos;
    float angle;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        moveSpeed = 1.0f;
        maxSpeed = 5.0f;

        setPlayerCamFollow();
    }

    // Update is called once per frame
    void Update()
    {
        detectKeyPress();
        pointAtMouse();
        detectClick();
    }

    /* Waits for the user to press wasd and moves them in that direction.
     * moveSpeed is the force applied when the button is pressed.
     * maxSpeed is the maximum speed that the object will move at.*/
    void detectKeyPress()
    {
        if (Input.GetKey("w"))
        {
            if (rb.velocity.y < maxSpeed)
            {
                rb.AddForce(new Vector3(0, moveSpeed, 0));
            }
        }
        if (Input.GetKey("s"))
        {
            if (rb.velocity.y > -maxSpeed)
            {
                rb.AddForce(new Vector3(0, -moveSpeed, 0));
            }
        }
        if (Input.GetKey("d"))
        {
            if (rb.velocity.x < maxSpeed)
            {
                rb.AddForce(new Vector3(moveSpeed, 0, 0));
            }
        }
        if (Input.GetKey("a"))
        {
            if (rb.velocity.x > -maxSpeed)
            {
                rb.AddForce(new Vector3(-moveSpeed, 0, 0));
            }
        }
    }

    /* Uses the position of the cursor on the screen and the player position.
     * Ensures that the player is constantly aiming in the direction of the cursor.*/
    void pointAtMouse()
    {
        mousePos = Input.mousePosition;
        mousePos.z = 5.23f;
        objectPos = Camera.main.WorldToScreenPoint(transform.position);
        mousePos.x -= objectPos.x;
        mousePos.y -= objectPos.y;
        angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    /* Checks for the user pressing the mouse cursor.
     * Fires a projectile for each attached weapon.
     Checks for a number at the end of the string, calls a different function if found.*/
    void detectClick()
    {
        if (Input.GetMouseButton(0))
        {
            foreach (Transform weapon in transform)
            {
                if (weapon.gameObject.layer != 9)
                {
                    string[] splitWeaponName = weapon.gameObject.name.Split(" ");
                    if (splitWeaponName.Length == 2)
                    {
                        weapon.gameObject.GetComponent<Weapon>().fire(int.Parse(splitWeaponName[1]));
                    }
                    else
                    {
                        weapon.gameObject.GetComponent<Weapon>().fire();
                    }
                }
            }
        }
    }

    /*Increases the movement and maximum speed of the tank.
     *Called whenever the tank gains an upgrade point (every 30 seconds).*/
    public void levelUpSpeed()
    {
        moveSpeed += GameController.speedIncrease;
        maxSpeed += GameController.speedIncrease;
    }

    void setPlayerCamFollow()
    {
        CameraMovement cm = GameObject.Find("Main Camera").GetComponent<CameraMovement>();
        cm.player = gameObject;
    }
}
