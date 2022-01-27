using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class LobbyPlayerList : MonoBehaviour
{
    public List<GameObject> playerLobbyGridObjects;

    // Start is called before the first frame update
    public void BuildPlayerListGUI(Player[] playerList)
    {
        DisablePlayerListGUI();
        Debug.Log("Building player List GUI");
        for (int i = 0; i < playerList.Length; i++)
        {
            Debug.Log($"This is a player: {playerList[i].NickName}");
            playerLobbyGridObjects[i].SetActive(true);
            playerLobbyGridObjects[i].GetComponentInChildren<Text>().text = playerList[i].NickName;
        }
    }

    public void DisablePlayerListGUI()
    {
        foreach (GameObject go in playerLobbyGridObjects)
        {
            go.SetActive(false);
        }
    }
}
