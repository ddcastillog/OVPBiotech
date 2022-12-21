using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System;
using Firebase.Firestore;
using Firebase.Extensions;

namespace OVPBiotechSpace
{
    public class HomeScreen : MenuScreen
    {
        public static event Action HomeScreenShown;
        const string k_PlayLevelButtonName = "home-play__level-button";

        VisualElement m_PlayLevelButton;

        [SerializeField]
        private string pathquestion = "question";
        [SerializeField]
        private string pathdifficultyLevel = "difficultyLevel";
        [SerializeField]
        private string pathcategory = "category";
        

        protected override void SetVisualElements()
        {
            base.SetVisualElements();
            m_PlayLevelButton = m_Root.Q(k_PlayLevelButtonName);           
            getData();
        }
        void getData()
        {
            FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
            //db.TerminateAsync();
            //db.ClearPersistenceAsync();
            db.Collection(pathquestion).GetSnapshotAsync(Source.Cache).ContinueWithOnMainThread(task =>
            {
                if (task.Result.Count <= 0)
                {
                    db.Collection(pathquestion).GetSnapshotAsync();
                }
            });
            db.Collection(pathdifficultyLevel).GetSnapshotAsync(Source.Cache).ContinueWithOnMainThread(task =>
            {
                if (task.Result.Count <= 0)
                {
                    db.Collection(pathdifficultyLevel).GetSnapshotAsync();
                }
            });
            db.Collection(pathcategory).GetSnapshotAsync(Source.Cache).ContinueWithOnMainThread(task =>
            {
                if (task.Result.Count <= 0)
                {
                    db.Collection(pathcategory).GetSnapshotAsync();
                }
            });
        }
        

        protected override void RegisterButtonCallbacks()
        {
            m_PlayLevelButton?.RegisterCallback<ClickEvent>(ClickPlayButton);
        }

        private void ClickPlayButton(ClickEvent evt)
        {
            AudioManager.PlayDefaultButtonSound();
            SceneManager.LoadSceneAsync(1);
        }

        public override void ShowScreen()
        {
            base.ShowScreen();
            HomeScreenShown?.Invoke();
        }
    }
}