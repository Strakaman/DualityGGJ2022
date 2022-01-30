using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DigiBullet : MonoBehaviour
{
    // Start is called before the first frame update

    public new ParticleSystem particleSystem;
    public LayerMask whoDoIHit;
    public TrailRenderer trailRenderer;
    public new Rigidbody rigidbody;
    public bool demoMode;


    //set by bullet data setup
    public string ownerTeam;
    public DigiPlayer bulletOwner;
    void Start()
    {
        if (demoMode)
        {
            GetComponent<Rigidbody>().velocity = new Vector3(-5, 0, -5);
        }
        else
        {
            Destroy(this.gameObject, 2f);
        }
    }

    public void SetBulletProperties(string shootingTeam, DigiPlayer shootingPlayer, Vector3 velocity)
    {
        ownerTeam = shootingTeam;
        bulletOwner = shootingPlayer;
        rigidbody.velocity = velocity;
        if (ownerTeam.Equals(Constants.GREEN_TEAM))
        {
            trailRenderer.startColor = Constants.greenTeamColor;
            ParticleSystem.MainModule mainMod = particleSystem.main;
            mainMod.startColor = Constants.greenTeamColor;
        }
        else
        {
            trailRenderer.startColor = Constants.purpleTeamColor;
            ParticleSystem.MainModule mainMod = particleSystem.main;
            mainMod.startColor = Constants.purpleTeamColor;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other == null) { return; }
        GameObject victimeGameObject = other.gameObject;
        Debug.Log($"Hit: {victimeGameObject.name} Layer Mask: {LayerMask.LayerToName(victimeGameObject.layer)}");
        if (victimeGameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            Destroy(this.gameObject);
        }
        else if (victimeGameObject.layer == LayerMask.NameToLayer("Player"))
        {
            DigiPlayer playerVictim = victimeGameObject.GetComponent<DigiPlayer>();
            if (playerVictim == null) { bulletOwner.playerScore.IncreaseScore(1); return; } //this is for fake wall, should not actually happen
            string team = DigiGameManager.instance.GetPlayerTeam(playerVictim.photonView.ControllerActorNr);
            if (!ownerTeam.Equals(team)) //owner team of the bullet is different than player that got hit
            {
                if (bulletOwner.photonView.IsMine)
                {
                    bulletOwner.playerScore.IncreaseScore(1);
                    bulletOwner.EnemyHit();
                }
                Destroy(this.gameObject);
            }
        }
    }
}
