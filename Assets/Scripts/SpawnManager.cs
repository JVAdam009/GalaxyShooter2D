using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject enemyObj;
    
    [SerializeField] private GameObject[]  PowerupObjs; 
    
    [SerializeField] private int spawnRate  = 5;
    
    
    [SerializeField] private GameObject enemyContainer;

    private bool _stopSpawning = false;
    // Start is called before the first frame update
    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemiesRoutine());
        StartCoroutine(SpawnPowerUpsRoutine());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator SpawnEnemiesRoutine()
    {
        yield return new WaitForSeconds(1.2f);
        while (!_stopSpawning)
        {
            Instantiate(enemyObj, new Vector3(Random.Range(-8, 8), 7.4f, 0f), Quaternion.identity,enemyContainer.transform);
            yield return new WaitForSeconds(spawnRate);
        }
    }
    
    IEnumerator SpawnPowerUpsRoutine()
    {
        
        yield return new WaitForSeconds(1.2f);
        while (!_stopSpawning)
        {
            yield return new WaitForSeconds(Random.Range(3f,7f));
            int randomPowerUp = Random.Range(0, 4);
            Instantiate(PowerupObjs[randomPowerUp], new Vector3(Random.Range(-8, 8), 7.4f, 0f), Quaternion.identity);
          
        }
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }
}
