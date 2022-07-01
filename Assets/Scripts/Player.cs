using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float _speed = 3.5f;
    
     
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    CaluclateMovement();
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
