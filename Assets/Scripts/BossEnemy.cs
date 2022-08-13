using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemy : MonoBehaviour
{
    public enum States {NONE, INTRO, ATTACKING, DYING}

    [SerializeField] private States _currentState;
    [SerializeField] private uimanager _uimanager;

    [Header("Intro State Variables")] [SerializeField]
    private Transform _targetPoint;

    [SerializeField] private float _speed = 2f;
    [Header("Attack State Variables")] 

    [SerializeField]
    private List<SubBoss>    _subBosses;
    [SerializeField]
    private List<GameObject> _fireEffects;

    [SerializeField] private int _health = 2;

    [SerializeField] private float _AttackTimer;
    [SerializeField] private GameObject _explosionPrefab;
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private Transform _LaserFocalPoint;

    private bool _canFire = true;

    private float _timer = -1;

    private SpawnManager _spawnManager;
    void Start()
    {
        _currentState = States.INTRO;
        for (int i = 0; i < 10; i++)
        {
            _lineRenderer.SetPosition(i,_LaserFocalPoint.position);
        }
        _timer = Time.time + _AttackTimer;
        _spawnManager = GameObject.Find("SpawnManager")?.GetComponent<SpawnManager>();
        _uimanager =GameObject.Find("UI_Manager")?.GetComponent<uimanager>();
        _uimanager.ShowBar();

        _targetPoint = _spawnManager.GetBossTargetPoint();

    }

    // Update is called once per frame
    void Update()
    {
        ProcessMovement(_currentState);
        ProcessActions(_currentState);
    }

    public void RemoveSubboss(SubBoss sub)
    {
        var index = _subBosses.IndexOf(sub);
        _subBosses.Remove(sub);
        _fireEffects[index].SetActive(true);
        _fireEffects.RemoveAt(index);
        
    }


    void ProcessActions(States state)
    {
        switch (state)
        {
            case States.ATTACKING:
                if (_canFire && Time.time > _timer)
                {
                    StartCoroutine(FireLaserBeam());
                    _canFire = false;
                    _timer = Time.time + _AttackTimer;
                }
                break; 
        }
    }

    IEnumerator FireLaserBeam()
    {
        var position = _LaserFocalPoint.position;

        _currentState = States.INTRO;
        for (int i = 0; i < 10; i++)
        {
            _lineRenderer.SetPosition(i,position);
        }
        
        yield return new WaitForSeconds(0.2f);
        
        _lineRenderer.SetPosition(9,Vector3.down * 2);
        
        yield return new WaitForSeconds(0.2f);
        
        _lineRenderer.SetPosition(9,position);
        
        yield return new WaitForSeconds(0.2f);
        
        _lineRenderer.SetPosition(9,Vector3.down * 2);
        
        yield return new WaitForSeconds(0.2f);
        
        _lineRenderer.SetPosition(9,position);
        
        yield return new WaitForSeconds(0.2f);
        
        _lineRenderer.SetPosition(9,Vector3.down * 2);
        
        yield return new WaitForSeconds(0.2f);
        
        _lineRenderer.SetPosition(9,position);
        
        yield return new WaitForSeconds(0.2f);
        
        _lineRenderer.SetPosition(9,Vector3.down * 2);
        
        yield return new WaitForSeconds(0.2f);
        
        
        RaycastHit2D hit = Physics2D.Raycast(position, Vector2.down);
        if (hit.collider && hit.transform.tag.Equals("Player"))
        {
            _lineRenderer.SetPosition(9,hit.point);
            var player = hit.transform.GetComponent<Player>();
            player.DamagePlayer();
        }
        else
        {
            _lineRenderer.SetPosition(9,Vector3.down * 8);
        }

        _canFire = true;
    }

    void ProcessMovement(States state)
    {
        switch (state)
        {
            case States.INTRO:
                IntroMovement();
                break;
        }
    }

    void IntroMovement()
    {
        transform.position = Vector3.MoveTowards(transform.position, _targetPoint.position, _speed*Time.deltaTime);

        if (Vector3.Distance(transform.position, _targetPoint.position) <= .01f)
        {
            _currentState = States.ATTACKING;
            foreach (SubBoss sub in _subBosses)
            {
                sub.EnterIntroState();
                
            }
        }
    }
    




    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag.Equals("Laser"))
        {
            if (_subBosses.Count <= 0)
            {
                _health--;
                _uimanager.SubtractFromBar();


                if (_health <= 0)
                {
                    GetComponent<Collider2D>().enabled = false;
                    _uimanager.HideBar();
                    var explosion = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
                    explosion.transform.localScale = Vector3.one * 2.5f;
                    explosion.transform.position = Vector3.back + Vector3.up * 2f;
                    Destroy(explosion, 2.3f);
                    Destroy(gameObject,1.2f);
                    _spawnManager.BossDied();
                }
            }
        }
    }
}
