using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class NonogramHelpers : MonoBehaviour
{
    private static string K_DEFAULT_NONOGRAM_DIR = Application.streamingAssetsPath;
    private static string K_NONOGRAM_FILE_EXTENSION = ".nono";

    public static void SaveNonogram(Nonogram nonogram, string category)
    {
        NonogramSaveData data = new NonogramSaveData();
        data.Init(nonogram);

        //#TODO: Use nonogramFile category as a new folder
        string nonogramPath = K_DEFAULT_NONOGRAM_DIR + Path.DirectorySeparatorChar + category + Path.DirectorySeparatorChar + nonogram.GetNonogramName() + K_NONOGRAM_FILE_EXTENSION;
        Debug.Log("Saving nonogramFile at: " + nonogramPath);

        //if (!Directory.Exists(nonogramPath)) 
        //{
        //    Debug.LogError("Directory does not exist. Create the missing directory before saving the nonogramFile.");
        //    return;
        //}

        string nonogramJson = JsonUtility.ToJson(data);
        using (StreamWriter writer = new StreamWriter(nonogramPath, false))
        {
            writer.Write(nonogramJson);
        }
    }

    public static List<NonogramSet> LoadAllNonograms()
    {
        List<NonogramSet> nonogramSets = new List<NonogramSet>();

        DirectoryInfo directoryInfo = new DirectoryInfo(K_DEFAULT_NONOGRAM_DIR);
        DirectoryInfo[] allFolders = directoryInfo.GetDirectories();
        foreach (DirectoryInfo folder in allFolders)
        {
            Debug.Log("Folder found: " + folder.Name);

            FileInfo[] nonograms = folder.GetFiles("*" + K_NONOGRAM_FILE_EXTENSION);
            if (nonograms.Length == 0)
                continue;

            NonogramSet newSet = new NonogramSet();
            newSet.SetName(folder.Name);

            foreach (FileInfo nonogramFile in nonograms)
            {
                using (StreamReader reader = new StreamReader(nonogramFile.FullName))
                {
                    string nonogramJson = reader.ReadToEnd();
                    NonogramSaveData nonogramSaveData = JsonUtility.FromJson<NonogramSaveData>(nonogramJson);
                    if (nonogramSaveData == null)
                    {
                        Debug.LogError("Error while trying to read nonogramFile data. The file may be corrupted or invalid");
                        continue;
                    }

                    Nonogram newNonogram = nonogramSaveData.ConvertToNonogram();
                    if (newNonogram == null)
                    {
                        //Error already present
                        continue;
                    }
                    newNonogram.SetNonogramName(nonogramFile.Name);
                    newSet.GetNonograms().Add(newNonogram);
                }
            }

            nonogramSets.Add(newSet);
        }

        return nonogramSets;
    }
}
