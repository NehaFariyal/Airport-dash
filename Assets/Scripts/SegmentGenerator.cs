using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SegmentGenerator: Spawns procedural track segments for the Airport Dash terminal concourse.
/// Automatically places obstacles (aeroplanes, luggage, airport carts, stanchion barriers)
/// and collectibles (coins, gems) while ensuring the player is firmly enclosed within the path.
/// Author: Sk. Neha Fariyal - 2303A51598 (Airport Dash)
/// </summary>
public class SegmentGenerator : MonoBehaviour
{
    [Header("Segment Prefabs")]
    [Tooltip("Airport concourse segment prefabs")]
    public GameObject[] segmentPrefabs;

    [Header("Fallback Segment Generation (if prefabs not assigned)")]
    public GameObject defaultSegmentTemplate;
    public GameObject[] obstaclePrefabs; // [0] = Luggage, [1] = Airport Cart, [2] = Stanchion Barrier, [3] = Airplane
    public GameObject airplaneObstaclePrefab;
    public GameObject flyingAirplanePrefab;
    public GameObject coinPrefab;
    public GameObject gemPrefab;

    [Header("Player Tracking")]
    public Transform playerTransform;

    [Header("Generation Parameters")]
    [Tooltip("Length of each track segment along Z")]
    public float segmentLength = 50f;
    [Tooltip("Number of segments kept active at any time")]
    public int initialSegmentsOnScreen = 7;
    [Tooltip("Distance ahead of player to trigger new segment spawn")]
    public float spawnDistanceThreshold = 200f;

    private float nextSpawnZ = 0f;
    private readonly List<GameObject> activeSegments = new List<GameObject>();

    private void Start()
    {
        if (playerTransform == null)
        {
            var playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) playerTransform = playerObj.transform;
        }

        // Spawn initial safe starting segments
        for (int i = 0; i < initialSegmentsOnScreen; i++)
        {
            // First 2 segments are clear of obstacles for a smooth start
            bool isSafeZone = (i < 2);
            SpawnSegment(isSafeZone);
        }
    }

    private void Update()
    {
        if (playerTransform == null) return;

        // When player gets close to the end of spawned segments, spawn more
        if (nextSpawnZ - playerTransform.position.z < spawnDistanceThreshold)
        {
            SpawnSegment(false);
            CleanUpOldSegments();
        }
    }

    public void SpawnSegment(bool isSafeZone = false)
    {
        GameObject newSegment = null;

        if (segmentPrefabs != null && segmentPrefabs.Length > 0)
        {
            int prefabIndex = isSafeZone ? 0 : Random.Range(0, segmentPrefabs.Length);
            newSegment = Instantiate(segmentPrefabs[prefabIndex], new Vector3(0, 0, nextSpawnZ), Quaternion.identity);
        }
        else if (defaultSegmentTemplate != null)
        {
            newSegment = Instantiate(defaultSegmentTemplate, new Vector3(0, 0, nextSpawnZ), Quaternion.identity);
            newSegment.SetActive(true);

            if (!isSafeZone)
            {
                PopulateSegmentWithAirportHazards(newSegment, nextSpawnZ);
            }
        }
        else
        {
            // Procedurally create standard airport concourse segment
            newSegment = CreateAirportConcourseSegment(nextSpawnZ, isSafeZone);
        }

        newSegment.name = $"Airport_Segment_{Mathf.RoundToInt(nextSpawnZ)}";

        // Add solid invisible boundary guard rails to PREVENT player going out of path
        AddBoundaryColliders(newSegment, nextSpawnZ);

        activeSegments.Add(newSegment);
        nextSpawnZ += segmentLength;
    }

    private void AddBoundaryColliders(GameObject segment, float baseZ)
    {
        // Left Guard Rail Wall
        GameObject leftBarrier = new GameObject("LeftPathBoundary");
        leftBarrier.transform.SetParent(segment.transform, false);
        leftBarrier.transform.localPosition = new Vector3(-4.6f, 2.0f, segmentLength / 2f);
        BoxCollider leftCol = leftBarrier.AddComponent<BoxCollider>();
        leftCol.size = new Vector3(0.4f, 4.0f, segmentLength);

        // Right Guard Rail Wall
        GameObject rightBarrier = new GameObject("RightPathBoundary");
        rightBarrier.transform.SetParent(segment.transform, false);
        rightBarrier.transform.localPosition = new Vector3(4.6f, 2.0f, segmentLength / 2f);
        BoxCollider rightCol = rightBarrier.AddComponent<BoxCollider>();
        rightCol.size = new Vector3(0.4f, 4.0f, segmentLength);
    }

    private void PopulateSegmentWithAirportHazards(GameObject segment, float baseZ)
    {
        // Lanes: Left (-3.0), Center (0), Right (+3.0)
        float[] lanes = new float[] { -3.0f, 0f, 3.0f };

        // 1. Aeroplane Obstacle / Hazards
        // Spawn an aeroplane on the track or apron (35% chance per segment)
        GameObject planePrefabToUse = airplaneObstaclePrefab;
        if (planePrefabToUse == null && obstaclePrefabs != null)
        {
            foreach (var obs in obstaclePrefabs)
            {
                if (obs != null && obs.name.ToLower().Contains("airplane"))
                {
                    planePrefabToUse = obs;
                    break;
                }
            }
        }

        if (planePrefabToUse != null && Random.value < 0.45f)
        {
            float planeLane = lanes[Random.Range(0, lanes.Length)];
            Vector3 planePos = new Vector3(planeLane, 0f, baseZ + Random.Range(18f, 32f));
            GameObject planeObs = Instantiate(planePrefabToUse, planePos, Quaternion.Euler(0, 180f, 0), segment.transform);
            planeObs.tag = "Obstacle";
        }
        else if (obstaclePrefabs != null && obstaclePrefabs.Length > 0)
        {
            // Standard Obstacle (Luggage / Cart / Stanchion)
            float obstacleLane = lanes[Random.Range(0, lanes.Length)];
            GameObject chosenObstacle = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];

            if (chosenObstacle != null)
            {
                Vector3 obstaclePos = new Vector3(obstacleLane, 0f, baseZ + Random.Range(15f, 35f));
                GameObject obs = Instantiate(chosenObstacle, obstaclePos, Quaternion.identity, segment.transform);
                obs.tag = "Obstacle";
            }
        }

        // 2. Flying Overhead Aeroplane (25% chance in the sky)
        if (flyingAirplanePrefab != null && Random.value < 0.35f)
        {
            Vector3 flyPos = new Vector3(Random.Range(-25f, 25f), 18f, baseZ);
            Instantiate(flyingAirplanePrefab, flyPos, Quaternion.Euler(5f, Random.Range(10f, 30f), 0), segment.transform);
        }

        // 3. Spawn Coins in alternate lane
        if (coinPrefab != null)
        {
            int coinLaneIndex = Random.Range(0, lanes.Length);
            float coinLane = lanes[coinLaneIndex];
            int coinCountInRow = Random.Range(3, 6);
            float startZ = baseZ + 8f;

            for (int c = 0; c < coinCountInRow; c++)
            {
                Vector3 coinPos = new Vector3(coinLane, 0.85f, startZ + (c * 2.8f));
                Instantiate(coinPrefab, coinPos, Quaternion.identity, segment.transform);
            }
        }

        // 4. Rare Gem Spawn (approx 35% chance per segment)
        if (gemPrefab != null && Random.value < 0.35f)
        {
            float gemLane = lanes[Random.Range(0, lanes.Length)];
            Vector3 gemPos = new Vector3(gemLane, 1.0f, baseZ + Random.Range(20f, 40f));
            Instantiate(gemPrefab, gemPos, Quaternion.identity, segment.transform);
        }
    }

    private GameObject CreateAirportConcourseSegment(float baseZ, bool isSafeZone)
    {
        GameObject segment = new GameObject($"Concourse_{baseZ}");
        segment.transform.position = new Vector3(0, 0, baseZ);

        // Floor
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.name = "TerminalFloor";
        floor.transform.parent = segment.transform;
        floor.transform.localPosition = new Vector3(0, -0.25f, segmentLength / 2f);
        floor.transform.localScale = new Vector3(10f, 0.5f, segmentLength);

        // Add side walls (Airport glass windows)
        GameObject leftWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        leftWall.name = "LeftTerminalWall";
        leftWall.transform.parent = segment.transform;
        leftWall.transform.localPosition = new Vector3(-5f, 2.5f, segmentLength / 2f);
        leftWall.transform.localScale = new Vector3(0.5f, 5f, segmentLength);

        GameObject rightWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rightWall.name = "RightTerminalWall";
        rightWall.transform.parent = segment.transform;
        rightWall.transform.localPosition = new Vector3(5f, 2.5f, segmentLength / 2f);
        rightWall.transform.localScale = new Vector3(0.5f, 5f, segmentLength);

        if (!isSafeZone)
        {
            PopulateSegmentWithAirportHazards(segment, baseZ);
        }

        return segment;
    }

    private void CleanUpOldSegments()
    {
        if (playerTransform == null) return;

        // Remove segments that are behind player
        for (int i = activeSegments.Count - 1; i >= 0; i--)
        {
            if (activeSegments[i] != null && activeSegments[i].transform.position.z + segmentLength < playerTransform.position.z - 30f)
            {
                Destroy(activeSegments[i]);
                activeSegments.RemoveAt(i);
            }
        }
    }
}
