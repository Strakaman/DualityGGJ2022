using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class DigiNetManager : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        DigiConnect();
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
        PhotonNetwork.CreateRoom("TestRoom", new RoomOptions { MaxPlayers = 8 });
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        Debug.Log("We created a room");
    }
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("successfully joined a room");
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


}
