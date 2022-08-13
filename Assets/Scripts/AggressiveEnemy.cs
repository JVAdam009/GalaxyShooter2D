using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AggressiveEnemy : MonoBehaviour
{
    [SerializeField] private float _speed = 4f;

    [SerializeField] private float _aggressionRange = 4f;

    [SerializeField] private AudioClip _explosionAudioClip;

    [SerializeField] private AudioSource _sfxSource;

    [SerializeField] private GameObject _explosionPrefab;
    
    private Player player;
    
    private GameManager _gameManager;

    private SpawnManager _spawnManager;

    private bool _attackPlayer = false;

    private Vector3 _playerLastPosition;

     
    
    

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player")?.GetComponent<Player>();
        _gameManager = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
 

        _spawnManager    = GameObject.Find("SpawnManager")?.GetComponent<SpawnManager>();
        
        InvokeRepeating(nameof(ScanForPlayer),0.1f,0.1f);
    }

    void ScanForPlayer()
    {
        
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer < _aggressionRange)
        {
            _attackPlayer = true;
            _playerLastPosition = player.transform.position;
        }
        else
        {
            _attackPlayer = false;
        }
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
            transform.position = new Vector3(Random.Range(-7, 7), 7.4f, 0f);
            _attackPlayer = false;
        }

        if (player == null)
        {
            return;
        }


        if (_attackPlayer)
        {
            transform.position =
                Vector3.MoveTowards(transform.position, player.transform.position, _speed * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector3.down * (_speed * Time.deltaTime));
        }
        float distanceToPlayer = Vector3.Distance(transform.position, _playerLastPosition);

        if (distanceToPlayer < 0.1f)
        {
            _attackPlayer = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(transform.position,_aggressionRange);
    }
    
    private void OnTriggerEnter2D(Collider2D col)
    {
       
        if (col.gameObject.tag.Equals("Player"))
        {
            GameObject explosion = Instantiate(_explosionPrefab,transform.position + Vector3.back*2f,Quaternion.identity);
            Destroy(explosion,3f);

            player?.DamagePlayer();
            GetComponent<Collider2D>().enabled = false;
             _sfxSource.clip = _explosionAudioClip;
            _sfxSource?.Play();
            _speed = 0; 
            _gameManager.StartCameraShake();
            _spawnManager.EnemyDied();
           
            Destroy(gameObject,1f);
            Destroy(GetComponent<Collider2D>());

        }
        else if (col.gameObject.tag.Equals("Laser"))
        {
            int points = Random.Range(7, 12);
            player?.AddScore(points);
            GameObject explosion = Instantiate(_explosionPrefab,transform.position + Vector3.back*2f,Quaternion.identity);
            Destroy(explosion,3f);

            GetComponent<Collider2D>().enabled = false;
           
 
                  _speed = 0; 
                _sfxSource.clip = _explosionAudioClip;
                _sfxSource?.Play();
                _gameManager.StartCameraShake();
                _spawnManager.EnemyDied(); 
                Destroy(GetComponent<Collider2D>());
                Destroy(gameObject,1f);
                Destroy(col.gameObject);
         
            
        }
    }

}
