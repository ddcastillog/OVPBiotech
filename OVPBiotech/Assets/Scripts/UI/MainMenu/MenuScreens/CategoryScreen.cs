using System;
using UnityEngine;
using UnityEngine.UIElements;
using Firebase.Firestore;
using Firebase.Extensions;
using System.Collections.Generic;

namespace OVPBiotechSpace
{

    // links to additional UI Toolkit resources
    public class CategoryScreen : MenuScreen
    {
        [SerializeField]
        private string pathcategory = "category";
        const string k_CountainerCategory = "countainer__categoria";
        const string k_styleVSCategory = "vs-category";
        const string k_styleVSCategoryActive = "vs-category-active";
        const string k_lblCategory = "lbl-category";
        VisualElement m_CountainerCategory;
        GameData gameData;

        private List<Category> categoryList = new List<Category>();
        public static event Action<GameData> SettingsUpdated;
        List<VisualElement> listBtn;
        void OnEnable()
        {
            SaveManager.GameDataLoaded += OnGameDataLoaded;
        }

        void OnDisable()
        {
            SaveManager.GameDataLoaded -= OnGameDataLoaded;
        }
        protected override void SetVisualElements()
        {
            base.SetVisualElements();
            m_CountainerCategory = m_Root.Q<VisualElement>(k_CountainerCategory);
        }

        protected override void RegisterButtonCallbacks()
        {
            base.RegisterButtonCallbacks();
            getCategory();
        }

        async void getCategory()
        {
            FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
            QuerySnapshot categorys;
            categorys = await db.Collection(pathcategory).GetSnapshotAsync(Source.Cache);
            if (categorys.Count > 0)
            {
                foreach (DocumentSnapshot document in categorys.Documents)
                {
                    categoryList.Add(document.ConvertTo<Category>());
                }
            }
            else
            {
                categorys = await db.Collection(pathcategory).GetSnapshotAsync();
                foreach (DocumentSnapshot document in categorys.Documents)
                {
                    categoryList.Add(document.ConvertTo<Category>());
                }
            }
            initBtnCategory();
        }
        void initBtnCategory()
        {
            if (this.gameData.categoryList.Count <= 0)
            {
                foreach (var c in categoryList)
                {
                    this.gameData.categoryList.Add(new CategorySave(c.id));
                }
            }
            else
            {
                List<CategorySave> aux = new List<CategorySave>();
                foreach (var c in categoryList)
                {
                    CategorySave auxList = this.gameData.categoryList.Find(a => a.id == c.id);
                    if (auxList != null)
                    {
                        aux.Add(new CategorySave(c.id, auxList.IsSelect));                        
                    }
                    else
                    {
                        aux.Add(new CategorySave(c.id));
                    }
                }
                this.gameData.categoryList = aux;
            }
            for (int i = 0; i < categoryList.Count; i++)
            {
                VisualElement aux = new VisualElement();
                aux.AddToClassList(k_styleVSCategory);
                if (this.gameData.categoryList[i].IsSelect)
                {
                    aux.AddToClassList(k_styleVSCategoryActive);
                }
                aux?.RegisterCallback<ClickEvent, int>(BtnCategory, i);
                //image
                Label text = new Label();
                text.AddToClassList(k_lblCategory);
                text.text = categoryList[i].name;
                //Addd
                aux.Add(text);
                m_CountainerCategory?.Add(aux);
            }
            listBtn = (List<VisualElement>)m_CountainerCategory.Children();

        }
        private void BtnCategory(ClickEvent e, int index)
        {
            AudioManager.PlayDefaultButtonSound();
            listBtn[index].AddToClassList(k_styleVSCategoryActive);
            if (this.gameData.categoryList[index].IsSelect)
            {
                this.gameData.categoryList[index].IsSelect = false;
                listBtn[index].RemoveFromClassList(k_styleVSCategoryActive);
            }
            else
            {
                this.gameData.categoryList[index].IsSelect = true;
                listBtn[index].AddToClassList(k_styleVSCategoryActive);
            }                 
            SettingsUpdated?.Invoke(gameData);
        }

        void OnGameDataLoaded(GameData gameData)
        {
            if (gameData == null)
                return;
            this.gameData = gameData;
            SettingsUpdated?.Invoke(this.gameData);
        }
    }
}