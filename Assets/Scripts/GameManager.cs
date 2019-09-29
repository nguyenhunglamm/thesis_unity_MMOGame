using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
public class GameManager : MonoBehaviourPunCallbacks
{
    #region Private Fields

    int timePassed;

    bool textActivated = false;
    #endregion
    #region Public Fields
    public bool endGame = false;

    public GameObject player;

    public GameObject winText;

    public GameObject loseText;
    public GameObject drawText;

    public GameObject timer;

    public Slider hpBar;

    public Slider enemyHPBar;

    public GameObject computer;

    public GameObject movementJoystick;

    public GameObject rotateJoystick;


    #endregion
    TMP_Text timerText;
    #region Photon Callbacks

    #endregion
    #region MonoBehaviour Callbacks

    void Awake()
    {

    }

    void Start()
    {
        timerText = timer.GetComponent<TMP_Text>();
        timePassed = 0;
        InvokeRepeating("timerCount", 1, 1);
    }

    private void Update()
    {
        if (endGame)
        {
            CancelInvoke("timerCount");
        }
        if (endGame)
        {
            movementJoystick.SetActive(false);
            rotateJoystick.SetActive(false);
        }
        if (timePassed >= 99 && !endGame)
        {
            if (hpBar.value < enemyHPBar.value)
            {
                player.GetComponent<CharacterManager>().die();
                computer.GetComponent<AI>().win();
            }
            else if (hpBar.value > enemyHPBar.value)
            {
                computer.GetComponent<AI>().die();
                player.GetComponent<CharacterManager>().win();
            }
            else
            {
                player.GetComponent<CharacterManager>().enabled = false;
                player.GetComponent<CharacterGesture>().enabled = false;
                computer.GetComponent<AI>().CancelInvoke();
                computer.GetComponent<AI>().enabled = false;               
                draw();
            }
        }
        if (Input.GetMouseButtonDown(0) && textActivated)
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    #endregion
    #region Public Methods

    void timerCount()
    {
        timePassed++;
        if (timePassed <= 99)
        {
            timerText.text = "" + (99 - timePassed);
        }
    }

    public void win()
    {
        endGame = true;
        Invoke("setWin", 2);
    }
    public void setWin()
    {
        textActivated = true;
        winText.SetActive(true);
    }

    public void lose()
    {
        endGame = true;
        Invoke("setLose", 2);
    }  
    public void setLose()
    {
        textActivated = true;
        loseText.SetActive(true);
    }

    public void draw() {
        endGame = true;
        drawText.SetActive(true);
        Invoke("setDraw", 3);
    }
    public void setDraw() {
        textActivated = true;
    }


    #endregion
}
