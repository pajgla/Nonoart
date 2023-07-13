using CI.QuickSave;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Save
{
    public class NonogramCompletionSaveData
    {
        public bool m_IsCompleted = false;
        public float m_Time = -1.0f;
    }

    public static class SaveRoots
    {
        public const string K_SavegameDataRoot = "savegameData";
    }

    public static class GlobalSaveProperties
    {
        //Global save values
        public const string K_GlobalSaveRoot = "global";
        //ADD BELOW
        public const string K_IsTutorialCompleted = "tutorialCompleted";
        public const string K_AreSoundsMuted = "areSoundsMuted";
        public const string K_IsMusicMuted = "isMusicMuted";
    }

    public static class SpecificSaveProperties
    {
        public const string K_IsCompleted = "isCompleted";
        public const string K_CompletionTime = "completionTime";
        public const string K_NonogramSaveVersion = "savegameVersion";
    }

    public class SavegameManager : BaseSingleton<SavegameManager>
    {
        // BE CAREFUL WITH THESE !!! CHANGING ANY OF THE PROPERTIES CAN BREAK THE GAME OR MESS UP SOMEONE'S SAVE


        //####################################################################
        //#                                                                  #
        //#                          DANGER ZONE                             #  
        //#                                                                  #
        //####################################################################
        //Increase if new save/load will not work properly with older saves. For example, if we change the name of one of the savegame properties, it cannot be read
        //and the player will be stuck in loading screen. Instead, increase the savegame version and implement different loading logic for different versions, if possible
        int m_SavegameVersion = 1;


        public Save.NonogramCompletionSaveData LoadNonogramData(string nonogramID, QuickSaveSettings saveSettings = null)
        {
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            ///                                                                                                                            ///
            ///                                                                                                                            ///
            ///                                                                                                                            ///
            ///                         BE CAREFUL HERE. CHANGING ANYTHING CAN BREAK SOMEONE'S SAVE GAME !!!                               ///
            ///                                                                                                                            ///
            ///                                                                                                                            ///
            ///                                                                                                                            ///
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            QuickSaveReader reader = QuickSaveReader.Create(nonogramID);

            int savegameVersion;
            if (reader.TryRead<int>(SpecificSaveProperties.K_NonogramSaveVersion, out savegameVersion))
            {
                if (savegameVersion != m_SavegameVersion)
                {
                    //#TODO: Implement different savegame loadings
                    Debug.LogError("Savegame version is not compatibile anymore. Check for savegame version before loading the save.");
                    return null;
                }
            }
            else
            {
                Debug.LogError("Nonogram Save Version is missing. It should be assigned while saving the nonogram");
                return null;
            }

            Save.NonogramCompletionSaveData nonogramSaveData = new Save.NonogramCompletionSaveData();
            nonogramSaveData.m_IsCompleted = reader.Read<bool>(SpecificSaveProperties.K_IsCompleted);
            nonogramSaveData.m_Time = reader.Read<float>(SpecificSaveProperties.K_CompletionTime);

            return nonogramSaveData;
        }

        public void SaveNonogramData(string nonogramID, Save.NonogramCompletionSaveData saveData, QuickSaveSettings saveSettings = null)
        {
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            ///                                                                                                                            ///
            ///                                                                                                                            ///
            ///                                                                                                                            ///
            ///                         BE CAREFUL HERE. CHANGING ANYTHING CAN BREAK SOMEONE'S SAVE GAME !!!                               ///
            ///                                                                                                                            ///
            ///                                                                                                                            ///
            ///                                                                                                                            ///
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            QuickSaveWriter writer = QuickSaveWriter.Create(nonogramID);

            //Saving savegame version
            writer.Write(SpecificSaveProperties.K_NonogramSaveVersion, m_SavegameVersion);
            writer.Write(SpecificSaveProperties.K_IsCompleted, saveData.m_IsCompleted);
            writer.Write(SpecificSaveProperties.K_CompletionTime, saveData.m_Time);

            writer.Commit();
        }

        public void InitializeGlobalSave()
        {
            if (!DoesGlobalRootExists())
            {
                QuickSaveWriter writer = QuickSaveWriter.Create(GlobalSaveProperties.K_GlobalSaveRoot);
                writer.Commit();
            }
        }

        public T GetGlobalSaveValue<T>(string keyName)
        {
            if (!DoesGlobalRootExists())
            {
                Debug.LogError("Global Root Save does not exist! It should be initialized at the start of the game!");
                return default(T);
            }

            QuickSaveReader reader = QuickSaveReader.Create(GlobalSaveProperties.K_GlobalSaveRoot);
            T result = default(T);
            reader.TryRead<T>(keyName, out result);
            return result;
        }

        public void SetGlobalSaveValue<T>(string keyName, T value)
        {
            if (!DoesGlobalRootExists())
            {
                Debug.LogError("Global Root Save does not exist! It should be initialized at the start of the game!");
            }

            QuickSaveWriter writer = QuickSaveWriter.Create(GlobalSaveProperties.K_GlobalSaveRoot);
            writer.Write(keyName, value);
            writer.Commit();
        }

        private bool DoesGlobalRootExists()
        {
            return QuickSaveBase.CheckIfRootExists(GlobalSaveProperties.K_GlobalSaveRoot);
        }

        public bool CheckIfNonogramSaveExists(string nonogramID)
        {
            bool doesSavegameExist = QuickSaveBase.CheckIfRootExists(nonogramID);
            if (doesSavegameExist)
            {
                QuickSaveReader reader = QuickSaveReader.Create(nonogramID);
                int saveVersion;
                if (reader.TryRead<int>(SpecificSaveProperties.K_NonogramSaveVersion, out saveVersion))
                {
                    if (saveVersion == m_SavegameVersion)
                    {
                        return true;
                    }
                    else
                    {
                        //#TODO: Implement savegame version loading
                        Debug.LogError("Invalid save version");
                    }
                }
            }

            return false;
        }

        public void DeleteSavegame(string nonogramID)
        {
            if (!QuickSaveBase.CheckIfRootExists(nonogramID))
            {
                Debug.LogWarning("Savegame with ID " + nonogramID + " does not exist but we are trying to delete it.");
                return;
            }

            QuickSaveBase.DeleteSave(nonogramID);
        }

        public void DeleteRootSaveData()
        {
            if (DoesGlobalRootExists())
            {
                DeleteSavegame(GlobalSaveProperties.K_GlobalSaveRoot);
            }
        }

        public static string GenerateUniqueID()
        {
            return Guid.NewGuid().ToString();
        }
    }
}