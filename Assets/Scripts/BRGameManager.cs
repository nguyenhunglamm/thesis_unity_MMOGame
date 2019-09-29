using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class BRGameManager : MonoBehaviourPunCallbacks
{
    #region Public Fields

    public Transform[] playerSpawn;
    public GameObject[] player;

    public GameObject winText;
    public GameObject loseText;
    public GameObject drawText;

    public GameObject movementJoystick;

    public Slider hpBar;
    public Slider enemyHPBar;

    public GameObject timer;
    #endregion
    #region Private Fields

    bool textActivated = false;
    bool endGame = false;
    private int numberOfDeath = 0;
    #endregion
    #region Functions

    #endregion
    #region MonoBehaviour

    void Start()
    {
        int playerID = PhotonNetwork.LocalPlayer.ActorNumber - 1;
        PhotonNetwork.Instantiate(player[playerID].name, playerSpawn[playerID].position, playerSpawn[playerID].rotation);
        numberOfDeath = 0;
    }

    void Update()
    {
        if (numberOfDeath == 3 && !endGame)
        {
            win();
            BRCharacterManager.localPlayer.GetComponent<BRCharacterManager>().win();
        }
        if (endGame)
        {
            movementJoystick.SetActive(false);
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
        win();
        OnlineCharacterManager.localPlayer.GetComponent<OnlineCharacterManager>().win();
    }
    public void win()
    {
        endGame = true;
        CancelInvoke();
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
        CancelInvoke();
        Invoke("setLose", 2);
        numberOfDeath++;
    }
    public void setLose()
    {
        textActivated = true;
        loseText.SetActive(true);
    }
    public void draw() {
        endGame = true;
        CancelInvoke();
        drawText.SetActive(true);
        Invoke("setDraw", 3);
    }
    public void setDraw() {
        textActivated = true;
    }
    #endregion
}
