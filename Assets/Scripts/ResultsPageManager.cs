using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System;

public class ResultsPageManager : MonoBehaviour
{
    public static ResultsPageManager instance;
    public ResultsTemplate resultsTemplatePrefab;
    public RectTransform greenGridHolder;
    public RectTransform purpleGridHolder;
    public Button rematchButton;
    public GameObject HUDContainer;
    public GameObject resultsGUIContainer;
    
    void Start()
    {
        instance = this;
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            rematchButton.interactable = true;
        }
        else
        {
            rematchButton.interactable = false;
        }
    }
    public void DisplayResults()
    {
        resultsGUIContainer.SetActive(true);
        HUDContainer.SetActive(false);
        foreach(Player p in PhotonNetwork.PlayerList)
        {
            string name = p.NickName;
            string score = ((int)p.CustomProperties[Constants.SCORE_KEY]).ToString();
            string switches = ((int)p.CustomProperties[Constants.NUM_SWITCHES_KEY]).ToString();
            string shots = ((int)p.CustomProperties[Constants.ENEMIES_SHOT_KEY]).ToString();
            float timeGreenFloat = (float)p.CustomProperties[Constants.TIME_GREEN_KEY];
            TimeSpan timeGreenSpan = TimeSpan.FromSeconds(timeGreenFloat);
            string timeGreen = timeGreenSpan.ToString(@"mm\:ss");
            float timePurpleFloat = (float)p.CustomProperties[Constants.TIME_PURPLE_KEY];
            TimeSpan timePurpleSpan = TimeSpan.FromSeconds(timePurpleFloat);
            string timePurple = timePurpleSpan.ToString(@"mm\:ss");
            ResultsTemplate rt = Instantiate<ResultsTemplate>(resultsTemplatePrefab,Vector3.zero,Quaternion.identity);
            rt.SetStuff(name, score, switches, shots, timeGreen, timePurple);
            if ((string)p.CustomProperties[Constants.TEAM_KEY] == Constants.GREEN_TEAM)
            {
                //rt.transform.parent = greenGridHolder;
                rt.transform.SetParent(greenGridHolder);
                rt.transform.localScale = greenGridHolder.localScale;
            }
            else
            {
                //rt.transform.parent = purpleGridHolder;
                rt.transform.SetParent(purpleGridHolder);
                rt.transform.localScale = purpleGridHolder.localScale;
                Debug.Log($"The size delta is {purpleGridHolder.sizeDelta}");
            }
        }
    }

    public void OnClick_Quit()
    {
        //PhotonNetwork.LeaveRoom();
        Application.Quit();
    }

    public void OnClick_PlayAgain()
    {
        SceneManager.LoadScene(1);
    }
}
