using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject enemyObj;
    
    [SerializeField] private GameObject[]  PowerupObjs; 
    
    [SerializeField] private int spawnRate  = 5;
    
    
    [SerializeField] private GameObject enemyContainer;
    
    [Space(10)]
    [Header("Enemy Waves")]
    
    [SerializeField] private int _waveNumber = 1;

    [SerializeField] private int _numEnemiesToStartWith = 5;

    [SerializeField] private int _additionalEnemiesToAdd = 2;

    private bool _startNextWave;

    private bool _stopSpawning = false;

    private int _numEnemiesDestroyed = 0;

    private int _numEnemiesSpawned = 0;

    private bool _spawnRoutineRunning = false;
    
    private uimanager _uiManager;
    // Start is called before the first frame update
    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemiesRoutine());
        StartCoroutine(SpawnPowerUpsRoutine());
    }
    private void Start()
    {
        _uiManager    = GameObject.Find("UI_Manager")?.GetComponent<uimanager>();
    }

    private void Update()
    {
        if (_startNextWave)
        {
            StartCoroutine(StartNextWave());
            _startNextWave = false;
        }

       
    }

    IEnumerator StartNextWave()
    {
        _numEnemiesSpawned = 0;
        _numEnemiesDestroyed = 0;
        _stopSpawning = false;

        yield return  null;
 
        StartSpawning();

         
    }
    
    public void EnemyDied()
    {
        _numEnemiesDestroyed++;
        if (_numEnemiesDestroyed == (_numEnemiesToStartWith+(_waveNumber*_additionalEnemiesToAdd)))
        {
            _startNextWave = true;
            _waveNumber++;
            _stopSpawning = true;
            StopAllCoroutines();
            _uiManager.StartWaveText();
        }
    }

    IEnumerator SpawnEnemiesRoutine()
    {
        
        yield return new WaitForSeconds(3.2f);
        _spawnRoutineRunning = true;
        while (!_stopSpawning && (_numEnemiesSpawned) < (_numEnemiesToStartWith+(_waveNumber*_additionalEnemiesToAdd)))
        {
            Instantiate(enemyObj, new Vector3(Random.Range(-7, 7), 7.4f, 0f), Quaternion.identity,enemyContainer.transform);
            _numEnemiesSpawned++;
            yield return new WaitForSeconds(spawnRate);
        }
        _spawnRoutineRunning = false;

    }
    
    IEnumerator SpawnPowerUpsRoutine()
    {
        
        yield return new WaitForSeconds(3.2f);
        while (!_stopSpawning)
        {
            yield return new WaitForSeconds(Random.Range(3f,7f));
            int randomPowerUp = Random.Range(0, 11);
            Instantiate(PowerupObjs[randomPowerUp], new Vector3(Random.Range(-8, 8), 7.4f, 0f), Quaternion.identity);
          
        }
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }
}
