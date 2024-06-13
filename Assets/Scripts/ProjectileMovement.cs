using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMovement : MonoBehaviour
{
    float speed = 1.0f;
    float maxSpeed = 5.0f;
    float scale = 0.02f;
    float damage = 1.0f;
    float spreadAngle = 2.5f;

    int maxHits = 1;
    int currentHits = 0;

    Rigidbody2D rb;

    Vector3 mousePos;
    Vector3 objectPos;
    Vector3 targetPos;

    float angle;

    bool gotDirection;
    bool isPlayer = true;
    bool angleGiven = false;

    int givenAngle = 0;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gotDirection = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!gotDirection)
        {
            if (isPlayer)
            {
                if (!angleGiven)
                {
                    pointAtMouse();
                }
                else
                {
                    pointAtMouse(givenAngle);
                }
            }
            else
            {
                if (!angleGiven)
                {
                    pointAtTarget(targetPos);
                }
                else
                {
                    pointAtTarget(targetPos, givenAngle);
                }
            }
        }
        if (rb.velocity.magnitude < maxSpeed)
        {
            rb.AddRelativeForce(new Vector3(speed, 0, 0));
        }
    }


    /* Called by the weapon object.
     * Sets the size, velocity and other settings then enables the projectile.*/
    public void initiateProjectile(float newSpeed, float newMaxSpeed, float newScale, float dam, int hits, float spread)
    {
        speed = newSpeed;
        maxSpeed = newMaxSpeed;
        transform.localScale = new Vector3(newScale, newScale, newScale);
        damage = dam;
        maxHits = hits;
        spreadAngle = spread;
    }

    public void initiateProjectile(float newSpeed, float newMaxSpeed, float newScale, float dam, int hits, float spread, int newAngle)
    {
        angleGiven = true;
        givenAngle = newAngle;
        initiateProjectile(newSpeed, newMaxSpeed, newScale, dam, hits, spread);
    }

    public void initiateEnemyProjectile(float newSpeed, float newMaxSpeed, float newScale, float dam, int hits, float spread, Vector3 target)
    {
        speed = newSpeed;
        maxSpeed = newMaxSpeed;
        transform.localScale = new Vector3(newScale, newScale, newScale);
        damage = dam;
        maxHits = hits;
        spreadAngle = spread;

        targetPos = target;

        isPlayer = false;
        gameObject.tag = "Enemy";
    }

    public void initiateEnemyProjectile(float newSpeed, float newMaxSpeed, float newScale, float dam, int hits, float spread, Vector3 target, int newAngle)
    {
        angleGiven = true;
        givenAngle = newAngle;
        initiateEnemyProjectile(newSpeed, newMaxSpeed, newScale, dam, hits, spread, target);
    }

    void pointAtMouse()
    {
        mousePos = Input.mousePosition;
        mousePos.z = 5.23f;
        objectPos = Camera.main.WorldToScreenPoint(transform.position);
        mousePos.x -= objectPos.x;
        mousePos.y -= objectPos.y;
        angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, generateSpreadAngle(angle)));
        gotDirection = true;
    }

    void pointAtMouse(int angle)
    {
        pointAtMouse();
        updateAngle(angle);
    }

    void pointAtTarget(Vector3 targetPos)
    {
        Vector3 diff = targetPos - transform.position;
        diff.Normalize();
        float zRot = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, generateSpreadAngle(zRot));
        gotDirection = true;
        gameObject.layer = 10;
    }

    void pointAtTarget(Vector3 targetPos, int angle)
    {
        pointAtTarget(targetPos);
        updateAngle(angle);
    }

    void updateAngle(int angle)
    {
        Quaternion tempRot = transform.rotation;
        tempRot *= Quaternion.Euler(0, 0, angle);
        transform.rotation = tempRot;
    }

    float generateSpreadAngle(float oldAngle)
    {
        return oldAngle + Random.Range(-spreadAngle, spreadAngle);
    }

    //Destroys the projectile when it collides with the borders.
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player" && !isPlayer)
        {
            //Player has been hit by an enemy projectile.
            if (damageCollision(collision))
            {
                collision.gameObject.GetComponentInChildren<TankManager>().gameOver();
            }
            else
            {
                givePoints(false, 1);
            }
            Destroy(gameObject);
        }
        else if (collision.gameObject.tag == "Enemy" && isPlayer)
        {
            //Enemy has been hit by a player projectile.
            if (damageCollision(collision))
            {
                givePoints(true, collision.gameObject.GetComponent<TankManager>().destroyedPoints());
                collision.gameObject.GetComponentInChildren<TankManager>().gameOver();
            }
            else
            {
                givePoints(true, 1);
            }
            Destroy(gameObject);
        }
        else if (collision.gameObject.tag == "Player" && isPlayer)
        {
            //Player has been hit by their own projectile.
        }
        else if (collision.gameObject.tag == "Enemy" && !isPlayer)
        {
            //Enemy has been hit by their own projectile.
        }
        else if (collision.gameObject.tag == "Border")
        {
            //Border has been hit by a projectile
            Destroy(gameObject);
        }
        else if (collision.gameObject.name.Contains("Object"))
        {
            //Object has been hit.
            givePoints(isPlayer , collision.gameObject.GetComponent<Object>().value);
            hit();
        }

    }

    /*Checks to see if the hit object is a tank or a bullet.
     *Damages the target if it is a tank.*/
    bool damageCollision(Collision2D target)
    {
        bool isTank = target.gameObject.GetComponent<TankManager>();
        if (isTank)
        {
             return target.gameObject.GetComponent<TankManager>().takeDamage(damage);
        }
        return false;
    }

    /*Gives points to either the player or the enemy if the other was hit.
     *The nested if prevents an error when a tank's projectile hits an object after the tank has been destroyed.*/
    void givePoints(bool isPlayer, int value)
    {
        if (isPlayer)
        {
            if (GameObject.Find("Player"))
            {
                GameObject.Find("Player").GetComponent<TankManager>().gainPoints(value);
            }
        }
        else
        {
            if (GameObject.Find("Enemy"))
            {
                GameObject.Find("Enemy").GetComponent<TankManager>().gainPoints(value);
            }
        }
    }

    /*Allows the projectile to hit multiple objects.*/
    public void hit()
    {
        currentHits++;
        if (currentHits >= maxHits)
        {
            Destroy(gameObject);
        }
    }
}
