using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class HUDManager : MonoBehaviour
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

    private void Update()
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
        //get green modifier
        //get purple modifier
        //set text for both
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
        //update player multiplier
    }

    public void ChangeTeamColor(Color newColor)
    {
        playerTeamColor.color = newColor;
    }
}
