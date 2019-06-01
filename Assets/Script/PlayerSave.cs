using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FileValue
{
    public string keyName;
    public float keyValue;

    public FileValue(string name, float value)
    {
        keyName = name;
        keyValue = value;
    }
}

public class PlayerSave : MonoBehaviour {

    List<FileValue> prefsContents;
    public static PlayerSave sharedInstance;
    public float upgradeCost;
    Text coinDisplay;
    Text RoFDisplay;
    Text shieldDisplay;
    Text livesDisplay;

    private void Awake()
    {
        Debug.Log("nut");
        sharedInstance = this;
        prefsContents = new List<FileValue>();
        GetPlayerPrefsContents();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void GetPlayerPrefsContents()
    {
        //money
        prefsContents.Add(new FileValue("money", PlayerPrefs.GetFloat("money", 1000.0f)));
        //rate of file
        prefsContents.Add(new FileValue("RoF", PlayerPrefs.GetFloat("RoF", 0.3f)));
        //shield
        prefsContents.Add(new FileValue("shield", PlayerPrefs.GetFloat("shield", 1.0f)));
        //lives
        prefsContents.Add(new FileValue("lives", PlayerPrefs.GetFloat("lives", 3.0f)));
    }

    public void SetCoinDisplay(Text textComp)
    {
        coinDisplay = textComp;
        UpdateCoinDisplay();
    }

    public void SetRoFDisplay(Text textComp)
    {
        RoFDisplay = textComp;
        UpdateRoFDisplay();
    }

    public float GetRoF()
    {
        return prefsContents[1].keyValue;
    }

    public void SetLivesDisplay(Text textComp)
    {
        livesDisplay = textComp;
        UpdateLivesDisplay();
    }

    public float GetLives()
    {
        return prefsContents[3].keyValue;
    }

    public void SetShieldDisplay(Text textComp)
    {
        shieldDisplay = textComp;
        UpdateShieldDisplay();
    }

    public float GetShield()
    {
        return prefsContents[2].keyValue;
    }

    public void UpgradeShield()
    {
        //if the player has accumulated atleast 100 coints and the shield upgrade is not already full
        if(prefsContents[0].keyValue>=100.0f && prefsContents[2].keyValue<3)
        {
            PlayerPrefs.SetFloat("shield", prefsContents[2].keyValue + 1.0f);
            prefsContents[2].keyValue = PlayerPrefs.GetFloat("shield");
            PlayerPrefs.SetFloat("money", PlayerPrefs.GetFloat("money", 1000.0f) - upgradeCost);
            prefsContents[0].keyValue = PlayerPrefs.GetFloat("money");
            UpdateCoinDisplay();
            UpdateShieldDisplay();
        }
        else
        {
            Debug.Log("shield upgrade already at max");
        }
    }

    public void UpgradeLives()
    {
        //if the player has accumulated atleast 100 coints and the shield upgrade is not already full
        if (prefsContents[0].keyValue >= 100.0f && prefsContents[3].keyValue < 6)
        {
            PlayerPrefs.SetFloat("lives", prefsContents[3].keyValue + 1.0f);
            prefsContents[3].keyValue = PlayerPrefs.GetFloat("lives");
            PlayerPrefs.SetFloat("money", PlayerPrefs.GetFloat("money", 1000.0f) - upgradeCost);
            prefsContents[0].keyValue = PlayerPrefs.GetFloat("money");
            UpdateCoinDisplay();
            UpdateLivesDisplay();
        }
        else
        {
            Debug.Log("lives upgrade already at max");
        }
    }

    public void UpgradeRoF()
    {
        //if the player has accumulated atleast 100 coints and the shield upgrade is not already full
        if (prefsContents[0].keyValue >= 100.0f && prefsContents[1].keyValue < 0.6)
        {
            PlayerPrefs.SetFloat("RoF", prefsContents[1].keyValue + 0.1f);
            prefsContents[1].keyValue = PlayerPrefs.GetFloat("RoF");
            PlayerPrefs.SetFloat("money", PlayerPrefs.GetFloat("money", 1000.0f) - upgradeCost);
            prefsContents[0].keyValue = PlayerPrefs.GetFloat("money");
            UpdateCoinDisplay();
            UpdateRoFDisplay();
        }
        else
        {
            Debug.Log("RoF upgrade already at max");
        }
    }

    public void UpdateCoinDisplay()
    {
        coinDisplay.text = prefsContents[0].keyValue.ToString();
    }

    public void UpdateRoFDisplay()
    {
        RoFDisplay.text = prefsContents[1].keyValue.ToString();
    }

    public void UpdateShieldDisplay()
    {
        shieldDisplay.text = prefsContents[2].keyValue.ToString();
    }

    public void UpdateLivesDisplay()
    {
        livesDisplay.text = prefsContents[3].keyValue.ToString();
    }

    public void ResetPlayerPrefs()
    {
        PlayerPrefs.SetFloat("money", 1000.0f);
        prefsContents[0].keyValue = PlayerPrefs.GetFloat("money");
        UpdateCoinDisplay();
        PlayerPrefs.SetFloat("RoF", 0.3f);
        prefsContents[1].keyValue = PlayerPrefs.GetFloat("RoF");
        UpdateRoFDisplay();
        PlayerPrefs.SetFloat("shield", 1.0f);
        prefsContents[2].keyValue = PlayerPrefs.GetFloat("shield");
        UpdateShieldDisplay();
        PlayerPrefs.SetFloat("lives", 3.0f);
        prefsContents[3].keyValue = PlayerPrefs.GetFloat("lives");
        UpdateLivesDisplay();
        Debug.Log("PlayerPrefs has been reset");
    }
}
