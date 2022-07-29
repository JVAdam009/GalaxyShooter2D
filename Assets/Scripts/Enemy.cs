using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _speed = 4f;

    [SerializeField] private AudioClip _laserAudioClip;

    [SerializeField] private AudioClip _explosionAudioClip;

    [SerializeField] private AudioSource _sfxSource;

    [SerializeField] private GameObject _LaserPrefab;

    [SerializeField] private GameObject _explosionPrefab;
    
    private Player player;

    private Coroutine _laserCoroutine;

    private Animator _animator;

    private GameManager _gameManager;

    private bool _useNewMovement = false;

    [SerializeField]  private int _enemyID = 1;

    [SerializeField] private GameObject _shieldPrefab;

    private bool _hasShield = false;
    
    private SpawnManager _spawnManager;

    [SerializeField] private float _reverseFiringRange = 5f;

    private bool _hasFiredInReverse = false;

    private bool _hasFiredAtPowerUp = false;

    private bool _avoidLaserFire = false;
 
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player")?.GetComponent<Player>();
        _gameManager = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
        _animator = gameObject.GetComponent<Animator>();
        _laserCoroutine = StartCoroutine(FireLaser());

        _spawnManager    = GameObject.Find("SpawnManager")?.GetComponent<SpawnManager>();
        if (Random.Range(0, 100) > 50 && _enemyID == 1)
        {
            _useNewMovement = true;
        }
        
        if (Random.Range(0, 100) > 50)
        {
            _hasShield = true;

            GameObject shield = Instantiate(_shieldPrefab, transform.position, Quaternion.identity, gameObject.transform);
            shield.GetComponent<SpriteRenderer>().color = Color.red;
            shield.transform.localScale = Vector3.one;

        }

        if (_enemyID == 1)
        {
            InvokeRepeating(nameof(ScanForPowerUp),0.1f, 0.1f);
        
            InvokeRepeating(nameof(ScanForLaser),0.1f, 0.1f);
            
            InvokeRepeating(nameof(ScanForPlayerReverse),0.1f, 0.1f);

        }
        
        
    }

    public void SetID(int id)
    {
        _enemyID = id;
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }

 

    void ScanForPlayerReverse()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(0,2f), new Vector2(0, _reverseFiringRange));

        if (hit.collider != null && !_hasFiredAtPowerUp)
        {
            var fire = Random.Range(0, 100);
            if(hit.collider.tag.Equals("Player") && fire >= 25)
            {
                FireLaserInReverse();
                _hasFiredAtPowerUp = true;
            }
        }
    }

    void ScanForPowerUp()
    {
        RaycastHit2D hit = Physics2D.CircleCast(transform.position + (Vector3.down * 2), 1.25f, Vector2.down);

        if (hit.collider != null && !_hasFiredInReverse)
        {
            if(hit.collider.tag.Equals("PowerUp") )
            {
                FireAtPowerup();

                StartCoroutine(ResetPowerupFireRate());
            }
        }
    }
    
    void ScanForLaser()
    {
        RaycastHit2D hit = Physics2D.CircleCast(transform.position + (Vector3.down * 2), 1.25f, Vector2.down);

        if (hit.collider != null)
        {
            float avoidLaser = Random.Range(0, 100);
            if(hit.collider.tag.Equals("Laser")  && avoidLaser > 55)
            {
                _avoidLaserFire = true;
                StartCoroutine(AvoidLasers());
            }
        }
    }

    IEnumerator AvoidLasers()
    {
        float origX = transform.position.x;
        while (_avoidLaserFire)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(origX+4,transform.position.y),(_speed*2 )* Time.deltaTime);

            if (transform.position.x >= origX + 4)
            {
                _avoidLaserFire = false;
            }
            yield return null;
        }

        yield return null;
    }

    void FireLaserInReverse()
    {
        GameObject laserGO = Instantiate(_LaserPrefab, transform.position + (Vector3.up * 1.75f), Quaternion.identity);
        Laser laser = laserGO.GetComponent<Laser>();
        laser.Speed = _speed * 2.5f;
        laser.SetDamagePlayer();
        laser.tag = "EnemyLaser";
        _hasFiredInReverse = true;
        StartCoroutine(ResetReverseFireRate());
    }

    void FireAtPowerup()
    {
        GameObject laserGO = Instantiate(_LaserPrefab, transform.position + Vector3.down, Quaternion.identity);
        Laser laser = laserGO.GetComponent<Laser>();
        laser.Speed = -_speed * 1.5f;
        laser.SetDamagePlayer();
        laser.tag = "EnemyLaser";
    }

    IEnumerator ResetReverseFireRate()
    {
        yield return new WaitForSeconds(1.2f);
        _hasFiredInReverse = false;
    }
    
    IEnumerator ResetPowerupFireRate()
    {
        yield return new WaitForSeconds(1.2f);
        _hasFiredAtPowerUp = false;
    }

    void Movement()
    {
        if (transform.position.y <= -5.45)
        {
            transform.position = new Vector3(Random.Range(-7, 7), 7.4f, 0f);
        }

        if (transform.position.x > 11)
        {
            transform.position = new Vector3(-11, 5.36f, 0f);
        }

        if (_useNewMovement)
        {
            float xsin = Mathf.Sin(Time.time );

            transform.Translate(new Vector3(xsin, -1, 0) * (_speed * Time.deltaTime));
            Vector3 clampedPosition = transform.position;
            clampedPosition.x =Mathf.Clamp(clampedPosition.x, -9f, 9f);
            transform.position = clampedPosition;
        }
        else 
        {
            if (_enemyID == 1)
            {
                transform.Translate(Vector3.down * (_speed * Time.deltaTime));
            }
            else if (_enemyID == 2)
            {
                transform.Translate(Vector2.right * (_speed * Time.deltaTime));
            }
        }

    }

    IEnumerator FireLaser()
    {
        while (true)
        {
            if (_enemyID == 1)
            {
                
                int fireTime = Random.Range(3, 7);
                yield return new WaitForSeconds(fireTime);
                GameObject laserGO = Instantiate(_LaserPrefab, transform.position + Vector3.down, Quaternion.identity);
                Laser laser = laserGO.GetComponent<Laser>();
                laser.Speed = -_speed * 1.5f;
                laser.SetDamagePlayer();
                laser.tag = "EnemyLaser";
            }
            else if (_enemyID == 2)
            {
                int fireTime = Random.Range(3, 7);
                yield return new WaitForSeconds(fireTime);
                Instantiate(_LaserPrefab, transform.position + Vector3.down, Quaternion.identity);
            }
            AudioSource.PlayClipAtPoint(_laserAudioClip,transform.position);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
       
        if (col.gameObject.tag.Equals("Player"))
        {
            
            player?.DamagePlayer();
            GetComponent<Collider2D>().enabled = false;
            _animator?.SetTrigger("onEnemyDeath");
            _sfxSource.clip = _explosionAudioClip;
            _sfxSource?.Play();
            _hasShield = false;
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
            _speed = 0; 
            _gameManager.StartCameraShake();
            _spawnManager.EnemyDied();
            StopCoroutine(_laserCoroutine);
            Destroy(GetComponent<Collider2D>());

        }
        else if (col.gameObject.tag.Equals("Laser"))
        {
            int points = Random.Range(7, 12);
            player?.AddScore(points);
            
            GetComponent<Collider2D>().enabled = false;
           

            if (_enemyID == 2)
            {
                GameObject explosion = Instantiate(_explosionPrefab,transform.position + Vector3.back*2f,Quaternion.identity);
                Destroy(explosion,3f);
            }

            if (!_hasShield)
            {
                _animator?.SetTrigger("onEnemyDeath");
                _speed = 0; 
                _sfxSource.clip = _explosionAudioClip;
                _sfxSource?.Play();
                _gameManager.StartCameraShake();
                _spawnManager.EnemyDied();
                StopCoroutine(_laserCoroutine);
                Destroy(GetComponent<Collider2D>());
                Destroy(gameObject,1f);
                Destroy(col.gameObject);
            }
            else
            {
                _hasShield = false;
                foreach (Transform child in transform)
                {
                    Destroy(child.gameObject);
                }
                
                StartCoroutine(ReEnableCollider(GetComponent<Collider2D>()));
                Destroy(col.gameObject);
            }
        }
    }

    IEnumerator ReEnableCollider(Collider2D col)
    {
        yield return new WaitForSeconds(1.1f);
        col.enabled = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(transform.position,new Vector3(0,_reverseFiringRange));
        
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position + (Vector3.down * 3),2.5f);
    }

 
}
