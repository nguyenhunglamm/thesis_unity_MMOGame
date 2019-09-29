using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    #region Public Fields

    public GameObject connecting;

    public GameObject matchWindow_VS;

    public GameObject matchWindow_BR;

    public InputField roomInput;

    public InputField nameInputVS;

    public InputField nameInputBR;

    public GameObject matching;

    public Text joining;

    public GameObject cancel;

    public GameObject mainWindow;

    public GameObject title;
    public GameObject VSModeText;

    #endregion
    #region Private Fields

    [SerializeField]
    private string gameVersion = "1";

    [SerializeField]
    private byte maxPlayersPerRoom = 2;

    private bool gameModeBR = false; //false = 1v1, true = BR
    #endregion
    #region MonoBehaviour Callbacks

    void Awake()
    {
        //Sync scene in the same room
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            mainWindow.SetActive(false);
            title.SetActive(false);
        }
    }

    #endregion
    #region Public Methods

    public void ConnectToServer_1v1()
    {
        if (PhotonNetwork.IsConnected && !gameModeBR)
        {
            matchWindow_VS.SetActive(true);
            VSModeText.SetActive(true);
        }
        else
        {
            connecting.SetActive(true);
            gameModeBR = false;
            maxPlayersPerRoom = 2;
            PhotonNetwork.GameVersion = gameVersion;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public void ConnectToServer_BR()
    {
        if (PhotonNetwork.IsConnected && gameModeBR)
        {
            matchWindow_BR.SetActive(true);
        }
        else
        {
            connecting.SetActive(true);
            gameModeBR = true;
            maxPlayersPerRoom = 4;
            PhotonNetwork.GameVersion = gameVersion;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public void Disconnect()
    {
        PhotonNetwork.Disconnect();
    }

    public void JoinRandomRoom()
    {
        if (!gameModeBR && PhotonNetwork.CurrentRoom == null && nameInputVS.text != "")
        {
            matchWindow_VS.SetActive(false);
            matching.SetActive(true);
            cancel.SetActive(true);
            PhotonNetwork.JoinRandomRoom();
        }
        if (gameModeBR && PhotonNetwork.CurrentRoom == null && nameInputBR.text != "")
        {
            matchWindow_BR.SetActive(false);
            matching.SetActive(true);
            cancel.SetActive(true);
            PhotonNetwork.JoinRandomRoom();
        }
    }

    public void CreateRoom()
    {
        if (roomInput.text != "" && nameInputVS.text != "")
        {
            matchWindow_VS.SetActive(false);
            joining.text = "Room name: " + roomInput.text + "\n waiting for your friends...";
            cancel.SetActive(true);
            PhotonNetwork.JoinOrCreateRoom(roomInput.text, new RoomOptions { MaxPlayers = maxPlayersPerRoom }, TypedLobby.Default);
        }
        else
        {
            Debug.Log("Invalid name");
        }
    }

    public void Cancel()
    {
        if (PhotonNetwork.CurrentRoom != null)
        {
            PhotonNetwork.LeaveRoom();
        }
    }

    public void LoadArena()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.Log("PhotonNetwork : Trying to Load a level but we are not the master Client");
        }
        else
        {
            if (gameModeBR)
            {
                PhotonNetwork.LoadLevel("BatteRoyale");
            }
            else
                PhotonNetwork.LoadLevel("1v1");
        }
    }

    #endregion
    #region PUN Callbacks

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
        Debug.Log("Current on Master: " + PhotonNetwork.CountOfPlayersOnMaster);
        connecting.SetActive(false);
        matching.SetActive(false);
        joining.text = "";
        if (gameModeBR)
        {
            matchWindow_BR.SetActive(true);
        }
        else
        {
            VSModeText.SetActive(true);
            matchWindow_VS.SetActive(true);
        }
        cancel.SetActive(false);
        Debug.Log("Player ID: " + PhotonNetwork.LocalPlayer.ActorNumber);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Disconnected");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("No room available. Creating one...");
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room " + PhotonNetwork.CurrentRoom.Name + " with player name " + PhotonNetwork.NickName + ", waiting for another player...");
        Debug.Log("Current: " + PhotonNetwork.CurrentRoom.PlayerCount);
        if (PhotonNetwork.CurrentRoom.Name == roomInput.text)
        {
            PhotonNetwork.CurrentRoom.IsVisible = false;
            Debug.Log("Private room: " + PhotonNetwork.CurrentRoom.Name);
        }
        Debug.Log("Player ID in room: " + PhotonNetwork.LocalPlayer.ActorNumber);
    }

    public override void OnLeftRoom()
    {
        Debug.Log("Left Room");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("Someone joined room, current: " + PhotonNetwork.CurrentRoom.PlayerCount + "players");
        if (PhotonNetwork.CurrentRoom.PlayerCount == maxPlayersPerRoom)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            LoadArena();
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("Opp has left the room");
    }

    #endregion
}