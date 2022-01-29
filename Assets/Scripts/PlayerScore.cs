﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerScore : MonoBehaviourPunCallbacks, IPunObservable
{
    public int score;
    public TextMesh scoreDisplay;
    // Start is called before the first frame update
    void Start()
    {
        score = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            if (Keyboard.current.cKey.isPressed)
            {
                IncreaseScore(1);
            }
        }
        scoreDisplay.text = $"Score: {score}";
    }

    public void IncreaseScore(int howMuch)
    {
        score += howMuch;
        //int currentScore = (int)PhotonNetwork.LocalPlayer.CustomProperties[Constants.SCORE_KEY];
        //currentScore += howMuch;
        //Hashtable scoreToSet = new Hashtable { { Constants.SCORE_KEY, currentScore } };
        //PhotonNetwork.LocalPlayer.SetCustomProperties(scoreToSet);
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