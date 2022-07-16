using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingProjectile : MonoBehaviour
{
    public enum TARGET{PLAYER,ENEMY,NONE}
    private Transform _target;

    private Rigidbody2D _rigidbody2D;

    private Vector2 _directionToTarget;

    private float _rotationAmount;

    private float _rotationSpeed = 200;

    [SerializeField] private float _speed = 6f;

    [SerializeField] private TARGET _choosableTarget = TARGET.PLAYER;
    // Start is called before the first frame update
    void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        if (_choosableTarget == TARGET.PLAYER)
        {
            _target = GameObject.FindWithTag("Player")?.transform;
        }
        else if(_choosableTarget == TARGET.ENEMY)
        {
            GameObject[] enemies;
            enemies = GameObject.FindGameObjectsWithTag("Enemy");

            float distance = Mathf.Infinity;
            Vector3 position = transform.position;

            foreach (GameObject enemy in enemies)
            {
                Vector3 diff = enemy.transform.position - position;
                float currentDistance = diff.sqrMagnitude;
                if (currentDistance < distance)
                {
                    _target = enemy.transform;
                    distance = currentDistance;
                }
            }
        }
        else
        {
            _target = null;
        }

        StartCoroutine(LoseTracking());
    }

    IEnumerator LoseTracking()
    {
        yield return new WaitForSeconds(4f);

        _choosableTarget = TARGET.NONE;
        
        Destroy(gameObject,5f);
    }
 

    private void FixedUpdate()
    {

        if (_target == null && _choosableTarget != TARGET.NONE)
        {
            return;
        }

        if (_choosableTarget != TARGET.NONE)
        {
            _directionToTarget = _target.position - transform.position;
        
            _directionToTarget.Normalize();

            _rotationAmount = Vector3.Cross(_directionToTarget, transform.up).z;

            _rigidbody2D.angularVelocity = -_rotationAmount * _rotationSpeed;
        
            transform.Translate(Vector3.up * (_speed * Time.deltaTime));
        }
        else
        {
            transform.Translate(Vector3.up * (_speed * Time.deltaTime));
            _rigidbody2D.angularVelocity = 0;
        }

        
    }
}
