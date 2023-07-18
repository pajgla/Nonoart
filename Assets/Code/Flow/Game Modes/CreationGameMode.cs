using Save;
using System.Collections.Generic;
using UIViewModel;
using UnityEngine;
using UnityEngine.UI;

public class CreationGameMode : GameMode
{
    const string K_CREATE_NEW_CATEGORY_STRING = "<b>Create New Category</b>";
    const string K_EASTER_EGG_CATEGORY_NAME = "Easter Egg Category";

    int m_GridWidth = 0;
    int m_GridHeight = 0;

    [Header("References")]
    [SerializeField] ColorPickerViewModel m_ColorPickerViewModelRef = null;
    [SerializeField] GridConfigurationViewModel m_ConfigurationViewModelRef = null;
    [SerializeField] ControlBarViewModel m_ControlBarViewModelRef = null;
    [SerializeField] NonogramConfigScreenViewModel m_NonogramConfigScreenViewModelRef = null;

    //View Models
    ColorPickerViewModel m_ColorPickerViewModel = null;
    GridConfigurationViewModel m_ConfigurationViewModel = null;
    ControlBarViewModel m_ControlBarViewModel = null;
    NonogramConfigScreenViewModel m_NonogramConfigScreenViewModel = null;

    GridMovementController m_GridMovementController = null;
    GridController m_GridSpawner = null;
    DrawingController m_DrawingController = null;

    private void Start()
    {
        GameManager.Get().GetGlobalEvents().e_OnTileClicked += OnTileClicked;

        m_GridSpawner = Instantiate(m_GridController);
        m_GridSpawner.Init();

        m_ConfigurationViewModel = ViewModelHelper.SpawnAndInitialize(m_ConfigurationViewModelRef);
        m_ConfigurationViewModel.GetStartButton().onClick.AddListener(OnStartButtonPressed);

        m_GridMovementController = Instantiate(m_GridMovementControllerRef);
        m_GridMovementController.Init(m_GridSpawner.GetGridHolder());

        m_NonogramConfigScreenViewModel = ViewModelHelper.SpawnAndInitialize(m_NonogramConfigScreenViewModelRef);
        InitializeNonogramConfigScreen();

        m_ControlBarViewModel = ViewModelHelper.SpawnAndInitialize(m_ControlBarViewModelRef);
        m_ControlBarViewModel.ChangeLivesTextVisibility(false);
        m_ControlBarViewModel.ChangeTimeTextVisibility(false);
        m_ControlBarViewModel.GetCreateNonogramButton().onClick.AddListener(OnCreateButtonClickedCallback);

        m_DrawingController = new DrawingController();
    }

    public void InitializeNonogramConfigScreen()
    {
        m_NonogramConfigScreenViewModel.GetCreateNonogramButton().onClick.AddListener(OnConfigScreenCreateNonogramButtonClicked);
        m_NonogramConfigScreenViewModel.GetNonogramCategoryDropDown().onValueChanged.AddListener(OnNonogramCategoryChanged);
        m_NonogramConfigScreenViewModel.GetCreateCategoryButton().onClick.AddListener(OnNewCategoryButtonClicked);

        m_NonogramConfigScreenViewModel.PopulateCategoriesDropdown(K_CREATE_NEW_CATEGORY_STRING);

        m_NonogramConfigScreenViewModel.SetSortingLayer("UI");
        m_NonogramConfigScreenViewModel.ChangeViewModelVisibility(false);
    }

    private void OnNonogramCategoryChanged(int optionIndex)
    {
        string selectedCategoryName = m_NonogramConfigScreenViewModel.GetSelectedCategoryName();

        //#TODO: Bit hacky. Find a better place to store info and do this logic
        if (selectedCategoryName == K_CREATE_NEW_CATEGORY_STRING)
        {
            m_NonogramConfigScreenViewModel.GetNewCategoryPanel().SetActive(true);
        }
    }

    private void OnNewCategoryButtonClicked()
    {
        string categoryName = m_NonogramConfigScreenViewModel.GetNewCategoryNameInputField().text;
        string categoryNameWithTags = "<b>" + categoryName + "</b>";
        //Activate little easter egg
        if (categoryNameWithTags ==  K_CREATE_NEW_CATEGORY_STRING)
        {
            categoryName = K_EASTER_EGG_CATEGORY_NAME;
        }
        
        if (NonogramHelpers.CreateNewCategory(categoryName))
        {
            //#TODO: notification
            GameManager.Get().LoadNonogramSets();
            m_NonogramConfigScreenViewModel.PopulateCategoriesDropdown(K_CREATE_NEW_CATEGORY_STRING);
            m_NonogramConfigScreenViewModel.GetNewCategoryPanel().SetActive(false);
        }
    }

    public void OnStartButtonPressed()
    {
        m_ConfigurationViewModel.ChangeViewModelVisibility(false);
        m_GridMovementController.SetCanDrag(true);
        m_GridWidth = (int)m_ConfigurationViewModel.GetWidthValueSelector().GetValue();
        m_GridHeight = (int)m_ConfigurationViewModel.GetHeightValueSelector().GetValue();

        m_ColorPickerViewModel = ViewModelHelper.SpawnAndInitialize(m_ColorPickerViewModelRef);

        StartCoroutine(m_GridSpawner.SpawnEmptyGrid(m_GridWidth, m_GridHeight));
    }

    private void OnCreateButtonClickedCallback()
    {
        m_NonogramConfigScreenViewModel.ChangeViewModelVisibility(true);
        m_NonogramConfigScreenViewModel.GetNewCategoryPanel().SetActive(false);
    }

    private void OnConfigScreenCreateNonogramButtonClicked()
    {
        string nonogramName = m_NonogramConfigScreenViewModel.GetNonogramNameInputField().text;
        string nonogramCategory = m_NonogramConfigScreenViewModel.GetSelectedCategoryName();

        string newID = SavegameManager.GenerateUniqueID();
        Nonogram newNonogram = m_GridSpawner.CreateNonogram();
        newNonogram.SetNonogramName(nonogramName);
        newNonogram.SetNonogramID(newID);
        NonogramHelpers.SaveNonogram(newNonogram, nonogramCategory);

        m_NonogramConfigScreenViewModel.GetNonogramNameInputField().text = string.Empty;
        m_NonogramConfigScreenViewModel.ChangeViewModelVisibility(false);
    }

    private void OnTileClicked(GridTile tile, KeyCode keyCode)
    {
        if (keyCode == KeyCode.Mouse0) // Left click
        {
            OnTileLeftMouseClicked(tile);
        }
        else if (keyCode == KeyCode.Mouse1) //Right click
        {
            OnTileRightMouseClicked(tile);
        }
        //#TODO: Middle mouse click
    }

    private void OnTileLeftMouseClicked(GridTile tile)
    {
        Color selectedColor = m_ColorPickerViewModel.GetSelectedColor();

        if (m_DrawingController.CanPaintTile(tile))
        {
            PaintTile(tile, selectedColor);
        }
    }

    private void OnTileRightMouseClicked(GridTile tile)
    {
        if (tile.GetIsColored() == false)
        {
            return;
        }

        if (m_DrawingController.CanPaintTile(tile) )
        {
            StripTile(tile);
        }
    }

    private void PaintTile(GridTile tile, Color selectedColor)
    {
        tile.SetRequiredColor(selectedColor);
        tile.Paint(selectedColor);
        int row = tile.GetWidthIndex() - 1;
        int column = tile.GetHeightIndex() - 1;
        UpdateClues(row, column);
    }

    private void StripTile(GridTile tile)
    {
        tile.Strip();
        int row = tile.GetWidthIndex() - 1;
        int column = tile.GetHeightIndex() - 1;
        UpdateClues(row, column);
    }

    private void UpdateClues(int row, int column)
    {
        m_GridSpawner.DeleteCluesFromWidget(row, column, false); //Horizontal
        m_GridSpawner.RefreshPixelCountWidget(row, column, false, true);
        m_GridSpawner.DeleteCluesFromWidget(row, column, true); //Vertical
        m_GridSpawner.RefreshPixelCountWidget(row, column, true, true);
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
        {
            m_DrawingController.ResetLineDrawing();
        }
    }

    public void OpenColorPicker()
    {

    }
}
