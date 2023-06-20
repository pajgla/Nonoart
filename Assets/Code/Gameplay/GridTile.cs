using UnityEngine;
using UnityEngine.UI;

public enum ETileDecoration
{
    None,
    RightSeparator,
    TopSeparator,
    TopRightSeparator
}

public enum ETileBackgroundColor
{
    White,
    Gray
}

[RequireComponent(typeof(ExpandedButton))]
public class GridTile : MonoBehaviour
{
    [SerializeField] Image m_BackgroundImageComponent = null;
    [SerializeField] Image m_ForegroundImageComponent = null;

    Color m_RequiredColor = Color.white;
    ETileBackgroundColor m_TileBackgroundColor = ETileBackgroundColor.White;
    bool m_IsColored = false;
    int m_WidthIndex = 0;
    int m_HeightIndex = 0;
    bool m_IsSolved = false;
    bool m_IsMarked = false;

    bool m_IsCursorOver = false;

    private void Update()
    {
        if (m_IsCursorOver)
        {
            GlobalEvents globalEvents = GameManager.Get().GetGlobalEvents();
            if (Input.GetMouseButton(0)) //Left click
            {
                globalEvents.Invoke_OnTileClicked(this, KeyCode.Mouse0);
            }
            else if (Input.GetMouseButton(1)) //Right click
            {
                globalEvents.Invoke_OnTileClicked(this, KeyCode.Mouse1);
            }
            else if (Input.GetMouseButton(2)) //Middle click
            {
                globalEvents.Invoke_OnTileClicked(this, KeyCode.Mouse2);
            }
        }
    }

    public void OnCursorEnter()
    {
        m_IsCursorOver = true;
    }

    public void OnCursorExit()
    {
        m_IsCursorOver = false;
    }

    public void Init(ComplitedTileData data)
    {
        SetRequiredColor(data.m_Color);
        m_WidthIndex = data.m_WidthIndex;
        m_HeightIndex = data.m_HeightIndex;
    }

    // Getters and Setters
    public Color GetRequiredColor() { return m_RequiredColor; }
    public void SetRequiredColor(Color color) 
    {
        m_RequiredColor = color;
        SetIsColored(true);
    }

    public void Solve(Color color)
    {
        if (color != GetRequiredColor())
        {
            Debug.LogError("Tried to solve a tile with wrong color");
            return;
        }
        if (GetIsColored() == false)
        {
            Debug.LogError("Trying to solve an empty tile");
            return;
        }

        m_ForegroundImageComponent.color = color;
        m_ForegroundImageComponent.enabled = true;

        SetIsMarked(false);
        SetIsSolved(true);

        GameManager.Get().GetGlobalEvents().Invoke_OnTileSolved(this);
    }

    //Used for tile creation
    public void Paint(Color color)
    {
        m_ForegroundImageComponent.color = color;
        m_ForegroundImageComponent.enabled = true;
        SetIsColored(true);
        SetIsMarked(false);
    }

    public void SetForegroundImage(Sprite sprite)
    {
        GetForegroundImageComponent().sprite = sprite;
        GetForegroundImageComponent().enabled = true;
    }

    public bool GetIsMarked() { return m_IsMarked; }
    public void SetIsMarked(bool isMarked) { m_IsMarked = isMarked; }
    public bool GetIsColored() { return m_IsColored; }
    public void SetIsColored(bool isColored) { m_IsColored = isColored; }
    public ETileBackgroundColor GetTileBackgroundColor() { return m_TileBackgroundColor; }
    public void SetTileBackgroundColor(ETileBackgroundColor color) { m_TileBackgroundColor = color; }
    public int GetWidthIndex() { return m_WidthIndex; }
    public void SetWidthIndex(int index) { m_WidthIndex = index; }
    public int GetHeightIndex() { return m_HeightIndex; }
    public void SetHeightIndex(int index) { m_HeightIndex = index;}
    public Image GetBackgroundImageComponent() { return m_BackgroundImageComponent; }
    public Image GetForegroundImageComponent() { return m_ForegroundImageComponent; }
    public bool GetIsSolved() { return m_IsSolved; }
    public void SetIsSolved(bool isSolved) { m_IsSolved = isSolved; }
}
