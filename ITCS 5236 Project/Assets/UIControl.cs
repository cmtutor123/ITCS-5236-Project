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
    private VisualElement upgradeScreen;
    private VisualElement background;
    private VisualElement inGame;
    private VisualElement endGame;
    private VisualElement[] readyB;
    private VisualElement[] playerUpgrades;
    private Button[] playerUpChoices;
    private VisualElement[] shipB;
    private ProgressBar[] playerHP;
    private ProgressBar baseHP;
    private Label[] playerNames;
    private Label[] abilityText;
    private VisualElement[] playerHUD;
    int playersJoined = 0;
    int playersReady = 0;
    int[] playerShips = {1, 1, 1, 1};
    public bool gameStarted = false;
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
        upgradeScreen = root.Q<VisualElement>("UpgradeMenu");
        inGame = root.Q<VisualElement>("InGame");
        endGame = root.Q<VisualElement>("Endgame");


        readyB = new VisualElement[4];
        shipB = new VisualElement[4];

        background = root.Q<VisualElement>("Background");

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
        
        //Player Upgrade sections
        playerUpgrades = new VisualElement[4];
        playerUpgrades[0] = upgradeScreen.Q<VisualElement>("P1U");
        playerUpgrades[1] = upgradeScreen.Q<VisualElement>("P2U");
        playerUpgrades[2] = upgradeScreen.Q<VisualElement>("P3U");
        playerUpgrades[3] = upgradeScreen.Q<VisualElement>("P4U");
        
        

        playerHUD = new VisualElement[4];
        playerHUD[0] = inGame.Q<VisualElement>("P1");
        playerHUD[1] = inGame.Q<VisualElement>("P2");
        playerHUD[2] = inGame.Q<VisualElement>("P3");
        playerHUD[3] = inGame.Q<VisualElement>("P4");
        playerHP = new ProgressBar[4];
        playerHP[0] = inGame.Q<ProgressBar>("P1HP");
        playerHP[1] = inGame.Q<ProgressBar>("P2HP");
        playerHP[2] = inGame.Q<ProgressBar>("P3HP");
        playerHP[3] = inGame.Q<ProgressBar>("P4HP");
        playerNames = new Label[4];
        playerNames[0] = inGame.Q<Label>("P1N");
        playerNames[1] = inGame.Q<Label>("P2N");
        playerNames[2] = inGame.Q<Label>("P3N");
        playerNames[3] = inGame.Q<Label>("P4N");

        baseHP = inGame.Q<ProgressBar>("BaseHP");

        Button startButton = startScreen.Q<Button>("Start");
        Button settingsButton = startScreen.Q<Button>("Settings");
        Button creditButton = startScreen.Q<Button>("Credits");
        Button quitButton = startScreen.Q<Button>("Exit");
        Button backButton = creditsScreen.Q<Button>("Back");
        Button back2Button = settingsScreen.Q<Button>("Back");
        Button restartButton = endGame.Q<Button>("Restart");

        playerUpChoices = new Button[12];
        abilityText = new Label[12];
        for(int i = 0; i < 12; i++){
            Debug.Log(i);
            playerUpChoices[i] = playerUpgrades[i/3].Q<Button>("P" + ((i/3)+1) + "A" + ((i%3)+1) + "B");
			Debug.Log(playerUpChoices[i]);
            abilityText[i] = playerUpgrades[i/3].Q<Label>("P" + ((i%3)+1) + "A" + ((i%3)+1));
            playerUpChoices[i].RegisterCallback<ClickEvent>(ev => {
                //gameManager.UpgradePlayer((i/3), (i%3));
            });
        }

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
        back2Button.RegisterCallback<ClickEvent>(ev => {
            startScreen.style.display = DisplayStyle.Flex;
            settingsScreen.style.display = DisplayStyle.None;
        });
        backButton.RegisterCallback<ClickEvent>(ev => {
            startScreen.style.display = DisplayStyle.Flex;
            creditsScreen.style.display = DisplayStyle.None;
        });
        quitButton.RegisterCallback<ClickEvent>(ev => {
            Application.Quit();
        });
        restartButton.RegisterCallback<ClickEvent>(ev => {
            gameManager.RestartGame();
        });
    }

    void Update(){
        if(gameStarted){
            for(int i=0; i < playersJoined; i++){
                playerHP[i].value = gameManager.GetPlayerHealth(i);
            }
            baseHP.value = gameManager.GetBaseHealth();
        }
    }

    void startGame()
    {
        selectScreen.style.display = DisplayStyle.None;
        background.style.display = DisplayStyle.None;
        inGame.style.display = DisplayStyle.Flex;
        for(int i=0; i < 4; i++){
            if(i < playersJoined){
                playerHUD[i].style.display = DisplayStyle.Flex;
            } else {
                playerHUD[i].style.display = DisplayStyle.None;
            }
        }
        gameManager.StartGame();
    }

    public void updatePlayerSelect()
    {
        for (int i = 0; i < 4; i++)
        {
            if (i < playersJoined)
            {
                selectScreen.Q<VisualElement>("Player" + (i + 1)).style.display = DisplayStyle.Flex;
                upgradeScreen.Q<VisualElement>("P" + (i + 1) + "U").style.display = DisplayStyle.Flex;
                shipB[i].style.backgroundImage = new StyleBackground(Resources.Load<Sprite>("Ship1"));
            }
            else
            {
                selectScreen.Q<VisualElement>("Player" + (i + 1)).style.display = DisplayStyle.None;
                upgradeScreen.Q<VisualElement>("P" + (i + 1) + "U").style.display = DisplayStyle.None;
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
        shipB[playerId].style.backgroundImage = new StyleBackground(Resources.Load<Sprite>("Ship" + shipId));
    }

    public void EndGame()
    {
        // Need assets
        background.style.display = DisplayStyle.Flex;
        inGame.style.display = DisplayStyle.None;
        endGame.style.display = DisplayStyle.Flex;
    }

    public void SetAbilityText(Label abilityText, string title, string desc)
    {
        abilityText.text = "<b>" + title + "</b>\n\n" + desc;
    }

    public void SetAbilityText(int index, string title, string desc)
    {
        SetAbilityText(abilityText[index], title, desc);
    }

    public void ShowUpgradeSelect()
    {
        inGame.style.display = DisplayStyle.None;
        upgradeScreen.style.display = DisplayStyle.Flex;
    }

    public void HideUpgradeSelect()
    {
        inGame.style.display = DisplayStyle.Flex;
        upgradeScreen.style.display = DisplayStyle.None;
    }
}
