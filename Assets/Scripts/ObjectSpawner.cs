using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject object1;
    public GameObject object2;
    public GameObject object3;
    public GameObject object4;

    public int maxObjects = 50;
    public int currentObjects = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        currentObjects = getObjectCount();
        if (currentObjects < maxObjects)
        {
            spawnRandomObject();
        }
    }

    /*Pick a random object and spawn position.
     *Creates the object in that position.*/
    void spawnRandomObject()
    {
        float newX = Random.Range(-18.5f, 18.5f);
        float newY = Random.Range(-13.5f, 13.5f);
        float newZ = -1.0f;
        List<GameObject> objects = new List<GameObject> { object1, object2, object3, object4 };
        int index = Random.Range(0, objects.Count);

        GameObject newObject = Instantiate(objects[index], new Vector3(newX, newY, newZ), Quaternion.identity);
    }

    int getObjectCount()
    {
        return GameObject.FindGameObjectsWithTag("Object").Length;
    }
}
