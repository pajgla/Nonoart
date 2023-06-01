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

        List<CategorySelectionWidget> m_InstantiatedCategoryWidgets = new List<CategorySelectionWidget>();

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
            foreach (Nonogram nonogram in set.GetNonograms())
            {
                LevelSelectionWidget newWidget = Instantiate(m_LevelSelectionWidgetPrefab);
                newWidget.SetLevelImage(nonogram.ConvertToTexture());
                newWidget.transform.SetParent(m_LevelsHolder, false);
                newWidget.SetNonogram(nonogram);
                newWidget.OnLevelSelectedEvent += OnLevelSelected;
            }
        }

        private void OnLevelSelected(Nonogram nonogram)
        {
            GameModeData gameModeData = new GameModeData();
            gameModeData.m_Nonogram = nonogram;
            GameManager.Get().StartGameMode(EGameModeType.Solving, gameModeData);
        }
    }
}
