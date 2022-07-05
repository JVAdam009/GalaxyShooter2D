using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    [SerializeField] private float _speed = 3.5f;

    [SerializeField] private GameObject LaserObjPrefab;

    [SerializeField] private GameObject TripleLaserObjPrefab;
    
    [SerializeField] private GameObject ShieldVisualizer;

    [SerializeField] private GameObject LeftEngineOBj;
    
    [SerializeField] private GameObject RightEngineOBj;
    
    [SerializeField] private GameObject ExplosionObj;
    
    [SerializeField] private float _laserVerticalOffset = 0.7f;

    [SerializeField] private float _fireCoolDown = -1f;

    [SerializeField] private float _fireRate = 0.5f;
    
    [SerializeField] private int Lives = 3;
    
    [SerializeField] private int Score = 0;

    [SerializeField] private bool _isTripleShotActive = false;
    
    [SerializeField] private bool _isShieldActive = false;

    [SerializeField] private AudioClip _laserAudioClip;

    [SerializeField] private AudioSource _sfxSource;

    private SpawnManager _spawnManager;
    
    private uimanager _uiManager;
    
    private GameManager _gameManager;

    private bool _leftEngineOn = false;

    private bool _rightEngineOn = false;
    
    // Start is called before the first frame update
    void Start()
    {
        _spawnManager = GameObject.Find("SpawnManager")?.GetComponent<SpawnManager>();
        _uiManager    = GameObject.Find("UI_Manager")?.GetComponent<uimanager>();
        _gameManager    = GameObject.Find("GameManager")?.GetComponent<GameManager>();
    }

    public void AddScore(int points)
    {
        Score += points;
        _uiManager?.SetScore(Score);
    }
    public void DamagePlayer()
    {
        if (_isShieldActive)
        {
            _isShieldActive = false;
            ShieldVisualizer.SetActive(false);
            return;
        }
        Lives -= 1;
        ChooseEngine();
        _uiManager.SetLive(Lives);
        if (Lives == 0)
        {
            _spawnManager?.OnPlayerDeath();
            _uiManager?.ShowGameOverText();
            _gameManager?.SetGameOver();
            GameObject explosion = Instantiate(ExplosionObj, transform.position, Quaternion.identity);
            Destroy(explosion, 2.4f);
            
            Destroy(gameObject,.4f);
        }
    }

    void ChooseEngine()
    {
        if (!_leftEngineOn && !_rightEngineOn)
        {
            int engineChoice = Random.Range(0, 2);
            if (engineChoice > 0)
            {
                LeftEngineOBj.SetActive(true);
                _leftEngineOn = true;
            }
            else
            {
                RightEngineOBj.SetActive(true);
                _rightEngineOn = true;
            }
        }
        else if (!_leftEngineOn && _rightEngineOn)
        {
            LeftEngineOBj.SetActive(true);
            _leftEngineOn = true;
        }
        else if (_leftEngineOn && !_rightEngineOn)
        {
            RightEngineOBj.SetActive(true);
            _rightEngineOn = true;
        }
    }
    
    public void ActivateShields()
    {
        _isShieldActive = true;
        ShieldVisualizer.SetActive(true);
    }

    public void ActivateSpeedBoost()
    {
        _speed = 6.5f;
        StartCoroutine(SpeedCooldown());
    }
    
    IEnumerator SpeedCooldown()
    {
        yield return new WaitForSeconds(5f);

        _speed = 3.5f;
    }

    public void ActivateTripleShot()
    {
        _isTripleShotActive = true;
        StartCoroutine(TripleShotCooldown());
    }

    IEnumerator TripleShotCooldown()
    {
        yield return new WaitForSeconds(5f);

        _isTripleShotActive = false;
    }

    // Update is called once per frame
    void Update()
    {
    CaluclateMovement();
    
    FireLaser();
    }

    void FireLaser()
    {
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _fireCoolDown)
        {
            if (_isTripleShotActive)
            {  
                Instantiate(TripleLaserObjPrefab, transform.position, Quaternion.identity);
            }
            else
            {
                Instantiate(LaserObjPrefab, transform.position+new Vector3(0,_laserVerticalOffset,0), Quaternion.identity);
            }
            _fireCoolDown = Time.time + _fireRate;

            _sfxSource.clip = _laserAudioClip;
            
            _sfxSource.Play();
        }
    }

    void CaluclateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
      
        float verticalInput = Input.GetAxis("Vertical");
       
        transform.Translate(((Vector3.right * horizontalInput) + (Vector3.up * verticalInput)) * _speed * Time.deltaTime);
        
        
        if (transform.position.y > 0)
        {
            transform.position = new Vector3(transform.position.x, 0, 0);
        }
        else if (transform.position.y <= -3.8f)
        {
            transform.position = new Vector3(transform.position.x, -3.8f, 0);
        }
        
        
        if (transform.position.x > 13.4f)
        {
            transform.position = new Vector3(-13.4f,transform.position.y,  0);
        }
        else if (transform.position.x <= -13.4f)
        {
            transform.position = new Vector3(13.4f,transform.position.y,  0);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag.Equals("Laser"))
        {
            Laser laser = col.gameObject.GetComponent<Laser>();
            if (laser?.CanDamagePlayer() == true)
            {
                DamagePlayer();
                Destroy(laser.gameObject);
            }
        }
    }
}
