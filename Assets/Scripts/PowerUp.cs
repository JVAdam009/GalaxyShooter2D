using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class PowerUp : MonoBehaviour
{
    [SerializeField] private float speed = 3f;

    //Powerup IDs : 0 = triple shot 1 = speed 2 = shields 3 = Ammo 4 = Wrench 5 = Super Shot 6 = Power Down 7 = Homing Missiles
    [SerializeField] private int ID = 0;

    [SerializeField] private AudioClip _powerUpSFX;
    
    
    

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
            AudioSource.PlayClipAtPoint(_powerUpSFX,transform.position);

            switch (ID)
            {
                case 0:
                {
                    player?.ActivateTripleShot();
                    player?.RefillAmmo();
                }
                    break;
                case 1:
                {
                    player?.ActivateSpeedBoost();
                }
                    break;
                case 2:
                {
                    player?.ActivateShields();
                }
                    break;
                case 3:
                {
                    player?.RefillAmmo();
                }
                    break;
                case 4:
                {
                    player?.Heal();
                }
                    break; 
                case 5:
                {
                    player?.ActivateWideShot();
                }
                    break; 
                case 6:
                {
                    player?.ActivateNegativePowerUp();
                }
                    break;
                case 7:
                {
                    player?.ActivateMissilePowerUp();
                }
                    break;
            }
            
            Destroy(gameObject);

 
        }
        else if (col.tag.Equals("EnemyLaser"))
        {
            Destroy(col.gameObject);
            Destroy(gameObject);
        }
    }
}
