using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DigiBullet : MonoBehaviour
{
    public GameObject sphereGameObject;
    public ParticleSystem projectileParticleSystem;
    public ParticleSystem collisionParticleSystem;

    public LayerMask whoDoIHit;
    public TrailRenderer trailRenderer;
    public bool demoMode;

    SphereCollider mySphereCollider;
    Rigidbody myRigidBody;
    bool collisionAnimationPlaying = false;

    //set by bullet data setup
    public string ownerTeam;
    public DigiPlayer bulletOwner;

    void Awake()
    {
        mySphereCollider = GetComponent<SphereCollider>();
        myRigidBody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        if (demoMode)
        {
           myRigidBody.velocity = new Vector3(-5, 0, -5);
        }
        else
        {
            Invoke(nameof(DestroyIfUnflagged), 2f);
        }
    }

    public void SetBulletProperties(string shootingTeam, DigiPlayer shootingPlayer, Vector3 velocity)
    {
        ownerTeam = shootingTeam;
        bulletOwner = shootingPlayer;
        myRigidBody.velocity = velocity;
        if (ownerTeam.Equals(Constants.GREEN_TEAM))
        {
            trailRenderer.startColor = Constants.greenTeamColor;
            ParticleSystem.MainModule mainMod = projectileParticleSystem.main;
            mainMod.startColor = Constants.greenTeamColor;
        }
        else
        {
            trailRenderer.startColor = Constants.purpleTeamColor;
            ParticleSystem.MainModule mainMod = projectileParticleSystem.main;
            mainMod.startColor = Constants.purpleTeamColor;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other == null) { return; }
        GameObject victimeGameObject = other.gameObject;
        //Debug.Log($"Hit: {victimeGameObject.name} Layer Mask: {LayerMask.LayerToName(victimeGameObject.layer)}");
        if (victimeGameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            myRigidBody.velocity = Vector3.zero; //put here in an attempt to immediately stop the object.
            StartCoroutine(CollisionAnimation());
        }
        else if (victimeGameObject.layer == LayerMask.NameToLayer("Player"))
        {
            DigiPlayer playerVictim = victimeGameObject.GetComponent<DigiPlayer>();
            if (playerVictim == null) { bulletOwner.playerScore.IncreaseScore(1); return; } //this is for fake wall, should not actually happen
            string team = DigiGameManager.instance.GetPlayerTeam(playerVictim.photonView.ControllerActorNr);
            if (!ownerTeam.Equals(team)) //owner team of the bullet is different than player that got hit
            {
                myRigidBody.velocity = Vector3.zero;
                if (bulletOwner.photonView.IsMine)
                {
                    bulletOwner.playerScore.IncreaseScore(1);
                    bulletOwner.EnemyHit();
                }
                StartCoroutine(CollisionAnimation());
            }
        }
    }

    IEnumerator CollisionAnimation()
    {
        collisionAnimationPlaying = true;
        projectileParticleSystem.Stop();
        trailRenderer.enabled = false;
        sphereGameObject.SetActive(false);
        ParticleSystem.MainModule mainMod = projectileParticleSystem.main;
        mainMod.startColor = Constants.GetTeamColor(ownerTeam);
        collisionParticleSystem.Play();
        yield return new WaitForSeconds(.15f);
        Destroy(this.gameObject);
    }

    void DestroyIfUnflagged()
    {
        if (collisionAnimationPlaying) { return; } //already flagged to destroy so Coroutine will handle it.
        Destroy(this.gameObject);
    }
}
