﻿using UnityEngine;
using Photon.Pun;

public class PlayerGun : MonoBehaviourPunCallbacks
{
    public DigiBullet bulletPrefab;
    public int bulletTimeJKSpeed;
    public Transform muzzleTransform;
    public ParticleSystem gunParticle;
    public PlayerScore playerScore;
    public DigiPlayer owningPlayer;
    int playerLayer = 11;

    public int scoreIncreaseBase = 1;
    public int scoreIncreaseMultiplier = 2;
    DigiPlayerAnimation animatorScript;

    void Start()
    {
        if (playerScore == null)
        {
            playerScore = GetComponent<PlayerScore>();
        }
        animatorScript = GetComponent<DigiPlayerAnimation>();
    }

    public void setOwningPlayer(DigiPlayer digiPlayer)
    {
        owningPlayer = digiPlayer;
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            if (InputManager.instance.fireStarted)
            {
                photonView.RPC(nameof(RPC_Shoot), RpcTarget.All);
            }
            else
            {
                animatorScript.isShooting = false;
            }
        }
    }

    [PunRPC]
    void RPC_Shoot()
    {
        /*gunParticle.Play();
        animatorScript.isShooting = true;
        Ray ray = new Ray(muzzleTransform.position, muzzleTransform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            GameObject go = hit.collider.gameObject;
            Debug.Log($"Hit: {go.name} Layer Mask: {go.layer}");
            playerScore.IncreaseScore(scoreIncreaseBase * scoreIncreaseMultiplier);
        }*/
        DigiBullet bullet = Instantiate<DigiBullet>(bulletPrefab, muzzleTransform.position, Quaternion.identity);
        Vector3 bulletVelocity = muzzleTransform.forward * bulletTimeJKSpeed;
        bullet.SetBulletProperties(DigiGameManager.instance.GetPlayerTeam(photonView.ViewID), owningPlayer, bulletVelocity);
        animatorScript.isShooting = true;

    }
}
