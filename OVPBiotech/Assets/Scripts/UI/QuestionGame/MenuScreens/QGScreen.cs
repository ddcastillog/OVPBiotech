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
        [SerializeField]
        private Sprite used5050Sprite;
        [SerializeField]
        private Sprite usedAudienceSprite;
        [SerializeField]
        private Sprite usedSwitchSprite;
        [SerializeField]
        private Sprite usedExpertSprite;

        const string k_BtnOptions = "btn-option";
        const string k_lblOptions = "lbl-option";
        const string k_lblQuestion = "lbl-question";
        const string k_lblExpert = "lbl-Expert";
        const string k_countainerExpert = "countainer-expertV";
        const string k_countainerExpertStyle = "countainer-expert-on";
        const string k_IsCorrect = "bnt-IsCorrect";
        const string k_IsIncorrect = "btn-IsIncorrect";
        const string k_QGPanelActive = "qg-panel-active";
        const string k_QGPanel = "QGPanel";
        const string k_5050 = "btn-5050";
        const string k_audience = "btn-audience";
        const string k_switch = "btn-switch";
        const string k_expert = "btn-expert";
        const string k_visibilityOff = "visibility-off";
        const string k_displayOn = "display-on";

        FirebaseFirestore db;
        Label m_lblQuestion;
        VisualElement m_QGPanel;
        List<Button> m_BtnOptions = new List<Button>();
        List<Label> m_lblOptions = new List<Label>();
        List<Question> questionsList = new List<Question>();
        VisualElement m_countainerExpert;
        Label m_lblExpert;
        int indexQuestions = 0;
        Button m_5050;
        Button m_audience;
        Button m_switch;
        Button m_expert;
        bool isClick = true;
        bool used5050 = false;
        bool used5050Right = false;
        bool usedAudience = false;
        bool usedSwitch = false;
        bool usedExpert = false;
        private int probabilityOfCorrectAnswer = 90;
        private int persentsOfRightAnswer = 80;
        private bool[] isAnswerAvailable = new bool[4];
        public static event Action<String> updateScore;
        private int correctAnswerOptions = 0;
        void OnEnable()
        {
            QGScoreScreen.ReplayGame += getQuestions;
        }
        void OnDisable()
        {
            QGScoreScreen.ReplayGame -= getQuestions;
        }
        protected override void SetVisualElements()
        {
            base.SetVisualElements();
            m_lblQuestion = m_Root.Q<Label>(k_lblQuestion);
            m_QGPanel = m_Root.Q<VisualElement>(k_QGPanel);
            m_5050 = m_Root.Q<Button>(k_5050);
            m_audience = m_Root.Q<Button>(k_audience);
            m_switch = m_Root.Q<Button>(k_switch);
            m_expert = m_Root.Q<Button>(k_expert);
            m_countainerExpert = m_Root.Q<VisualElement>(k_countainerExpert);
            m_lblExpert = m_Root.Q<Label>(k_lblExpert);
            for (int i = 1; i < 5; i++)
            {
                m_BtnOptions.Add(m_Root.Q<Button>(k_BtnOptions + i));
                m_lblOptions.Add(m_Root.Q<Label>(k_lblOptions + i));
                isAnswerAvailable[i - 1] = true;
            }
            db = FirebaseFirestore.DefaultInstance;
            getQuestions();
        }

        protected override void RegisterButtonCallbacks()
        {
            for (int i = 0; i < 4; i++)
            {
                m_BtnOptions[i]?.RegisterCallback<ClickEvent, int>(IsAnswerCorrect, i);
            }
            m_5050.RegisterCallback<ClickEvent>(btn5050);
            m_audience.RegisterCallback<ClickEvent>(btnAudience);
            m_switch.RegisterCallback<ClickEvent>(btnSwitch);
            m_expert.RegisterCallback<ClickEvent>(btnExpert);
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
                        StartCoroutine("animationCorrect", index);
                    }
                    else
                    {
                        AudioManager.PlayDefeatSound();
                        StartCoroutine("animationIncorrect", index);
                    }
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
                m_QGPanel.AddToClassList(k_QGPanelActive);
                used5050 = false;
                used5050Right = false;
                usedAudience = false;
                usedSwitch = false;
                usedExpert = false;
            });
        }
        void NextQuestions()
        {
            for (int i = 0; i < 4; i++)
            {
                m_BtnOptions[i].RemoveFromClassList(k_visibilityOff);
                isAnswerAvailable[i] = true;
                m_lblOptions[i].RemoveFromClassList(k_displayOn);

            }
            used5050Right = false;
            m_countainerExpert.RemoveFromClassList(k_countainerExpertStyle);
            m_countainerExpert.RemoveFromClassList(k_countainerExpertStyle);
            if (indexQuestions < questionsList.Count)
            {
                m_lblQuestion.text = questionsList[indexQuestions].q_question;
                m_BtnOptions[0].text = questionsList[indexQuestions].q_option1;
                m_BtnOptions[1].text = questionsList[indexQuestions].q_option2;
                m_BtnOptions[2].text = questionsList[indexQuestions].q_option3;
                m_BtnOptions[3].text = questionsList[indexQuestions].q_option4;
            }
            else
            {
                m_QGPanel.RemoveFromClassList(k_QGPanelActive);
                int total = usedSwitch ? (questionsList.Count - 1) : questionsList.Count;
                updateScore.Invoke(correctAnswerOptions + "/" + total);
                m_MainMenuUIManager?.ShowQGScoreScreen();
            }
        }
        void btn5050(ClickEvent e)
        {
            if (!used5050)
            {
                int[] wrongAnswers = new int[2];
                do
                {
                    wrongAnswers[0] = UnityEngine.Random.Range(1, 5);
                }
                while (wrongAnswers[0] == questionsList[indexQuestions].q_option_correct);
                do
                {
                    wrongAnswers[1] = UnityEngine.Random.Range(1, 5);
                }
                while ((wrongAnswers[1] == questionsList[indexQuestions].q_option_correct) || (wrongAnswers[1] == wrongAnswers[0]));
                used5050 = true;
                used5050Right = true;
                m_BtnOptions[wrongAnswers[0] - 1].AddToClassList(k_visibilityOff);
                m_BtnOptions[wrongAnswers[1] - 1].AddToClassList(k_visibilityOff);
                m_lblOptions[wrongAnswers[0] - 1].RemoveFromClassList(k_displayOn);
                m_lblOptions[wrongAnswers[1] - 1].RemoveFromClassList(k_displayOn);
                isAnswerAvailable[wrongAnswers[0] - 1] = false;
                isAnswerAvailable[wrongAnswers[1] - 1] = false;
                m_5050.style.backgroundImage = new StyleBackground(used5050Sprite);                
            }
        }
        void btnAudience(ClickEvent e)
        {
            if (!usedAudience)
            {
                usedAudience = true;
                m_audience.style.backgroundImage = new StyleBackground(usedAudienceSprite);
                int[] results = new int[4];
                int idOfRightAnswer = questionsList[indexQuestions].q_option_correct; // (1 to 4)

                ///////////////////////////////////////////////////////////////////////////////////////////////////
                //if lifeline 5050 was used for this question
                //we have 2 avaliable answers
                ///////////////////////////////////////////////////////////////////////////////////////////////////
                if (used5050Right)
                {
                    int idOfWrongAnswer = 1; // (1 to 4)                

                    //finding id of wrong answer
                    for (int i = 0; i < 4; i++)
                    {
                        if (isAnswerAvailable[i] == true && idOfRightAnswer != i + 1)
                        {
                            idOfWrongAnswer = i + 1;
                        }
                        //all other answers persents is set to 0
                        else
                        {
                            results[i] = 0;
                        }
                    }

                    //if audience should give wrong answer
                    if (UnityEngine.Random.Range(1, 101) > probabilityOfCorrectAnswer) //probability of this is 100 - probabilityOfCorrectAnswer %
                    {
                        //assigning wrong persents then rigth
                        results[idOfWrongAnswer - 1] = UnityEngine.Random.Range(51, 101);
                        results[idOfRightAnswer - 1] = 100 - results[idOfWrongAnswer - 1];
                    }
                    //if audience should give right answer
                    else
                    {
                        //assigning rigth persents then wrong
                        results[idOfRightAnswer - 1] = UnityEngine.Random.Range(51, 101);
                        results[idOfWrongAnswer - 1] = 100 - results[idOfRightAnswer - 1];
                    }

                }
                ///////////////////////////////////////////////////////////////////////////////////////////////////
                //if lifeline 5050 was NOT used for this question
                //we have 4 avaliable answers
                ///////////////////////////////////////////////////////////////////////////////////////////////////
                else
                {
                    //if audience should give wrong answer 
                    if (UnityEngine.Random.Range(1, 101) > probabilityOfCorrectAnswer) //probability of this is 100 - probabilityOfCorrectAnswer %
                    {
                        int newIndex;
                        do
                        {
                            newIndex = UnityEngine.Random.Range(1, 5);
                        }
                        while (newIndex == results[idOfRightAnswer - 1]);
                    }

                    //creating correct answer persentage
                    results[idOfRightAnswer - 1] = UnityEngine.Random.Range(40, 90);

                    int notUsedPersents = 100 - results[idOfRightAnswer - 1];


                    int indexOfQuestion = 1;
                    //creating other persentages
                    while (indexOfQuestion < 5)
                    {
                        //if it's not correct answer
                        if (indexOfQuestion != idOfRightAnswer)
                        {

                            //if it's last question, is's persents will be all pers. that was not used
                            if (indexOfQuestion == 4)
                            {
                                results[indexOfQuestion - 1] = notUsedPersents;
                            }
                            else
                            {
                                //taking random value from notUsedPersents but smaller than persents of correct answer
                                //if not used pers. biger than pers. of correct answer
                                if (notUsedPersents > results[idOfRightAnswer - 1])
                                {
                                    results[indexOfQuestion - 1] = UnityEngine.Random.Range(0, results[idOfRightAnswer - 1] - 1);
                                    notUsedPersents -= results[indexOfQuestion - 1];
                                }
                                else
                                {
                                    results[indexOfQuestion - 1] = UnityEngine.Random.Range(0, notUsedPersents);
                                    notUsedPersents -= results[indexOfQuestion - 1];
                                }

                            }

                        }
                        indexOfQuestion++;
                    }

                }

                for (int i = 0; i < 4; i++)
                {
                    m_lblOptions[i].AddToClassList(k_displayOn);
                    m_lblOptions[i].text = results[i] + "%";
                }
            }
        }
        void btnSwitch(ClickEvent e)
        {
            if (!usedSwitch)
            {
                indexQuestions++;
                NextQuestions();
                usedSwitch = true;
                m_switch.style.backgroundImage = new StyleBackground(usedSwitchSprite);
            }
        }
        void btnExpert(ClickEvent e)
        {
            if (!usedExpert)
            {
                usedExpert = true;
                m_expert.style.backgroundImage = new StyleBackground(usedExpertSprite);
                if (used5050Right)
                {
                    //probability of true equals to persentsOfRightAnswer%
                    if (UnityEngine.Random.Range(1, 101) < persentsOfRightAnswer)
                    {
                        //returnig correct answer
                        ApplyLifeline(questionsList[indexQuestions].q_option_correct);
                    }
                    else
                    {
                        int idOfWrongAnswer = 1; // (1 to 4)                

                        //finding id of wrong answer
                        for (int i = 0; i < 4; i++)
                        {
                            if (isAnswerAvailable[i] == true && questionsList[indexQuestions].q_option_correct != i + 1)
                            {
                                idOfWrongAnswer = i + 1;
                            }
                        }
                        ApplyLifeline(idOfWrongAnswer);
                    }
                }
                ///////////////////////////////////////////////////////////////////////////////////////////////////
                //if lifeline 5050 was used for this question
                //we have 2 avaliable answers
                ///////////////////////////////////////////////////////////////////////////////////////////////////
                else
                {
                    //probability of true equals to persentsOfRightAnswer%
                    if (UnityEngine.Random.Range(1, 101) < persentsOfRightAnswer)
                    {
                        //returnig correct answer
                        ApplyLifeline(questionsList[indexQuestions].q_option_correct);
                    }
                    else
                    {
                        //returning wrong answer
                        int wrongAnswer;

                        do
                        {
                            wrongAnswer = UnityEngine.Random.Range(1, 5);
                        }
                        while (wrongAnswer == questionsList[indexQuestions].q_option_correct);
                        ApplyLifeline(wrongAnswer);
                    }
                }

            }
        }
        void ApplyLifeline(int answer)
        {

            switch (answer)
            {
                case 1:
                    StartCoroutine(LifelinePhoneAnimation("A"));
                    break;
                case 2:
                    StartCoroutine(LifelinePhoneAnimation("B"));
                    break;
                case 3:
                    StartCoroutine(LifelinePhoneAnimation("C"));
                    break;
                case 4:
                    StartCoroutine(LifelinePhoneAnimation("D"));
                    break;
            }
        }
        public IEnumerator LifelinePhoneAnimation(string answer)
        {
            m_countainerExpert.AddToClassList(k_countainerExpertStyle);
            m_lblExpert.text = "Creo que la respuesta es ...";
            yield return new WaitForSeconds(1f);
            m_lblExpert.text = "<size=160><color=#FF0000>" + answer + "</color></size>";
        }
    }
}
