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
       UpdateGUI(PlayerPrefs.GetInt(Constants.CharacterHead,0));
    }

    void UpdateGUI(int indexSelected)
    {
        characterNameText.text = $"{(Shape)indexSelected} Head";
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
    }
}
