using UnityEngine;
using UnityEngine.AI;

public class SpawnerController : MonoBehaviour
{
    [Header("Monster")]
    [SerializeField] private GameObject monsterPrefab;

    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Spawn Timing")]
    [SerializeField] private float spawnTime = 2f;
    [SerializeField] private float minimalSpawnTime = 1f;
    [SerializeField] private float spawnTimeDecrease = 0.1f;

    [Header("Spawn Position")]
    [SerializeField] private float navMeshSearchRadius = 3f;

    private float currentTime;

    private void Update()
    {
        currentTime += Time.deltaTime;

        if (currentTime >= spawnTime)
        {
            currentTime = 0f;
            SpawnMonster();
            DecreaseSpawnTime();
        }
    }

    private void SpawnMonster()
    {
        if (monsterPrefab == null)
        {
            Debug.LogWarning("SpawnerScript: Monster prefab is not assigned.");
            return;
        }

        Vector3 spawnPosition = transform.position;

        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, navMeshSearchRadius, NavMesh.AllAreas))
        {
            spawnPosition = hit.position;
        }

        GameObject monsterInstance = Instantiate(monsterPrefab, spawnPosition, transform.rotation);

        MobController monsterScript = monsterInstance.GetComponent<MobController>();

        if (monsterScript != null)
        {
            monsterScript.Target = target;
        }
        else
        {
            Debug.LogWarning("SpawnerScript: Spawned monster does not have MonsterScript.");
        }
    }

    private void DecreaseSpawnTime()
    {
        if (spawnTime > minimalSpawnTime)
        {
            spawnTime -= spawnTimeDecrease;

            if (spawnTime < minimalSpawnTime)
            {
                spawnTime = minimalSpawnTime;
            }
        }
    }
}