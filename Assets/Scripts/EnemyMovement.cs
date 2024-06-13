using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public Rigidbody2D rb;

    float moveSpeed;
    float maxSpeed;

    Vector3 targetPos;
    float zPos = -1.0f;

    float waitTime = 5.0f;
    float nextMoveTime = 0.0f;

    float zRot;

    public GameObject target;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        targetPos = newTargetPos();

        moveSpeed = 1.0f;
        maxSpeed = 5.0f;
    }

    // Update is called once per frame
    void Update()
    {
        updatePosition();
        updateAngle();
        fireWeapon();
    }

    public Vector3 newTargetPos()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
        rb.velocity = Vector3.zero;

        float newX = Random.Range(-18.5f, 18.5f);
        float newY = Random.Range(-13.5f, 13.5f);
        return new Vector3 (newX, newY, zPos);
    }

    public void targetPlayer(Vector3 playerPos)
    {
        rb.velocity = Vector3.zero;
        targetPos = playerPos;
        updateAngle();
    }

    void updatePosition()
    {
        if (Time.time > nextMoveTime)
        {
            while (Time.time > nextMoveTime)
            {
                nextMoveTime += waitTime;
            }
            rb.velocity = Vector3.zero;
            targetPos = newTargetPos();
            updateAngle();
        }
        if (rb.velocity.magnitude < maxSpeed)
        {
            rb.AddRelativeForce(new Vector3(0, moveSpeed, 0));
            //rb.AddForce(transform.up * moveSpeed);
        }
    }

    public void updateAngle()
    {
        //DEBUG ONLY
        target = GameObject.Find("DEBUGEnemyTargetPos");
        target.transform.position = targetPos;
        //DEBUG ONLY

        //Gets the angle and rotates the enemy in that direction.
        Vector3 diff = targetPos - transform.position;
        diff.Normalize();
        zRot = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, zRot - 90);
    }

    void fireWeapon()
    {
        foreach (Transform weapon in transform)
        {
            if (weapon.gameObject.layer != 9)
            {
                string[] splitWeaponName = weapon.gameObject.name.Split(" ");
                if (splitWeaponName.Length == 2)
                {
                    weapon.gameObject.GetComponent<Weapon>().enemyFire(targetPos, int.Parse(splitWeaponName[1]));
                }
                else
                {
                    weapon.gameObject.GetComponent<Weapon>().enemyFire(targetPos);
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

    /*If the enemy goes past the target position and hits a wall,
     *it will generate a new position and move that way.
     *This does not reset the timer.*/
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Border")
        {
            targetPos = newTargetPos();
            updateAngle();
        }
    }
}
