using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public enum EnumScene
    {
        MainScene,
        PlayScene,
    }

    public void BPlayButton()
    {
        SceneManager.LoadSceneAsync((int)EnumScene.PlayScene); //"PlayScene"
    }

    public void BQuitButton()
    {
        Application.Quit();
    }
}
