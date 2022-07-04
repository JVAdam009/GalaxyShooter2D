using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class PowerUp : MonoBehaviour
{
    [SerializeField] private float speed = 3f;

    //Powerup IDs : 0 = triple shot 1 = speed 2 = shields
    [SerializeField] private int ID = 0;
    
    
    
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
        transform.Translate(Vector3.down * (speed * Time.deltaTime));
        if (transform.position.y < -6.4f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag.Equals("Player"))
        {
            Player player = col.gameObject.GetComponent<Player>();

            switch (ID)
            {
                case 0:
                {
                    player?.ActivateTripleShot();
                    Destroy(gameObject);
                }
                    break;
                case 1:
                {
                    player?.ActivateSpeedBoost();
                    Destroy(gameObject);
                }
                    break;
                case 2:
                {
                    player?.ActivateShields();
                    Destroy(gameObject);
                }
                    break;
            }
 
        }
    }
}
