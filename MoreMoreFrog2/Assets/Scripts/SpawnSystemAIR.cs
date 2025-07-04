using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSystemAIR : MonoBehaviour
{
    public bool allowAirSpawn = true;
    public string spawnPointTag = "EnemySpawnPoint"; // กำหนดแท็กของ spawn point
    public GameObject enemyPrefab;

    public float spawnInterval = 5f; // ระยะห่างการ spawn แต่ละครั้ง
    public int enemiesPerWave = 3;   // จำนวนที่จะ spawn ต่อรอบ

    private GameObject[] spawnPoints;

    void Start()
    {
        spawnPoints = GameObject.FindGameObjectsWithTag(spawnPointTag);
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            SpawnObjectAIR(spawnPoints);
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnObjectAIR(GameObject[] spawnPoints)
    {
        List<GameObject> tempSpawnPoints = new List<GameObject>(spawnPoints);
        Shuffle(tempSpawnPoints);

        int spawnCount = Mathf.Min(enemiesPerWave, tempSpawnPoints.Count);

        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 origin = tempSpawnPoints[i].transform.position;

            if (allowAirSpawn)
            {
                Instantiate(enemyPrefab, origin, Quaternion.identity);
            }
            else
            {
                if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit))
                {
                    Vector3 location = hit.point + Vector3.up * 0.05f;
                    Instantiate(enemyPrefab, location, Quaternion.identity);
                }
            }
        }
    }

    void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}
