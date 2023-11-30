using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public void ClickStart()
    {
        LevelManager.PlayScene(Scenes.Game);
    }
    public void ClickExit()
    {
        Application.Quit();
    }
}
