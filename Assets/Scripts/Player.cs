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

    [SerializeField] private GameObject _homingProjectilePrefab;

    [SerializeField] private GameObject _laserObjPrefab;

    [SerializeField] private GameObject _tripleLaserObjPrefab;

    [SerializeField] private GameObject _wideLaserObjPrefab;
    
    [SerializeField] private GameObject _shieldVisualizer;
    
    [SerializeField] private GameObject _wideShotVisualizer;

    [SerializeField] private GameObject _leftEngineOBj;
    
    [SerializeField] private GameObject _rightEngineOBj;
    
    [SerializeField] private GameObject ExplosionObj;
    
    [SerializeField] private float _laserVerticalOffset = 0.7f;

    [SerializeField] private float _fireCoolDown = -1f;

    [SerializeField] private float _fireRate = 0.5f;

    [SerializeField] private float _thrusterSpeedIncrease = 2.5f;
    
    [SerializeField] private float _thrusterBoostAmountLeft = 100;
    
    [SerializeField] private int _lives = 3;
    
    [SerializeField] private int _shieldHitsLeft = 3;
    
    [SerializeField] private int _score = 0;
    
    [SerializeField] private int _AmmoCountLeft = 15;

    [SerializeField] private bool _isTripleShotActive = false;

    [SerializeField] private bool _isWideShotActive = false;
    
    [SerializeField] private bool _isShieldActive = false;

    [SerializeField] private AudioClip _laserAudioClip;

    [SerializeField] private AudioSource _sfxSource;

    private SpawnManager _spawnManager;
    
    private uimanager _uiManager;
    
    private GameManager _gameManager;

    private bool _leftEngineOn = false;

    private bool _rightEngineOn = false;

    private bool _depletethruster = false;

    private bool _NegativePowerActive = false;

    private float _originalSPeed;

    [SerializeField] private float _drawInPowerUpSpeed = 6f;

    [SerializeField] private GameObject _PowerupContainer;

    private bool _fireMissiles = false;
    
    

    private SpriteRenderer shieldRenderer;
    // Start is called before the first frame update
    void Start()
    {
        _spawnManager = GameObject.Find("SpawnManager")?.GetComponent<SpawnManager>();
        _uiManager    = GameObject.Find("UI_Manager")?.GetComponent<uimanager>();
        _gameManager    = GameObject.Find("GameManager")?.GetComponent<GameManager>();
        _originalSPeed = _speed;
        shieldRenderer = _shieldVisualizer.GetComponent<SpriteRenderer>();
        
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
        
        _gameManager.StartCameraShake();
        if (_lives == 0)
        {
            KillPlayer();
        }
        
    }

    void DamageShield()
    {
        _shieldHitsLeft -= 1;
       UpdateShield();
        _gameManager.StartCameraShake();
    }

    void UpdateShield()
    {
      
        switch (_shieldHitsLeft)
        {
            case 3:
                shieldRenderer.color = new Color(1f, 1f, 1f, 1f);
                break;
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

    void HealEngine()
    {
        if (_leftEngineOn && _rightEngineOn)
        {
            int engineChoice = Random.Range(0, 2);
            if (engineChoice > 0)
            {
                _leftEngineOBj.SetActive(false);
                _leftEngineOn = false;
            }
            else
            {
                _rightEngineOBj.SetActive(false);
                _rightEngineOn = false;
            }
        }
        else if (!_leftEngineOn && _rightEngineOn)
        {
            _rightEngineOBj.SetActive(false);
            _rightEngineOn = false;
        }
        else if (_leftEngineOn && !_rightEngineOn)
        {
            _leftEngineOBj.SetActive(false);
            _leftEngineOn = false;
        }
    }

    public void RefillAmmo()
    {
        _AmmoCountLeft = 15;
        _uiManager.SetAmmoText(_AmmoCountLeft);
    }

    public void Heal()
    {
        if (_lives < 3)
        {
            _lives += 1;
            _uiManager.SetLive(_lives);
            HealEngine();
        }
    }

    public void ActivateNegativePowerUp()
    {
        StartCoroutine(NegativePowerActive());
        _NegativePowerActive = true;
    }
    
    public void ActivateMissilePowerUp()
    {
        StartCoroutine(MissilePowerActive());
        _fireMissiles = true;
    }
    IEnumerator MissilePowerActive()
    {
         
        yield return new WaitForSeconds(5f);
        _fireMissiles = false;
    }
    
    IEnumerator NegativePowerActive()
    {
        _speed *= 0.25f;

        yield return new WaitForSeconds(5f);
        _speed = _originalSPeed;
        _NegativePowerActive = false;
    }
    public void ActivateShields()
    {
        _isShieldActive = true;
        _shieldHitsLeft = 3;
        _shieldVisualizer.SetActive(true);
        UpdateShield();
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

    public void ActivateWideShot()
    {
        _isWideShotActive = true;
        _wideShotVisualizer.SetActive(true);
        RefillAmmo();
        StartCoroutine(WideShotCooldown());
    }

    IEnumerator WideShotCooldown()
    {
        yield return new WaitForSeconds(5f);
        _wideShotVisualizer.SetActive(false);
        _isWideShotActive = false;
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
    ThrusterAmount();
    FireLaser();
    DrawInPowerups();
    }

    void DrawInPowerups()
    {
        if (Input.GetKey(KeyCode.C))
        {
            foreach (Transform powerUp in _PowerupContainer.transform)
            {
                powerUp.position = Vector3.MoveTowards(powerUp.position, transform.position,
                    _drawInPowerUpSpeed * Time.deltaTime);
            }
            
        }
    }

    void ThrusterAmount()
    {

        
        if (_depletethruster)
        {
            if (_thrusterBoostAmountLeft > 0)
            {
                _thrusterBoostAmountLeft -= 50 * Time.deltaTime;
            } 
        }
        else
        {
            if (_thrusterBoostAmountLeft < 100)
            {
                _thrusterBoostAmountLeft += 50 * Time.deltaTime;
            }
        }
    }
    void FireLaser()
    {
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _fireCoolDown && _AmmoCountLeft > 0)
        {
            
            _AmmoCountLeft -= 1;
            
            _uiManager.SetAmmoText(_AmmoCountLeft);
            
            if (_isTripleShotActive && !_isWideShotActive)
            {  
                Instantiate(_tripleLaserObjPrefab, transform.position, Quaternion.identity);
            }
            else if (_isWideShotActive)
            {
                Instantiate(_wideLaserObjPrefab, transform.position, Quaternion.identity);
            }
            else if (_fireMissiles)
            {
                Instantiate(_homingProjectilePrefab, transform.position+new Vector3(0,_laserVerticalOffset,0), Quaternion.identity);
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

        if (Input.GetKeyDown(KeyCode.LeftShift) && _thrusterBoostAmountLeft > 0 && !_NegativePowerActive)
        {
            _speed += _thrusterSpeedIncrease;
            _depletethruster = true;
           
        }
        else if( (Input.GetKeyUp(KeyCode.LeftShift) || _thrusterBoostAmountLeft <= 0 ) && !_NegativePowerActive)
        {

            _speed = _originalSPeed;
            _depletethruster = false;
        }
        
        
        
        _uiManager.SetThrustValue(_thrusterBoostAmountLeft);
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
        if (col.tag.Equals("EnemyLaser"))
        {
                DamagePlayer();
                Destroy(col.gameObject);
        }
    }
}
