using BattleEntity;
using UnityEngine;
using System.Collections;

public class EnemyProjectiles : MonoBehaviour
{
    [SerializeField] private GameObject projectile;
    private float spawnInterval = 0.05f;
    private float minX = -2f;
    private float maxX = 2f;
    private float spawnY = 0.818f;
    private float projectileSpeed = 3.0f; 
    private float minSpawnDistance = 0.5f;
    private float lastSpawnX = 0.818f;

    public void SpawnProjectiles(BattleEnemyEntity[] battleEnemyEntities)
    {
        if (battleEnemyEntities == null || battleEnemyEntities.Length == 0)
            return;

        int enemies = battleEnemyEntities.Length;
        StartCoroutine(SpawnProjectilesRoutine(enemies));
    }

    private IEnumerator SpawnProjectilesRoutine(int enemies)
    {
        while (true)
        {
            for (int i = 0; i < enemies; i++)
            {
                SpawnSingleProjectile();
                yield return new WaitForSeconds(0.1f);

            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnSingleProjectile()
    {
        float randomX;
        do
        {
            randomX = Random.Range(minX, maxX);
        } while (Mathf.Abs(randomX - lastSpawnX) < minSpawnDistance);

        lastSpawnX = randomX;
        
        Vector3 spawnPosition = new Vector3(randomX, spawnY, 0);

        GameObject newProjectile = Instantiate(projectile, spawnPosition, Quaternion.identity);
        newProjectile.transform.SetParent(this.transform);
    }

    private void Update()
    {
        MoveProjectiles();
    }

    private void MoveProjectiles()
    {
        foreach (Transform child in transform)
        {
            child.Translate(Vector3.down * projectileSpeed * Time.deltaTime);
        }
    }
}