using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;

public class DigiPlayer : MonoBehaviourPunCallbacks
{
    public PlayerGun playerGun;
    public CharacterController charController;
    public FirstPersonController firstpersonController;
    public PlayerScore playerScore;
    public DigiPlayerAnimation playerAnimation;
    public PlayerTeam playerTeam;

    private void Awake()
    {
        playerGun.setOwningPlayer(this);
    }

    public void PushMatchStats()
    {

            Debug.Log("Here are the game stats that I pushed");

    }
}
