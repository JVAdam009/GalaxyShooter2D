using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject enemyObj;
    
    [SerializeField] private GameObject tripleShotPowerupObj;
    
    [SerializeField] private int spawnRate  = 5;
    
    
    [SerializeField] private GameObject enemyContainer;

    private bool _stopSpawning = false;
    // Start is called before the first frame update
    void Start()
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
        while (!_stopSpawning)
        {
            Instantiate(enemyObj, new Vector3(Random.Range(-10, 10), 7.4f, 0f), Quaternion.identity,enemyContainer.transform);
            yield return new WaitForSeconds(spawnRate);
        }
    }
    
    IEnumerator SpawnPowerUpsRoutine()
    {
        while (!_stopSpawning)
        {
            yield return new WaitForSeconds(Random.Range(3f,7f));
            Instantiate(tripleShotPowerupObj, new Vector3(Random.Range(-10, 10), 7.4f, 0f), Quaternion.identity,enemyContainer.transform);
          
        }
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }
}
