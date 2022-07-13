using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    [SerializeField] private bool _isGameOver = false;

    [SerializeField] private float _cameraShakeDuration = .75f;

    [SerializeField] private Camera _gameCamera;

    [SerializeField] private float _cameraShakeStrength = .4f;
    
    

    [SerializeField] private float _cameraShakeOffset = .4f;

    private Vector3 _originalCameraPosition = new Vector3(0, 1, -10);
    

    public void SetGameOver()
    {
        _isGameOver = true;
    }

    public void StartCameraShake()
    {
        StartCoroutine(CameraShake());
    }

    IEnumerator CameraShake()
    {
        float timeElasped = 0f;
        Vector3 shake = Vector3.zero;
        
        shake.y = 1;
        shake.z = -10f;
        while (_cameraShakeDuration > timeElasped)
        {
            
            shake.x = Random.Range(-_cameraShakeOffset,_cameraShakeOffset) * _cameraShakeStrength;
            shake.y = Random.Range(-_cameraShakeOffset,_cameraShakeOffset) * _cameraShakeStrength;
            
            _gameCamera.transform.localPosition = shake;

            timeElasped += Time.deltaTime;
            
            yield return null;
        }

        _gameCamera.transform.localPosition = _originalCameraPosition;
        yield return null;
    }

    

    // Update is called once per frame
    void Update()
    {
        if (_isGameOver && Input.GetKeyDown(KeyCode.R))
        {
            Scene scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.name);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit(0);
        }
    }
}
