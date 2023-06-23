using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LoadInitialScene : MonoBehaviour
{
    public void LoadLevelScene()
    {
        SceneManager.LoadSceneAsync("Level");
    }
}
