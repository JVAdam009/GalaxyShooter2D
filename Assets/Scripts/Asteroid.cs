using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed = 4f;


    [SerializeField] private GameObject _explosionObj;
    
    

    private SpawnManager _spawnManager;

    private GameManager _gameManager;
    
    private uimanager _uiManager;

    private void Start()
    {
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        _gameManager = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
        _uiManager = GameObject.FindWithTag("UIManager").GetComponent<uimanager>();
    }

    // Update is called once per frame
    void Update()
    {
        Rotate();
    }

    void Rotate()
    {
        transform.Rotate(Vector3.forward * (_rotationSpeed * Time.deltaTime));
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag.Equals("Laser"))
        {
            GetComponent<CircleCollider2D>().enabled = false;
            GameObject explosion = Instantiate(_explosionObj,transform.position,Quaternion.identity);
            _gameManager.StartCameraShake();
            _uiManager.StartWaveText();
            Destroy(explosion,3f);
            Destroy(col.gameObject);
            _spawnManager.StartSpawning();
            Destroy(gameObject,.4f);
        }
    }
 
}
