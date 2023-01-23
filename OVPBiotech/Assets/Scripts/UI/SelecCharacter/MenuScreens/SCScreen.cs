using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System;

namespace OVPBiotechSpace
{
    public class SCScreen : MenuScreenSC
    {
        const string k_BtnStartGame = "btn-Start-game";
        const string k_BtnSC = "btn-sc";
        const string k_countainerBtnSC = "countainer-btnSC";
        const string k_lblName = "lbl-name";
        const string k_lblDescription = "lbl-description";
        const string k_imageStyle = "btn-image";
        const string k_BtnActive = "btn-sc-active";

        [SerializeField]
        private List<GameObject> models;
        [SerializeField]
        private string pathCharacters = "GameData/Characters";
        //variables
        Button m_BtnStartGame;
        ScrollView m_countainerBtnSC;
        Label m_lblName;
        Label m_lblDescription;
        private Character[] Characters;
        private List<VisualElement> m_BtnSC = new List<VisualElement>();
        GameData gameData;
        //Action
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
            m_BtnStartGame = m_Root.Q<Button>(k_BtnStartGame);
            m_countainerBtnSC = m_Root.Q<ScrollView>(k_countainerBtnSC);
            m_lblName = m_Root.Q<Label>(k_lblName);
            m_lblDescription = m_Root.Q<Label>(k_lblDescription);
            Characters = Resources.LoadAll<Character>(pathCharacters);
        }

        protected override void RegisterButtonCallbacks()
        {
            m_BtnStartGame?.RegisterCallback<ClickEvent>(BtnStartGame);
            for (int i = 0; i < models.Count; i++)
            {
                VisualElement aux = new VisualElement();
                aux.AddToClassList(k_BtnSC);
                aux?.RegisterCallback<ClickEvent, int>(btnSC, i);                
                //image
                VisualElement image = new VisualElement();
                image.AddToClassList(k_imageStyle);
                image.style.backgroundImage = new StyleBackground(Characters[i].btnImage);
                //Addd
                aux.Add(image);
                m_countainerBtnSC?.Add(aux);
                m_BtnSC.Add(aux);
            }
            
        }
        private void btnSC(ClickEvent e, int index)
        {
            AudioManager.PlayDefaultButtonSound();
            showChracter(index);
            gameData.selectCharacter = index;
            SettingsUpdated?.Invoke(gameData);
        }
        private void showChracter(int index)
        {
            for (int i = 0; i < models.Count; i++)
            {
                models[i].SetActive(false);
                m_BtnSC[i].RemoveFromClassList(k_BtnActive);
            }
            models[index].SetActive(true);
            m_BtnSC[index].AddToClassList(k_BtnActive);
            m_lblName.text = Characters[index].nombre;
            m_lblDescription.text = Characters[index].description;
        }
        private void BtnStartGame(ClickEvent e)
        {
            SceneManager.LoadSceneAsync((int)NumberScenes.QUESTIONGAME);
        }
        void OnGameDataLoaded(GameData gameData)
        {
            if (gameData == null)
                return;
            this.gameData = gameData;
            showChracter(gameData.selectCharacter);
            SettingsUpdated?.Invoke(this.gameData);
        }
    }
}
