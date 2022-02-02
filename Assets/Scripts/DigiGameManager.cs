using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
using Photon.Realtime;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Random = UnityEngine.Random;
using System;

public class DigiGameManager : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;
    public Text statusText;
    public Dictionary<int, DigiPlayer> playerDictionary;
    public static DigiGameManager instance;

    int matchTimeinSeconds = 180;
    public float timeLeft { get; private set; }
    //public string WinningTeam { get; private set; }
    public bool timeUp = false;
    public bool gameStarted = false;
    public bool gameOver = false;
    public bool initFinished = false;
    private void Awake()
    {
        instance = this;
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Start()
    {
        Vector3 pos = new Vector3(Random.Range(1, 5), 1.25f, Random.Range(1, 5));
        PhotonNetwork.Instantiate(playerPrefab.name, pos, Quaternion.identity);
        Invoke("MakeDigiPlayerDictionary",1f);
        InitMatch();
    }

    void Update()
    {
        if (initFinished)
        {
            initFinished = false; 
            StartCoroutine(GameTimer());
        }
        if (timeUp == true)
        {
            timeUp = false;
            GameOver();
        }
        if (gameStarted && !gameOver)
        {
            HUDManager.instance.UpdateTimeLeft();
        }
    }

    #region PlayerRefs
    public void MakeDigiPlayerDictionary()
    {
        playerDictionary = new Dictionary<int, DigiPlayer>();
        DigiPlayer[] digiPlayers = FindObjectsOfType<DigiPlayer>();
        foreach (DigiPlayer dp in digiPlayers)
        {
            if (!playerDictionary.ContainsKey(dp.photonView.ControllerActorNr))
            {
                //seems like during reload the photon view may get a new ID but playedID will not
                playerDictionary.Add(dp.photonView.ControllerActorNr, dp);
                /*Debug.Log($"Photon View ID: {dp.photonView.ViewID} " +
                    $"Creator Actor Number: {dp.photonView.CreatorActorNr} " +
                    $"Controller Actor Number: {dp.photonView.ControllerActorNr} " +
                    $"Owner Actor Number: {dp.photonView.OwnerActorNr}");*/
            }
        }
        foreach(Player playa in PhotonNetwork.PlayerList)
        {
            //Debug.Log($"Digital Player {playa.NickName} ID: {playa.ActorNumber}");
        }
    }

    public DigiPlayer GetDigiPlayer(int photonViewID)
    {
        return playerDictionary[photonViewID];
    }

    public DigiPlayer FindLocalDigiPlayer()
    {
        foreach(DigiPlayer dp in playerDictionary.Values)
        {
            if(dp.photonView.ControllerActorNr == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                return dp;
            }
        }
        return null;
    }

    public Player GetPlayer(int photonViewID)
    {
        DigiPlayer dp;
        try { dp = playerDictionary[photonViewID]; }
        catch (KeyNotFoundException)
        {
            MakeDigiPlayerDictionary();
            dp = playerDictionary[photonViewID];
        }
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
    #endregion

    // Update is called once per frame
    #region GameLoop
    void InitMatch()
    {
        timeLeft = matchTimeinSeconds;
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable { { Constants.TIME_LEFT_KEY, timeLeft } });
        }
        initFinished = false;
        gameStarted = false;
        timeUp = false;
        gameOver = false;
        if(PhotonNetwork.IsMasterClient)
        {
            SplitTeams();
        }
        StartCoroutine(GameInit());
    }

    void SplitTeams()
    {
        int fullPlayerCount = PhotonNetwork.PlayerList.Length;
        int halfPlayerCount = fullPlayerCount / 2;
        List<int> playerIndexList = new List<int>();
        for (int i = 0; i < halfPlayerCount; i++)
        {
            int index = Random.Range(0, fullPlayerCount);
            while (playerIndexList.Contains(index))
            {
                index = Random.Range(0, fullPlayerCount);
            }
            playerIndexList.Add(index);
        }
        for(int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            Hashtable teamToSet;
            if (playerIndexList.Contains(i))
            {
                teamToSet = new Hashtable { { Constants.TEAM_KEY, Constants.GREEN_TEAM } };
            }
            else
            {
                teamToSet = new Hashtable { { Constants.TEAM_KEY, Constants.PURPLE_TEAM } };
            }
            PhotonNetwork.PlayerList[i].SetCustomProperties(teamToSet);
        }
        Invoke("TryingToInvokeSpawn", 2f);
    }

    void TryingToInvokeSpawn()
    {
        SpawnManager.instance.SetAllPlayerSpawns();
    }

    void GameOver()
    {
        gameOver = true;
        CalculateGameStats();
        if (PhotonNetwork.IsMasterClient) //never outshine the master
        {
            CalculateWinner(); //only the master client will determine the winner
        }
        StartCoroutine(GameOverShenanigans());
    }

    private void CalculateGameStats()
    {
        DigiPlayer myPlayer = FindLocalDigiPlayer();
        if (myPlayer != null) //should never happen
        {
            myPlayer.PushMatchStats();
        }
    }
    private void CalculateWinner()
    {


        int greenScore = DigiTeamsManager.instance.GetTeamScore(Constants.GREEN_TEAM);
        int purpleScore = DigiTeamsManager.instance.GetTeamScore(Constants.PURPLE_TEAM);
        string resultingWinner = string.Empty;
        if (purpleScore > greenScore)
        {
            resultingWinner = Constants.PURPLE_TEAM;
        }
        else
        {
            resultingWinner = Constants.GREEN_TEAM;
        }

        //sets the winning team on all clients so that each local player can consume result
        //photonView.RPC(nameof(RPC_SetWinningTeam), RpcTarget.All, resultingWinner);
        Hashtable winningHash = new Hashtable { { Constants.WINNING_TEAM_KEY, resultingWinner } };
        PhotonNetwork.CurrentRoom.SetCustomProperties(winningHash);
    }

    [PunRPC]
    void RPC_SetWinningTeam(string winningTeamResult)
    {
        //WinningTeam = winningTeamResult;
    }
    IEnumerator GameInit()
    {
        statusText.gameObject.SetActive(false);
        AudioManager.instance.PlaySound2D(Constants.Sound_Ready) ;
        yield return new WaitForSeconds(3f);
        statusText.text = "READY...";
        statusText.color = Constants.purpleTeamColor;
        statusText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        statusText.text = "GO!";
        statusText.color = Constants.greenTeamColor;
        yield return new WaitForSeconds(1f);
        statusText.gameObject.SetActive(false);
        initFinished = true;
    }
    IEnumerator GameTimer()
    {
        gameStarted = true;
        float timeLoopPeriod = .5f;
        while (gameStarted && GetTimeLeft() > 0)
        {
            yield return new WaitForSeconds(timeLoopPeriod);
            timeLeft -= timeLoopPeriod;
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable { { Constants.TIME_LEFT_KEY, timeLeft } });
            }
  }
        timeUp = true;
    }

    public int GetMatchTime()
    {
        return matchTimeinSeconds;
    }
    public float GetTimeLeft()
    {
        try
        {
            return (float)PhotonNetwork.CurrentRoom.CustomProperties[Constants.TIME_LEFT_KEY];
        }
        catch (NullReferenceException)
        { 
            return timeLeft;
        }
    }
    IEnumerator GameOverShenanigans()
    {
        statusText.text = "Time's Up!";
        AudioManager.instance.PlaySound2D(Constants.Sound_TimeUp);
        statusText.color = Color.white;
        statusText.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        statusText.text = "The winner is...";
        AudioManager.instance.PlaySound2D(Constants.Sound_Winner);
        statusText.color = Color.white;
        yield return new WaitForSeconds(2f);
        string winningTeam = (string)PhotonNetwork.CurrentRoom.CustomProperties[Constants.WINNING_TEAM_KEY];
        if (winningTeam.Equals(Constants.GREEN_TEAM))
        {
            statusText.text = "Green Team!";
            statusText.color = Constants.greenTeamColor;
            AudioManager.instance.PlaySound2D(Constants.Sound_Green);
        }
        else
        {
            statusText.text = "Purple Team!";
            statusText.color = Constants.purpleTeamColor;
            AudioManager.instance.PlaySound2D(Constants.Sound_Purple);

        }
        yield return new WaitForSeconds(2f);
        statusText.gameObject.SetActive(false);
        ResultsPageManager.instance.DisplayResults();
        yield return null;
    }

    public void TriggerNewMatch()
    {
        //photonView.RPC(nameof(RPC_LoadNewMatch), RpcTarget.All);
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(1);
        }

    }
    public void QuitSelected()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel(0);
    }

    [PunRPC]
    public void RPC_LoadNewMatch()
    {
        SceneManager.LoadScene(1);
    }
    #endregion

    #region PunCallbacks
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        MakeDigiPlayerDictionary();
    }
    #endregion
}
