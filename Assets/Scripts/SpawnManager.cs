using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject[] enemyObj;
    
    [SerializeField] private GameObject _BossObj;
    
    [SerializeField] private GameObject[]  PowerupObjs; 
    
    [SerializeField] private int spawnRate  = 5;
    
    
    [SerializeField] private GameObject enemyContainer;
    
    [Space(10)]
    [Header("Enemy Waves")]
    
    [SerializeField] private int _waveNumber = 1;

    [SerializeField] private int _numEnemiesToStartWith = 5;

    [SerializeField] private int _additionalEnemiesToAdd = 1;

    [SerializeField] private GameObject _PowerupContainer;
    
    [SerializeField] private Transform _BossSpawn;
    
    
    
    [SerializeField] private Transform _BossTargetSpot;

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

    public Transform GetBossTargetPoint()
    {
        return _BossTargetSpot;
    }
    public void BossSpawning()
    {
        Instantiate(_BossObj, _BossSpawn.position, Quaternion.identity,enemyContainer.transform);
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

        if (_waveNumber % 10 == 0)
        {
            BossSpawning();
        }
        else
        {
            StartSpawning();
        }

         
    }

    public void BossDied()
    {
        _startNextWave = true;
        _waveNumber++;
        _stopSpawning = true;
        StopAllCoroutines();
        _uiManager.StartWaveText();
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
            int randomEnemy = GenerateEnemy(Random.Range(0, 60));
            Instantiate(enemyObj[randomEnemy], new Vector3(Random.Range(-7, 7), 7.4f, 0f), Quaternion.identity,enemyContainer.transform);
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
            int randomPowerUp = GeneratePowerUp(Random.Range(0, 100));
            Instantiate(PowerupObjs[randomPowerUp], new Vector3(Random.Range(-8, 8), 7.4f, 0f), Quaternion.identity,_PowerupContainer.transform);
          
        }
    }

    private int GeneratePowerUp(int range)
    {
       if(range is >= 0 and < 10)
       {
           return 4;
       }
       else if (range is >= 10 and < 15)
       {
           return 5;
       }
       else if (range is >= 20 and < 30)
       {
           return 3;
       }
       else if (range is >= 30 and < 40)
       {
           return 2;
       }
       else if (range is >= 40 and < 50)
       {
           return 1;
       }
       else if (range is >= 50 and < 55)
       {
           return 0;
       }
       else if (range is >= 60 and < 70)
       {
           return 6;
       }
       else if (range is >= 70 and < 75)
       {
           return 7;
       }
       else
       {
           return 1;
       }
    }
    
    private int GenerateEnemy(int range)
    {
        if(range is >= 0 and  < 40)
        {
            return 0;
        }
        else if(range is >= 40 and < 50)
        {
            return 1;
        }
        else if(range is >= 50 and < 55)
        {
            return 2;
        }
        else
        {
            return 0;
        }
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }
}
