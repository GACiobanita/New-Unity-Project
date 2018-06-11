using UnityEngine;
using UnityEngine.SceneManagement;

//need to do more as will manage most scene information and level information
public class SceneManagement : MonoBehaviour {

    GameObject areaInstance;
    string levelName="Levels/Level1";

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoad;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoad;
    }

    //setup the framerate of the game
    //automatically set when unity game boots on an android device, separate of this
    private void Awake()
    {
        Application.targetFrameRate = 300;
        areaInstance = Instantiate(Resources.Load("GameManager") as GameObject);
        DontDestroyOnLoad(areaInstance);
        DontDestroyOnLoad(this);
    }

    //function used by UI button, leads to test scene
    public void LoadGameScene()
    {
        if(areaInstance==null)
        {
            areaInstance = Instantiate(Resources.Load("GameManager") as GameObject);
            DontDestroyOnLoad(areaInstance);
        }
        SceneManager.LoadSceneAsync("test");
    }

    //pause the game
    public void ChangeGameState()
    {
        if (Time.timeScale == 1.0f)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }

    //game is paused and anything tied to the time.delta time will stop
    public void PauseGame()
    {
        Time.timeScale = 0.0f;
    }

    //time resumes as normal
    //0.5 is half speed, 1.0 is normal speed, 2.0 is twice the speed, 0.0 is stopped entirelly
    public void ResumeGame()
    {
        Time.timeScale = 1.0f;
    }

    void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        if(scene.name=="test")
        {
            areaInstance.GetComponent<GameArea>().GetLevelFilename(levelName);
            areaInstance.GetComponent<GameArea>().GetLevelInformation();
        }
    }
}
