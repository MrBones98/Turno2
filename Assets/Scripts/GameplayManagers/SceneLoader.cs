using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneLoader : MonoBehaviour
{
   
    public void GoToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
    public int GetCurrentSceneIndex()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }
    public void ReloadScene()
    {
        SceneManager.LoadScene(GetCurrentSceneIndex());
    }
}
