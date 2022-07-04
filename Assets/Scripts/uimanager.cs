using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class uimanager : MonoBehaviour
{

    [SerializeField] private TMP_Text scoretext;

    [SerializeField] private Sprite[] liveSprites;

    [SerializeField] private Image _livesImg;

    [SerializeField] private GameObject gameOverText;

    [SerializeField] private GameObject restartText; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

     

    public void SetScore(int score)
    {
        scoretext.text = "Score: " + score;
    }

    public void SetLive(int currentlives)
    {
        _livesImg.sprite = liveSprites[currentlives];
    }

    public void ShowGameOverText()
    {
        StartCoroutine(FlickerGameOver());
        restartText.SetActive(true);
    }

    public void EnableGameOverText()
    {
        gameOverText.SetActive(true);
    }
    
    public void DisableGameOverText()
    {
        gameOverText.SetActive(false);
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
        
    }
}
