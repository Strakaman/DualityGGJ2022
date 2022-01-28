using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DigiPlayerAnimation : MonoBehaviourPunCallbacks
{
    [SerializeField]
    Animator myAnimator;
    public GameObject[] characterHeads;
    public int characterHeadSelectionIndex;

    public bool isWalking;
    public bool isShooting;
    public bool isDancing;
    // Start is called before the first frame update
    void Awake()
    {
        isWalking = false;
        isShooting = false;
        isDancing = false;
        myAnimator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        if (photonView.IsMine)
        {
            int characterSelection = PlayerPrefs.GetInt(Constants.CharacterHead, 0);
            //all buffered meaning players that join after will get this call also, no effect now
            photonView.RPC(nameof(RPC_SetCharacterHead), RpcTarget.AllBuffered,characterSelection);
        }
    }

    void Update()
    {
        if (photonView.IsMine){
            myAnimator.SetBool(nameof(isWalking), isWalking);
            myAnimator.SetBool(nameof(isDancing), isDancing);
            myAnimator.SetBool(nameof(isShooting), isShooting);
        }

    }

    [PunRPC]
    void RPC_SetCharacterHead(int characterSelection)
    {
        if (characterSelection < 0 || characterSelection > characterHeads.Length - 1 || 
            characterHeads[characterSelection] == null)
        {
            characterHeadSelectionIndex = 0;
        }
        else
        {
            characterHeadSelectionIndex = characterSelection;
        }
        characterHeads[characterHeadSelectionIndex].SetActive(true);
    }
}
