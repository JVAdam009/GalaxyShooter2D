using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SubBoss : MonoBehaviour
{
    public enum STATE {INTRO,FIGHT, NONE}
    [SerializeField] private Transform _gotoPosition;
    [SerializeField] private int Health = 5;
    [SerializeField] private float _speed = 5;
    [SerializeField] private float _firerate = 3;
    [SerializeField] private Collider2D _collider;
    [SerializeField] private GameObject _shieldGameObject;
    [SerializeField] private GameObject _laserGameObject; 
    [SerializeField] private BossEnemy _boss; 
    private uimanager _uimanager;
    private Player _player;

    [SerializeField] private STATE _currentState = STATE.NONE;

    private float _FiringTimer = -1;

    private void Start()
    {
        _player = GameObject.FindWithTag("Player")?.GetComponent<Player>();
        _uimanager =GameObject.Find("UI_Manager")?.GetComponent<uimanager>();

    }

    public void EnterIntroState()
    {
        _currentState = STATE.INTRO;
    }

    // Update is called once per frame
    void Update()
    {
        switch (_currentState)
        {
            case STATE.INTRO:
              IntroUpdate();
                break;
            case STATE.FIGHT:
                FightUpdate();
                break;
            case STATE.NONE:
                break;
        }
    }

    void IntroUpdate()
    {
        transform.position =
            Vector3.MoveTowards(transform.position, _gotoPosition.position, Time.deltaTime * _speed);

        if (Vector3.Distance(transform.position, _gotoPosition.position) <= 0.1f)
        {
            _collider.enabled = true;
            _shieldGameObject.SetActive(true);
            _currentState = STATE.FIGHT;
            _FiringTimer = Time.time + _firerate;
        }
    }

    void Attack()
    {
        if (_player == null)
        {
            return;
        }
        _firerate = Random.Range(1, 3);

        Vector3 diff = _player.transform.position - transform.position;
        diff.Normalize();
 
        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rot_z + 90);
        LaserFire();
        

        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }

     

    void LaserFire()
    {
        
        GameObject laserGO = Instantiate(_laserGameObject, transform.position,transform.rotation);
        Laser laser = laserGO.GetComponent<Laser>();
        laser.Speed = -_speed * 1.5f;
        laser.SetDamagePlayer();
        laser.tag = "EnemyLaser";

    }
    void FightUpdate()
    {
        if (Time.time > _FiringTimer)
        {
            Attack();
            _FiringTimer = Time.time + _firerate;
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag.Equals("Laser"))
        {
            Health--;
            _uimanager.SubtractFromBar();

            Destroy(col.gameObject);
            if (Health <= 0)
            {
                _boss.RemoveSubboss(this);
                Destroy(gameObject);
            }
        }
    }
}
