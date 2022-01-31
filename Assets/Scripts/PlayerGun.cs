using UnityEngine;
using Photon.Pun;

public class PlayerGun : MonoBehaviourPunCallbacks
{
    public DigiBullet bulletPrefab;
    public int bulletTimeJKSpeed;
    public Transform muzzleTransform;
    public ParticleSystem gunParticle;
    public PlayerScore playerScore;
    public DigiPlayer owningPlayer;

    public int scoreIncreaseBase = 1;
    public int scoreIncreaseMultiplier = 2;
    DigiPlayerAnimation animatorScript;
    public float shootingAnimationCooldown = .3f;
    float shootingAnimationResetDuration;

    void Start()
    {
        shootingAnimationResetDuration = 0f;
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
            if (!DigiGameManager.instance.gameStarted || DigiGameManager.instance.gameOver) { return; }
            if (InputManager.instance.fireStarted)
            {
                photonView.RPC(nameof(RPC_Shoot), RpcTarget.All);
            }
            shootingAnimationResetDuration += Time.deltaTime;
            if (shootingAnimationResetDuration > shootingAnimationCooldown)
            {
                RefreshShootingAnimationCooldown();
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
        bullet.SetBulletProperties(DigiGameManager.instance.GetPlayerTeam(photonView.ControllerActorNr), owningPlayer, bulletVelocity);
        animatorScript.isShooting = true;
        RefreshShootingAnimationCooldown();
    }

    void RefreshShootingAnimationCooldown()
    {
        shootingAnimationResetDuration = 0;
    }
}
