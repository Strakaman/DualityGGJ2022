using UnityEngine;
using Photon.Pun;

public class PlayerGun : MonoBehaviourPunCallbacks
{
    public Transform muzzleTransform;
    public ParticleSystem gunParticle;
    public PlayerScore playerScore;
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

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            Debug.Log("update gun hit");
            if (InputManager.instance.fireStarted)
            {
                Debug.Log("fire started");
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
        gunParticle.Play();
        animatorScript.isShooting = true;
        Ray ray = new Ray(muzzleTransform.position, muzzleTransform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            GameObject go = hit.collider.gameObject;
            Debug.Log($"Hit: {go.name} Layer Mask: {go.layer}");
            playerScore.IncreaseScore(scoreIncreaseBase * scoreIncreaseMultiplier);
        }
    }
}
