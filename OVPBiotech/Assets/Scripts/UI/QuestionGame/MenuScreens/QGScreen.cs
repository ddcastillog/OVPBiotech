using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Firebase.Firestore;
using Firebase.Extensions;
using System;

namespace OVPBiotechSpace
{
    public class QGScreen : MenuScreenQG
    {
        [SerializeField]
        private string pathquestion = "question";
        [SerializeField]
        private string pathdifficultyLevel = "difficultyLevel";
        [SerializeField]
        private string pathcategory = "category";
        [SerializeField]
        private List<GameObject> character;

        #region  UI-Style
        const string k_BtnOptions = "btn-option";
        const string k_lblOptions = "lbl-option";
        const string k_lblQuestion = "lbl-question";
        const string k_IsCorrect = "bnt-IsCorrect";
        const string k_IsIncorrect = "btn-IsIncorrect";
        const string k_QGPanelActive = "qg-panel-active";
        const string k_QGPanel = "QGPanel";
        const string k_visibilityOff = "visibility-off";
        const string k_displayOn = "display-on";
        #endregion

        #region Variables
        FirebaseFirestore db;
        Label m_lblQuestion;
        VisualElement m_QGPanel;
        List<Button> m_BtnOptions = new List<Button>();
        List<Label> m_lblOptions = new List<Label>();
        List<Question> questionsList = new List<Question>();
        List<Question> questionsListAll = new List<Question>();
        List<DifficultyLevel> difficultyLevelList = new List<DifficultyLevel>();
        List<Category> categoryList = new List<Category>();
        int indexQuestions = 0;
        int indexDifficultyLevel = 0;
        int indexDifficultyLevelScale;
        bool isClick = true;
        private int correctAnswerOptions = 0;
        private int AnswerQuestions = 0;
        private int AnswerQuestionsConsecutively = 0;
        //Action
        public static event Action<String> UpdateScore;
        public static event Action<int> UpdateQuestions;
        public static event Action ResetGame;
        #endregion

        void OnEnable()
        {
            QGScoreScreen.ReplayGame += ReplayGame;
            WildcardScreen.Update5050 += update5050;
            WildcardScreen.UpdateAudience += updateAudience;
            WildcardScreen.UpdateSwitch += updateSwitch;
            SaveManager.GameDataLoaded += OnGameDataLoaded;
        }

        void OnDisable()
        {
            QGScoreScreen.ReplayGame -= ReplayGame;
            WildcardScreen.Update5050 -= update5050;
            WildcardScreen.UpdateAudience -= updateAudience;
            WildcardScreen.UpdateSwitch -= updateSwitch;
            SaveManager.GameDataLoaded -= OnGameDataLoaded;
        }
        protected override void SetVisualElements()
        {
            base.SetVisualElements();
            m_lblQuestion = m_Root.Q<Label>(k_lblQuestion);
            m_QGPanel = m_Root.Q<VisualElement>(k_QGPanel);
            for (int i = 1; i < 5; i++)
            {
                m_BtnOptions.Add(m_Root.Q<Button>(k_BtnOptions + i));
                m_lblOptions.Add(m_Root.Q<Label>(k_lblOptions + i));
            }
            db = FirebaseFirestore.DefaultInstance;
            GetData();
        }

        protected override void RegisterButtonCallbacks()
        {
            for (int i = 0; i < 4; i++)
            {
                m_BtnOptions[i]?.RegisterCallback<ClickEvent, int>(IsAnswerCorrect, i);
            }
        }
        void GetData()
        {
            db.Collection(pathcategory).GetSnapshotAsync(Source.Cache).ContinueWithOnMainThread(task =>
            {
                QuerySnapshot snapshot = task.Result;
                foreach (DocumentSnapshot document in snapshot.Documents)
                {
                    categoryList.Add(document.ConvertTo<Category>());
                }
            });
            db.Collection(pathdifficultyLevel).OrderBy("min").GetSnapshotAsync(Source.Cache).ContinueWithOnMainThread(task =>
            {
                QuerySnapshot snapshot = task.Result;
                foreach (DocumentSnapshot document in snapshot.Documents)
                {
                    difficultyLevelList.Add(document.ConvertTo<DifficultyLevel>());
                }
                if (difficultyLevelList.Count > 0)
                {
                    indexDifficultyLevelScale = difficultyLevelList[indexDifficultyLevel].min;
                    db.Collection(pathquestion).GetSnapshotAsync(Source.Cache).ContinueWithOnMainThread(task =>
                    {
                        QuerySnapshot snapshot = task.Result;
                        foreach (DocumentSnapshot document in snapshot.Documents)
                        {
                            questionsListAll.Add(document.ConvertTo<Question>());
                        }
                        getQuestions();
                    });

                }
                else
                {
                    print("No hay niveles de dificultad");
                }
            });
        }
        void getQuestions()
        {
            questionsList = questionsListAll.FindAll(q => q.q_scaleDifficulty == indexDifficultyLevelScale);
            while (questionsList.Count <= 0 && indexDifficultyLevel < difficultyLevelList.Count)
            {
                if (indexDifficultyLevelScale < difficultyLevelList[indexDifficultyLevel].max)
                {
                    indexDifficultyLevelScale++;
                }
                else
                {
                    indexDifficultyLevel++;
                    if (indexDifficultyLevel < difficultyLevelList.Count)
                    {
                        indexDifficultyLevelScale = difficultyLevelList[indexDifficultyLevel].min;
                    }
                }
                questionsList = questionsListAll.FindAll(q => q.q_scaleDifficulty == indexDifficultyLevelScale);
                print(questionsList.Count);
            }
            if (questionsList.Count > 0)
            {
                NextQuestions();
            }
            else
            {
                print("No hay preguntas");
            }
        }        
        private void IsAnswerCorrect(ClickEvent e, int index)
        {
            if (isClick)
            {
                if (indexQuestions < questionsList.Count)
                {
                    if (questionsList[indexQuestions].q_option_correct == (index + 1))
                    {
                        AudioManager.PlayVictorySound();
                        correctAnswerOptions++;
                        AnswerQuestionsConsecutively++;
                        StartCoroutine("animationCorrect", index);
                    }
                    else
                    {
                        AudioManager.PlayDefeatSound();
                        AnswerQuestionsConsecutively = 0;
                        StartCoroutine("animationIncorrect", index);
                    }
                    AnswerQuestions++;
                }
            }
        }
        IEnumerator animationCorrect(int index)
        {
            isClick = false;
            m_BtnOptions[index].AddToClassList(k_IsCorrect);
            yield return new WaitForSeconds(.5f);
            m_BtnOptions[index].RemoveFromClassList(k_IsCorrect);
            indexQuestions++;
            NextQuestions();
            isClick = true;
        }
        IEnumerator animationIncorrect(int index)
        {
            isClick = false;
            m_BtnOptions[index].AddToClassList(k_IsIncorrect);
            yield return new WaitForSeconds(.5f);
            m_BtnOptions[index].RemoveFromClassList(k_IsIncorrect);
            indexQuestions++;
            NextQuestions();
            isClick = true;
        }
        void ReplayGame()
        {
            ResetGame.Invoke();
            indexQuestions = 0;
            correctAnswerOptions = 0;
            AnswerQuestions = 0;
            NextQuestions();
            m_QGPanel.AddToClassList(k_QGPanelActive);
        }
        void NextQuestions()
        {
            ResetStyle();
            if (indexQuestions < questionsList.Count)
            {
                m_QGPanel.AddToClassList(k_QGPanelActive);
                UpdateQuestions.Invoke(questionsList[indexQuestions].q_option_correct);
                m_lblQuestion.text = questionsList[indexQuestions].q_question;
                m_BtnOptions[0].text = questionsList[indexQuestions].q_option1;
                m_BtnOptions[1].text = questionsList[indexQuestions].q_option2;
                m_BtnOptions[2].text = questionsList[indexQuestions].q_option3;
                m_BtnOptions[3].text = questionsList[indexQuestions].q_option4;
            }
            else
            {
                m_QGPanel.RemoveFromClassList(k_QGPanelActive);
                UpdateScore.Invoke(correctAnswerOptions + "/" + AnswerQuestions);
                m_MainMenuUIManager?.ShowQGScoreScreen();
            }
        }
        void ResetStyle()
        {
            for (int i = 0; i < 4; i++)
            {
                m_BtnOptions[i].RemoveFromClassList(k_visibilityOff);
                m_lblOptions[i].RemoveFromClassList(k_displayOn);
            }
        }
        #region Metodo Action
        void update5050(int[] wrongAnswers)
        {
            m_BtnOptions[wrongAnswers[0] - 1].AddToClassList(k_visibilityOff);
            m_BtnOptions[wrongAnswers[1] - 1].AddToClassList(k_visibilityOff);
            m_lblOptions[wrongAnswers[0] - 1].RemoveFromClassList(k_displayOn);
            m_lblOptions[wrongAnswers[1] - 1].RemoveFromClassList(k_displayOn);
        }
        void updateAudience(int[] results, bool used5050)
        {
            for (int i = 0; i < 4; i++)
            {
                if (results[i] != 0 || !used5050)
                {
                    m_lblOptions[i].AddToClassList(k_displayOn);
                    m_lblOptions[i].text = results[i] + "%";
                }
            }
        }
        void updateSwitch()
        {
            indexQuestions++;
            NextQuestions();
        }
        #endregion
        // syncs saved data from the GameDataManager to the UI elements
        void OnGameDataLoaded(GameData gameData)
        {
            if (gameData == null)
                return;
            if (gameData.selectCharacter < character.Count)
            {
                character[gameData.selectCharacter].SetActive(true);
            }
        }
    }
}
