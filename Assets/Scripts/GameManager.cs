using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using TMPro;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{



    [SerializeField] private Material pathMaterial;
    [SerializeField] private Material cityMaterial;
    [SerializeField] private Material cityLite;
    [SerializeField] private Material baseMaterial;
    [SerializeField] private Material baseLite;
    [SerializeField] private Material minefieldMaterial;
    [SerializeField] private Material minefieldDark;
    [SerializeField] private Material safeMaterial;
    [SerializeField] private Material safeDark;
    [SerializeField] private Material correctMaterial;
    [SerializeField] private Material incorrectMaterial;
    [SerializeField] private Transform gameboardContainer;
    [SerializeField] private TMP_Text timerText;

    public enum GameState
    {
        Base, Mine, Minesweeper, Battle
    }

    public int towerCounter = 0;
    public int mineCounter = 0;
    public int troopCounter = 0;
    public int clicksCounter = 0;
    private int numberOfRows, numberOfColumns;
    private int currentColumnsNumber;


    private float timeoutDuration;  // Duration of the timeout in minutes
    private bool isTimerRunning;    // Flag to track if the timer is running


    private Dictionary<Vector2Int, int> playerPositions;
    private Dictionary<Vector2Int, Transform> gameboardDictionary;
    private Dictionary<Transform, Vector2Int> gameboardDictionaryLookup;

    private GameObject city;
    private GameState currentState;
    private GameObject selectedMine;
    private GameObject selectedTower;
    private GameObject selectedTroop;
    private GameObject selectedSquareSafe;

    private List<GameObject> spawnedTroops = new List<GameObject>();
    private List<GameObject> spawnedTowers = new List<GameObject>();


    [SerializeField] private GameObject cityPrefab0;

    [SerializeField] public GameObject minePrefab0;

    [SerializeField] public GameObject towerPrefab0;
    [SerializeField] public GameObject towerPrefab1;
    [SerializeField] public GameObject towerPrefab2;
    [SerializeField] public GameObject towerPrefab3;
    public bool gameOver = false;

    [SerializeField] public GameObject troopPrefab0;
    [SerializeField] public GameObject troopPrefab1;
    [SerializeField] public GameObject troopPrefab2;
    [SerializeField] public GameObject troopPrefab3;


    [SerializeField] public GameObject bulletTowerPrefab0;
    [SerializeField] public GameObject bulletTowerPrefab1Basket;
    [SerializeField] public GameObject bulletTowerPrefab1Beach;
    [SerializeField] public GameObject bulletTowerPrefab1Foot;
    [SerializeField] public GameObject bulletTowerPrefab2;
    [SerializeField] public GameObject bulletTowerPrefab3;

    [SerializeField] public GameObject bulletTroopPrefab0;
    [SerializeField] public GameObject bulletTroopPrefab1;
    [SerializeField] public GameObject bulletTroopPrefab2;
    [SerializeField] public GameObject bulletTroopPrefab3;



    [SerializeField] public GameObject Ready;

    [SerializeField] private AudioSource timerSound;
    [SerializeField] private AudioSource timerDing;

    [SerializeField] public GameObject towerSelectionBar;
    [SerializeField] public GameObject troopSelectionBar;


    public static bool gameWon;

    private void Start()
    {
        troopSelectionBar.SetActive(false);
    }

    bool printed = false;
    private void Update()
    {
        if (CurrentState == GameState.Mine)
        {
            towerSelectionBar.gameObject.SetActive(false);
            Ready.gameObject.SetActive(true);
        }
        else
        {
            Ready.gameObject.SetActive(false);
        }
        if (CurrentState == GameState.Minesweeper && !printed)
        {
            printed = true;

            GenerateDictionary();

            DestroyBoard();

            string createdJson = SerializeGameData(Matchmaking.matchID);

            string recievedJson = ExchangeGamePageData(createdJson);

            BuildBoard(recievedJson);

            StartTimer(0.1f);
            selectedTroop = troopPrefab1;


            this.City = Instantiate(cityPrefab0, new Vector3Int(0, 16), Quaternion.identity);
            City city = this.City.GetComponent<City>();
            city.Initialize(500);
            print(SerializeGameData("123", city.health.HealthAmt, troopCounter));

        }
        if (currentState == GameState.Battle)
        {
            troopSelectionBar.SetActive(true);
        }
    }

    private string ExchangeGamePageData(string jsonData)
    {
        IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
        //IPAddress ipAddress = IPAddress.Parse("130.229.191.142"); // replace with server IP address
        //IPAddress ipAddress = IPAddress.Parse("130.229.185.251");
        IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000); // replace with server port number

        // Create a TCP/IP socket
        Socket clientSocket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        try
        {
            // Connect to the remote endpoint
            clientSocket.Connect(remoteEP);

            // Send data to the server
            var bytes = Encoding.UTF8.GetBytes(jsonData);
            clientSocket.Send(bytes);

            // Receive data from the server
            byte[] recvData = new byte[1024];
            int bytesRecv = clientSocket.Receive(recvData);
            jsonData = Encoding.UTF8.GetString(recvData, 0, bytesRecv);
            clientSocket.Close();

            return jsonData;
            //return recvStr;
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return null;
            //return gameCode;
        }
    }



    public void ColorObject(GameObject gameObject, Material material)
    {
        if (gameObject != null && material != null)
        {
            SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sharedMaterial = material;
            }
        }
    }

    private void DestroyBoard()
    {
        foreach (KeyValuePair<Vector2Int, int> kvp in playerPositions)
        {
            Transform square = gameboardDictionary[kvp.Key];
            if (kvp.Value < 0)
            {
                square.GetComponent<SquareMinefield>().RemoveMine();
            }
            else if (kvp.Value > 0)
            {
                square.GetComponent<SquareBase>().RemoveTower();
            }
        }
    }

    private void BuildBoard(string json)
    {
        GameData obtainedInfo = JsonConvert.DeserializeObject<GameData>(json);
        Dictionary<Vector2Int, int> enemyPositions = obtainedInfo.Positions;

        foreach (var kvp in enemyPositions)
        {
            Transform square = gameboardDictionary[kvp.Key];
            if (kvp.Value < 0)
            {
                square.GetComponent<SquareMinefield>().AddMine(-1);
                // Hide the mines here
            }
            else if (kvp.Value > 0)
            {
                switch (kvp.Value)
                {
                    case 1:
                        SelectedTower = towerPrefab0;
                        break;
                    case 2:
                        SelectedTower = towerPrefab1;
                        break;
                    case 3:
                        SelectedTower = towerPrefab2;
                        break;
                    case 4:
                        SelectedTower = towerPrefab3;
                        break;
                    default:
                        break;
                }

                square.GetComponent<SquareBase>().AddTower(kvp.Value);
            }
        }
        UpdateMinesweeper(enemyPositions);
    }

    public string SerializeGameData(string matchId)
    {
        GameData gameData = new GameData();
        gameData.MatchId = matchId;
        gameData.MessageType = "Start";
        gameData.Positions = playerPositions;
        string json = JsonConvert.SerializeObject(gameData);
        return json;
    }

    public string SerializeGameData(string matchId, float cityHealth, int troopsUsed)
    {
        GameData gameData = new GameData();
        gameData.MatchId = matchId;
        gameData.MessageType = "End";
        gameData.CityHealth = cityHealth;
        gameData.TroopsUsed = troopsUsed;
        string json = JsonConvert.SerializeObject(gameData);
        return json;
    }

    private void GenerateDictionary()
    {
        playerPositions = new Dictionary<Vector2Int, int>();
        gameboardDictionary = new Dictionary<Vector2Int, Transform>();
        gameboardDictionaryLookup = new Dictionary<Transform, Vector2Int>();

        for (int i = 0; i < gameboardContainer.childCount; i++)
        {
            Transform section = gameboardContainer.GetChild(i);
            currentColumnsNumber = section.childCount;

            for (int j = 0; j < currentColumnsNumber; j++)
            {
                Transform column = section.GetChild(j);
                numberOfRows = column.childCount;

                for (int k = 0; k < numberOfRows; k++)
                {
                    Transform square = column.GetChild(k);
                    Vector2Int position = new Vector2Int(k, j + numberOfColumns);
                    gameboardDictionary.Add(position, square);
                    gameboardDictionaryLookup.Add(square, position);

                    if (section.name.Equals("SquaresBase"))
                    {
                        if (square.gameObject.GetComponent<SquareBase>().HasTower())
                        {
                            playerPositions.Add(position, square.gameObject.GetComponent<SquareBase>().TowerId);
                        }
                    }
                    else if (section.name.Equals("SquaresMinefield"))
                    {
                        if (square.gameObject.GetComponent<SquareMinefield>().HasMine())
                        {
                            playerPositions.Add(position, -1);
                        }
                    }
                }
            }
            numberOfColumns += currentColumnsNumber;
        }
    }

    public void UpdateMinesweeper(Dictionary<Vector2Int, int> enemyPositions)
    {

        foreach (var kvp in enemyPositions)
        {
            List<Vector2Int> neighborPositions = new List<Vector2Int>();
            if (kvp.Value < 0)
            {
                Vector2Int minePosition = kvp.Key;
                for (int rowOffset = -1; rowOffset <= 1; rowOffset++)
                {
                    for (int columnOffset = -1; columnOffset <= 1; columnOffset++)
                    {
                        if (rowOffset == 0 && columnOffset == 0)
                            continue; // Skip the current position itself

                        Vector2Int neighbor = new Vector2Int(minePosition.x + rowOffset, minePosition.y + columnOffset);

                        if (neighbor.x < 0 || neighbor.x > numberOfRows - 1)
                            continue; // Skip positions with row < 0

                        if (neighbor.y < 0 || neighbor.y > numberOfColumns - 1)
                            continue; // Skip positions with column < 0

                        neighborPositions.Add(neighbor);

                    }
                }
                foreach (Vector2Int mp in neighborPositions)
                {
                    if (GetObject(mp).gameObject.GetComponent<SquareMinefield>())
                    {
                        SquareMinefield sm = GetObject(mp).gameObject.GetComponent<SquareMinefield>();
                        sm.SurroundingMines = sm.SurroundingMines + 1;

                    }
                }
            }
        }
    }

    // Method to start the timer with the specified timeout duration in minutes
    public void StartTimer(float minutes)
    {
        if (isTimerRunning)
        {
            Debug.LogWarning("Timer is already running.");
            return;
        }
        timerSound.Play();
        timeoutDuration = minutes;
        StartCoroutine(TimerCoroutine());
    }

    // Coroutine for the timer
    private System.Collections.IEnumerator TimerCoroutine()
    {
        isTimerRunning = true;
        float elapsedTime = 0f;

        while (elapsedTime < timeoutDuration * 60f)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimerText(elapsedTime);  // Update the timer text
            yield return null;  // Wait for the next frame
        }

        // Timeout reached
        Debug.Log("Timeout reached!");
        timerSound.Stop();
        timerDing.Play();
        isTimerRunning = false;
        if (CurrentState == GameState.Mine) {
            CurrentState = GameState.Minesweeper;
        }
        if (CurrentState == GameState.Minesweeper) {
            CurrentState = GameState.Battle;
        }
    }

    public void GameOver()
    {
        string jsonString2;
        if (Matchmaking.playerNumber == 0)
        {
            jsonString2 = "{\"MatchId\":\"123\",\"MessageType\":\"End\",\"CityHealth\":100.0,\"TroopsUsed\":0,\"Positions\":null}";
        }
        else
        {
            jsonString2 = "{\"MatchId\":\"123\",\"MessageType\":\"End\",\"CityHealth\":100.0,\"TroopsUsed\":10,\"Positions\":null}";
        }
        string recievedJson = ExchangeGamePageData(jsonString2);
        if (recievedJson == "Win")
        {
            gameWon = true;
        }
        else
        {
            gameWon = false;
        }
        Debug.Log(recievedJson);
        SceneManager.LoadScene("Win screen");
    }

    // Method to update the timer text
    private void UpdateTimerText(float elapsedTime)
    {
        if (timerText != null)
        {
            float remainingTime = timeoutDuration * 60f - elapsedTime;
            TimeSpan timeSpan = TimeSpan.FromSeconds(remainingTime);
            string formattedTime = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
            timerText.text = formattedTime;
        }
    }

    /* Getters and Setters */
    public Material PathMaterial => pathMaterial;
    public Material CityMaterial => cityMaterial;
    public Material CityLite => cityLite;
    public Material BaseMaterial => baseMaterial;
    public Material BaseLite => baseLite;
    public Material MinefieldMaterial => minefieldMaterial;
    public Material MinefieldDark => minefieldDark;
    public Material SafeMaterial => safeMaterial;
    public Material SafeDark => safeDark;
    public Material CorrectMaterial => correctMaterial;
    public Material IncorrectMaterial => incorrectMaterial;

    public GameState CurrentState
    {
        get { return currentState; }
        set { currentState = value; }
    }

    public GameObject City
    {
        get { return city; }
        set { city = value; }
    }


    public GameObject SelectedMine
    {
        get { return selectedMine; }
        set { selectedMine = value; }
    }

    public GameObject SelectedTower
    {
        get { return selectedTower; }
        set { selectedTower = value; }
    }

    public GameObject SelectedTroop
    {
        get { return selectedTroop; }
        set { selectedTroop = value; }
    }

    public GameObject SelectedSquareSafe
    {
        get { return selectedSquareSafe; }
        set { selectedSquareSafe = value; }
    }

    public GameObject TowerPrefab1
    {
        get { return towerPrefab1; }
        set { towerPrefab1 = value; }
    }

    public GameObject TroopPrefab1
    {
        get { return troopPrefab1; }
        set { troopPrefab1 = value; }
    }


    public List<GameObject> SpawnedTroops
    {
        get { return spawnedTroops; }
        set { spawnedTroops = value; }
    }

    public void AddTroop(GameObject troop)
    {
        SpawnedTroops.Add(troop);
    }

    public void RemoveTroop(GameObject troop)
    {
        SpawnedTroops.Remove(troop);
    }

    public List<GameObject> SpawnedTowers
    {
        get { return spawnedTowers; }
        set { spawnedTowers = value; }
    }

    public void AddTower(GameObject tower)
    {
        SpawnedTowers.Add(tower);
    }

    public void RemoveTower(GameObject tower)
    {
        SpawnedTowers.Remove(tower);
    }

    public Vector2Int GetPosition(GameObject gameObject)
    {
        return gameboardDictionaryLookup[gameObject.transform];
    }

    public GameObject GetObject(Vector2Int vector2Int)
    {
        return gameboardDictionary[vector2Int].gameObject;
    }

    /* GameManager Singleton */
    private static GameManager instance;
    public static GameManager Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

