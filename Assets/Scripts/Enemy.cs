using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _speed = 4f;

    private Player player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
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

    private void OnTriggerEnter2D(Collider2D col)
    {
       
        if (col.gameObject.tag.Equals("Player"))
        {
            Destroy(gameObject);
            player?.DamagePlayer();

        }
        else if (col.gameObject.tag.Equals("Laser"))
        {
            int points = Random.Range(7, 12);
            player?.AddScore(points);
            Destroy(gameObject);
            Destroy(col.gameObject);
        }
    }
}
