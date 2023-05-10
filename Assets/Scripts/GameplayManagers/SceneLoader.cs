using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;
    public delegate void OnSceneLoaded();
    public static event OnSceneLoaded onSceneLoaded;
    public delegate void OnSceneLoadedWithIndex();
    public static event OnSceneLoadedWithIndex onSceneLoadedWithIndex;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    public void GoToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
    public int GetCurrentSceneIndex()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }
    public void GoToGameScene()
    {
        SceneManager.LoadScene(1);
        onSceneLoaded?.Invoke();
    }
    public void GoToGameSceneWithLevelIndex(int index)
    {
        SceneManager.LoadScene(1);
        ScriptableObjectLoader.Instance.Index = index;
        onSceneLoadedWithIndex();
    }
    public void ReloadScene()
    {
        SceneManager.LoadScene(GetCurrentSceneIndex());
    }
}
