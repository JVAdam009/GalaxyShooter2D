using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _speed = 4f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }

    void Movement()
    {
        if (transform.position.y <= -5.45)
        {
            transform.position = new Vector3(Random.Range(-10, 10), 7.4f, 0f);
        }
        
        transform.Translate(Vector3.down * (_speed * Time.deltaTime));
    }

    private void OnTriggerEnter(Collider other)
    {
       
        if (other.gameObject.tag.Equals("Player"))
        {
            Player player = other.gameObject.GetComponent<Player>();
           
            Destroy(gameObject);
            player?.DamagePlayer();

        }
        else if (other.gameObject.tag.Equals("Laser"))
        {
            Destroy(gameObject);
            Destroy(other.gameObject);
        }
    }
}
