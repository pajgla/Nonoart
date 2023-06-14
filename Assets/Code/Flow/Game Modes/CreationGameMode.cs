using UIViewModel;
using UnityEngine;
using UnityEngine.UI;

public class CreationGameMode : GameMode
{
    int m_GridWidth = 0;
    int m_GridHeight = 0;

    [Header("References")]
    [SerializeField] ColorPickerViewModel m_ColorPickerViewModelRef = null;
    [SerializeField] GridConfigurationViewModel m_ConfigurationViewModelRef = null;
    [SerializeField] CreationControlBarViewModel m_ControlBarViewModelRef = null;
    [SerializeField] NonogramConfigScreenViewModel m_NonogramConfigScreenViewModelRef = null;

    ColorPickerViewModel m_ColorPickerViewModel = null;
    GridConfigurationViewModel m_ConfigurationViewModel = null;
    CreationControlBarViewModel m_ControlBarViewModel = null;
    NonogramConfigScreenViewModel m_NonogramConfigScreenViewModel = null;

    GridMovementController m_GridMovementController = null;
    GridController m_GridSpawner = null;

    //Line drawing
    [SerializeField] bool m_LineDrawing = false;
    bool m_IsLineDrawingInProgress = false;
    Vector2 m_StartingLineDrawingTilePosition = Vector2.zero;
    Vector2 m_LineDrawingDirection = Vector2.zero;

    private void Start()
    {
        GameManager.Get().GetGlobalEvents().e_OnTileClicked += OnTileClicked;

        m_GridSpawner = Instantiate(m_GridController);
        m_GridSpawner.Init();

        m_ConfigurationViewModel = ViewModelHelper.SpawnAndInitialize(m_ConfigurationViewModelRef);
        m_ConfigurationViewModel.GetStartButton().onClick.AddListener(OnStartButtonPressed);

        m_GridMovementController = Instantiate(m_GridMovementControllerRef);
        m_GridMovementController.Init(m_GridSpawner.GetGridHolder());
    }

    public void OnStartButtonPressed()
    {
        m_ConfigurationViewModel.ChangeViewModelVisibility(false);
        m_GridMovementController.SetCanDrag(true);
        m_GridWidth = (int)m_ConfigurationViewModel.GetWidthValueSelector().GetValue();
        m_GridHeight = (int)m_ConfigurationViewModel.GetHeightValueSelector().GetValue();

        m_ColorPickerViewModel = ViewModelHelper.SpawnAndInitialize(m_ColorPickerViewModelRef);

        m_ControlBarViewModel = ViewModelHelper.SpawnAndInitialize(m_ControlBarViewModelRef);
        m_ControlBarViewModel.GetCreateButton().onClick.AddListener(OnCreateButtonClickedCallback);

        StartCoroutine(m_GridSpawner.SpawnEmptyGrid(m_GridWidth, m_GridHeight));
    }

    private void OnCreateButtonClickedCallback()
    {
        Nonogram newNonogram = m_GridSpawner.CreateNonogram();
        newNonogram.SetNonogramName("Testing");
        NonogramHelpers.SaveNonogram(newNonogram, "Beginner");

        //m_NonogramConfigScreenViewModel = ViewModelHelper.SpawnAndInitialize(m_NonogramConfigScreenViewModelRef);
        //m_NonogramConfigScreenViewModel.GetNewCategoryPanel().SetActive(false);
    }

    private void OnTileClicked(GridTile tile, KeyCode keyCode)
    {
        if (keyCode == KeyCode.Mouse0)
        {
            OnTileLeftMouseClicked(tile);
        }
        //#TODO: Middle mouse click
    }

    private void OnTileLeftMouseClicked(GridTile tile)
    {
        Color selectedColor = m_ColorPickerViewModel.GetSelectedColor();

        if (tile.GetRequiredColor() == selectedColor)
        {
            return;
        }

        if (m_LineDrawing)
        {
            if (m_IsLineDrawingInProgress)
            {
                Vector2 currentTilePos = new Vector2(tile.GetWidthIndex(), tile.GetHeightIndex());
                Vector2 normalizedDir = (currentTilePos - m_StartingLineDrawingTilePosition).normalized;
                if (new Vector2(Mathf.Abs(normalizedDir.x), Mathf.Abs(normalizedDir.y)) == new Vector2(Mathf.Abs(m_LineDrawingDirection.x), Mathf.Abs(m_LineDrawingDirection.y)))
                {
                    PaintTile(tile, selectedColor);
                }
            }
            else
            {
                if (m_StartingLineDrawingTilePosition == Vector2.zero)
                {
                    m_StartingLineDrawingTilePosition = new Vector2(tile.GetWidthIndex(), tile.GetHeightIndex());
                }
                else
                {
                    m_LineDrawingDirection = (new Vector2(tile.GetWidthIndex(), tile.GetHeightIndex()) - m_StartingLineDrawingTilePosition).normalized;
                    m_IsLineDrawingInProgress = true;
                }

                PaintTile(tile, selectedColor);
            }
        }
        else
        {
            PaintTile(tile, selectedColor);
        }
    }

    private void PaintTile(GridTile tile, Color selectedColor)
    {
        tile.SetRequiredColor(selectedColor);
        tile.Paint(selectedColor);
        int row = tile.GetWidthIndex() - 1;
        int column = tile.GetHeightIndex() - 1;
        m_GridSpawner.DeleteCluesFromWidget(row, column, false); //Horizontal
        m_GridSpawner.RefreshPixelCountWidget(row, column, false, true);
        m_GridSpawner.DeleteCluesFromWidget(row, column, true); //Vertical
        m_GridSpawner.RefreshPixelCountWidget(row, column, true, true);
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            m_IsLineDrawingInProgress = false;
            m_StartingLineDrawingTilePosition = Vector2.zero;
            m_LineDrawingDirection = Vector2.zero;
        }
    }

    public void OpenColorPicker()
    {

    }
}
