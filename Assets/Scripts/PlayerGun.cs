﻿using UnityEngine;
using Photon.Pun;

public class PlayerGun : MonoBehaviourPunCallbacks
{
    public Transform muzzleTransform;
    public ParticleSystem gunParticle;
    int playerLayer = 11;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Debug.Log("hit");
                photonView.RPC(nameof(RPC_Shoot), RpcTarget.All);
            }
        }
    }

    [PunRPC]
    void RPC_Shoot()
    {
        gunParticle.Play();
        Ray ray = new Ray(muzzleTransform.position, muzzleTransform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            GameObject go = hit.collider.gameObject;
            Debug.Log($"Hit: {go.name} Layer Mask: {go.layer}");
        }
    }
}
