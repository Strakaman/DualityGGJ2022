using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class DigiPlayer : MonoBehaviourPunCallbacks
{
    public PlayerGun playerGun;
    public CharacterController charController;
    public FirstPersonController firstpersonController;
    public PlayerScore playerScore;
    public DigiPlayerAnimation playerAnimation;
    public PlayerTeam playerTeam;

    private int enemiesShot;
    private void Awake()
    {
        playerGun.setOwningPlayer(this);
    }

    private void Start()
    {
        enemiesShot = 0;
    }

    public void EnemyHit()
    {
        enemiesShot++;
    }

    public void PushMatchStats()
    {
        playerTeam.UpdateLastTeamDuration(); //update the time spent on whatever your last team was.
        Hashtable matchStats = new Hashtable { 
            { Constants.ENEMIES_SHOT_KEY, enemiesShot },
            { Constants.NUM_SWITCHES_KEY, playerTeam.amountSwitched },
            { Constants.TIME_GREEN_KEY, Mathf.Round(playerTeam.timeOnGreenTeam) },
            { Constants.TIME_PURPLE_KEY, Mathf.Round(playerTeam.timeOnPurpleTeam) }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(matchStats);



        Debug.Log("Here are the game stats that I pushed");

    }
}
