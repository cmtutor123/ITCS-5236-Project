using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIControl : MonoBehaviour
{
    private VisualElement root;
    private VisualElement startScreen;
    private VisualElement settingsScreen;
    private VisualElement selectScreen;
    private VisualElement creditsScreen;
    private VisualElement background;
    int players = 1;
    // Start is called before the first frame update
    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        startScreen = root.Q<VisualElement>("StartMenu");
        settingsScreen = root.Q<VisualElement>("SettingsMenu");
        creditsScreen = root.Q<VisualElement>("CreditsMenu");
        selectScreen = root.Q<VisualElement>("SelectMenu");

        VisualElement Player1 = selectScreen.Q<VisualElement>("Player1");
        VisualElement Player2 = selectScreen.Q<VisualElement>("Player2");
        VisualElement Player3 = selectScreen.Q<VisualElement>("Player3");
        VisualElement Player4 = selectScreen.Q<VisualElement>("Player4");
        VisualElement Ship1 = Player1.Q<VisualElement>("S1");
        VisualElement Ship2 = Player2.Q<VisualElement>("S2");
        VisualElement Ship3 = Player3.Q<VisualElement>("S3");
        VisualElement Ship4 = Player4.Q<VisualElement>("S4");
        Button Player1Select = Player1.Q<Button>("B1");
        Button Player2Select = Player2.Q<Button>("B2");
        Button Player3Select = Player3.Q<Button>("B3");
        Button Player4Select = Player4.Q<Button>("B4");

        background = root.Q<VisualElement>("Background");

        Button startButton = startScreen.Q<Button>("Start");
        Button settingsButton = startScreen.Q<Button>("Settings");
        Button creditButton = startScreen.Q<Button>("Credits");
        Button quitButton = startScreen.Q<Button>("Exit");
        Button backButton = creditsScreen.Q<Button>("Back");

        startButton.RegisterCallback<ClickEvent>(ev => {
            startScreen.style.display = DisplayStyle.None;
            selectScreen.style.display = DisplayStyle.Flex;
            //Code to determine amount of players in game
            if(true)
            {
                //players++
            }
            for (int i = 0; i < 4; i++)
            {
                if (i < players)
                {
                    selectScreen.Q<VisualElement>("Player" + (i + 1)).style.display = DisplayStyle.Flex;
                }
                else
                {
                    selectScreen.Q<VisualElement>("Player" + (i + 1)).style.display = DisplayStyle.None;
                }
            }
        });
        
        Player1Select.RegisterCallback<ClickEvent>(ev => {
            players--;
            if(players < 1){
                startGame();
            }
        });
        Player2Select.RegisterCallback<ClickEvent>(ev => {
            players--;
            if(players < 1){
                startGame();
            }
        });
        Player3Select.RegisterCallback<ClickEvent>(ev => {
            players--;
            if(players < 1){
                startGame();
            }
        });
        Player4Select.RegisterCallback<ClickEvent>(ev => {
            players--;
            if(players < 1){
                startGame();
            }
        });
        settingsButton.RegisterCallback<ClickEvent>(ev => {
            startScreen.style.display = DisplayStyle.None;
            settingsScreen.style.display = DisplayStyle.Flex;
        });
        creditButton.RegisterCallback<ClickEvent>(ev => {
            startScreen.style.display = DisplayStyle.None;
            creditsScreen.style.display = DisplayStyle.Flex;
        });
        backButton.RegisterCallback<ClickEvent>(ev => {
            startScreen.style.display = DisplayStyle.Flex;
            settingsScreen.style.display = DisplayStyle.None;
            creditsScreen.style.display = DisplayStyle.None;
        });
        quitButton.RegisterCallback<ClickEvent>(ev => {
            Application.Quit();
        });
    }

    void startGame()
    {
        selectScreen.style.display = DisplayStyle.None;
        background.style.display = DisplayStyle.None;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
