using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEditor;

public class NPCSpawner : MonoBehaviour
{
    public static NPCSpawner instance { get; private set; }
    private string mobsFolderPath = "Assets/Prefabs/NPC";
    public Transform npcContainer;
    [SerializeField] private int numberOfMobsToSpawn = 10;
    public Tilemap tilemap;
    private List<GameObject> mobPrefabs = new List<GameObject>();

    public GameObject map;
    private CaveGenerator caveGenerator;
    private void Awake()
    {
        instance = this;
        caveGenerator = map.GetComponent<CaveGenerator>();

    }
    private void Start()
    {
        LoadMobPrefabs();
    }

    private void LoadMobPrefabs()
    {
        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { mobsFolderPath });
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab != null)
            {
                mobPrefabs.Add(prefab);
            }
        }

        if (mobPrefabs.Count == 0)
        {
            Debug.LogError("No mob prefabs found in the specified folder.");
        }
    }

    public void SpawnMobs()
    {
        if (mobPrefabs.Count == 0)
        {
            Debug.LogWarning("No mob prefabs to spawn.");
            return;
        }

        for (int i = 0; i < numberOfMobsToSpawn; i++)
        {
            SpawnRandomMob();
        }
    }

    private void SpawnRandomMob()
    {
        if (mobPrefabs.Count == 0)
        {
            Debug.LogWarning("No mob prefabs to spawn.");
            return;
        }

        Vector2Int randomTilePos = caveGenerator.GetRandomWalkableTile();

        int randomIndex = Random.Range(0, mobPrefabs.Count);
        GameObject mobPrefab = mobPrefabs[randomIndex];

        Vector3 spawnPosition = tilemap.CellToWorld((Vector3Int)randomTilePos);
        GameObject mobInstance = Instantiate(mobPrefab, spawnPosition, Quaternion.identity);
        mobInstance.transform.parent = npcContainer;
    }
    public void DespawnAllMobs()
    {
        foreach (Transform child in npcContainer)
        {
            Destroy(child.gameObject);
        }
    }
}
