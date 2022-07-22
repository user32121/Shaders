using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject prefab;
    public int spawnInterval;  //in (nonphysics) frames
    int counter;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (counter++ > spawnInterval)
        {
            Instantiate(prefab, transform.position, Quaternion.identity);
            counter -= spawnInterval;
        }
    }
}
