using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Profiling;

public class NonogramHelpers : MonoBehaviour
{
    private static string K_DEFAULT_NONOGRAM_DIR = Application.streamingAssetsPath;
    private static string K_NONOGRAM_FILE_EXTENSION = "nono";

    public static void SaveNonogram(Nonogram nonogram, string category = null, bool overwrite = false)
    {
        Profiler.BeginSample("NonogramHelpers.SaveNonogram");

        if (category == null)
        {
            category = nonogram.GetNonogramCategory();
        }

        if (!DoesCategoryExist(category))
        {
            Debug.LogError("Directory does not exist. This is forbidden. Create the directory before creating nonogram");
            return;
        }

        NonogramSaveData data = new NonogramSaveData();
        data.Init(nonogram);

        string nonogramPath = BuildNonogramPath(nonogram.GetNonogramName(), category);

        if (File.Exists(nonogramPath) && overwrite == false)
        {
            Debug.LogError("Trying to save nonogram that already exists. This is prevented to avoid accidental data loss. Use \"overwrite = true\" to force it.");
            return;
        }

        Debug.Log("Saving nonogramFile at: " + nonogramPath);

        string nonogramJson = JsonUtility.ToJson(data);
        string obfuscatedJson = ObfuscateJsonString(nonogramJson);
        using (StreamWriter writer = new StreamWriter(nonogramPath, false))
        {
            writer.Write(obfuscatedJson);
        }

        Profiler.EndSample();
    }

    private static string BuildNonogramPath(string nonogramName, string nonogramCategory)
    {
        return K_DEFAULT_NONOGRAM_DIR + Path.DirectorySeparatorChar + nonogramCategory + Path.DirectorySeparatorChar + nonogramName + "." + K_NONOGRAM_FILE_EXTENSION;
    }

    private static string ObfuscateJsonString(string json)
    {
        byte[] jsonBytes = Encoding.UTF8.GetBytes(json);
        return Convert.ToBase64String(jsonBytes);
    }

    private static string DeobfuscateJsonString(string obfuscatedJson)
    {
        byte[] obfuscatedBytes = Convert.FromBase64String(obfuscatedJson);
        return Encoding.UTF8.GetString(obfuscatedBytes);
    }

    public static bool DoesCategoryExist(string directory)
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

        if (DoesCategoryExist(categoryName))
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
        //Profiling
        Profiler.BeginSample("NonogramHelper.LoadAllNonograms");

        List<NonogramSet> nonogramSets = new List<NonogramSet>();

        DirectoryInfo directoryInfo = new DirectoryInfo(K_DEFAULT_NONOGRAM_DIR);
        DirectoryInfo[] allFolders = directoryInfo.GetDirectories();
        foreach (DirectoryInfo folder in allFolders)
        {
            Debug.Log("Folder found: " + folder.Name);

            FileInfo[] nonograms = folder.GetFiles("*" + "." + K_NONOGRAM_FILE_EXTENSION);
            //if (nonograms.Length == 0)
            //    continue;

            NonogramSet newSet = new NonogramSet();
            newSet.SetName(folder.Name);

            foreach (FileInfo nonogramFile in nonograms)
            {
                using (StreamReader reader = new StreamReader(nonogramFile.FullName))
                {
                    string obfuscatedNonogramJson = reader.ReadToEnd();
                    string deobfuscatedJson = DeobfuscateJsonString(obfuscatedNonogramJson);
                    NonogramSaveData nonogramSaveData = JsonUtility.FromJson<NonogramSaveData>(deobfuscatedJson);
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

                    string nonogramName = Path.GetFileNameWithoutExtension(nonogramFile.FullName);
                    newNonogram.SetNonogramName(nonogramName);
                    newSet.AddNonogram(newNonogram);
                }
            }

            nonogramSets.Add(newSet);
        }

        Profiler.EndSample();

        return nonogramSets;
    }
}
