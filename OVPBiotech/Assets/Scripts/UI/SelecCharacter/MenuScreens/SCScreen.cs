using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

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
        

        [SerializeField]
        private List<GameObject> models;
        Button m_BtnStartGame;
        VisualElement m_countainerBtnSC;
        Label m_lblName;
        Label m_lblDescription;
        [SerializeField]
        private string pathCharacters = "GameData/Characters";
        private Character[] Characters;
        protected override void SetVisualElements()
        {
            base.SetVisualElements();
            m_BtnStartGame = m_Root.Q<Button>(k_BtnStartGame);
            m_countainerBtnSC = m_Root.Q<VisualElement>(k_countainerBtnSC);
            m_lblName = m_Root.Q<Label>(k_lblName);
            m_lblDescription = m_Root.Q<Label>(k_lblDescription);
            Characters = Resources.LoadAll<Character>(pathCharacters);
        }

        protected override void RegisterButtonCallbacks()
        {
            m_BtnStartGame?.RegisterCallback<ClickEvent>(BtnStartGame);
            for (int i = 0; i < models.Count; i++)
            {
                VisualElement aux=new VisualElement();
                aux.AddToClassList(k_BtnSC);
                aux?.RegisterCallback<ClickEvent,int>(btnSC,i);
                //image
                VisualElement image=new VisualElement();
                image.AddToClassList(k_imageStyle);
                image.style.backgroundImage = new StyleBackground(Characters[i].btnImage);
                //Addd
                aux.Add(image);
                m_countainerBtnSC?.Add(aux);
            }
            showChracter(0);
        }
        private void btnSC(ClickEvent e, int index)
        {
            showChracter(index);
        }
        private void showChracter(int index)
        {
            for (int i = 0; i < models.Count; i++)
            {
                models[i].SetActive(false);
            }
            models[index].SetActive(true);
            m_lblName.text = Characters[index].name;
            m_lblDescription.text = Characters[index].description;

        }
        private void BtnStartGame(ClickEvent e)
        {
            SceneManager.LoadScene(2);
        }        
    }
}
