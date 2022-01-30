using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultsTemplate : MonoBehaviour
{
    public Text playerName;
    public Text score;
    public Text numSwitches;
    public Text shotsLanded;
    public Text timeGreen;
    public Text timePurple;

    public void SetStuff(string pN, string s, string nS, string sL, string tG, string tP)
    {
        playerName.text = pN;
        score.text = s;
        numSwitches.text = nS;
        shotsLanded.text = sL;
        timeGreen.text = tG;
        timePurple.text = tP;
    }
}
