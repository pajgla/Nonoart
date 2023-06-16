using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UIViewModel
{
    public class NonogramConfigScreenViewModel : ViewModel
    {
        [SerializeField] GameObject m_CreatePanel = null;
        [SerializeField] GameObject m_NewCategoryPanel = null;
        [SerializeField] TMPro.TMP_InputField m_NonogramNameInputField = null;
        [SerializeField] TMPro.TMP_Dropdown m_NonogramCategoryDropDown = null;
        [SerializeField] Button m_CreateNonogramButton = null;
        [SerializeField] TMPro.TMP_InputField m_NewCategoryNameInputField = null;
        [SerializeField] Button m_CreateCategoryButton = null;

        //Logic

        public void PopulateCategoriesDropdown(string newCategoryString)
        {
            TMPro.TMP_Dropdown categoryDropDown = GetNonogramCategoryDropDown();
            categoryDropDown.ClearOptions();

            //Populate nonogram categories
            List<NonogramSet> nonogramSets = GameManager.Get().GetNonogramSets();
            List<TMPro.TMP_Dropdown.OptionData> categoryOptionDataList = new List<TMPro.TMP_Dropdown.OptionData>();
            foreach (NonogramSet set in nonogramSets)
            {
                TMPro.TMP_Dropdown.OptionData newOptionData = new TMPro.TMP_Dropdown.OptionData();
                newOptionData.text = set.GetName();

                categoryOptionDataList.Add(newOptionData);
            }

            //Add create new category option
            TMPro.TMP_Dropdown.OptionData createNewCategoryOptionData = new TMPro.TMP_Dropdown.OptionData();
            createNewCategoryOptionData.text = newCategoryString;
            categoryOptionDataList.Add(createNewCategoryOptionData);

            categoryDropDown.AddOptions(categoryOptionDataList);

            
        }

        //Getters And Setters
        public GameObject GetCreatePanel() { return m_CreatePanel; }
        public GameObject GetNewCategoryPanel() { return m_NewCategoryPanel; }
        public TMPro.TMP_InputField GetNonogramNameInputField() {  return m_NonogramNameInputField; }
        public TMPro.TMP_Dropdown GetNonogramCategoryDropDown() { return m_NonogramCategoryDropDown; }
        public Button GetCreateNonogramButton() {  return m_CreateNonogramButton; }
        public TMPro.TMP_InputField GetNewCategoryNameInputField() { return m_NewCategoryNameInputField; }
        public Button GetCreateCategoryButton() {  return m_CreateCategoryButton; }

        public string GetSelectedCategoryName()
        {
            return GetNonogramCategoryDropDown().captionText.text;
        }
    }
}
