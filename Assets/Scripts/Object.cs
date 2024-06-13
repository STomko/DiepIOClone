using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object : MonoBehaviour
{
    public int value;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*If a tank collides with this object, they gain points equal to the value.
     *However, they also lose that amount of health.*/
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<TankManager>())
        {
            collision.gameObject.GetComponent<TankManager>().hitByObject(value);
        }
        Destroy(gameObject);
    }
}
