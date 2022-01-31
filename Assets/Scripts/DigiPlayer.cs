using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    public Image playerIndicator;

    private int enemiesShot;
    private void Awake()
    {
        playerGun.setOwningPlayer(this);
    }

    private void Start()
    {
        enemiesShot = 0;
        if (!photonView.IsMine)
        {
            playerIndicator.enabled = false;
        }
        if (photonView.IsMine)
        {
            CameraFollow.instance.TellCameraToFollowMe(transform);
        }
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

    public void MoveToGreenSpot(int spawnIndex)
    {
        photonView.RPC(nameof(MoveGreenTransform), RpcTarget.All, spawnIndex);
    }

    public void MoveToPurpleSpot(int spawnIndex)
    {
        photonView.RPC(nameof(MovePurpleTransform), RpcTarget.All, spawnIndex);
    }

    [PunRPC]
    void MoveGreenTransform(int spawnIndex)
    {
        gameObject.GetComponent<CharacterController>().enabled = false;
        Debug.Log($"{PhotonNetwork.LocalPlayer.NickName} is at {gameObject.transform.position} and going to {SpawnManager.instance.greenSpawns[spawnIndex].position}");
        gameObject.transform.position = SpawnManager.instance.greenSpawns[spawnIndex].position;
        Debug.Log($"{PhotonNetwork.LocalPlayer.NickName} is at {gameObject.transform.position}");
        gameObject.GetComponent<CharacterController>().enabled = true;
    }

    [PunRPC]
    void MovePurpleTransform(int spawnIndex)
    {
        gameObject.GetComponent<CharacterController>().enabled = false;
        Debug.Log($"{PhotonNetwork.LocalPlayer.NickName} is at {gameObject.transform.position} and going to {SpawnManager.instance.purpleSpawns[spawnIndex].position}");
        gameObject.transform.position = SpawnManager.instance.purpleSpawns[spawnIndex].position;
        Debug.Log($"{PhotonNetwork.LocalPlayer.NickName} is at {gameObject.transform.position}");
        gameObject.GetComponent<CharacterController>().enabled = true;
    }
}
