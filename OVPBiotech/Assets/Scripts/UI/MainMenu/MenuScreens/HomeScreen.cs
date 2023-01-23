using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System;



namespace OVPBiotechSpace
{
    public class HomeScreen : MenuScreen
    {
        public static event Action HomeScreenShown;
        const string k_PlayLevelButtonName = "home-play__level-button";

        VisualElement m_PlayLevelButton;        

        protected override void SetVisualElements()
        {
            base.SetVisualElements();
            m_PlayLevelButton = m_Root.Q(k_PlayLevelButtonName);          
        }        
        

        protected override void RegisterButtonCallbacks()
        {
            m_PlayLevelButton?.RegisterCallback<ClickEvent>(ClickPlayButton);
        }

        private void ClickPlayButton(ClickEvent evt)
        {
            AudioManager.PlayDefaultButtonSound();
            SceneManager.LoadSceneAsync((int)NumberScenes.SELECT_CHARACTER);
        }

        public override void ShowScreen()
        {
            base.ShowScreen();
            HomeScreenShown?.Invoke();
        }
    }
}