using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using Firebase.Firestore;
using Firebase.Extensions;
using UnityEngine.Assertions;
using Unity.VisualScripting;
using System;

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
        Label m_lblQuestion;
        List<Button> m_BtnQuestions = new List<Button>();
        List<Question> questionsList = new List<Question>();
        int indexQuestions = 0;            
        protected override void SetVisualElements()
        {
            base.SetVisualElements();
            m_lblQuestion = m_Root.Q<Label>(k_lblQuestion);
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
            if (indexQuestions < questionsList.Count)
            {
                if (questionsList[indexQuestions].q_option_correct == (index+1))
                {
                    AudioManager.PlayVictorySound();
                    m_BtnQuestions[index].AddToClassList(k_IsCorrect);
                }
                else
                {
                    AudioManager.PlayDefeatSound();
                    m_BtnQuestions[index].AddToClassList(k_IsIncorrect);
                }
                indexQuestions++;
                NextQuestions();
            }            
        }
        void getQuestions()
        {            
            db.Collection(pathquestion).GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                QuerySnapshot snapshot = task.Result;                 
                foreach (DocumentSnapshot document in snapshot.Documents)
                {
                    questionsList.Add(document.ConvertTo<Question>());                   
                }
                NextQuestions();
            });            
        }
        void NextQuestions()
        {
            if (indexQuestions < questionsList.Count)
            {
                m_lblQuestion.text= questionsList[indexQuestions].q_question;
                m_BtnQuestions[0].text = questionsList[indexQuestions].q_option1;
                m_BtnQuestions[1].text = questionsList[indexQuestions].q_option2;
                m_BtnQuestions[2].text = questionsList[indexQuestions].q_option3;
                m_BtnQuestions[3].text = questionsList[indexQuestions].q_option4;                
            }
        }
    }
}
