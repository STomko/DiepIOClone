using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximityDetection : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        targetPlayer(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        targetPlayer(collision);
    }

    void targetPlayer(Collider2D target)
    {
        transform.parent.gameObject.GetComponent<EnemyMovement>().targetPlayer(target.gameObject.transform.position);
    }
}
