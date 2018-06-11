using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public static class FileReader
{
    private static StreamReader reader;
    private static TextAsset levelFile;
    private static string path;
    private static int cameraSpeed;
    private static int cameraTargetPos;
    private static List<string> levelEnemies;

    // Use this for initialization
    public static void OpenExplorer()
    {
        path = EditorUtility.OpenFilePanel("Select level file", "", "txt");
        if (path.Length != 0)
            ReadDesktopFile();
    }

    private static void ReadDesktopFile()
    {
        levelEnemies = new List<string>();
        reader = new StreamReader(path);
        string[] levelFile = reader.ReadToEnd().Split('\n');
        for (int i = 0; i < levelFile.Length; i++)
        {
            levelFile[i] = levelFile[i].TrimEnd('\r');
        }
        string[] cameraValues = levelFile[0].Split('_');
        cameraSpeed = int.Parse(cameraValues[0]);
        cameraTargetPos = int.Parse(cameraValues[1]);
        for (int i = 2; i <= levelFile.Length - 1; i++)
        {
            levelEnemies.Add(levelFile[i]);
        }
        reader.Close();

        Creator.sharedInstance.GetLevelSizeFromCameraPos(cameraTargetPos);
        Creator.sharedInstance.LoadCameraSpeed(cameraSpeed);
        Creator.sharedInstance.CreateSpawnPoints(levelEnemies);
    }

    public static void ReadLevelFile(string filename, ref float cameraSpeed, ref float cameraTargetPos, ref string[] resources,ref List<string> levelEnemies)
    {
        levelFile = Resources.Load(filename) as TextAsset;
        string[] lineSplit = levelFile.text.Split('\n');
        for(int i=0; i<lineSplit.Length;i++)
        {
            lineSplit[i] = lineSplit[i].TrimEnd('\r');
        }
        string[] cameraValues = lineSplit[0].Split('_');
        cameraSpeed = float.Parse(cameraValues[0]);
        cameraTargetPos = float.Parse(cameraValues[1]);
        resources = lineSplit[1].Split('_');
        for(int i=2; i<lineSplit.Length-1; i++)
        {
            levelEnemies.Add(lineSplit[i]);
        }
    } 
}
