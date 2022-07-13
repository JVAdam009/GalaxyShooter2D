using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class uimanager : MonoBehaviour
{

    [SerializeField] private TMP_Text _scoretext;

    [SerializeField] private Sprite[] _liveSprites;

    [SerializeField] private Image _livesImg;

    [SerializeField] private GameObject _gameOverText;

    [SerializeField] private GameObject _restartText; 

    [SerializeField] private TMP_Text _ammoCountText;

    [SerializeField] private Slider _thrustSlider;

    [SerializeField] private TMP_Text _waveText;

    private bool _pulsateAmmoColor = false;
    
    private bool _showWaveText;


    public void SetAmmoText(int ammoCount)
    {
        _ammoCountText.text = "Ammo: " + ammoCount + " / 15" ;

        if (ammoCount < 1)
        {
            _ammoCountText.color = Color.red;
            _pulsateAmmoColor = true;
        }
        else
        {
            _ammoCountText.color = Color.white;
            _pulsateAmmoColor = false;
        }
    }

    public void StartWaveText()
    {
        StartCoroutine(WaveTextFade());
    }

    IEnumerator WaveTextFade()
    {
        _showWaveText = true;
        yield return null;
        StartCoroutine(StopWaveText());
    }

    IEnumerator StopWaveText()
    {
        yield return new WaitForSeconds(3f);
        _showWaveText = false;
    }

    public void SetScore(int score)
    {
        _scoretext.text = "Score: " + score;
    }

    public void SetLive(int currentlives)
    {
        _livesImg.sprite = _liveSprites[currentlives];
    }

    public void ShowGameOverText()
    {
        StartCoroutine(FlickerGameOver());
        _restartText.SetActive(true);
    }

    public void EnableGameOverText()
    {
        _gameOverText.SetActive(true);
    }
    
    public void DisableGameOverText()
    {
        _gameOverText.SetActive(false);
    }

    IEnumerator FlickerGameOver()
    {
        while (true)
        {
            EnableGameOverText();
            yield return new WaitForSeconds(.7f);
            DisableGameOverText();
            yield return new WaitForSeconds(.7f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_pulsateAmmoColor == true)
        {
            OutofAmmoPulseColor();
        }
        else
        {
            NormalAmmoColor();
        }

        if (_showWaveText)
        {
            _waveText.enabled = true;
            _waveText.color = new Color(1, 1, 1, Mathf.PingPong(Time.time, 1));
        }
        else
        {
            _waveText.enabled = false;
        }
        
    }

    public void SetThrustValue(float value)
    {
        _thrustSlider.value = value;
    }

    void OutofAmmoPulseColor()
    {
        _ammoCountText.color = new Color(1, 0, 0, Mathf.PingPong(Time.time, 1));
    }

    void NormalAmmoColor()
    {
        _ammoCountText.color = Color.white;
    }
}
