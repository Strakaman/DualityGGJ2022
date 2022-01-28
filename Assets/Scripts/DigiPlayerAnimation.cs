using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DigiPlayerAnimation : MonoBehaviour
{
    [SerializeField]
    Animator myAnimator;

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

    void Update()
    {
        myAnimator.SetBool(nameof(isWalking), isWalking);
        myAnimator.SetBool(nameof(isDancing), isDancing);
        myAnimator.SetBool(nameof(isShooting), isShooting);

        /*if (isShooting)
        {
            myAnimator.SetTrigger(nameof(isShooting));
            isShooting = false;
        }*/
    }
}
