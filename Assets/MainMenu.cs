using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour {

    private const string TutorialSeenOnceKey = "TutorialSeenOnce";

    public GameObject TutorialPrefab;

    private void Start()
    {
#if UNITY_EDITOR && DELETE_PREFS
        PlayerPrefs.DeleteAll();
#endif

        if ( PlayerPrefs.GetInt( TutorialSeenOnceKey, 0 ) == 0 )
        {
            StartCoroutine( ShowTutorial() );
        }
    }

    public void OpenTutorial()
    {
        StartCoroutine( ShowTutorial() );
    }

    private IEnumerator ShowTutorial()
    {
        GameObject clone = Instantiate( TutorialPrefab );

        while ( clone != null )
        {
            yield return null;
        }

        PlayerPrefs.SetInt( TutorialSeenOnceKey, 1 );
    }
}
