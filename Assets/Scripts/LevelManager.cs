using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    internal int level = 1;
    int levelCount;
    public static LevelManager instance;
    private void Awake()
    {
        if (instance==null)
        {
            instance = this;
            levelCount = SceneManager.sceneCountInBuildSettings;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    /*
         * 1=0%3=0
         * 2=1%3=1
         * 3=2%3=2
         * 4=3%3=0
         * 5=4%3=1
         * 6=5%3=2
         * 7=6&3=0
         */
    internal void NextLevel()
    {
        level += 1;
        SceneManager.LoadScene((level-1)%levelCount);
        
    }
    internal void LevelFailed()
    {
        SceneManager.LoadScene((level - 1) % levelCount);
    }
   
}
