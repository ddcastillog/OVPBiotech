using System;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

namespace OVPBiotechSpace
{
    // shows the menu buttons (bottom left) and level experience meter (top left)
    public class MenuBar : MenuScreen
    {

        // menu button (bottom)
        const string k_HomeScreenMenuButton = "menu__home-button";
        const string k_CategoryScreenMenuButton = "menu__category-button";
        const string k_InfoScreenMenuButton = "menu__info-button";       
        const string k_BoardsScreenMenuButton = "menu__boards-button";

        

      

        // classes/selectors for toggling between active and inactive states
        const string k_LabelInactiveClass = "menu__label";
        const string k_LabelActiveClass = "menu__label--active";
        const string k_IconInactiveClass = "menu__icon";
        const string k_IconActiveClass = "menu__icon--active";

        const string k_ButtonInactiveClass = "menu__button";
        const string k_ButtonActiveClass = "menu__button--active";       

        // UI Buttons
        Button m_HomeScreenMenuButton;
        Button m_CategoryScreenMenuButton;
        Button m_InfoScreenMenuButton;        
        Button m_BoardsScreenMenuButton;

        

        // root element to apply theme
        VisualElement m_ThemeRootElement;

        protected override void SetVisualElements()
        {
            base.SetVisualElements();

            m_HomeScreenMenuButton = m_Root.Q<Button>(k_HomeScreenMenuButton);
            m_CategoryScreenMenuButton = m_Root.Q<Button>(k_CategoryScreenMenuButton);
            m_InfoScreenMenuButton = m_Root.Q<Button>(k_InfoScreenMenuButton);
            m_BoardsScreenMenuButton = m_Root.Q<Button>(k_BoardsScreenMenuButton);
        }

        protected override void RegisterButtonCallbacks()
        {
            base.RegisterButtonCallbacks();

            // register action when each button is clicked
            m_HomeScreenMenuButton?.RegisterCallback<ClickEvent>(ShowHomeScreen);
            m_CategoryScreenMenuButton?.RegisterCallback<ClickEvent>(ShowCategoryScreen);
            m_InfoScreenMenuButton?.RegisterCallback<ClickEvent>(ShowInfoScreen);
            m_BoardsScreenMenuButton?.RegisterCallback<ClickEvent>(ShowBoardsScreen);            
        }       

        void ShowHomeScreen(ClickEvent evt)
        {
            ActivateButton(m_HomeScreenMenuButton);
            m_MainMenuUIManager?.ShowHomeScreen();
            
        }

        void ShowCategoryScreen(ClickEvent evt)
        {
            ActivateButton(m_CategoryScreenMenuButton);
            m_MainMenuUIManager?.ShowCategoryScreen();
           

        }
        void ShowInfoScreen(ClickEvent evt)
        {
            ActivateButton(m_InfoScreenMenuButton);
            m_MainMenuUIManager?.ShowInfoScreen();
            

        }
        
        void ShowBoardsScreen(ClickEvent evt)
        {           
            ActivateButton(m_BoardsScreenMenuButton);           
        }

        void ActivateButton(Button menuButton)
        {
            if (menuButton == null)
                return;

            HighlightElement(menuButton, k_ButtonInactiveClass, k_ButtonActiveClass, m_Root);

            // enable the label and disable others
            Label label = menuButton.Q<Label>(className: k_LabelInactiveClass);
            HighlightElement(label, k_LabelInactiveClass, k_LabelActiveClass, m_Root);

            // enable the icon and disable others
            VisualElement icon = menuButton.Q<VisualElement>(className: k_IconInactiveClass);
            HighlightElement(icon, k_IconInactiveClass, k_IconActiveClass, m_Root);

        }        
        

        // toggles between a highlighted/active class and an inactive class
        void HighlightElement(VisualElement visualElem, string inactiveClass, string activeClass, VisualElement root)
        {
            if (visualElem == null)
                return;

            VisualElement currentSelect = root.Query<VisualElement>(className: activeClass);

            if (currentSelect == visualElem)
            {
                return;
            }

            // de-highlight whatever is currently active
            currentSelect?.RemoveFromClassList(activeClass);
            currentSelect?.AddToClassList(inactiveClass);

            visualElem.RemoveFromClassList(inactiveClass);
            visualElem.AddToClassList(activeClass);
        }
    }
}