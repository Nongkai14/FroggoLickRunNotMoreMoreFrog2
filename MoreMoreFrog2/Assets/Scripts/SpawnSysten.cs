using UnityEngine;
using System.Collections.Generic;

public class SpawnSystem : MonoBehaviour
{
    public GameObject[] spawnPoints;
    public GameObject enemyPrefab;

    void Start()
    {
        SpawnObject();
    }

    void SpawnObject()
    {
        List<GameObject> tempSpawnPoints = new List<GameObject>(spawnPoints);
        Shuffle(tempSpawnPoints);

        int spawnCount = Mathf.Min(3, tempSpawnPoints.Count);

        for (int i = 0; i < spawnCount; i++)
        {
            RaycastHit hit;
            Vector3 origin = tempSpawnPoints[i].transform.position;

            if (Physics.Raycast(origin, Vector3.down, out hit))
            {
                Vector3 location = hit.point + Vector3.up * 0.05f;
                Instantiate(enemyPrefab, location, Quaternion.identity);
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
