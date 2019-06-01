using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


//need to do more as will manage most scene information and level information
public class SceneManagement : MonoBehaviour {

    string currentUIScreen;
    Animation menuAnim;
    GameObject areaInstance;
    public static SceneManagement sharedInstance;
    string levelName="Levels/Level1";

    void OnEnable()
    {
        sharedInstance = this;
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

    public void LoadVerticalTestScene()
    {
        SceneManager.LoadSceneAsync("verticalTest");
    }

    public void LoadHorizontalTestScene()
    {
        SceneManager.LoadSceneAsync("horizontalLeftUI");
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
        if(scene.name=="OpeningScene")
        {
            currentUIScreen = "StartScreen";
            PlayerSave.sharedInstance.SetCoinDisplay(GameObject.Find("CoinDisplay").GetComponent<Text>());
            PlayerSave.sharedInstance.SetRoFDisplay(GameObject.Find("RoFDisplay").GetComponent<Text>());
            PlayerSave.sharedInstance.SetLivesDisplay(GameObject.Find("LivesDisplay").GetComponent<Text>());
            PlayerSave.sharedInstance.SetShieldDisplay(GameObject.Find("ShieldDisplay").GetComponent<Text>());
            menuAnim = GameObject.Find("Canvas").GetComponent<Animation>();
        }
    }

    public string GetCurrentScene()
    {
        return SceneManager.GetActiveScene().name;
    }

    public void UpgradesMenu()
    {
        if(currentUIScreen=="StartScreen")
        {
            menuAnim["upgradeMenu"].speed = 1;
            menuAnim.Play("upgradeMenu");
            currentUIScreen = "UpgradeScreen";
        }
        else
        {
            menuAnim["upgradeMenu"].speed = -1;
            menuAnim["upgradeMenu"].time = menuAnim["upgradeMenu"].length;
            menuAnim.Play("upgradeMenu");
            currentUIScreen = "StartScreen";
        }
    }
}
