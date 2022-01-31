using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;

public class PlayerTeam : MonoBehaviourPunCallbacks
{
    public bool switchOnCooldown = true;
    public Coroutine cooldownCoroutine = null;
    public int cooldownTime = 5;
    public Image teamIndicator;
    public SkinnedMeshRenderer playerSkinRenderer;
    public float timeSinceLastSwitch { get; private set; }
    public int amountSwitched { get; private set; }
    public float timeOnGreenTeam { get; private set; }
    public float timeOnPurpleTeam { get; private set; }

    public int minNumOfPlayersForTeam { get; private set; }

    private void Start()
    {
        timeSinceLastSwitch = 0f;
        amountSwitched = 0;
        timeOnGreenTeam = 0f;
        timeOnPurpleTeam = 0f;
        minNumOfPlayersForTeam = 1;
        if (photonView.IsMine)
        {
            switchOnCooldown = false;
            /*int random = Random.Range(0, 2);
            string[] teams = new string[] { Constants.GREEN_TEAM, Constants.PURPLE_TEAM };
            Color[] colors = new Color[] { Constants.greenTeamColor, Constants.purpleTeamColor };
            Hashtable teamToSet = new Hashtable { { Constants.TEAM_KEY, teams[random] } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(teamToSet);
            HUDManager.instance.ChangeTeamColor(colors[random]);
            photonView.RPC("ChangeTeamIndicatorColor", RpcTarget.All, teams[random]);
            Debug.Log($"{PhotonNetwork.LocalPlayer.NickName} is on the {teams[random]}");*/
        }
    }
    private void Update()
    {
        if (DigiGameManager.instance.gameStarted && !DigiGameManager.instance.gameOver)
        {
            timeSinceLastSwitch += Time.deltaTime;
        }
        if (photonView.IsMine)
        {
            if (!DigiGameManager.instance.gameStarted || DigiGameManager.instance.gameOver)
            {
                return;
            }
            if (CanSwitchTeams())
            {
                if (InputManager.instance.switchTeamsStarted)
                {

                    SwitchTeams();
                }
            }
            else if (!switchOnCooldown)
            {
                UpdateSwitchTeamsHUD(false, "");
            }
            //DebugPrintAllPlayerTeams();
        }
    }
    public void SwitchTeams()
    {
        switchOnCooldown = true;
        string currentTeam = (string)PhotonNetwork.LocalPlayer.CustomProperties[Constants.TEAM_KEY];
        string newTeam = string.Empty;
        if (currentTeam == Constants.GREEN_TEAM)
        {
            Hashtable newTeamToSet = new Hashtable { { Constants.TEAM_KEY, Constants.PURPLE_TEAM } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(newTeamToSet);
            newTeam = Constants.PURPLE_TEAM;
            HUDManager.instance.ChangeTeamColor(Constants.purpleTeamColor);
            photonView.RPC("ChangeTeamIndicatorColor", RpcTarget.All, Constants.PURPLE_TEAM);
            timeOnGreenTeam += timeSinceLastSwitch;
        }
        else
        {
            Hashtable newTeamToSet = new Hashtable { { Constants.TEAM_KEY, Constants.GREEN_TEAM } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(newTeamToSet);
            newTeam = Constants.GREEN_TEAM;
            HUDManager.instance.ChangeTeamColor(Constants.greenTeamColor);
            photonView.RPC("ChangeTeamIndicatorColor", RpcTarget.All, Constants.GREEN_TEAM);
            timeOnPurpleTeam += timeSinceLastSwitch;
        }
        timeSinceLastSwitch = 0f;
        amountSwitched++;
        Debug.Log($"{PhotonNetwork.LocalPlayer.NickName} Team has been set from {currentTeam} to {newTeam}");
        cooldownCoroutine = StartCoroutine(Cooldown(cooldownTime));
     
    }

    /// <summary>
    /// Because we only update the time on the team during switching, when the match ends, the time
    /// you spent on your last color was not counted. This method gets that last time period and adds it
    /// to whatever team you were on at the end of the match
    /// </summary>
    public void UpdateLastTeamDuration()
    {
        string currentTeam = (string)PhotonNetwork.LocalPlayer.CustomProperties[Constants.TEAM_KEY];
        if (currentTeam == Constants.GREEN_TEAM)
        {
            timeOnGreenTeam += timeSinceLastSwitch;
        }
        else
        {
            timeOnPurpleTeam += timeSinceLastSwitch;
        }    
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
        UpdateSwitchTeamsHUD(false, timeLeft.ToString());
        while (timeLeft > 0)
        {
            UpdateSwitchTeamsHUD(false, timeLeft.ToString());
            yield return new WaitForSeconds(1);
            timeLeft -= 1;
        }

        UpdateSwitchTeamsHUD(true, "");
        switchOnCooldown = false;
    }

    public void UpdateSwitchTeamsHUD(bool canSwitch, string text)
    {
        HUDManager.instance.cooldownLayer.SetActive(!canSwitch);
        HUDManager.instance.ChangeCooldownText(text);
    }
    public bool CanSwitchTeams()
    {
        if (switchOnCooldown) { return false; }
        if (!MinSwitchCriteriaMet()) { return false; }
        return true;
    }

    public bool MinSwitchCriteriaMet()
    {
        string teamName = DigiGameManager.instance.GetPlayerTeam(photonView.ControllerActorNr);
        int numOfPplOnTeam = 0;
        if (PhotonNetwork.PlayerList.Length == 1) { return true; } //for solo testing 
        foreach(Player p in PhotonNetwork.PlayerList)
        {
            if (p.CustomProperties[Constants.TEAM_KEY].Equals(teamName));
            {
                numOfPplOnTeam++;
            }
        }
        if (numOfPplOnTeam > minNumOfPlayersForTeam)
        {
            return true;
        }
        return false;
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {

        if (changedProps.ContainsKey(Constants.TEAM_KEY))
        {
            string newTeam = (string)changedProps[Constants.TEAM_KEY];
            if (targetPlayer.ActorNumber == photonView.ControllerActorNr) //means its mine
            {
                HUDManager.instance.ChangeTeamColor(Constants.GetTeamColor(newTeam));
                photonView.RPC("ChangeTeamIndicatorColor", RpcTarget.All, newTeam);
            }
            //ChangeTeamIndicatorColor(newTeam);
        }

    }
    [PunRPC]
    void ChangeTeamIndicatorColor(string teamName)
    {
        if (teamName == Constants.GREEN_TEAM)
        {
            teamIndicator.color = Constants.greenTeamColor;
            playerSkinRenderer.materials[0].color = Constants.greenTeamColor;
        }
        else if (teamName == Constants.PURPLE_TEAM)
        {
            teamIndicator.color = Constants.purpleTeamColor;
            playerSkinRenderer.materials[0].color = Constants.purpleTeamColor;
        }
    }
}
