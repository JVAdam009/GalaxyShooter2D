using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed = 4f;


    [SerializeField] private GameObject _explosionObj;

    private SpawnManager _spawnManager;

    private void Start()
    {
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
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
            GameObject explosion = Instantiate(_explosionObj,transform.position,Quaternion.identity);
            Destroy(explosion,3f);
            Destroy(col.gameObject);
            _spawnManager.StartSpawning();
            Destroy(gameObject,.4f);
        }
    }
 
}
