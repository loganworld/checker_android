using UnityEngine;
using UnityEngine.SceneManagement;

public class PlaySceneLoader : MonoBehaviour
{
    public MenuAudio MenuAudio;
    
    public void LoadMultiPlayScene()
    {
        SceneManager.LoadScene("MultiDashboard", LoadSceneMode.Single);
    }

    public void LoadMainPlayScene()
    {
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
    }

    public void LoadMenuPlayScene()
    {
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }

    public void LoadTestPlayScene()
    {
        SceneManager.LoadScene("test", LoadSceneMode.Single);
    }
    public void FadeMenuMusic()
    {
        MenuAudio.FadeMenuMusic();
    }
}