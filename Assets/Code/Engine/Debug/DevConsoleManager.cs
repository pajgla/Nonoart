#if UNITY_EDITOR
using Save;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UIViewModel;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DevConsoleManager : BaseSingleton<DevConsoleManager>
{
    public class DevCommand
    {
        string m_ID = string.Empty;
        string m_Description = string.Empty;

        Action<string[]> m_ExecuteAction = null;

        public DevCommand(string ID, string description, Action<string[]> executeAction)
        {
            m_ID = ID;
            m_Description = description;
            m_ExecuteAction = executeAction;
        }

        public void Execute(string[] args)
        {
            m_ExecuteAction(args);
        }

        public string GetID() { return m_ID; }
        public string GetDescription() { return m_Description;}
    }

    [Header("References")]
    [SerializeField] DevConsoleViewModel m_DevConsoleViewModelRef = null;

    DevConsoleViewModel m_DevConsoleViewModel = null;

    List<DevCommand> m_RegisteredCommands = new List<DevCommand>();
    List<string> m_CommandUseHistory = new List<string>();
    int m_CurrentHistorySelectionIndex = 0;

    private void Start()
    {
        m_DevConsoleViewModel = ViewModelHelper.SpawnAndInitialize(m_DevConsoleViewModelRef);
        m_DevConsoleViewModel.transform.SetParent(gameObject.transform);
        m_DevConsoleViewModel.ChangeViewModelVisibility(false);

        m_DevConsoleViewModel.GetInputField().onSubmit.AddListener(OnInputFieldSubmit);

        AddDefaultCommands();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            TMP_InputField inputField = m_DevConsoleViewModel.GetInputField();
            m_DevConsoleViewModel.ToggleViewModelVisibility();
            if (m_DevConsoleViewModel.GetIsVisible())
            {
                inputField.ActivateInputField();
            }
            
            if (inputField.text.Length > 0)
                inputField.text = inputField.text.Remove(inputField.text.Length - 1, 1);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (m_CommandUseHistory.Count == 0)
                return;

            m_CurrentHistorySelectionIndex--;
            if (m_CurrentHistorySelectionIndex < 0)
                m_CurrentHistorySelectionIndex = m_CommandUseHistory.Count - 1;

            TMP_InputField inputField = m_DevConsoleViewModel.GetInputField();
            inputField.text = m_CommandUseHistory[m_CurrentHistorySelectionIndex];
            inputField.MoveToEndOfLine(false, false);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (m_CommandUseHistory.Count == 0)
                return;

            m_CurrentHistorySelectionIndex++;
            if (m_CurrentHistorySelectionIndex > m_CommandUseHistory.Count - 1)
                m_CurrentHistorySelectionIndex = 0;

            TMP_InputField inputField = m_DevConsoleViewModel.GetInputField();
            inputField.text = m_CommandUseHistory[m_CurrentHistorySelectionIndex];
        }
    }

    private void OnInputFieldSubmit(string input)
    {
        //Erase text from input field
        TMP_InputField inputField = m_DevConsoleViewModel.GetInputField();
        inputField.text = string.Empty;
        inputField.ActivateInputField();

        m_CommandUseHistory.Add(input);
        m_CurrentHistorySelectionIndex = -1;

        if (input[0] != '/')
        {
            ConsolePrint("Invalid input: Every command needs to start with '/'", "red");
            return;
        }

        List<string> arguments = new List<string>();
        string commandID = string.Empty;
        string currentArg = string.Empty;
        bool isReadingCommandID = true;
        //Parse input
        foreach (char c in input) 
        {
            if (isReadingCommandID)
            {
                if (commandID == string.Empty)
                {
                    if (c == '/')
                    {
                        //We do not want to include '/' in ID
                        continue;
                    }
                }

                if (Char.IsWhiteSpace(c))
                {
                    isReadingCommandID = false;
                    continue;
                }
                else if (!Char.IsLetter(c))
                {
                    ConsolePrint("Invalid character in command ID. ID can only contain letters", "red");
                    return;
                }
                else
                {
                    commandID += c;
                }
            }
            else
            {
                if (currentArg == string.Empty)
                {
                    if (Char.IsWhiteSpace(c))
                        continue;
                }

                if (c != ',')
                {
                    currentArg += c;
                }
                else
                {
                    arguments.Add(currentArg);
                    currentArg = string.Empty;
                }
            }
        }

        if (currentArg != string.Empty)
        {
            arguments.Add(currentArg);
        }

        foreach (DevCommand command in m_RegisteredCommands)
        {
            if (command.GetID() == commandID)
            {
                command.Execute(arguments.ToArray());
                return;
            }
        }

        ConsolePrint("Command not found", "red");
    }

    private void ConsolePrint(string output, string color = "")
    {
        string formattedOutput = string.Empty;
        if (color != "")
        {
            formattedOutput += "<color=\"" + color + "\">" + output + "</color>";   
        }
        else
        {
            formattedOutput = output;
        }

        m_DevConsoleViewModel.AppendOutputString(formattedOutput);
    }

    private void ExecuteCommand(DevCommand command, string[] args)
    {
        command.Execute(args);
    }

    public void RegisterCommand(DevCommand command)
    {
        m_RegisteredCommands.Add(command);
    }

    // -------------------- Commands ------------------------

    private void AddDefaultCommands()
    {
        DevCommand helpCommand = new DevCommand("help", "Shows all available commands. Use /help *commandID* (without forwardslash) to show command's description", (input) =>
        {
            string outputString = string.Empty;

            if (input == null || input.Length == 0)
            {
                outputString = "ID".PadRight(40) + "Description\n---------------------------------------------------------------\n";
                foreach (DevCommand command in m_RegisteredCommands)
                {
                    //string space = String.Concat(Enumerable.Repeat(" ", 40 - command.GetID().Length));
                    outputString += "/" + command.GetID().PadRight(40) + command.GetDescription() + "\n";
                }
            }
            else
            {
                if (input.Length != 1)
                {
                    ConsolePrint("ERROR: Invalid number of arguments. Use /help *command ID* without forwardslash or without any arguments to show list of all registered commands", "red");
                    return;
                }

                string commandToSearch = input[0];
                bool isFound = false;
                foreach (DevCommand command in m_RegisteredCommands)
                {
                    if (command.GetID() == commandToSearch)
                    {
                        outputString = "/" + command.GetID() + "  -  " + command.GetDescription();
                        isFound = true;
                        break;
                    }
                }

                if (!isFound)
                {
                    ConsolePrint("Command not found", "yellow");
                    return;
                }
            }

            ConsolePrint(outputString);
        });

        DevCommand deleteSave = new DevCommand("deleteSave", "Deletes all saved data (global and nonogram completion data)", (input) =>
        {
            SavegameManager savegameManager = SavegameManager.Get();
            savegameManager.DeleteAllSaves();

            ConsolePrint("All save data is erased. Restart the game to apply.");
        });

        DevCommand completeNonogram = new DevCommand("completeLevel", "Completes current nonogram level", (input) =>
        {
            GameManager gameManager = GameManager.Get();
            GameMode activeGameMode = gameManager.GetActiveGameMode();
            if (gameManager.IsInGameMode() == false || activeGameMode.GetGameModeType() != EGameModeType.Solving)
            {
                ConsolePrint("Not in Solving Game Mode", "red");
                return;
            }

            SolvingGameMode solvingGameMode = (SolvingGameMode)activeGameMode;
            if (solvingGameMode == null)
            {
                ConsolePrint("Something went wrong while casting active game mode. Inspect the logic", "red");
                return;
            }

            solvingGameMode.Solve();
        });

        DevCommand testCommand = new DevCommand("test", "Used for testing command logic. You can use any number of arguments", (input) =>
        {
            string output = "Arguments used: ";
            foreach (string argument in input)
            {
                output += argument + ",";
            }
            output.Remove(output.Length - 1);

            ConsolePrint(output);
        });

        RegisterCommand(helpCommand);
        RegisterCommand(deleteSave);
        RegisterCommand(completeNonogram);
        RegisterCommand(testCommand);
    }
}

#endif