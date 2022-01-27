using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

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