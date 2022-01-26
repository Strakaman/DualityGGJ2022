using UnityEngine;
using Photon.Pun;

public class DigiGameManager : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;
    void Start()
    {
        Vector3 pos = new Vector3(Random.Range(1, 5), 1.25f, Random.Range(1, 5));
        PhotonNetwork.Instantiate(playerPrefab.name, pos, Quaternion.identity);   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
