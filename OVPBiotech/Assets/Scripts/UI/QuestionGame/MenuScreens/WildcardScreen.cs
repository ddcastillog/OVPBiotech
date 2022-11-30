using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;
namespace OVPBiotechSpace
{
    public class WildcardScreen : MenuScreenQG
    {
        [Header("Normal Sprite")]
        [SerializeField]
        private Sprite normal5050Sprite;
        [SerializeField]
        private Sprite normalAudienceSprite;
        [SerializeField]
        private Sprite normalSwitchSprite;
        [SerializeField]
        private Sprite normalExpertSprite;
        [Header("Used Sprite")]
        [SerializeField]
        private Sprite used5050Sprite;
        [SerializeField]
        private Sprite usedAudienceSprite;
        [SerializeField]
        private Sprite usedSwitchSprite;
        [SerializeField]
        private Sprite usedExpertSprite;

        #region UI-Style
        const string k_5050 = "btn-5050";
        const string k_audience = "btn-audience";
        const string k_switch = "btn-switch";
        const string k_expert = "btn-expert";
        const string k_countainerExpert = "countainer-expertV";
        const string k_countainerExpertStyle = "countainer-expert-on";
        const string k_lblExpert = "lbl-Expert";
        #endregion

        #region Variables
        bool used5050 = false;
        bool used5050Right = false;
        bool usedAudience = false;
        bool usedSwitch = false;
        bool usedExpert = false;
        private int probabilityOfCorrectAnswer = 90;
        private int persentsOfRightAnswer = 80;
        private bool[] isAnswerAvailable = new bool[4];
        private int q_option_correct=1;
        //UI
        Button m_5050;
        Button m_audience;
        Button m_switch;
        Button m_expert;
        Label m_lblExpert;
        VisualElement m_countainerExpert;
        //actions
        public static event Action<int[]> Update5050;
        public static event Action<int[],bool> UpdateAudience;
        public static event Action UpdateSwitch;
        #endregion

        void OnEnable()
        {
            QGScreen.UpdateQuestions += UpdateQuestions;
            QGScreen.ResetGame += ResetGame;
        }
        void OnDisable()
        {
            QGScreen.UpdateQuestions -= UpdateQuestions;
            QGScreen.ResetGame -= ResetGame;
        }

        
        protected override void SetVisualElements()
        {
            base.SetVisualElements();
            m_5050 = m_Root.Q<Button>(k_5050);
            m_audience = m_Root.Q<Button>(k_audience);
            m_switch = m_Root.Q<Button>(k_switch);
            m_expert = m_Root.Q<Button>(k_expert);
            m_countainerExpert = m_Root.Q<VisualElement>(k_countainerExpert);
            m_lblExpert = m_Root.Q<Label>(k_lblExpert);
            for (int i = 1; i < 5; i++)
            {               
                isAnswerAvailable[i - 1] = true;
            }

        }
        protected override void RegisterButtonCallbacks()
        {
            m_5050.RegisterCallback<ClickEvent>(btn5050);
            m_audience.RegisterCallback<ClickEvent>(btnAudience);
            m_switch.RegisterCallback<ClickEvent>(btnSwitch);
            m_expert.RegisterCallback<ClickEvent>(btnExpert);
        }
        #region Wildcards
        void btn5050(ClickEvent e)
        {
            if (!used5050)
            {
                AudioManager.PlayDefaultButtonSound();
                int[] wrongAnswers = new int[2];
                do
                {
                    wrongAnswers[0] = UnityEngine.Random.Range(1, 5);
                }
                while (wrongAnswers[0] == q_option_correct);
                do
                {
                    wrongAnswers[1] = UnityEngine.Random.Range(1, 5);
                }
                while ((wrongAnswers[1] == q_option_correct) || (wrongAnswers[1] == wrongAnswers[0]));
                used5050 = true;
                used5050Right = true;
                Update5050?.Invoke(wrongAnswers);
                isAnswerAvailable[wrongAnswers[0] - 1] = false;
                isAnswerAvailable[wrongAnswers[1] - 1] = false;
                m_5050.style.backgroundImage = new StyleBackground(used5050Sprite);
            }
        }
        void btnAudience(ClickEvent e)
        {
            if (!usedAudience)
            {
                AudioManager.PlayDefaultButtonSound();
                usedAudience = true;
                m_audience.style.backgroundImage = new StyleBackground(usedAudienceSprite);
                int[] results = new int[4];
                int idOfRightAnswer = q_option_correct; // (1 to 4)

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
                    UpdateAudience?.Invoke(results, true);

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
                    UpdateAudience?.Invoke(results,false);
                }
            }
        }
        void btnSwitch(ClickEvent e)
        {
            if (!usedSwitch)
            {
                AudioManager.PlayDefaultButtonSound();
                UpdateSwitch.Invoke();
                usedSwitch = true;
                m_switch.style.backgroundImage = new StyleBackground(usedSwitchSprite);
            }
        }
        void btnExpert(ClickEvent e)
        {
            if (!usedExpert)
            {
                AudioManager.PlayDefaultButtonSound();
                usedExpert = true;
                m_expert.style.backgroundImage = new StyleBackground(usedExpertSprite);
                if (used5050Right)
                {
                    //probability of true equals to persentsOfRightAnswer%
                    if (UnityEngine.Random.Range(1, 101) < persentsOfRightAnswer)
                    {
                        //returnig correct answer
                        ApplyLifeline(q_option_correct);
                    }
                    else
                    {
                        int idOfWrongAnswer = 1; // (1 to 4)                

                        //finding id of wrong answer
                        for (int i = 0; i < 4; i++)
                        {
                            if (isAnswerAvailable[i] == true && q_option_correct != i + 1)
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
                        ApplyLifeline(q_option_correct);
                    }
                    else
                    {
                        //returning wrong answer
                        int wrongAnswer;

                        do
                        {
                            wrongAnswer = UnityEngine.Random.Range(1, 5);
                        }
                        while (wrongAnswer == q_option_correct);
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
        #endregion
        void UpdateQuestions(int q_option_correct)
        {
            this.q_option_correct = q_option_correct;
            for (int i = 0; i < 4; i++)
            {                
                isAnswerAvailable[i] = true;               
            }
            used5050Right = false;
            m_countainerExpert.RemoveFromClassList(k_countainerExpertStyle);
        }
        void ResetGame()
        {
            used5050 = false;
            used5050Right = false;
            usedAudience = false;
            usedSwitch = false;
            usedExpert = false;
            m_5050.style.backgroundImage = new StyleBackground(normal5050Sprite);
            m_audience.style.backgroundImage = new StyleBackground(normalAudienceSprite);
            m_switch.style.backgroundImage = new StyleBackground(normalSwitchSprite);
            m_expert.style.backgroundImage = new StyleBackground(normalExpertSprite);
        }
    }
}
