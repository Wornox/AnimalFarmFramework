using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MenuPage { Main, Custom};

public class MainMenuController : MonoBehaviour
{
    public GameObject mainMenuPage;
    public GameObject customPage;

    public bool mainPage = true;

    public static MenuPage currentPage;

    void OnEnable()
    {
        if(currentPage == null) currentPage = MenuPage.Main;
        else
        {
            switch (currentPage)
            {
                case MenuPage.Main:
                    GoToMainPage();
                    break;
                case MenuPage.Custom:
                    GoToCustomPage();
                    break;
            }

        }
    }

    public void GoToCustomPage()
    {
        mainMenuPage.SetActive(false);
        customPage.SetActive(true);
        currentPage = MenuPage.Custom;
    }

    public void GoToMainPage()
    {
        customPage.SetActive(false);
        mainMenuPage.SetActive(true);
        currentPage = MenuPage.Main;
    }

    public void ExitApp()
    {
        Application.Quit();
    }

}
