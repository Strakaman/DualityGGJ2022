using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CharacterSelect : MonoBehaviour
{
    // Start is called before the first frame update

    public Text characterNameText;
    public GameObject[] selectionHighlights;
    public void OnClickSelect(int index)
    {
        PlayerPrefs.SetInt(Constants.CharacterHead, index);
        UpdateGUI(index);
    }

    void OnEnable()
    {
       UpdateGUI(PlayerPrefs.GetInt(Constants.CharacterHead,(int)Shape.Random));
    }

    void UpdateGUI(int indexSelected)
    {
        characterNameText.text = $"{(Shape)indexSelected} Head";
        //loop through each box, highlight the one that matches selected head, unhighlight the rest.
        //assumes characterHeads array in DigiPlayerAnimation and selectionHighlights array share index
        for (int i = 0; i < selectionHighlights.Length; i++)
        {
            Transform t = selectionHighlights[i].transform;
            if (indexSelected == i)
            {
                t.GetComponent<Image>().color = Color.green;
            }
            else
            {
                t.GetComponent<Image>().color = Color.white;
            }
        }
        if (indexSelected == (int)Shape.Random) //since 999 won't be an index in the array, special check for random selection
        {
            selectionHighlights[selectionHighlights.Length-1].
                transform.GetComponent<Image>().color = Color.green;
        }
    }
}
