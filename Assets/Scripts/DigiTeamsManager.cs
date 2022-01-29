using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class DigiTeamsManager : MonoBehaviour
{
    public static DigiTeamsManager instance;

    private void Awake()
    {
        instance = this;    
    }

    public int GetTeamScore(string team)
    {
        int totalScore = 0;
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if ((string)player.CustomProperties[Constants.TEAM_KEY] == team)
            {
                totalScore += (int)player.CustomProperties[Constants.SCORE_KEY];
            }
        }

        return totalScore;
    }

}
