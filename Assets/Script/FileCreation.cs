using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

//system used to save level date for levels created in the level editor
//which will be directly used to get information from when playing the game
public class FileCreation : MonoBehaviour
{

    string savesFilePath;
    //spawn/control points that are currently active in the editor, which in turn contain the rest of the
    //information related to enemy and pathing
    List<GameObject> spInEditor;
    //very important, used for streamwriter calls in order to create and write to file
    StreamWriter sw;
    //unity UI input filed for file names
    public InputField fileNameInput;

	//the path where the level .txt files will be saved at
	void Start () {
        //System.environment.getfolderpath
        //=Gets the path to the system special folder that is identified by the specified enumeration.
        //system.environment.specialfolder.desktopdirectory
        //=Specifies enumerated constants used to retrieve directory paths to system special folders.
        //from msdn developer network
        savesFilePath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.DesktopDirectory);
	}

    //function called when attempting to save the file
    public void AcquireFileName()
    {
        if(fileNameInput.text.Length>1)
        {
            //use the file path , the desktop, + the file name + file extension
            savesFilePath=savesFilePath+ @"\" + fileNameInput.text+".txt";
            //get a list of editor spawn points
            spInEditor = Creator.sharedInstance.GetSpawnPointList();
            //create, write and close the file
            CreateSaveFile();
        }
        else
        {
            //will only create a file if there is a name with atleast 1 letter
            Debug.Log("baga si tu un nume la fisier coaie");
        }
    }

    string GetCameraValues()
    {
        string camera;
        camera = CameraControl.sharedInstance.GetCameraSpeed().ToString() + "_" + CameraControl.sharedInstance.GetTargetPosition().ToString();
        return camera;
    }

    void CreateSaveFile()
    {
        //Creates or overwrites a file using the specified path, filename taken from the path
        //then
        //.Dispose() Releases all resources used by the Stream in order to be able to write in the file
        File.Create(savesFilePath).Dispose();
        //create a streamwriter for the file using the same path, filename taken from path
        sw = new StreamWriter(savesFilePath);
        //the first line in the file is the camera speed and target position
        sw.WriteLine(GetCameraValues());
        //the third line is a list of all object types present in the level
        //in order to more easily tell the object pooler what to pool
        sw.WriteLine(GetObjectPoolerList());
        //the proceed to write all the spawn points in the level , one spawnpoint(together with it's information)
        //per line
        SpawnPointList();

        //close the streamwriter so file changes are save
        sw.Close();
    }


    string GetObjectPoolerList()
    {
        //get an empty string that will contain the names of all entities that will be present in the level
        string pooledObjLine=string.Empty;
        //get the first entity
        if (spInEditor.Count > 0)
        {
            pooledObjLine = spInEditor[0].GetComponent<EditorSpawnPoint>().objName;
            //starting from the second entity all names will have a "_" preceding them
            for (int i = 1; i < spInEditor.Count; i++)
            {
                string objName = spInEditor[i].GetComponent<EditorSpawnPoint>().objName;
                //if the obj name hasn't been listed
                if (!pooledObjLine.Contains(objName))
                {
                    pooledObjLine += "_" + objName;
                }
            }
        }

        //return the list
        return pooledObjLine;
    }

    void SpawnPointList()
    {
        //used per spawnpoint information
        //in the following format
        //sx,sy_int_x,y_x,y_...._1/0_entitytype_basespeed_loopspeed_oolspeed
        //sx,sy=origin point 2d pos
        //int=number of secondary points
        //x,y_x,y_..._=secondary points 2d pos
        //1/0=true/false if the entity will loop between points
        //entitytype=entity name
        string spawnPointList = string.Empty;

        //foreach spawn point in the list
        for(int i=0; i<spInEditor.Count; i++)
        {
            //add the 2d pos of the spawnpoint
            spawnPointList =
                spInEditor[i].transform.position.x //xpos
                + ","//positions to always be separated by "," in order to figure out they are together
                + spInEditor[i].transform.position.y //ypos
                + "_" //separated by "_"
                + (spInEditor[i].GetComponent<EditorSpawnPoint>().pointsGO.Count - 1)//number of secondary points
                + "_"; //separator
            if(spInEditor[i].GetComponent<EditorSpawnPoint>().pointsGO.Count>1)//if it has any secondary points
            {
                for (int j = 1; j < spInEditor[i].GetComponent<EditorSpawnPoint>().pointsGO.Count; j++)
                {
                    spawnPointList +=
                       spInEditor[i].GetComponent<EditorSpawnPoint>().pointsGO[j].transform.position.x//x pos
                       + ","
                       + spInEditor[i].GetComponent<EditorSpawnPoint>().pointsGO[j].transform.position.y//y pos
                       + "_"; //separator
                }
            }
            spawnPointList += spInEditor[i].GetComponent<EditorSpawnPoint>().IsLooping()//is there a loop?
                + "_";//separator
            spawnPointList += spInEditor[i].GetComponent<EditorSpawnPoint>().objName //name of the entity attached to this point
                + "_";//separator
            spawnPointList += spInEditor[i].GetComponent<EditorSpawnPoint>().GetBaseSpeed() //and it's speeds
                + "_";//separator
            spawnPointList += spInEditor[i].GetComponent<EditorSpawnPoint>().GetLoopSpeed()
                + "_";//separator
            spawnPointList += spInEditor[i].GetComponent<EditorSpawnPoint>().GetOolSpeed();

            sw.WriteLine(spawnPointList); //write the information and start a new line
            spawnPointList = string.Empty;//discard the current information for a new line to be written
        }
    }
}
