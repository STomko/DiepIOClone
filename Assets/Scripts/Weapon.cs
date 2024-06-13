using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    //Change me!
    /* Default speed: 3.5f
     * Default maxSpeed: 7.0f
     * Default scale: size of the weapon/50
     * Default wait: 0.5f
     * Default damage: 1.0f
     * Default backForce: 10.0f
     * Default maxHits: 1*/
    public float speed = 3.5f;
    public float maxSpeed = 7.0f;
    float scale;
    public float waitTime = 0.5f;
    float minWaitTime = 0.05f;
    public float damage = 1.0f;
    public float backForce = 20.0f;
    public float spreadAngle = 2.5f;

    public int maxHits = 1;

    float nextFireTime = 0.0f;

    public GameObject projectile;
    public GameObject spawnPoint;
    ParticleSystem muzzleFlash;

    // Start is called before the first frame update
    void Start()
    {
        scale = transform.localScale.y/50;
        muzzleFlash = transform.GetChild(1).GetComponent<ParticleSystem>();
        muzzleFlash.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void fire()
    {
        if (Time.time > nextFireTime)
        {
            while (Time.time > nextFireTime)
            {
                nextFireTime += waitTime;
            }
            fireProjectile();
            StartCoroutine(showMuzzleFlash());
            pushBack(gameObject.transform.parent.gameObject.GetComponent<PlayerMovement>().rb, backForce);
        }
    }

    public void fire(int angle)
    {
        if (Time.time > nextFireTime)
        {
            while (Time.time > nextFireTime)
            {
                nextFireTime += waitTime;
            }
            fireProjectile(angle);
            StartCoroutine(showMuzzleFlash());
            pushBack(gameObject.transform.parent.gameObject.GetComponent<PlayerMovement>().rb, backForce);
        }
    }

    public void enemyFire(Vector3 targetPos)
    {
        if (Time.time > nextFireTime)
        {
            while (Time.time > nextFireTime)
            {
                nextFireTime += waitTime;
            }
            enemyFireProjectile(targetPos);
            StartCoroutine(showMuzzleFlash());
            pushBack(gameObject.transform.parent.gameObject.GetComponent<EnemyMovement>().rb, backForce * 3);
        }
    }

    public void enemyFire(Vector3 targetPos, int angle)
    {
        if (Time.time > nextFireTime)
        {
            while (Time.time > nextFireTime)
            {
                nextFireTime += waitTime;
            }
            enemyFireProjectile(targetPos, angle);
            StartCoroutine(showMuzzleFlash());
            pushBack(gameObject.transform.parent.gameObject.GetComponent<EnemyMovement>().rb, backForce * 3);
        }
    }

    public void fireProjectile()
    {
        GameObject currentProjectile = Instantiate(projectile, spawnPoint.transform.position, Quaternion.identity);
        currentProjectile.SetActive(true);
        currentProjectile.gameObject.GetComponent<ProjectileMovement>().initiateProjectile(speed, maxSpeed, scale, damage, maxHits, spreadAngle);
    }

    public void fireProjectile(int givenAngle)
    {
        GameObject currentProjectile = Instantiate(projectile, spawnPoint.transform.position, Quaternion.identity);
        currentProjectile.SetActive(true);
        currentProjectile.gameObject.GetComponent<ProjectileMovement>().initiateProjectile(speed, maxSpeed, scale, damage, maxHits, spreadAngle, givenAngle);
    }

    public void enemyFireProjectile(Vector3 targetPos)
    {
        GameObject currentProjectile = Instantiate(projectile, spawnPoint.transform.position, Quaternion.identity);
        currentProjectile.SetActive(true);
        currentProjectile.gameObject.GetComponent<ProjectileMovement>().initiateEnemyProjectile(speed, maxSpeed, scale, damage, maxHits, spreadAngle, targetPos);
    }

    public void enemyFireProjectile(Vector3 targetPos, int givenAngle)
    {
        GameObject currentProjectile = Instantiate(projectile, spawnPoint.transform.position, Quaternion.identity);
        currentProjectile.SetActive(true);
        currentProjectile.gameObject.GetComponent<ProjectileMovement>().initiateEnemyProjectile(speed, maxSpeed, scale, damage, maxHits, spreadAngle, targetPos, givenAngle);
    }

    public void pushBack(Rigidbody2D rb, float pushForce)
    {
        rb.AddRelativeForce(new Vector3(-pushForce, 0, 0));
    }

    public void upgradeProjSpeed(float increase)
    {
        speed += increase;
        maxSpeed += increase;
    }

    public void upgradeProjPenetration(int increase)
    {
        maxHits += increase;
    }

    public void upgradeReloadSpeed(float increase)
    {
        if (waitTime > minWaitTime)
        {
            waitTime += increase;
        }
    }

    private IEnumerator showMuzzleFlash()
    {
        muzzleFlash.Play();
        yield return new WaitForSeconds(0.1f);
        muzzleFlash.Stop();
    }
}
