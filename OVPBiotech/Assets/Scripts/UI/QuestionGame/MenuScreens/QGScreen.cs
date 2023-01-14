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
        private List<GameObject> character;
        [SerializeField]
        private int maxQuestions;
        [SerializeField]
        private int rangeLevelUpDifficulty;

        #region  UI-Style
        const string k_BtnOptions = "btn-option";
        const string k_LblText = "lbl-text";
        const string k_lblOptions = "lbl-option";
        const string k_lblQuestion = "lbl-question";
        const string k_IsCorrect = "bnt-IsCorrect";
        const string k_IsIncorrect = "btn-IsIncorrect";
        const string k_QGPanelActive = "qg-panel-active";
        const string k_QGPanel = "QGPanel";
        const string k_visibilityOff = "visibility-off";
        const string k_displayOn = "display-on";
        const string k_vsExplanation = "vs-explanation";
        const string k_lblExplanation = "lbl-explanation";        
        #endregion

        #region Variables
        FirebaseFirestore db;
        Label m_lblQuestion;
        VisualElement m_QGPanel;
        VisualElement m_vsExplanation;
        Label m_lblExplanation;
        List<Button> m_BtnOptions = new List<Button>();
        List<Label> m_LblText = new List<Label>();
        List<Label> m_lblOptions = new List<Label>();
        List<Question> questionsList = new List<Question>();
        List<Question> questionsListAll = new List<Question>();
        List<DifficultyLevel> difficultyLevelList = new List<DifficultyLevel>();
        List<String> categorySaves = new List<String>();
        //Questions
        private int indexQuestionsRandom;
        private int correctAnswerOptions = 0;
        private int AnswerQuestions = 0;
        private int AnswerQuestionsConsecutively = 0;

        int indexDifficultyLevel = 0;
        int indexDifficultyLevelScale;
        bool isClick = true;
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
            m_vsExplanation = m_Root.Q<VisualElement>(k_vsExplanation);
            m_lblExplanation = m_Root.Q<Label>(k_lblExplanation);
            for (int i = 1; i < 5; i++)
            {
                m_BtnOptions.Add(m_Root.Q<Button>(k_BtnOptions + i));
                m_lblOptions.Add(m_Root.Q<Label>(k_lblOptions + i));
                m_LblText.Add(m_Root.Q<Label>(k_LblText + i));
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
            m_vsExplanation?.RegisterCallback<ClickEvent>(ClickvsExplanation);
        }
        void GetData()
        {

            db.Collection(pathdifficultyLevel).OrderBy("min").GetSnapshotAsync(Source.Cache).ContinueWithOnMainThread(task =>
            {
                QuerySnapshot snapshot = task.Result;
                foreach (DocumentSnapshot document in snapshot.Documents)
                {
                    difficultyLevelList.Add(document.ConvertTo<DifficultyLevel>());
                }
                if (difficultyLevelList.Count > 0)
                {
                    if (categorySaves.Count > 0)
                    {
                        db.Collection(pathquestion).WhereIn("q_category", categorySaves).GetSnapshotAsync(Source.Cache).ContinueWithOnMainThread(task =>
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
                        getQuestions();
                    }              
                }
                else
                {
                    print("No hay niveles de dificultad");
                }
            });
        }
        void getQuestions()
        {
            indexDifficultyLevelScale = difficultyLevelList[indexDifficultyLevel].min;
            questionsList = questionsListAll.FindAll(q => q.q_scaleDifficulty == indexDifficultyLevelScale);
            NextQuestions();
        }
        void ClickvsExplanation(ClickEvent e)
        {
            //Remove questions
            questionsListAll.Remove(questionsList[indexQuestionsRandom]);
            questionsList.RemoveAt(indexQuestionsRandom);
            NextQuestions();
            m_vsExplanation.RemoveFromClassList(k_displayOn);
            isClick = true;
        }
        private void IsAnswerCorrect(ClickEvent e, int index)
        {
            if (isClick)
            {
                if (indexQuestionsRandom < questionsList.Count)
                {
                    if (questionsList[indexQuestionsRandom].q_option_correct == (index + 1))
                    {
                        AudioManager.PlayVictorySound();
                        correctAnswerOptions++;                        
                        //add
                        AnswerQuestionsConsecutively++;
                        StartCoroutine("animationCorrect", index);
                    }
                    else
                    {
                        AudioManager.PlayDefeatSound();                        
                        //reset
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
            //Explication
            m_lblExplanation.text = questionsList[indexQuestionsRandom].q_explanation;
            m_vsExplanation.AddToClassList(k_displayOn);            
        }
        IEnumerator animationIncorrect(int index)
        {
            isClick = false;
            m_BtnOptions[index].AddToClassList(k_IsIncorrect);
            yield return new WaitForSeconds(.4f);
            m_BtnOptions[index].RemoveFromClassList(k_IsIncorrect);
            yield return new WaitForSeconds(.2f);
            m_BtnOptions[questionsList[indexQuestionsRandom].q_option_correct-1].AddToClassList(k_IsCorrect);
            yield return new WaitForSeconds(0.8f);
            m_BtnOptions[questionsList[indexQuestionsRandom].q_option_correct-1].RemoveFromClassList(k_IsCorrect);            
            //Explication
            m_lblExplanation.text = questionsList[indexQuestionsRandom].q_explanation;
            m_vsExplanation.AddToClassList(k_displayOn);            
        }
        void ReplayGame()
        {
            ResetGame.Invoke();
            correctAnswerOptions = 0;
            AnswerQuestions = 0;
            if (difficultyLevelList.Count > 0)
            {
                if (categorySaves.Count > 0)
                {
                    db.Collection(pathquestion).WhereIn("q_category", categorySaves).GetSnapshotAsync(Source.Cache).ContinueWithOnMainThread(task =>
                    {
                        questionsListAll = new List<Question>();
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
                    getQuestions();
                }
            }
            else
            {
                print("No hay niveles de dificultad");
            }
        }

        void NextQuestions()
        {
            ResetStyle();
            if (questionsListAll.Count > 0 && AnswerQuestions < maxQuestions)
            {
                //Aumento de nivel
                if (AnswerQuestionsConsecutively >= rangeLevelUpDifficulty && AnswerQuestionsConsecutively % rangeLevelUpDifficulty == 0)
                {
                    if (indexDifficultyLevelScale < difficultyLevelList[indexDifficultyLevel].max)
                    {
                        indexDifficultyLevelScale++;
                        questionsList = questionsListAll.FindAll(q => q.q_scaleDifficulty == indexDifficultyLevelScale);
                    }
                    else
                    {
                        if ((indexDifficultyLevel + 1) < difficultyLevelList.Count)
                        {
                            indexDifficultyLevel++;
                            indexDifficultyLevelScale = difficultyLevelList[indexDifficultyLevel].min;
                            questionsList = questionsListAll.FindAll(q => q.q_scaleDifficulty == indexDifficultyLevelScale);
                        }
                    }
                }
                if (AnswerQuestionsConsecutively <= 0)
                {
                    if (indexDifficultyLevelScale > difficultyLevelList[indexDifficultyLevel].min)
                    {
                        indexDifficultyLevelScale--;
                        questionsList = questionsListAll.FindAll(q => q.q_scaleDifficulty == indexDifficultyLevelScale);
                    }
                    else
                    {
                        if ((indexDifficultyLevel - 1) >= 0)
                        {
                            indexDifficultyLevel--;
                            indexDifficultyLevelScale = difficultyLevelList[indexDifficultyLevel].max;
                            questionsList = questionsListAll.FindAll(q => q.q_scaleDifficulty == indexDifficultyLevelScale);
                        }
                    }
                }
                if (questionsList.Count > 0)
                {
                    indexQuestionsRandom = UnityEngine.Random.Range(0, questionsList.Count);
                }
                else
                {
                    while (questionsList.Count <= 0)
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
                            else
                            {
                                indexDifficultyLevel = 0;
                                indexDifficultyLevelScale = difficultyLevelList[indexDifficultyLevel].min;
                            }
                        }
                        questionsList = questionsListAll.FindAll(q => q.q_scaleDifficulty == indexDifficultyLevelScale);
                    }
                    indexQuestionsRandom = UnityEngine.Random.Range(0, questionsList.Count);
                }
                print("Scala: " + indexDifficultyLevelScale + "\n" + "Nivel: "
                    + difficultyLevelList[indexDifficultyLevel].name + "\n" +
                    "consecutivo: " + AnswerQuestionsConsecutively);
                m_QGPanel.AddToClassList(k_QGPanelActive);
                UpdateQuestions.Invoke(questionsList[indexQuestionsRandom].q_option_correct);
                m_lblQuestion.text = questionsList[indexQuestionsRandom].q_question;
                m_LblText[0].text = questionsList[indexQuestionsRandom].q_option1;
                m_LblText[1].text = questionsList[indexQuestionsRandom].q_option2;
                m_LblText[2].text = questionsList[indexQuestionsRandom].q_option3;
                m_LblText[3].text = questionsList[indexQuestionsRandom].q_option4;
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
            //Remove questions
            questionsListAll.Remove(questionsList[indexQuestionsRandom]);
            questionsList.RemoveAt(indexQuestionsRandom);
            NextQuestions();
        }
        #endregion
        // syncs saved data from the GameDataManager to the UI elements
        void OnGameDataLoaded(GameData gameData)
        {
            if (gameData == null)
                return;
            foreach (var c in gameData.categoryList)
            {
                if (c.IsSelect)
                {
                    categorySaves.Add(c.id);
                }
            }            
            if (gameData.selectCharacter < character.Count)
            {
                character[gameData.selectCharacter].SetActive(true);
            }
        }
    }
}
