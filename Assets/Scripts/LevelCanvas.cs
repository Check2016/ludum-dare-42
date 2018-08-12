using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCanvas : MonoBehaviour {

	public void RestartLevel()
    {
        LevelManager.instance.RestartLevel();
    }

    public void GotoNextLevel()
    {
        LevelManager.instance.GotoNextLevel();
    }

    public void GotoMainMenu()
    {
        LevelManager.instance.GotoMainMenu();
    }
}
