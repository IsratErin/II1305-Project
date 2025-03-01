using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.MultiplayerModels;
using TMPro;
using UnityEngine.SceneManagement;
using PlayFab.ClientModels;
using PlayFab.ProfilesModels;
using PlayFab.Json;
using PlayFab.AuthenticationModels;
using Unity.Services.Lobbies;
using Unity.Services.Core;
using Unity.Services.Authentication;

public class Matchmaking : MonoBehaviour
{

    public GameObject LobbyUi;
    public GameObject FriendlyBattle;
    public Button joinFriendCode;

    //Button to leave queue
    public Button createFriendStart;

    public TMP_InputField FriendCodeInput;

    public TMP_Text FriendCodeText;
    public TMP_Text GameStateText;


    //Button to join queue
    public Button joinQueueButton;

    //Button to leave queue
    public Button leaveQueueButton;

    //Button to Join Friendly Battle
    public Button friendJoinQueueButton;

    //Button to Create Friendly Battle
    public Button friendCreateQueueButton;

    //Queue Selector
    public TMP_Dropdown queueSelector;

    //Queue status
    public TMP_Text queueStatus;

    public TMP_Text userNameAndTrophies;


    //ticket ID
    private string ticketId;

    //Both Players in the match
    private string[] Players;

    //IDK XD thanks CHAT
    private Coroutine pollTicketCoroutine;

    public GameObject timer;

    //Chosen Queue
    public static string Queue;

    //MasterID for leaderboard
    private string MasterID;

    //Number to determine host and client - 0 is host, 1 is client
    public static int playerNumber;

    //matchID so can share code on SERVER to create relay connection
    public static string matchID;
    public static string user1 = "";
    public static string user2 = "";

    private string lobbyCode = "";


    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.CurrentState = GameManager.GameState.Base;
        joinQueueButton.onClick.AddListener(StartMatchmaking);
        leaveQueueButton.onClick.AddListener(CancelMatchmaking);
        friendJoinQueueButton.onClick.AddListener(JoinFriend);
        friendCreateQueueButton.onClick.AddListener(CreateFriend);
        queueSelector.onValueChanged.AddListener(DropdownValueChanged);
        joinFriendCode.onClick.AddListener(JoinLobby);
        //createFriendStart.onClick.AddListener(JoinLobby);
        joinQueueButton.interactable = false;
        //updateTrophies();
    }

    void Update(){
        FriendCodeText.SetText(lobbyCode);
        if(FriendlyBattleScript.Ready == true){
            LobbyUi.gameObject.SetActive(false);
            GameManager.Instance.CurrentState = GameManager.GameState.Mine;     //MINESWEEPER STATE
            FriendlyBattle.gameObject.SetActive(false);
        }
    }

    public async void CreateLobby(){
        lobbyCode = await FriendlyBattleScript.Instance.CreateLobby(); //await FriendlyBattleScript.Instance.CreateLobby();
        FriendCodeText.SetText(lobbyCode);
        //Unity.Services.Lobbies.Models.Lobby lobby = FriendlyBattleScript.hostLobby;
        
    }

    public void JoinLobby(){
       FriendlyBattleScript.Instance.JoinLobby(FriendCodeInput.text);
    }

//One player Creates Friendly Game - needs improvement
    public void CreateFriend(){
        FriendlyBattle.gameObject.SetActive(true);
        playerNumber = 0;
        matchID = null;
        friendCreateQueueButton.gameObject.SetActive(false);
        friendJoinQueueButton.gameObject.SetActive(false);

        createFriendStart.gameObject.SetActive(true);
        FriendCodeText.gameObject.SetActive(true);
        CreateLobby();
        //Join Lobby
    }

//One player Joins Friendly Game - needs improvement
    public void JoinFriend(){
        FriendlyBattle.gameObject.SetActive(true);
        playerNumber = 1;
        matchID = null;
        friendJoinQueueButton.gameObject.SetActive(false);
        friendCreateQueueButton.gameObject.SetActive(false);

        joinFriendCode.gameObject.SetActive(true);
        FriendCodeInput.gameObject.SetActive(true);
        //Connect to lobby 
    }

//Queue Selector, either Normal, Competitive or Friendly
    public void DropdownValueChanged(int value){
        joinQueueButton.interactable = true;
        joinQueueButton.gameObject.SetActive(true);
        friendCreateQueueButton.gameObject.SetActive(false);
        friendJoinQueueButton.gameObject.SetActive(false);
        joinFriendCode.gameObject.SetActive(false);
        FriendCodeInput.gameObject.SetActive(false);
        createFriendStart.gameObject.SetActive(false);
        FriendCodeText.gameObject.SetActive(false);
        switch(queueSelector.options[value].text){
            case "Choose Queue":
                joinQueueButton.interactable = false;
                break;
            case "Normal":
                Queue = "DefaultQueue";
                break;
            case "Competitive":
                Queue = "Competitive";
                break;
            case "Friendly Battle":
                Queue = "Friendly";
                Authorize();
                joinQueueButton.gameObject.SetActive(false);
                friendCreateQueueButton.gameObject.SetActive(true);
                friendJoinQueueButton.gameObject.SetActive(true);
                break;
        }
    }

//Creates Mathcmaking Ticket
    public void StartMatchmaking() {
        System.Random random = new System.Random();
        joinQueueButton.gameObject.SetActive(false);
        leaveQueueButton.gameObject.SetActive(true);
        queueSelector.interactable = false;
        PlayFabMultiplayerAPI.CreateMatchmakingTicket(
            new CreateMatchmakingTicketRequest 
            {
                Creator = new MatchmakingPlayer 
                {
                    Entity = new PlayFab.MultiplayerModels.EntityKey {
                        Id = LoginSystem.EntityId,
                        Type = "title_player_account"
                    },
                    Attributes = new MatchmakingPlayerAttributes 
                    {
                       DataObject = new { 
                            trophies = Profile.userTrophies    //Setting Player Trophies to the ticket
                        }
                    }
                },

                GiveUpAfterSeconds = 120,
                QueueName = Queue
            },
            OnMatchmakingTicketCreated, 
            OnMatchmakingError
        );
    
    }

//Successful Ticket Creation (Player in Queue)
    private void OnMatchmakingTicketCreated(CreateMatchmakingTicketResult result) {
        ticketId = result.TicketId;
        pollTicketCoroutine = StartCoroutine(PollTicket());
        queueStatus.SetText("Ticket created");

    }

//Unsuccessful Ticket Creation (Not in Queue) NEEDS ERROR HANDLING
    private void OnMatchmakingError(PlayFabError error) {
        Debug.Log(error.GenerateErrorReport());
    }

//Looks for other tickets in the same queue
    private IEnumerator PollTicket() {
        while (true) {
            PlayFabMultiplayerAPI.GetMatchmakingTicket (
                new GetMatchmakingTicketRequest
                {
                    TicketId = ticketId,
                    QueueName = Queue
                },
                OnGetMatchmakingTicket,
                OnMatchmakingError
            );
            yield return new WaitForSeconds(6);
        }
    }

//Successfull matchmaking - Two players have matched
    private void OnGetMatchmakingTicket(GetMatchmakingTicketResult result) {
        queueStatus.text = $"Status: {result.Status}";

        switch(result.Status) {
            case "Matched":
                StopCoroutine(pollTicketCoroutine);
                StartMatch(result.MatchId);
                break;
            
            case "Canceled":
                break;
        }
    }

//Proceed to Start Match
    private void StartMatch(string matchId) {
        leaveQueueButton.gameObject.SetActive(false);
        queueStatus.text = $"Starting Match";
        
        PlayFabMultiplayerAPI.GetMatch(
            new GetMatchRequest
            {
                MatchId = matchId,
                QueueName = Queue,
                ReturnMemberAttributes = true
            },
            OnGetMatch,
            OnMatchmakingError
        );
    }

//Leaving Queue By Leave Queue Button, cancels ticket
    private void CancelMatchmaking()
    {
        PlayFabMultiplayerAPI.CancelMatchmakingTicket(
            new CancelMatchmakingTicketRequest 
            {
                TicketId = ticketId,
                QueueName = Queue
            },
            OnMatchmakingCanceled,
            OnMatchmakingError
        );
    }

//Successfully left matchmaking queue
    private void OnMatchmakingCanceled(CancelMatchmakingTicketResult result)
    {
        StopCoroutine(pollTicketCoroutine);
        queueStatus.text = $"Matchmaking canceled";
        leaveQueueButton.gameObject.SetActive(false);
        joinQueueButton.gameObject.SetActive(true);
        queueSelector.interactable = true;
    }

//Finds match and gets both players ids
    private void OnGetMatch(GetMatchResult result) {
        matchID = result.MatchId;

        //queueStatus.text = $"{getUsername(result.Members[0].Entity.Id)} vs {getUsername(result.Members[1].Entity.Id)}";
        Profile.Instance.getUsernameByID(result.Members[0].Entity.Id, username =>
        {
            user1 = username;
        });
        Profile.Instance.getUsernameByID(result.Members[1].Entity.Id, username2 =>
        {
            user2 = username2;
        });
        Players = new string[2] {result.Members[0].Entity.Id, result.Members[1].Entity.Id}; 
        AssignRoles(Players);

    }

//Assigning Host or Client to Player
   public void AssignRoles(string[] Players){
    if (LoginSystem.EntityId == Players[0]) {
            Debug.Log("HOST");
            playerNumber = 0;
    } else {
            Debug.Log("CLIENT");
            playerNumber = 1;
    }
    //Sending To Game Scene
    GameManager.Instance.CurrentState = GameManager.GameState.Mine;
    GameStateText.SetText("Place Mines");
    LobbyUi.gameObject.SetActive(false);
   }

    private async void Authorize() {
        await UnityServices.InitializeAsync();
        AuthenticationService.Instance.SignedIn += () => {
            Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
   }
   
}
