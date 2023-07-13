using Save;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIViewModel
{
    public class LevelSelectionViewModel : ViewModel
    {
        [Header("References")]
        [SerializeField] Transform m_CategoriesHolder = null;
        [SerializeField] Transform m_LevelsHolder = null;
        [SerializeField] CategorySelectionWidget m_CategorySelectionWidgetPrefab = null;
        [SerializeField] LevelSelectionWidget m_LevelSelectionWidgetPrefab = null;
        [SerializeField] Sprite m_UncompleteLevelSprite = null;

        NonogramSet m_SelectedNonogramSet = null;
        List<CategorySelectionWidget> m_InstantiatedCategoryWidgets = new List<CategorySelectionWidget>();
        List<LevelSelectionWidget> m_InstantiatedLevelSelectionWidgets = new List<LevelSelectionWidget>();

        public CategorySelectionWidget GetCategorySelectionWidgetPrefab() { return m_CategorySelectionWidgetPrefab; }

        public override void Initialize()
        {
            base.Initialize();

            GameManager gameManager = GameManager.Get();
            List<NonogramSet> sets = gameManager.GetNonogramSets();

            foreach (NonogramSet set in sets)
            {
                CategorySelectionWidget newCategoryWidget = Instantiate(m_CategorySelectionWidgetPrefab);
                newCategoryWidget.Init(set);
                newCategoryWidget.OnCategorySelectedEvent += OnCategorySelected;
                newCategoryWidget.transform.SetParent(m_CategoriesHolder, false);

                m_InstantiatedCategoryWidgets.Add(newCategoryWidget);
            }

            if (sets.Count > 0)
            {
                m_InstantiatedCategoryWidgets[0].OnCategorySelected();
            }
        }

        private void OnCategorySelected(NonogramSet set)
        {
            if (m_SelectedNonogramSet == set)
            {
                return;
            }

            //Delete old widgets
            foreach (LevelSelectionWidget levelWidget in m_InstantiatedLevelSelectionWidgets)
            {
                Destroy(levelWidget.gameObject);
            }
            m_InstantiatedLevelSelectionWidgets.Clear();

            foreach (Nonogram nonogram in set.GetNonograms())
            {
                LevelSelectionWidget newWidget = Instantiate(m_LevelSelectionWidgetPrefab);
                bool isCompleted = false;
                if (SavegameManager.Get().CheckIfNonogramSaveExists(nonogram.GetNonogramID()))
                {
                    NonogramCompletionSaveData saveData = SavegameManager.Get().LoadNonogramData(nonogram.GetNonogramID());
                    isCompleted = saveData != null && saveData.m_IsCompleted;
                }

                if (isCompleted)
                {
                    newWidget.SetLevelImage(nonogram.ConvertToTexture());
                }
                else
                {
                    newWidget.SetLevelImage(m_UncompleteLevelSprite);
                }
                newWidget.transform.SetParent(m_LevelsHolder, false);
                newWidget.SetNonogram(nonogram);
                newWidget.OnLevelSelectedEvent += OnLevelSelected;

                m_InstantiatedLevelSelectionWidgets.Add(newWidget);
            }

            m_SelectedNonogramSet = set;
        }

        private void OnLevelSelected(Nonogram nonogram)
        {
            GameModeData gameModeData = new GameModeData();
            gameModeData.m_Nonogram = nonogram;
            GameManager.Get().StartGameMode(EGameModeType.Solving, gameModeData);
        }
    }
}
