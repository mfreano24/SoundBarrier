using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Walls", menuName = "ScriptableObjects/WallSpawnManager", order = 1)]
public class WallSpawnManager : ScriptableObject
{
    //This is going to be a painful script to learn with but im determined

    public string prefabName;
    public int numPrefabs;
    public Vector3[] spawnPoints;
    public Vector3[] objectScales;
}
