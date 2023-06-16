using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class NonogramHelpers : MonoBehaviour
{
    private static string K_DEFAULT_NONOGRAM_DIR = Application.streamingAssetsPath;
    private static string K_NONOGRAM_FILE_EXTENSION = ".nono";

    public static void SaveNonogram(Nonogram nonogram, string category)
    {
        if (!DoesDirectoryExist(category))
        {
            Debug.LogError("Directory does not exist. This is forbidden. Create the directory before creating nonogram");
            return;
        }

        NonogramSaveData data = new NonogramSaveData();
        data.Init(nonogram);

        string nonogramPath = K_DEFAULT_NONOGRAM_DIR + Path.DirectorySeparatorChar + category + Path.DirectorySeparatorChar + nonogram.GetNonogramName() + K_NONOGRAM_FILE_EXTENSION;
        Debug.Log("Saving nonogramFile at: " + nonogramPath);

        string nonogramJson = JsonUtility.ToJson(data);
        using (StreamWriter writer = new StreamWriter(nonogramPath, false))
        {
            writer.Write(nonogramJson);
        }
    }

    public static bool DoesDirectoryExist(string directory)
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(K_DEFAULT_NONOGRAM_DIR);
        DirectoryInfo[] allFolders = directoryInfo.GetDirectories();
        foreach (DirectoryInfo folder in allFolders)
        {
            if (folder.Name == directory)
                return true;
        }

        return false;
    }    

    public static bool CreateNewCategory(string categoryName)
    {
        if (string.IsNullOrEmpty(categoryName))
        {
            Debug.LogError("Invalid category name provided");
            return false;
        }

        if (DoesDirectoryExist(categoryName))
        {
            Debug.LogError("Category name already exists");
            return false;
        }

        DirectoryInfo directoryInfo = new DirectoryInfo(K_DEFAULT_NONOGRAM_DIR);
        directoryInfo.CreateSubdirectory(categoryName);
        return true;
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
            //if (nonograms.Length == 0)
            //    continue;

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
