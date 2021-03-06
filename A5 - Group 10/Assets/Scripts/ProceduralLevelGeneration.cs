using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class ProceduralLevelGeneration : MonoBehaviour
{
    
    [SerializeField] private Transform startRoom;
    [SerializeField] private Transform endRoom;
    [SerializeField] private GameObject levelContainer;
    [SerializeField] private List<Transform> levelPrefabs;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private NavMeshBaker navMeshBaker;
    [SerializeField] public List<GameObject> Spawnable;

    [SerializeField] private int roomCount;

    private Vector3 lastRoomExitPoint;
    private GameObject player;
    [HideInInspector] public int levelsGenerated;

    private void Awake()
    {
        SpawnNewLevel();
        levelsGenerated++;
    }

    private void Start()
    {
        player = SpawnPlayer();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            StartCoroutine(GenerateNewLevel());
        }
    }

    private void SpawnRoom()
    {
        var chosenRoom = levelPrefabs[Random.Range(0, levelPrefabs.Count)];
        var roomStartPos = chosenRoom.Find("StartPoint").position;
        var spawnPosition = lastRoomExitPoint + chosenRoom.position - roomStartPos;
        var lastRoomTransform = SpawnRoom(chosenRoom, spawnPosition, Quaternion.identity);
        lastRoomExitPoint = lastRoomTransform.Find("EndPoint").position;
        foreach (Transform child in lastRoomTransform)
        {
            if (child.gameObject.CompareTag("SpawnPoint"))
            {
                var item = Spawnable[Random.Range(0, Spawnable.Count)];
                Instantiate(item, child.position, Quaternion.identity);
            }
        }
    }

    private Transform SpawnRoom(Transform roomPrefab, Vector3 spawnPosition, Quaternion rotation)
    {
        var roomTransform = Instantiate(roomPrefab, spawnPosition, rotation);
        roomTransform.SetParent(levelContainer.transform);
        return roomTransform;
    }
    
    private void SpawnNewLevel()
    {
        lastRoomExitPoint = startRoom.Find("EndPoint").position;

        for (var i = 0; i < roomCount; i++)
        {
            SpawnRoom();
        }

        var endRoomStartPos = endRoom.Find("StartPoint").position;
        var  endRoomSpawnPos = lastRoomExitPoint + endRoom.position - endRoomStartPos;
        SpawnRoom(endRoom, endRoomSpawnPos, Quaternion.Euler(0, 180, 0));
    }

    private GameObject SpawnPlayer()
    {
        var playerSpawnPosition = startRoom.Find("PlayerSpawnPoint").position;
         return Instantiate(playerPrefab, playerSpawnPosition, Quaternion.identity);
    }

    //The old level will be destroyed, and a new startRoom and player generated
    private IEnumerator GenerateNewLevel()
    {
        foreach (Transform child in levelContainer.transform)
        {
            Destroy(child.gameObject);
        }

        var pickups = GameObject.FindGameObjectsWithTag("Pickup");
        foreach (var pickup in pickups)
        {
            Destroy(pickup);
        }

        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var enemy in enemies)
        {
            Destroy(enemy);
        }
            
        player.gameObject.SetActive(false);

        var newStartRoom = Instantiate(startRoom, new Vector3(0, 0, 0), Quaternion.identity);
        newStartRoom.SetParent(levelContainer.transform);
        startRoom = newStartRoom;
        SpawnNewLevel();
        NavMesh.RemoveAllNavMeshData();
        yield return new WaitForSeconds(0.5f);
        navMeshBaker.BakeNavMesh();
        var oldPlayer = player;
        player = SpawnPlayer();
        Destroy(oldPlayer);
        levelsGenerated++;
    }
    
    
}
