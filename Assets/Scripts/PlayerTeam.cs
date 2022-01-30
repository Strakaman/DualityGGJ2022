﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;

public class PlayerTeam : MonoBehaviourPunCallbacks
{
    public bool canSwitchTeams = false;
    public Coroutine cooldownCoroutine = null;
    public int cooldownTime = 5;

    private void Start()
    {
        if (photonView.IsMine)
        {
            canSwitchTeams = true;
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
        canSwitchTeams = false;
        string currentTeam = (string)PhotonNetwork.LocalPlayer.CustomProperties[Constants.TEAM_KEY];
        string newTeam = string.Empty;
        if (currentTeam == Constants.GREEN_TEAM)
        {
            Hashtable newTeamToSet = new Hashtable { { Constants.TEAM_KEY, Constants.PURPLE_TEAM } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(newTeamToSet);
            newTeam = Constants.PURPLE_TEAM;
            HUDManager.instance.ChangeTeamColor(Constants.purpleTeamColor);
        }
        else
        {
            Hashtable newTeamToSet = new Hashtable { { Constants.TEAM_KEY, Constants.GREEN_TEAM } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(newTeamToSet);
            newTeam = Constants.GREEN_TEAM;
            HUDManager.instance.ChangeTeamColor(Constants.greenTeamColor);
        }

        Debug.Log($"{PhotonNetwork.LocalPlayer.NickName} Team has been set from {currentTeam} to {newTeam}");
        cooldownCoroutine = StartCoroutine(Cooldown(cooldownTime));
    }

    void DebugPrintAllPlayerTeams()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            Debug.Log($"{ player.NickName} is on the {player.CustomProperties[Constants.TEAM_KEY]}");
        }
    }

    IEnumerator Cooldown(int time)
    {
        int timeLeft = time;
        HUDManager.instance.cooldownLayer.SetActive(true);
        while (timeLeft > 0)
        {
            HUDManager.instance.ChangeCooldownText(timeLeft);
            yield return new WaitForSeconds(1);
            timeLeft -= 1;
        }

        HUDManager.instance.cooldownLayer.SetActive(false);
        canSwitchTeams = true;
    }
}
