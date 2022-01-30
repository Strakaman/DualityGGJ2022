using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class HUDManager : MonoBehaviourPunCallbacks
{
    public static HUDManager instance;

    public Text greenTeamScore;
    public Text greenTeamMultiplier;
    public Text purpleTeamScore;
    public Text purpleTeamMultiplier;
    public Text timeLeft;
    public Text playerName;
    public Text playerScore;
    public Text playerMultiplier;
    public Image playerTeamColor;
    public GameObject cooldownLayer;
    public Text cooldownTimeLeft;

    private void Awake()
    {
        instance = this;   
    }

    private void Start()
    {
        playerName.text = PhotonNetwork.LocalPlayer.NickName;
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        UpdateTeamScores();
        UpdateModifiers();
        UpdateTimeLeft();
        UpdatePlayerData();
    }

    void UpdateTeamScores()
    {
        int greenScore = DigiTeamsManager.instance.GetTeamScore(Constants.GREEN_TEAM);
        int purpleScore = DigiTeamsManager.instance.GetTeamScore(Constants.PURPLE_TEAM);
        greenTeamScore.text = greenScore.ToString();
        purpleTeamScore.text = purpleScore.ToString();
    }

    void UpdateModifiers()
    {
        greenTeamMultiplier.text = "x" + DigiTeamsManager.instance.greenMultiplier.ToString();
        purpleTeamMultiplier.text = "x" + DigiTeamsManager.instance.purpleMultiplier.ToString();
    }

    void UpdateTimeLeft()
    {
        //get remaining time
        //set text
    }

    void UpdatePlayerData()
    {
        int score = (int)PhotonNetwork.LocalPlayer.CustomProperties[Constants.SCORE_KEY];
        playerScore.text = score.ToString();

        string team = (string)PhotonNetwork.LocalPlayer.CustomProperties[Constants.TEAM_KEY];
        if (team == Constants.GREEN_TEAM)
        {
            playerMultiplier.text = "x" + DigiTeamsManager.instance.greenMultiplier.ToString();
        }
        else if (team == Constants.PURPLE_TEAM)
        {
            playerMultiplier.text = "x" + DigiTeamsManager.instance.purpleMultiplier.ToString();
        }
        else
        {
            playerMultiplier.text = string.Empty;
        }
    }

    public void ChangeTeamColor(Color newColor)
    {
        playerTeamColor.color = newColor;
    }

    public void ChangeCooldownText(int time)
    {
        cooldownTimeLeft.text = time.ToString();
    }
}
