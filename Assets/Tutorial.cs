using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour {

    public void CloseTutorial()
    {
        GetComponent<Animation>().Play( "TutorialClose" );
    }

    public void DestroyMe()
    {
        Destroy( gameObject );
    }
}
