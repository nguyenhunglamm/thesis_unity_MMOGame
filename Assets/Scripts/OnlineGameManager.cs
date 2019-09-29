using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class OnlineGameManager : MonoBehaviourPunCallbacks
{
    #region Public Fields

    public Transform[] playerSpawn;
    public GameObject[] player;

    public GameObject winText;
    public GameObject loseText;
    public GameObject drawText;

    public GameObject movementJoystick;
    public GameObject rotateJoystick;

    public Slider hpBar;
    public Slider enemyHPBar;

    public GameObject timer;
    #endregion
    #region Private Fields

    bool textActivated = false;
    bool endGame = false;
    int timePassed = 0;
    TMP_Text timerText;
    #endregion
    #region Functions

    void timerCount()
    {
        timePassed++;
        if (timePassed <= 99)
        {
            timerText.text = "" + (99 - timePassed);
        }
    }

    #endregion
    #region MonoBehaviour

    void Start()
    {
        int playerID = PhotonNetwork.LocalPlayer.ActorNumber - 1;
        PhotonNetwork.Instantiate(player[playerID].name, playerSpawn[playerID].position, playerSpawn[playerID].rotation);
        timerText = timer.GetComponent<TMP_Text>();
        timePassed = 0;
        InvokeRepeating("timerCount", 1, 1);
    }

    void Update()
    {
        if (endGame)
        {
            movementJoystick.SetActive(false);
            rotateJoystick.SetActive(false);
            CancelInvoke("timerCount");
        }
        else
        if (timePassed >= 99 && !endGame)
        {
            if (hpBar.value < enemyHPBar.value)
            {
                OnlineCharacterManager.localPlayer.GetComponent<OnlineCharacterManager>().die();
            }
            else if (hpBar.value > enemyHPBar.value)
            {
                OnlineCharacterManager.localPlayer.GetComponent<OnlineCharacterManager>().win();
                win();
            }
            else
            {
                OnlineCharacterManager.localPlayer.GetComponent<OnlineCharacterManager>().enabled = false;
                OnlineCharacterManager.localPlayer.GetComponent<OnlineCharacterGesture>().enabled = false;
                OnlineCharacterManager.localPlayer.GetComponent<OnlineCharacterGesture>().clear();
                draw();
            }
        }
        if (textActivated && Input.GetMouseButtonDown(0))
        {
            PhotonNetwork.LeaveRoom();
        }
    }
    #endregion
    #region End Game

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        if (!endGame)
        {

            win();
            OnlineCharacterManager.localPlayer.GetComponent<OnlineCharacterManager>().win();
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
    public void draw()
    {
        endGame = true;
        drawText.SetActive(true);
        Invoke("setDraw", 3);
    }
    public void setDraw()
    {
        textActivated = true;
    }
    #endregion
}
