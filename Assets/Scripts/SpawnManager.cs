using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class SpawnManager : MonoBehaviourPunCallbacks
{
    public static SpawnManager instance;

    public Transform[] greenSpawns;
    public Transform[] purpleSpawns;

    private void Awake()
    {
        instance = this;
    }

    public void SetAllPlayerSpawns()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            int greenSpawnIndex = 0;
            int purpleSpawnIndex = 0;

            foreach (Player player in PhotonNetwork.PlayerList)
            {
                string playerTeam = (string)player.CustomProperties[Constants.TEAM_KEY];
                Debug.Log($"The {player} is on the {playerTeam} for moving it's spawn");
                if (playerTeam == Constants.GREEN_TEAM)
                {
                    DigiGameManager.instance.GetDigiPlayer(player.ActorNumber).MoveToGreenSpot(greenSpawnIndex);
                    greenSpawnIndex++;
                }
                else if (playerTeam == Constants.PURPLE_TEAM)
                {
                    DigiGameManager.instance.GetDigiPlayer(player.ActorNumber).MoveToPurpleSpot(purpleSpawnIndex);
                    purpleSpawnIndex++;
                }
            }
        }
    }

}
