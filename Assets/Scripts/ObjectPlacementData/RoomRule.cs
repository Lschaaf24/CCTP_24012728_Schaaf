using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomRule", menuName = "Dungeon/Room Rule")]
public class RoomRule : ScriptableObject
{
    public RoomType roomType;
    public int minObjects = 1;
    public int maxObjects = 3;
    public int weight = 1;
    public List<GameObject> spawnablePrefabs;

    public int enemyCount = 3;
    public List<GameObject> enemyPrefabs;

}
