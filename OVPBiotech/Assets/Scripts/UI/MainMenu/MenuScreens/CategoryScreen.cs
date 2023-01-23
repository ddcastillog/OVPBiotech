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
        const string k_styleTgCategory = "tg-category";
        VisualElement m_CountainerCategory;
        GameData gameData;

        private List<Category> categoryList = new List<Category>();
        public static event Action<GameData> SettingsUpdated;

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
            foreach (DocumentSnapshot document in categorys.Documents)
            {
                categoryList.Add(document.ConvertTo<Category>());
            }

            initToggleCategory();
        }
        void initToggleCategory()
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
            SettingsUpdated?.Invoke(gameData);
            for (int i = 0; i < categoryList.Count; i++)
            {
                Toggle tg = new Toggle();
                tg.AddToClassList(k_styleTgCategory);
                tg.value = this.gameData.categoryList[i].IsSelect;
                tg?.RegisterCallback<ChangeEvent<bool>, int>(BtnCategory, i);
                tg.text = categoryList[i].name;
                m_CountainerCategory?.Add(tg);
            }
        }
        private void BtnCategory(ChangeEvent<bool> evt, int index)
        {
            AudioManager.PlayDefaultButtonSound();
            this.gameData.categoryList[index].IsSelect = evt.newValue;            
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