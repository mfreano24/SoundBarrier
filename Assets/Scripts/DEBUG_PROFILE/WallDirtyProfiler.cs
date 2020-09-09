using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallDirtyProfiler : MonoBehaviour
{
    public GameObject wallPrefab;
    public int numberToSpawn = 100;
    Wall[] objects;
    private void Awake()
    {
        objects = new Wall[numberToSpawn];
        for(int i = 0; i < numberToSpawn; i++)
        {
            objects[i] = Instantiate(wallPrefab,transform,false).GetComponent<Wall>();
            objects[i].transform.position = new Vector3(Random.Range(-50f, 50f), 0, Random.Range(-50f, 50f));
            objects[i].transform.Rotate(new Vector3(0, Random.Range(0, 360),0));
            StartCoroutine(objects[i].WallReaction());
        }
    }
}
