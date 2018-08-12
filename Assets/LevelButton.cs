using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelButton : MonoBehaviour
{
    public string SceneName = "Level1";
    public bool Unlockable = true;

    public TextMeshProUGUI Text;


    private void Start()
    {
        if ( Unlockable )
        {
            if ( PlayerPrefs.GetInt( "Unlocked_" + SceneName, 0 ) == 0 )
            {
                Text.color = Color.white;
                GetComponent<Button>().interactable = false;
                GetComponent<Image>().color = new Color( 0.9f, 0.9f, 0.9f, 1 );
            }
        }
    }

    public void OnClickButton()
    {
        SceneManager.LoadScene( SceneName );
    }
}
