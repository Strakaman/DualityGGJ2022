﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DigiPlayerAnimation : MonoBehaviourPunCallbacks
{
    [SerializeField]
    Animator myAnimator;
    public GameObject[] characterHeads;
    public int characterHeadSelectionIndex;
    public bool demoMode = false;
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
        if (demoMode)
        {
            myAnimator = GetComponent<Animator>();
        }
    }

    private void Start()
    {
        if (photonView !=null && photonView.IsMine && !demoMode)
        {
            int characterSelection = PlayerPrefs.GetInt(Constants.CharacterHead, 0);
            //all buffered meaning players that join after will get this call also, no effect now
            photonView.RPC(nameof(RPC_SetCharacterHead), RpcTarget.AllBuffered,characterSelection);
        }
    }

    void Update()
    {
        if (photonView!=null && photonView.IsMine && !demoMode){
            myAnimator.SetBool(nameof(isWalking), isWalking);
            myAnimator.SetBool(nameof(isDancing), isDancing);
            myAnimator.SetBool(nameof(isShooting), isShooting);
        }

    }

    [PunRPC]
    void RPC_SetCharacterHead(int characterSelection)
    {
        Local_SetCharacterHead(characterSelection);
    }

    public void Local_SetCharacterHead(int characterSelection)
    {
        if (characterSelection == (int)Shape.Random)
        {
            characterHeadSelectionIndex = UnityEngine.Random.Range(0, characterHeads.Length);
        }
        else if (characterSelection < 0 || characterSelection > characterHeads.Length - 1)
        {
            characterHeadSelectionIndex = 0;
        }
        else
        {
            characterHeadSelectionIndex = characterSelection;
        }
        foreach (GameObject go in characterHeads)
        {
            go.SetActive(false);
        }
        characterHeads[characterHeadSelectionIndex].SetActive(true);
    }



    Coroutine demoAnimationLoopCo;
    public override void OnEnable()
    {
        if (demoMode)
        {
            Local_SetCharacterHead(PlayerPrefs.GetInt(Constants.CharacterHead, 0));
        }
    }

    public override void OnDisable()
    {
        if (demoMode)
        {
            if (demoAnimationLoopCo != null)
            {
                StopCoroutine(demoAnimationLoopCo);
            }
        }
    }

    public void StartDemoAnimation()
    {
        if (demoMode)
        {
            demoAnimationLoopCo = StartCoroutine(DemoAnimationLoop());
        }
    }

    IEnumerator DemoAnimationLoop()
    {
        Debug.Log("Starting CoLoop");
        while(true)
        {
            myAnimator.Play("Rifle Run");
            isWalking = true;
            yield return new WaitForSeconds(2f);
        }
    }
}
