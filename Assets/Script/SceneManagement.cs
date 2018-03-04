using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour {

    private void Awake()
    {
        Application.targetFrameRate = 300;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void LoadGameScene()
    {
        Debug.Log("loading scene");
        SceneManager.LoadScene("test");
    }

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

    public void PauseGame()
    {
        Time.timeScale = 0.0f;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1.0f;
    }
}
