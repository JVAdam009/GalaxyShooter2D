using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] private float _speed = 8f;

    private bool _canDamagePlayer = false;

    public float Speed
    {
        get => _speed;
        set => _speed = value;
    }
    

    public void SetDamagePlayer()
    {
        _canDamagePlayer = true;
    }

    public bool CanDamagePlayer()
    {
        return _canDamagePlayer;
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }



    void Movement()
    {
        transform.Translate(Vector3.up * (_speed * Time.deltaTime));

        if (transform.position.y > 8f || transform.position.y < -8f )
        {
            Destroy(transform.root.gameObject);
        }
    }
}
