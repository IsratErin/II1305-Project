using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine.SceneManagement;


//using Unity.Netcode;
//using Unity.Netcode.Transports.UTP;
using System.Threading.Tasks;

public class FriendlyBattleScript : MonoBehaviour
{   
    public GameObject FriendlyBattle;
    public static Lobby hostLobby;

    int playerNumber = -1; 
    private float heartbeatTimer;
    private float lobbyUpdateTimer;
    private float lobbyPollTimer;


    public static bool Ready;


    // Start is called before the first frame update
    /*void Start()
    {   
        if(playerNumber == 0){
            CreateLobby();
        }
    }*/

    private static FriendlyBattleScript _instance;
    public static FriendlyBattleScript Instance { get { return _instance; } }

    private void Awake() {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

    private void Update(){
        if(playerNumber != -1){
            if(playerNumber == 0){
            HandleHeartbeat();
        }   
        UpdateLobbyPlayers();
        }
    }


    private async void HandleHeartbeat(){
        if(hostLobby != null){
            heartbeatTimer -= Time.deltaTime;
            if(heartbeatTimer < 0f){
                float heartbeatTimerMax = 15;
                heartbeatTimer = heartbeatTimerMax;
                await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
            }
        }
    }

    public async Task<string> CreateLobby(){
        playerNumber = 0;
        try{
            string lobbyName = "FriendlyBattle";
            int maxPlayers = 2;
            CreateLobbyOptions options = new CreateLobbyOptions{
                IsPrivate = false,
                Player = new Player{
                    Data = new Dictionary<string, PlayerDataObject>{
                        {"Name", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, LoginSystem.EntityId)}
                    }
                },
                Data = new Dictionary<string, DataObject>{
                    {"KEY_START_GAME", new DataObject(DataObject.VisibilityOptions.Member, "0")}
                }
            };
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);
            hostLobby = lobby;
            //gameCode = lobby.LobbyCode;
            return await Task.FromResult(lobby.LobbyCode);
            //CodeText.SetText("Code: " + lobby.LobbyCode);
        }catch(LobbyServiceException e){
            Debug.Log(e);
            return null;
        }
    
    }

    public async void JoinLobby(string CodeInput){
        playerNumber = 1;
        try{
            JoinLobbyByCodeOptions options = new JoinLobbyByCodeOptions{
                Player = new Player{
                    Data = new Dictionary<string, PlayerDataObject>{
                        {"Name", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, LoginSystem.EntityId)}
                    }
                }
            };
            Lobby lobby = await Lobbies.Instance.JoinLobbyByCodeAsync(CodeInput, options);
            hostLobby = lobby;
            //gameCode = CodeInput;
            //InputCode.gameObject.SetActive(false);
            //JoinClientButton.gameObject.SetActive(false);
        }catch(LobbyServiceException e){
            Debug.Log(e);
        }
    }

    private async void LeaveLobby(){
        if (playerNumber == 0){
            DeleteLobby();
        }else{
            try{
                await LobbyService.Instance.RemovePlayerAsync(hostLobby.Id, AuthenticationService.Instance.PlayerId);
                SceneManager.LoadScene("LobbyPage");
            }catch(LobbyServiceException e){
                Debug.Log(e);
            }
        }
        
    }

    private async void DeleteLobby(){
        try{
            await LobbyService.Instance.DeleteLobbyAsync(hostLobby.Id);
        }catch(LobbyServiceException e){
            Debug.Log(e);
        }
    }

    private async void StartGame(){
        if (hostLobby.Players.Count == 2){
            if(playerNumber == 0){
                //Debug.Log("UPDATING LOBBY");
                //string relayCode = await RelayScript.Instance.createFriendRelay();
                Lobby lobby = await Lobbies.Instance.UpdateLobbyAsync(hostLobby.Id, new UpdateLobbyOptions{
                    Data = new Dictionary<string, DataObject>{
                        {"KEY_START_GAME", new DataObject(DataObject.VisibilityOptions.Member, "1")}
                    }
                });
                hostLobby = lobby;
                //Debug.Log("GAME START");
                Ready = true;
                //START GAME ACCORDINGLY
            }else{
                if(hostLobby.Data["KEY_START_GAME"].Value != "0"){
                        Ready = true;
                    }
            }
        }else{
            Debug.Log("NOT ENOUGH PLAYERS");
        }
    }

    private async void UpdateLobbyPlayers(){
        if(hostLobby != null){
            lobbyUpdateTimer -= Time.deltaTime;
            if(lobbyUpdateTimer < 0f){
                float lobbyUpdateTimerMax = 1.1f;
                lobbyUpdateTimer = lobbyUpdateTimerMax;
                Lobby lobby;
                try{
                    lobby = await LobbyService.Instance.GetLobbyAsync(hostLobby.Id);
                    hostLobby = lobby;
                    //ShowPlayers(lobby);
                    StartGame();

                }catch(LobbyServiceException e){
                    Debug.Log(e);
                    //SceneManager.LoadScene("LobbyPage");
                }
            }
        }
    }

    /*private void ShowPlayers(Lobby lobby){
        if(lobby.Players.Count == 2){
            Player1Text.gameObject.SetActive(true);
            Player2Text.gameObject.SetActive(true);
            Player1Text.SetText(lobby.Players[0].Data["Name"].Value);
            Player2Text.SetText(lobby.Players[1].Data["Name"].Value);
        }else{
            Player1Text.gameObject.SetActive(true);
            Player1Text.SetText(hostLobby.Players[0].Data["Name"].Value);
        }
    }*/

}
