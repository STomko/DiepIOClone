using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankManager : MonoBehaviour
{
    public int score = 0;
    float health = 10.0f;
    float maxHealth = 10.0f;
    float healPercent = 0.05f;

    float waitTime = 2.0f;
    float nextHealTime = 0.0f;


    //NEEDS CHANGING
    public int upgradePoints = 0;

    public GameObject cam;
    public GameObject enemy;

    public GameController controller;

    bool destroyed = false;

    public int tier;
    public string branch;

    // Start is called before the first frame update
    void Start()
    {
        cam = GameObject.Find("Main Camera");
        controller = GameObject.Find("GameHandler").GetComponent<GameController>();
        if (gameObject.name =="Enemy")
        {
            enemy = GameObject.Find("Player");
        }
        else
        {
            enemy = GameObject.Find("Enemy");
        }
        if (!controller.hasPlayerRespawned)
        {
            transform.position = staringPosition();
            controller.hasPlayerRespawned = true;
        }

        for (int i = 0; i < GameController.currentUpgradeLevel; i++)
        {
            levelUpgrade();
        }

        if (controller.customTankSprite != null)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = controller.customTankSprite;
        }
        if (controller.customWeaponSprite != null)
        {
            foreach (Transform weapon in transform)
            {
                if (weapon.gameObject.layer != 9)
                {
                    weapon.gameObject.GetComponent<SpriteRenderer>().sprite = controller.customWeaponSprite;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextHealTime)
        {
            while (Time.time > nextHealTime)
            {
                nextHealTime += waitTime;
            }
            if (health < maxHealth)
            {
                health += maxHealth * healPercent;
            }
            else if (health > maxHealth)
            {
                health = maxHealth;
            }
        }
    }

    public bool takeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            if (!destroyed)
            {
                return true;
            }
        }
        return false;
    }

    public void gainPoints(int points)
    {
        score += points;
    }

    public void gainUpgradePoint()
    {
        upgradePoints += 1;
        levelUpgrade();
    }

    /*Upgrades the movement speed and health of the tank every level.
     *Also applies to the enemy tank.
     *This is called multiple times when the enemy tank respawns.*/
    public void levelUpgrade()
    {
        maxHealth += 2.0f;
        if (gameObject.name == "Player")
        {
            gameObject.GetComponent<PlayerMovement>().levelUpSpeed();
        }
        else
        {
            gameObject.GetComponent<EnemyMovement>().levelUpSpeed();
        }
    }

    public void upgradeRegen(float increase)
    {
        waitTime += increase;
    }

    public void upgradeProjSpeed(float increase)
    {
        foreach (Transform weapon in transform)
        {
            if (weapon.gameObject.layer != 9)
            {
                weapon.gameObject.GetComponent<Weapon>().upgradeProjSpeed(increase);
            }
        }
    }

    public void upgradeProjPenetration(int increase)
    {
        foreach (Transform weapon in transform)
        {
            if (weapon.gameObject.layer != 9)
            {
                weapon.gameObject.GetComponent<Weapon>().upgradeProjPenetration(increase);
            }
        }
    }

    public void upgradeReloadSpeed(float increase)
    {
        foreach (Transform weapon in transform)
        {
            if (weapon.gameObject.layer != 9)
            {
                weapon.gameObject.GetComponent<Weapon>().upgradeReloadSpeed(increase);
            }
        }
    }

    public bool spendUpgradePoint()
    {
        if (upgradePoints > 0)
        {
            upgradePoints--;
            return true;
        }
        return false;
    }

    /*Sets the camera to follow the enemy if the player has been destroyed.
     *If the enemy has been destroyed, the camera will continue to follow the player.
     *If both the player and enemy have been destroyed, the camera will point at the background.*/
    public void gameOver()
    {
        destroyed = true;
        if (enemy != null)
        {
            cam.GetComponent<CameraMovement>().setTargetObject(enemy);
        }
        else
        {
            cam.GetComponent<CameraMovement>().setTargetObject(GameObject.Find("Enemy"));
        }
        if (gameObject.name == "Enemy")
        {
            controller.respawnEnemy();
        }
        Destroy(gameObject);

    }

    /*Returns the current health as a percentage from 0 to 1.*/
    public float currentHealthPercent()
    {
        return health / maxHealth;
    }

    public void hitByObject(int value)
    {
        gainPoints(value);
        if (takeDamage(value))
        {
            gameOver();
        }
    }

    public void setEnemy(GameObject en)
    {
        enemy = en;
    }

    public int destroyedPoints()
    {
        return score / 2;
    }

    public Vector3 staringPosition()
    {
        float newX = Random.Range(-18.5f, 18.5f);
        float newY = Random.Range(-13.5f, 13.5f);
        float newZ = -1.0f;
        return new Vector3(newX, newY, newZ);
    }
}
