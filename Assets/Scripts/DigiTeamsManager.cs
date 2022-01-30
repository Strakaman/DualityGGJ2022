using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class DigiTeamsManager : MonoBehaviourPunCallbacks
{
    public static DigiTeamsManager instance;

    public int greenMultiplier = 1;
    public int purpleMultiplier = 1;

    private void Awake()
    {
        instance = this;    
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey(Constants.TEAM_KEY))
        {
            UpdateTeamMultipliers();
        }
    }

    public int GetTeamScore(string team)
    {
        int totalScore = 0;
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            string playerTeam = (string)player.CustomProperties[Constants.TEAM_KEY];
            if (playerTeam == team)
            {
                totalScore += (int)player.CustomProperties[Constants.SCORE_KEY];
            }
        }

        return totalScore;
    }

    //temporary function for multipliers
    public void UpdateTeamMultipliers()
    {
        int playersOnGreen = 0;
        int playersOnPurple = 0;
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if ((string)player.CustomProperties[Constants.TEAM_KEY] == Constants.GREEN_TEAM)
            {
                playersOnGreen++;
            }
            else if ((string)player.CustomProperties[Constants.TEAM_KEY] == Constants.PURPLE_TEAM)
            {
                playersOnPurple++;
            }
        }

        //reversing the players on team value is current multiplier structure
        greenMultiplier = playersOnPurple;
        purpleMultiplier = playersOnGreen;
    }
}
