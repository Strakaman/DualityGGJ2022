using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
using Photon.Realtime;
using System.Collections;
using UnityEngine.UI;

public class DigiGameManager : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;
    public Text statusText;
    public Dictionary<int, DigiPlayer> playerDictionary;
    public static DigiGameManager instance;

    int matchTimeinSeconds = 5;
    public int timeLeft { get; private set; }
    public string WinningTeam { get; private set; }
    public bool timeUp = false;
    public bool gameStarted = false;
    public bool gameOver = false;
    public bool initFinished = false;
    private void Awake()
    {
        instance = this;
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
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        MakeDigiPlayerDictionary();
    }

    #region PlayerRefs
    public void MakeDigiPlayerDictionary()
    {
        playerDictionary = new Dictionary<int, DigiPlayer>();
        DigiPlayer[] digiPlayers = FindObjectsOfType<DigiPlayer>();
        foreach (DigiPlayer dp in digiPlayers)
        {
            playerDictionary.Add(dp.photonView.ViewID, dp);
            /*Debug.Log($"Photon View ID: {dp.photonView.ViewID} " +
                $"Creator Actor Number: {dp.photonView.CreatorActorNr} " +
                $"Controller Actor Number: {dp.photonView.ControllerActorNr} " +
                $"Owner Actor Number: {dp.photonView.OwnerActorNr}");*/
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
    #endregion

    // Update is called once per frame
    #region GameLoop
    void InitMatch()
    {
        initFinished = false;
        gameStarted = false;
        timeUp = false;
        gameOver = false;
        timeLeft = matchTimeinSeconds;
        StartCoroutine(GameInit());

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
        photonView.RPC(nameof(RPC_SetWinningTeam), RpcTarget.All, resultingWinner);
    }

    [PunRPC]
    void RPC_SetWinningTeam(string winningTeamResult)
    {
        WinningTeam = winningTeamResult;
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
        HUDManager.instance.UpdateTimeLeft();
        while (gameStarted && timeLeft > 0)
        {
            yield return new WaitForSeconds(1);
            timeLeft -= 1;
            HUDManager.instance.UpdateTimeLeft();
        }
        timeUp = true;
    }

    IEnumerator GameOverShenanigans()
    {
        statusText.text = "Time's Up!";
        AudioManager.instance.PlaySound2D(Constants.Sound_TimeUp);
        statusText.color = Color.white;
        statusText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        statusText.text = "The winner is...";
        AudioManager.instance.PlaySound2D(Constants.Sound_Winner);
        statusText.color = Color.white;
        yield return new WaitForSeconds(2f);
        if (WinningTeam.Equals(Constants.GREEN_TEAM))
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
        yield return null;
    }
    #endregion
}
