using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class DigiNetManager : MonoBehaviourPunCallbacks
{
    public string potentialSessionName { get; private set; }
    static int maxPlayers = 8;
    public string potentialplayerName { get; private set; }
    public InputField sessionNameInputField;
    public InputField playerNameInputField;
    public Text createdRoomNameText;
    private List<RoomInfo> roomList = new List<RoomInfo>();
    public List<GameObject> playerLobbyGridObjects;
    public Button createroomButton;
    public Button joinLobbyButton;
    public Button startMatchButton;

    // Start is called before the first frame update
    void Start()
    {
        potentialplayerName = PlayerPrefs.GetString(Constants.playerName);
        if (!potentialplayerName.Equals(string.Empty))
        {
            playerNameInputField.SetTextWithoutNotify(potentialplayerName);
        }
        Debug.Log($"Lobby Name: {potentialplayerName}");
        joinLobbyButton.interactable = false;
        startMatchButton.interactable = false;
        createroomButton.interactable = true;
        DigiConnect();
    }

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;   
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region PUNOverrides
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);
        Debug.Log("Joining a random room failed.");
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
    }
    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        Debug.Log($"We created a room: {PhotonNetwork.CurrentRoom.Name}");
        createdRoomNameText.text = PhotonNetwork.CurrentRoom.Name;
        startMatchButton.interactable = true;
    }
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log($"successfully joined Room: {PhotonNetwork.CurrentRoom.Name} Count: {PhotonNetwork.CurrentRoom.PlayerCount}");
        PhotonNetwork.LocalPlayer.NickName = PlayerPrefs.GetString(Constants.playerName);
        BuildPlayerListGUI();
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
        PhotonNetwork.JoinRandomRoom();
        //PhotonNetwork.JoinLobby(new TypedLobby(potentialSessionName, LobbyType.Default));
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
        potentialplayerName = playerName;
        PlayerPrefs.SetString(Constants.playerName, playerName);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"Player Joined Room: {newPlayer.NickName}");
        BuildPlayerListGUI();
    }

    public override void OnPlayerLeftRoom (Player newPlayer)
    {
        Debug.Log($"Player Left Room: {newPlayer.NickName}");
        BuildPlayerListGUI();
    }

    public void BuildPlayerListGUI()
    {
        DisablePlayerListGUI();
        Debug.Log("Building player List GUI");
        for (int i=0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            Debug.Log($"This is a player: {PhotonNetwork.PlayerList[i].NickName}");
            playerLobbyGridObjects[i].SetActive(true);
            playerLobbyGridObjects[i].GetComponentInChildren<Text>().text = PhotonNetwork.PlayerList[i].NickName;
        }
    }

    public void DisablePlayerListGUI()
    {
        foreach(GameObject go in playerLobbyGridObjects)
        {
            go.SetActive(false);
        }
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
}
