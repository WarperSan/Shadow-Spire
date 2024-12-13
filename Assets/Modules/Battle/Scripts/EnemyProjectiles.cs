using BattleEntity;
using UnityEngine;
using System.Collections;

public class EnemyProjectiles : MonoBehaviour
{
    [SerializeField] private GameObject projectile;



    #region Data

    private int enemyCount;
    private BattleEntityType[] enemyTypes;

    #endregion

    #region Manager

    public void SetupProjectiles(BattleEnemyEntity[] battleEnemyEntities)
    {
        if (battleEnemyEntities == null || battleEnemyEntities.Length == 0)
            return;

        // Compile types
        var combinedType = BattleEntityType.NONE;

        foreach (var enemy in battleEnemyEntities)
            combinedType |= enemy.Type;

        enemyTypes = BattleEntity.BattleEntity.GetTypes(combinedType);

        // Calculate how many projectiles to spawn per seconds

        enemyCount = battleEnemyEntities.Length;
    }

    public IEnumerator SpawnProjectiles(float duration)
    {
        while (duration > 0)
        {
            for (int i = 0; i < enemyCount; i++)
            {
                var rdmType = enemyTypes[Random.Range(0, enemyTypes.Length)];
                SpawnSingleProjectile(rdmType);

                duration -= 0.1f;
                yield return new WaitForSeconds(0.1f);
            }

            duration -= spawnInterval;
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    public void CleanProjectiles()
    {
        // Destroy all projectiles
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        // Remove all the compiled data
    }

    #endregion

    #region Projectiles

    private float spawnInterval = 0.05f;
    private float minX = -2f;
    private float maxX = 2f;
    private float spawnY = 0.818f;
    private float projectileSpeed = 3.0f;
    private float minSpawnDistance = 0.5f;
    private float lastSpawnX = 0.818f;

    private void SpawnSingleProjectile(BattleEntityType type)
    {
        float randomX;
        do
        {
            randomX = Random.Range(minX, maxX);
        } while (Mathf.Abs(randomX - lastSpawnX) < minSpawnDistance);

        lastSpawnX = randomX;

        Vector3 spawnPosition = new(randomX, spawnY, 0);

        GameObject newProjectile = Instantiate(projectile, spawnPosition, Quaternion.identity);
        newProjectile.transform.SetParent(transform);

        if (newProjectile.TryGetComponent(out SpriteRenderer sprite))
        {
            sprite.color = BattleEntity.BattleEntity.GetTypeColor(type);
        }
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


    #endregion
}