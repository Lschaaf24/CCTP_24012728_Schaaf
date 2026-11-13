using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacement : MonoBehaviour
{
    [SerializeField] private Transform objectParent;

    public void ClearPlacedObjects()
    {
        if (objectParent == null) return;

        for (int i = objectParent.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(objectParent.GetChild(i).gameObject);
        }
    }

    public void PlaceObjectsInRoom(List<RoomData> rooms, HashSet<Vector2Int> floorTiles, HashSet<Vector2Int> corridorTiles)
    {
        HashSet<Vector2Int> reservedPositions = new HashSet<Vector2Int>(corridorTiles);

        foreach (RoomData room in rooms)
        {
            RoomRule rule = room.rule;
            if (rule == null || rule.spawnablePrefabs == null || rule.spawnablePrefabs.Count == 0)
                continue;

            int objectCount = Random.Range(rule.minObjects, rule.maxObjects + 1);
            int placed = 0, attempts = 0;

            while (placed < objectCount && attempts < 50 && rule.spawnablePrefabs.Count > 0)
            {
                attempts++;

                Vector2Int spawnPos = GetRandomPositionInRoom(room.bounds);
                if (reservedPositions.Contains(spawnPos) || !floorTiles.Contains(spawnPos))
                    continue;

                GameObject prefab = rule.spawnablePrefabs[Random.Range(0, rule.spawnablePrefabs.Count)];
                Vector3 worldPos = new Vector3(spawnPos.x + 0.5f, spawnPos.y + 0.5f, 0);
                Instantiate(prefab, worldPos, Quaternion.identity, objectParent);

                reservedPositions.Add(spawnPos);
                placed++;
            }

            int enemyPlaced = 0, enemyAttempts = 0;
            while (enemyPlaced < rule.enemyCount && enemyAttempts < 50 && rule.enemyPrefabs.Count > 0)
            {
                Vector2Int spawnPos = GetRandomPositionInRoom(room.bounds);
                if (reservedPositions.Contains(spawnPos) || !floorTiles.Contains(spawnPos))
                    continue;

                GameObject prefab = rule.enemyPrefabs[Random.Range(0, rule.enemyPrefabs.Count)];
                Vector3 worldPos = new Vector3(spawnPos.x + 0.5f, spawnPos.y + 0.5f, 0);
                Instantiate(prefab, worldPos, Quaternion.identity, objectParent);

                reservedPositions.Add(spawnPos);
                enemyPlaced++;
            }

/*             if (room.rule.roomType == RoomType.Treasure && rule.chestPrefab != null)
            {
                for (int i = 0; i < 10; i++) // Try 10 times max
                {
                    Vector2Int spawnPos = GetRandomPositionInRoom(room.bounds);
                    if (reservedPositions.Contains(spawnPos) || !floorTiles.Contains(spawnPos))
                        continue;

                    Vector3 worldPos = new Vector3(spawnPos.x + 0.5f, spawnPos.y + 0.5f, 0);
                    Instantiate(rule.chestPrefab, worldPos, Quaternion.identity, objectParent);

                    reservedPositions.Add(spawnPos);
                    break;
                }
            } */
        }
    }
        private Vector2Int GetRandomPositionInRoom(BoundsInt room)
    {
        int x = Random.Range(room.xMin + 1, room.xMax - 1);
        int y = Random.Range(room.yMin + 1, room.yMax - 1);
        return new Vector2Int(x, y);
    }


}
