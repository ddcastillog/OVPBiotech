using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace OVPBiotechSpace
{
    public class Timer : MonoBehaviour
    {
        [SerializeField] private float maxTime;
        [SerializeField] private Slider slider;
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private GameObject UI;
        public static event Action FinishTime;
        private float currentTime;
        private bool activeTime;
        
        private void Update()
        {
            if (activeTime)
            {
                ChangeCount();
            }
        }       
        private void ChangeCount()
        {
            currentTime -= Time.deltaTime;
            if (currentTime >= 0)
            {
                slider.value = currentTime;
                text.text = currentTime.ToString("f0");
            }
            if (currentTime <= 0)
            {
                ChangeTimer(false);
                FinishTime.Invoke();
            }
        }

        private void ChangeTimer(bool state)
        {
            activeTime = state;
        }
        public int GetCurrentTime()
        {
            return (int)currentTime;
        }
        public void StopTimer()
        {
            ChangeTimer(false);
        }
        public void ContinueTimer()
        {
            ChangeTimer(true);
        }
        public void ActiveTimer()
        {
            currentTime = maxTime;
            slider.maxValue = maxTime;
            ChangeTimer(true);
            UI.SetActive(true);
        }
        public void DisableTimer()
        {
            ChangeTimer(false);
            UI.SetActive(false);
        }
    }
}
