using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float _speed = 3.5f;

    [SerializeField] private GameObject LaserObjPrefab;
    
    [SerializeField] private float _laserVerticalOffset = 0.7f;

    [SerializeField] private float _fireCoolDown = -1f;

    [SerializeField] private float _fireRate = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        
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
            Instantiate(LaserObjPrefab, transform.position+new Vector3(0,_laserVerticalOffset,0), Quaternion.identity);
            _fireCoolDown = Time.time + _fireRate;
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
}
