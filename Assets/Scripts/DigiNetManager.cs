using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;

public class DigiNetManager : MonoBehaviourPunCallbacks
{
    public string potentialSessionName { get; private set; }
    static int maxPlayers = 8;
    public string potentialplayerName { get; private set; }
    public InputField sessionNameInputField;
    public InputField playerNameInputField;
    public Text createdRoomNameText;
    private List<RoomInfo> roomList = new List<RoomInfo>();
    public Button createroomButton;
    public Button joinLobbyButton;
    public Button joinRandomLobbyButton;
    public Button startMatchButton;
    public LobbyPlayerList playerListGUI;

    public GameObject lobbyMenu;
    public GameObject charSelectMenu;

    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.LocalPlayer.NickName.Equals(string.Empty))
        {
            potentialplayerName = PlayerPrefs.GetString(Constants.playerName);
            if (!potentialplayerName.Equals(string.Empty))
            {
                playerNameInputField.SetTextWithoutNotify(potentialplayerName);
                PhotonNetwork.LocalPlayer.NickName = potentialplayerName;
            }
            Debug.Log($"Lobby Name: {potentialplayerName}");
        }
        if (PhotonNetwork.IsConnected)
        {
            joinLobbyButton.interactable = false;
            joinRandomLobbyButton.interactable = false;
            createroomButton.interactable = false;
            createdRoomNameText.text = PhotonNetwork.CurrentRoom.Name;
            if (PhotonNetwork.IsMasterClient)
            {
                startMatchButton.interactable = true;
            }
            else
            {
                startMatchButton.interactable = false;
            }
            playerListGUI.BuildPlayerListGUI(PhotonNetwork.PlayerList);
        }
        else
        {
            joinLobbyButton.interactable = false;
            startMatchButton.interactable = false;
            DigiConnect();
        }
    }

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;   
    }


    #region PUNOverrides
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);
        Debug.Log("Joining a random room failed.");
        OnClickCreate();
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        joinRandomLobbyButton.interactable = true;
        createroomButton.interactable = true;
        PhotonNetwork.LocalPlayer.NickName = PlayerPrefs.GetString(Constants.playerName, $"Block Head: {UnityEngine.Random.Range(1, 1000)}");
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        Debug.Log("CreatingRoom Failed");
        createroomButton.interactable = true;
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        Debug.Log("Joining Specified Room Failed");
        joinLobbyButton.interactable = true;
        joinRandomLobbyButton.interactable = true;
    }
    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        Debug.Log($"We created a room: {PhotonNetwork.CurrentRoom.Name}");
        createdRoomNameText.text = PhotonNetwork.CurrentRoom.Name;
        startMatchButton.interactable = true;
        joinRandomLobbyButton.interactable = false;
        joinLobbyButton.interactable = false;
    }
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log($"successfully joined Room: {PhotonNetwork.CurrentRoom.Name} Count: {PhotonNetwork.CurrentRoom.PlayerCount}");
        if (PhotonNetwork.LocalPlayer.NickName.Equals(string.Empty)) {
            PhotonNetwork.LocalPlayer.NickName = PlayerPrefs.GetString(Constants.playerName, $"Block Head: {UnityEngine.Random.Range(1, 1000)}");
        }
        createroomButton.interactable = false;
        playerListGUI.BuildPlayerListGUI(PhotonNetwork.PlayerList);
    }

    #endregion
    public void DigiConnect()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public void OnClickJoin()
    {
        Debug.Log($"OnClick Triggered to Join {potentialSessionName}");
        if (potentialSessionName.Equals(string.Empty)) { return; }
        joinLobbyButton.interactable = false;
        joinRandomLobbyButton.interactable = false;
        //PhotonNetwork.JoinRandomRoom();
        PhotonNetwork.JoinRoom(potentialSessionName);
    }

    public void OnClickJoinRandom()
    {
        Debug.Log($"OnClick Triggered to Join Random");
        joinLobbyButton.interactable = false;
        joinRandomLobbyButton.interactable = false;
        PhotonNetwork.JoinRandomRoom();
    }

    public void OnClickCreate()
    {
        createroomButton.interactable = false;
        potentialSessionName = GenerateLobbyName();
        foreach(RoomInfo ri in roomList)
        {
            if (ri.Name.Equals(potentialSessionName))
            {
                OnClickCreate(); //if a session exists with that ID, just regen the session ID by recalling method
                return;
            }
        }
        PhotonNetwork.CreateRoom(potentialSessionName, new RoomOptions { MaxPlayers = (byte)maxPlayers });
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        this.roomList = roomList;
    }

    public void OnClickLoadLevel()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(1);
        }
    }

    //called by TextFieldOnChange
    public void SetSessionName(string sessionName)
    {
        potentialSessionName = sessionName.Trim().ToUpper();
        if (sessionName.Equals(string.Empty))
        {
            joinLobbyButton.interactable = false;
        }
        else
        {
            joinLobbyButton.interactable = true;
        }
    }

    public void SetPlayerName(string playerName)
    {
        if (playerName.Equals(string.Empty)) { return; }
        potentialplayerName = playerName;
        PlayerPrefs.SetString(Constants.playerName, playerName);
        PhotonNetwork.LocalPlayer.NickName = playerName;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"Player Joined Room: {newPlayer.NickName}");
        playerListGUI.BuildPlayerListGUI(PhotonNetwork.PlayerList);
    }

    public override void OnPlayerLeftRoom (Player newPlayer)
    {
        Debug.Log($"Player Left Room: {newPlayer.NickName}");
        playerListGUI.BuildPlayerListGUI(PhotonNetwork.PlayerList);
    }

    public string GenerateLobbyName()
    { 
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        var stringChars = new char[4];
        var random = new System.Random();

        for (int i = 0; i < stringChars.Length; i++)
        {
            stringChars[i] = chars[random.Next(chars.Length)];
        }

        string finalString = new String(stringChars);
        return finalString;
    }

    public void OpenCharacterSelect()
    {
        lobbyMenu.SetActive(false);
        charSelectMenu.SetActive(true);
    }

    public void CloseCharacterSelect()
    {
        lobbyMenu.SetActive(true);
        charSelectMenu.SetActive(false);
    }
}
