using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using Firebase.Firestore;
using Firebase.Extensions;
using UnityEngine.Assertions;
using Unity.VisualScripting;

namespace OVPBiotechSpace
{
    public class QGScreen : MenuScreenQG
    {
        [SerializeField]
        private string pathquestion = "question";
        const string k_BtnQuestions = "btn-option";
        const string k_lblQuestion = "lbl-question";
        const string k_IsCorrect = "bnt-IsCorrect";
        const string k_IsIncorrect = "btn-IsIncorrect";
        FirebaseFirestore db;
        List<Button> m_BtnQuestions = new List<Button>();
        List<Question> questionsList;
        void OnEnable()
        {
           
        }
        protected override void SetVisualElements()
        {
            base.SetVisualElements();
            for (int i = 1; i < 5; i++)
            {
                m_BtnQuestions.Add(m_Root.Q<Button>(k_BtnQuestions + i));
            }
            db = FirebaseFirestore.DefaultInstance;
            getQuestions();
        }

        protected override void RegisterButtonCallbacks()
        {
            for (int i = 0; i < 4; i++)
            {
                m_BtnQuestions[i]?.RegisterCallback<ClickEvent, int>(IsAnswerCorrect, i);
            }
        }
        private void IsAnswerCorrect(ClickEvent e, int index)
        {
            AudioManager.PlayVictorySound();
            m_BtnQuestions[index].AddToClassList(k_IsCorrect);
        }
        void getQuestions()
        {            
            db.Collection(pathquestion).GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                Assert.IsNull(task.Exception);
                questionsList = task.Result.ConvertTo<List<Question>>();
                Debug.Log("Hola");
                Debug.Log(questionsList[0].);
            });
        }
    }
}
