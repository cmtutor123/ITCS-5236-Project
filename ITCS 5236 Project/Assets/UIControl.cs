using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

public class UIControl : MonoBehaviour
{
    private GameManager gameManager;
    private MultiplayerManager multiplayerManager;

    private VisualElement root;
    private VisualElement startScreen;
    private VisualElement settingsScreen;
    private VisualElement selectScreen;
    private VisualElement creditsScreen;
    private VisualElement background;
    private VisualElement[] readyB;
    private VisualElement[] shipB;
    int playersJoined = 0;
    int playersReady = 0;
    int[] playerShips = {1, 1, 1, 1};
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        multiplayerManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<MultiplayerManager>();

        root = GetComponent<UIDocument>().rootVisualElement;

        startScreen = root.Q<VisualElement>("StartMenu");
        settingsScreen = root.Q<VisualElement>("SettingsMenu");
        creditsScreen = root.Q<VisualElement>("CreditsMenu");
        selectScreen = root.Q<VisualElement>("SelectMenu");

        readyB = new VisualElement[4];
        shipB = new VisualElement[4];

        VisualElement Player1 = selectScreen.Q<VisualElement>("Player1");
        VisualElement Player2 = selectScreen.Q<VisualElement>("Player2");
        VisualElement Player3 = selectScreen.Q<VisualElement>("Player3");
        VisualElement Player4 = selectScreen.Q<VisualElement>("Player4");
        readyB[0]= Player1.Q<VisualElement>("B1");
        readyB[1]= Player2.Q<VisualElement>("B2");
        readyB[2]= Player3.Q<VisualElement>("B3");
        readyB[3] = Player4.Q<VisualElement>("B4");
        shipB[0]= readyB[0].Q<VisualElement>("S1");
        shipB[1]= readyB[1].Q<VisualElement>("S2");
        shipB[2]= readyB[2].Q<VisualElement>("S3");
        shipB[3]= readyB[3].Q<VisualElement>("S4");

        background = root.Q<VisualElement>("Background");

        Button startButton = startScreen.Q<Button>("Start");
        Button settingsButton = startScreen.Q<Button>("Settings");
        Button creditButton = startScreen.Q<Button>("Credits");
        Button quitButton = startScreen.Q<Button>("Exit");
        Button backButton = creditsScreen.Q<Button>("Back");

        startButton.RegisterCallback<ClickEvent>(ev => {
            startScreen.style.display = DisplayStyle.None;
            selectScreen.style.display = DisplayStyle.Flex;
            updatePlayerSelect();
            multiplayerManager.EnablePlayerJoin();
        });
        
        /**Player1Select.RegisterCallback<ClickEvent>(ev => {
            readyPlayer();
        });
        Player2Select.RegisterCallback<ClickEvent>(ev => {
            readyPlayer();
        });
        Player3Select.RegisterCallback<ClickEvent>(ev => {
            readyPlayer();
        });
        Player4Select.RegisterCallback<ClickEvent>(ev => {
            readyPlayer();
        });**/
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
        gameManager.StartGame();
    }

    public void updatePlayerSelect()
    {
        for (int i = 0; i < 4; i++)
        {
            if (i < playersJoined)
            {
                selectScreen.Q<VisualElement>("Player" + (i + 1)).style.display = DisplayStyle.Flex;
            }
            else
            {
                selectScreen.Q<VisualElement>("Player" + (i + 1)).style.display = DisplayStyle.None;
            }
        }
    }

    public void joinPlayer()
    {
        if (playersJoined < 4) playersJoined++;
        updatePlayerSelect();
    }

    public void unjoinPlayer(int playerId)
    {
        if (playersJoined > 0) playersJoined--;
        updatePlayerSelect();
    }

    public void readyPlayer(int playerId)
    {
        playersReady++;
        if (playersJoined > 0 && playersReady == playersJoined)
        {
            multiplayerManager.DisablePlayerJoin();
            startGame();
        }
        //Need assets
        readyB[playerId].style.backgroundImage = null;
    }

    public void unreadyPlayer(int playerId)
    {
        playersReady--;
        readyB[playerId].style.backgroundImage = null;
    }
    public void PlayerChangeShips(int playerId, float direction){
        if(direction > 0){
            playerShips[playerId] += 1;
        } else {
            playerShips[playerId] -= 1;
        }
        if(playerShips[playerId] > 3){
            playerShips[playerId] = 1;
        } else if(playerShips[playerId] < 1){
            playerShips[playerId] = 3;
        }
        changeShip(playerId, playerShips[playerId]);
    }
    public void changeShip(int playerId, int shipId)
    {
        // Need assets
        shipB[playerId].style.backgroundImage = null;
    }

    public void EndGame()
    {
        // Need assets
        background.style.display = DisplayStyle.Flex;
        startScreen.style.display = DisplayStyle.Flex;
    }
}
