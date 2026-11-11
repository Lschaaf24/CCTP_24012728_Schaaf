using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoomFirstDungeonGenerator : RandomWalkDungeonGenerator
{
    [SerializeField]private int minRoomWidth = 4, minRoomHeight = 4;
    [SerializeField]private int dungeonWidth = 20, dungeonHeight = 20;
    [SerializeField] [Range(0, 10)]
    private int offset = 1;
    [SerializeField] private bool randomWalkRooms = false;
    [SerializeField] private RoomRule defaultRoomRule;
    [SerializeField] private RoomRule spawnRoomRule;
    [SerializeField] private RoomRule bossRoomRule;
    [SerializeField] private List<RoomRule> weightedRoomRules;

    [SerializeField] private GameObject playerPrefab;
    private GameObject playerInstance;
    private BoundsInt spawnBounds;
    private BoundsInt bossBounds;


    protected override void RunProceduralGeneration()
    {
        CreateRooms();
    }

    private void CreateRooms()
    {
        var boundsList = ProceduralGeneration.BinarySpacePartitioning(new BoundsInt((Vector3Int)startPosition, new Vector3Int(dungeonWidth, dungeonHeight, 0)), minRoomWidth, minRoomHeight);
        List<RoomData> roomDataList = new List<RoomData>();
        foreach (var bounds in boundsList)
        {
            roomDataList.Add(new RoomData { bounds = bounds, rule = defaultRoomRule });
        }

        List<Vector2Int> roomCenters = new List<Vector2Int>();
        foreach (var room in roomDataList)
        {
            roomCenters.Add((Vector2Int)Vector3Int.RoundToInt(room.bounds.center));
        }

        spawnBounds = roomDataList[0].bounds;
        roomDataList[0].rule = spawnRoomRule;

        Vector3 playerSpawnPos = new Vector3(spawnBounds.center.x + 0.5f, spawnBounds.center.y + 0.5f, 0);

        GameObject existingPlayer = GameObject.FindGameObjectWithTag("Player");
        if (playerInstance != null)
        {
            existingPlayer.transform.position = playerSpawnPos;
        }
        else
        {
            playerInstance = Instantiate(playerPrefab, playerSpawnPos, Quaternion.identity);
        }

        Vector2Int spawnCenter = roomCenters[0];

        int bossIndex = 0;
        float maxDistance = 0f;
        for (int i = 1; i < roomCenters.Count; i++)
        {
            float distance = Vector2.Distance(roomCenters[i], spawnCenter);
            if (distance > maxDistance)
            {
                maxDistance = distance;
                bossIndex = i;
            }
        }


        bossBounds = roomDataList[bossIndex].bounds;

        int minBossWidth = minRoomWidth + 20;
        int minBossHeight = minRoomHeight + 20;

        Vector3Int bossSize = bossBounds.size;
        Vector3Int bossPos = bossBounds.position;

        if (bossSize.x < minBossWidth)
        {
            int difference = minBossWidth - bossSize.x;
            bossPos.x -= difference / 2;
            bossSize.x = minBossWidth;
        }
        if (bossSize.y < minBossHeight)
        {
            int difference = minBossHeight - bossSize.y;
            bossPos.y -= difference / 2;
            bossSize.y = minBossHeight;
        }

        bossBounds = new BoundsInt(bossPos, bossSize);

        roomDataList[bossIndex].rule = bossRoomRule;
        roomDataList[bossIndex].bounds = bossBounds;

        for (int i = 0; i < roomDataList.Count; i++)
        {
            if (i == 0 || i == bossIndex)
                continue;
            roomDataList[i].rule = GetWeightedRandomRoomRule();
        }

        HashSet<Vector2Int> floor;
        if (randomWalkRooms)
        {
            floor = CreateRoomsRandomly(roomDataList);

        }
        else
        {
            floor = CreateSimpleRooms(roomDataList);
        }

       
        HashSet<Vector2Int> corridors = ConnectRooms(roomCenters);
        floor.UnionWith(corridors);

        tilemapVisualizer.PaintFloorWithType(floor);
        WallGenerator.CreateWalls(floor, tilemapVisualizer);
        objectPlacement.PlaceObjectsInRoom(roomDataList, floor , corridors);
        
    }

    private RoomRule GetWeightedRandomRoomRule()
    {
        int totalWeight = 0;
        foreach (var rule in weightedRoomRules)
        {
            totalWeight += rule.weight;
        }

        int randomValue = Random.Range(0, totalWeight);
        int cumulative = 0;

        foreach (var rule in weightedRoomRules)
        {
            cumulative += rule.weight;
            if (randomValue < cumulative)
                return rule;
        }

        return defaultRoomRule;
    }

    private HashSet<Vector2Int> CreateRoomsRandomly(List<RoomData> roomsList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        foreach(var roomData in roomsList)
        {
            var roomBounds = roomData.bounds;
            var roomCenter = new Vector2Int(Mathf.RoundToInt(roomBounds.center.x), Mathf.RoundToInt(roomBounds.center.y));
            var roomFloor = RunRandomWalk(randomWalkParameters, roomCenter);
            foreach (var position in roomFloor)
            {
                if (position.x >= (roomBounds.xMin + offset) && position.x <= (roomBounds.xMax - offset) && position.y >= (roomBounds.yMin - offset) && position.y <= (roomBounds.yMax - offset))
                {
                    floor.Add(position);    
                }
            }
        }
        return floor;
    }

    private HashSet<Vector2Int> ConnectRooms(List<Vector2Int> roomCenters)
    {
        HashSet<Vector2Int> corridors = new HashSet<Vector2Int>();
        var currentRoomCenter = roomCenters[Random.Range(0, roomCenters.Count)];
        roomCenters.Remove(currentRoomCenter);

        while (roomCenters.Count > 0)
        {
            Vector2Int closest = FindClosestPointTo(currentRoomCenter, roomCenters);
            roomCenters.Remove(closest);
            HashSet<Vector2Int> newCorridor = CreateCorridor(currentRoomCenter, closest);
            currentRoomCenter = closest;
            corridors.UnionWith(newCorridor);
        }
        return corridors;
    }

    private HashSet<Vector2Int> CreateCorridor(Vector2Int currentRoomCenter, Vector2Int destination)
    {
        HashSet<Vector2Int> corridor = new HashSet<Vector2Int>();
        var position = currentRoomCenter;
        corridor.Add(position);
        while (position.y != destination.y)
        {
            if (destination.y > position.y)
            {
                position += Vector2Int.up;
            }
            else if (destination.y < position.y)
            {
                position += Vector2Int.down;
            }
            corridor.Add(position);
            corridor.Add(position + Vector2Int.right);
        }
        while (position.x != destination.x)
        {
            if (destination.x > position.x)
            {
                position += Vector2Int.right;
            }
            else if (destination.x < position.x)
            {
                position += Vector2Int.left;
            }
            corridor.Add(position);
            corridor.Add(position + Vector2Int.up);
        }
        return corridor;
    }

    private Vector2Int FindClosestPointTo(Vector2Int currentRoomCenter, List<Vector2Int> roomCenters)
    {
        Vector2Int closest = Vector2Int.zero;
        float distance = float.MaxValue;
        foreach (var position in roomCenters)
        {
            float currentDistance = Vector2.Distance(position, currentRoomCenter);
            if (currentDistance < distance)
            {
                distance = currentDistance;
                closest = position;
            }

        }
        return closest;
    }

    private HashSet<Vector2Int> CreateSimpleRooms(List<RoomData> roomsList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();

        foreach (var roomData in roomsList)
        {
            var bounds = roomData.bounds;

            for (int col = offset; col < bounds.size.x - offset; col++)
            {
                for (int row = offset; row < bounds.size.y - offset; row++)
                {
                    Vector2Int position = (Vector2Int)bounds.min + new Vector2Int(col, row);
                    floor.Add(position);
                }
            }
        }
        return floor;
    }
}
