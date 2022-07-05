using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;
using Debug = System.Diagnostics.Debug;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    [SerializeField] private float _speed = 3.5f;

    [SerializeField] private GameObject _laserObjPrefab;

    [SerializeField] private GameObject _tripleLaserObjPrefab;
    
    [SerializeField] private GameObject _shieldVisualizer;

    [SerializeField] private GameObject _leftEngineOBj;
    
    [SerializeField] private GameObject _rightEngineOBj;
    
    [SerializeField] private GameObject ExplosionObj;
    
    [SerializeField] private float _laserVerticalOffset = 0.7f;

    [SerializeField] private float _fireCoolDown = -1f;

    [SerializeField] private float _fireRate = 0.5f;

    [SerializeField] private float _thrusterSpeedIncrease = 2.5f;
    
    [SerializeField] private int _lives = 3;
    
    [SerializeField] private int _shieldHitsLeft = 3;
    
    [SerializeField] private int _score = 0;

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
        _score += points;
        _uiManager?.SetScore(_score);
    }
    public void DamagePlayer()
    {
        if (_isShieldActive)
        {
            DamageShield();
            return;
        }
        _lives -= 1;
        ChooseEngine();
        _uiManager.SetLive(_lives);
        if (_lives == 0)
        {
            KillPlayer();
        }
    }

    void DamageShield()
    {
        _shieldHitsLeft -= 1;
        SpriteRenderer shieldRenderer = _shieldVisualizer.GetComponent<SpriteRenderer>();
        if (shieldRenderer == null)
        {
            throw new Exception("Shield Renderer Null");
        }
        switch(_shieldHitsLeft)
        {
            case 2:
                shieldRenderer.color = new Color(1f, .5f, .8f, 1f);
                break;
            case 1:
                shieldRenderer.color = new Color(1f, .5f, .5f, .5f);
                break;
            case 0:
                _isShieldActive = false;
                _shieldVisualizer.SetActive(false);
                break;
        }
    }

    void KillPlayer()
    {
        _spawnManager?.OnPlayerDeath();
        _uiManager?.ShowGameOverText();
        _gameManager?.SetGameOver();
        GameObject explosion = Instantiate(ExplosionObj, transform.position, Quaternion.identity);
        Destroy(explosion, 2.4f);
            
        Destroy(gameObject,.4f);
    }

    void ChooseEngine()
    {
        if (!_leftEngineOn && !_rightEngineOn)
        {
            int engineChoice = Random.Range(0, 2);
            if (engineChoice > 0)
            {
                _leftEngineOBj.SetActive(true);
                _leftEngineOn = true;
            }
            else
            {
                _rightEngineOBj.SetActive(true);
                _rightEngineOn = true;
            }
        }
        else if (!_leftEngineOn && _rightEngineOn)
        {
            _leftEngineOBj.SetActive(true);
            _leftEngineOn = true;
        }
        else if (_leftEngineOn && !_rightEngineOn)
        {
            _rightEngineOBj.SetActive(true);
            _rightEngineOn = true;
        }
    }
    
    public void ActivateShields()
    {
        _isShieldActive = true;
        _shieldVisualizer.SetActive(true);
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
                Instantiate(_tripleLaserObjPrefab, transform.position, Quaternion.identity);
            }
            else
            {
                Instantiate(_laserObjPrefab, transform.position+new Vector3(0,_laserVerticalOffset,0), Quaternion.identity);
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

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            _speed += _thrusterSpeedIncrease;
        }
        else if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            _speed -= _thrusterSpeedIncrease;
        }
        transform.Translate(((Vector3.right * horizontalInput) + (Vector3.up * verticalInput)) * (_speed * Time.deltaTime));

        
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
