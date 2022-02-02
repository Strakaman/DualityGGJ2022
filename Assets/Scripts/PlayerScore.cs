using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerScore : MonoBehaviourPunCallbacks, IPunObservable
{
    public int score;
    // Start is called before the first frame update
    void Awake()
    {
        Hashtable scoreToSet = new Hashtable { { Constants.SCORE_KEY, 0 } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(scoreToSet);
    }

    // Update is called once per frame
    void Update()
    {
        //if (photonView.IsMine)
        //{
        //    if (Keyboard.current.cKey.isPressed)
        //    {
        //        IncreaseScore(1);
        //    }
        //}
    }

    public void IncreaseScore(int howMuch)
    {
        if (!DigiGameManager.instance.gameStarted || DigiGameManager.instance.gameOver) { return; }
        int currentScore = (int)PhotonNetwork.LocalPlayer.CustomProperties[Constants.SCORE_KEY];
        currentScore += howMuch * GetTeamMultiplier((string)PhotonNetwork.LocalPlayer.CustomProperties[Constants.TEAM_KEY]);
        Hashtable scoreToSet = new Hashtable { { Constants.SCORE_KEY, currentScore } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(scoreToSet);
    }

    private int GetTeamMultiplier(string team)
    {
        int multiplier = 0;
        if (team == Constants.GREEN_TEAM)
        {
            multiplier = DigiTeamsManager.instance.greenMultiplier;
        }
        else if (team == Constants.PURPLE_TEAM)
        {
            multiplier = DigiTeamsManager.instance.purpleMultiplier;
        }

        return multiplier;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(score);
        }
        else
        {
            score = (int)stream.ReceiveNext();
        }
    }
}