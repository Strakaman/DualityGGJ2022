using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
using Photon.Realtime;

public class DigiGameManager : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;
    public Dictionary<int, DigiPlayer> playerDictionary;
    public static DigiGameManager instance;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        Vector3 pos = new Vector3(Random.Range(1, 5), 1.25f, Random.Range(1, 5));
        PhotonNetwork.Instantiate(playerPrefab.name, pos, Quaternion.identity);
        Invoke("MakeDigiPlayerDictionary",1f);

    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        MakeDigiPlayerDictionary();
    }

    public void MakeDigiPlayerDictionary()
    {
        playerDictionary = new Dictionary<int, DigiPlayer>();
        DigiPlayer[] digiPlayers = FindObjectsOfType<DigiPlayer>();
        foreach (DigiPlayer dp in digiPlayers)
        {
            playerDictionary.Add(dp.photonView.ViewID, dp);
            Debug.Log($"Photon View ID: {dp.photonView.ViewID} " +
                $"Creator Actor Number: {dp.photonView.CreatorActorNr} " +
                $"Controller Actor Number: {dp.photonView.ControllerActorNr} " +
                $"Owner Actor Number: {dp.photonView.OwnerActorNr}");
        }
        foreach(Player playa in PhotonNetwork.PlayerList)
        {
            Debug.Log($"Digital Player {playa.NickName} ID: {playa.ActorNumber}");
        }
    }

    public DigiPlayer GetDigiPlayer(int photonViewID)
    {
        return playerDictionary[photonViewID];
    }

    public Player GetPlayer(int photonViewID)
    {
        DigiPlayer dp = playerDictionary[photonViewID];
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            if (p.ActorNumber == dp.photonView.ControllerActorNr)
            {
                return p;
            }
        }
        return null;
    }
    public string GetPlayerTeam(int photonViewID)
    {
        Player p = GetPlayer(photonViewID);
        return (string)p.CustomProperties[Constants.TEAM_KEY];
    }    


    // Update is called once per frame
    void Update()
    {
        
    }
}
