using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;

public class PlayerTeam : MonoBehaviourPunCallbacks
{
    public bool canSwitchTeams = false;
    public Coroutine cooldownTimer = null;

    private void Start()
    {
        if (photonView.IsMine)
        {
            int random = Random.Range(0, 2);
            string[] teams = new string[] { Constants.GREEN_TEAM, Constants.PURPLE_TEAM };
            Hashtable teamToSet = new Hashtable { { Constants.TEAM_KEY, teams[random] } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(teamToSet);
            Debug.Log($"{PhotonNetwork.LocalPlayer.NickName} is on the {teams[random]}");
        }
    }
    private void Update()
    {
        if (photonView.IsMine)
        {
            if (InputManager.instance.switchTeamsStarted)
            {
                if (canSwitchTeams)
                {
                    SwitchTeams();
                }
            }

            //DebugPrintAllPlayerTeams();
        }
    }
    public void SwitchTeams()
    {
        string currentTeam = (string)PhotonNetwork.LocalPlayer.CustomProperties[Constants.TEAM_KEY];
        string newTeam = string.Empty;
        if (currentTeam == Constants.GREEN_TEAM)
        {
            Hashtable newTeamToSet = new Hashtable { { Constants.TEAM_KEY, Constants.PURPLE_TEAM } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(newTeamToSet);
            newTeam = Constants.PURPLE_TEAM;
        }
        else
        {
            Hashtable newTeamToSet = new Hashtable { { Constants.TEAM_KEY, Constants.GREEN_TEAM } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(newTeamToSet);
            newTeam = Constants.GREEN_TEAM;
        }

        Debug.Log($"{PhotonNetwork.LocalPlayer.NickName} Team has been set from {currentTeam} to {newTeam}");
    }

    void DebugPrintAllPlayerTeams()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            Debug.Log($"{ player.NickName} is on the {player.CustomProperties[Constants.TEAM_KEY]}");
        }
    }
}
