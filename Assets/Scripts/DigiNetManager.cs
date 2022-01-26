using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class DigiNetManager : MonoBehaviourPunCallbacks
{
    public string potentialSessionName { get; private set; }
    static int maxPlayers = 8;
    public string potentialplayerName { get; private set; }
    public InputField sessionNameInputField;
    public InputField playerNameInputField;

    // Start is called before the first frame update
    void Start()
    {
        potentialSessionName = PlayerPrefs.GetString(Constants.sessionName);
        potentialplayerName = PlayerPrefs.GetString(Constants.playerName);
        if (!potentialSessionName.Equals(string.Empty))
        {
            sessionNameInputField.SetTextWithoutNotify(potentialSessionName);
        }
        if (!potentialplayerName.Equals(string.Empty))
        {
            playerNameInputField.SetTextWithoutNotify(potentialplayerName);
        }
        Debug.Log($"Lobby Name: {potentialSessionName}");
        Debug.Log($"Lobby Name: {potentialplayerName}");

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
        PhotonNetwork.CreateRoom(potentialSessionName, new RoomOptions { MaxPlayers = (byte)maxPlayers });
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        Debug.Log($"We created a room: {PhotonNetwork.CurrentRoom.Name}");
    }
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log($"successfully joined Room: {PhotonNetwork.CurrentRoom.Name} Count: {PhotonNetwork.CurrentRoom.PlayerCount}");
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(1);
        }
    }
    #endregion
    public void DigiConnect()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public void OnClickPlay()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    //called by TextFieldOnChange
    public void SetSessionName(string sessionName)
    {
        potentialSessionName = sessionName;
        PlayerPrefs.SetString(Constants.sessionName, sessionName);
    }

    public void SetPlayerName(string playerName)
    {
        potentialplayerName = playerName;
        PlayerPrefs.SetString(Constants.playerName, playerName);
    }
}
