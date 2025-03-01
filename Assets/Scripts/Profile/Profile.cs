using System;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.ProfilesModels;
using UnityEngine;
using TMPro;

public class Profile : MonoBehaviour
{

    public TMP_Text trophyText;
    public TMP_Text usernameText;

    public static int userTrophies;

    public static string userMasterId;

    public static string userUsername;

    private int userWins;

    private int userLosses;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void UpdateProfile(){
        getPlayFabIDFromEntityID(LoginSystem.EntityId, PlayFabId =>
            {
                userMasterId = PlayFabId;
                getUsernameByID(LoginSystem.EntityId, username =>
                    {
                        userUsername = username;
                        usernameText.SetText(userUsername);
                    });
                getTrophiesByID(null, trophies =>
                    {
                        userTrophies = trophies;
                        trophyText.SetText(userTrophies.ToString());
                    });
                getPlayerWins();
                getPlayerLosses();
            });
    }

    public void getUsernameByID(string entityid, Action<string> callback){
        string username = null;
        getPlayFabIDFromEntityID(entityid, PlayFabId =>
            {
                GetAccountInfoRequest req = new GetAccountInfoRequest(){
                    PlayFabId = PlayFabId
                };
                PlayFabClientAPI.GetAccountInfo(req, result =>
                {
                    username = result.AccountInfo.Username.ToString();
                    callback(username);
                }, error => Debug.LogError(error.GenerateErrorReport()));
        });
    }

    public void getPlayFabIDFromEntityID(string entityid, Action<string> callback){
        string PlayFabId = null;
        GetEntityProfileRequest request = new GetEntityProfileRequest()
            {
                Entity = new PlayFab.ProfilesModels.EntityKey { Id = entityid, Type = "title_player_account" }
            };
        PlayFabProfilesAPI.GetProfile(request, res =>
        {
            PlayFabId = res.Profile.Lineage.MasterPlayerAccountId.ToString();
            callback(PlayFabId);
        }, error => Debug.LogError(error.GenerateErrorReport()));
    }


    //GETTIN STATS

    //TROPHIES
    /*public void getTrophiesByID(string id){
        if(id == null){
            id = userMasterId;
        }
        var request = new GetLeaderboardAroundPlayerRequest{
            StatisticName = "Trophies",
            PlayFabId = id,    //id for any player, MasterID for the user playing on the device
            MaxResultsCount = 1
        };
        PlayFabClientAPI.GetLeaderboardAroundPlayer(request, res =>
        {
            //return res.Leaderboard[0].StatValue.ToString();
        }, error => Debug.LogError(error.GenerateErrorReport()));
   } */

    public void getTrophiesByID(string id, Action<int> callback){
        if (id == null){
            id = userMasterId;
        }
        var request = new GetLeaderboardAroundPlayerRequest
            {
                StatisticName = "Trophies",
                PlayFabId = id,
                MaxResultsCount = 1
            };
        PlayFabClientAPI.GetLeaderboardAroundPlayer(request, res =>
            {
                int trophies = res.Leaderboard[0].StatValue;
                callback(trophies);
     
            }, error => Debug.LogError(error.GenerateErrorReport()));
    }

    //WINS
   public void getPlayerWins(){
    var request = new GetLeaderboardAroundPlayerRequest{
        StatisticName = "Wins",
        MaxResultsCount = 1,
        PlayFabId = userMasterId
    };
    PlayFabClientAPI.GetLeaderboardAroundPlayer(request, OnLeaderboardWinsGet, OnError);
   }

   void OnLeaderboardWinsGet(GetLeaderboardAroundPlayerResult result){
        userWins = result.Leaderboard[0].StatValue;
   }

    //LOSSES
   public void getPlayerLosses(){
    var request = new GetLeaderboardAroundPlayerRequest{
        StatisticName = "Wins",
        MaxResultsCount = 1,
        PlayFabId = userMasterId
    };
    PlayFabClientAPI.GetLeaderboardAroundPlayer(request, OnLeaderboardLossesGet, OnError);
   }

   void OnLeaderboardLossesGet(GetLeaderboardAroundPlayerResult result){
        userLosses = result.Leaderboard[0].StatValue;
   }


    //UPDATING STATS

    //TROPHIES
   public void updateTrophies(int trophies){
        if(userTrophies + trophies < 0){
            userTrophies = 30;
        } 
        var request = new UpdatePlayerStatisticsRequest {
            Statistics = new List<StatisticUpdate>{
                new StatisticUpdate{
                    StatisticName = "Trophies",
                    Value = userTrophies + trophies    //New Trophie Value
                }
            }
        };
        PlayFabClientAPI.UpdatePlayerStatistics(request, OnLeaderboardTrophyUpdate, OnError);
    }

    void OnLeaderboardTrophyUpdate(UpdatePlayerStatisticsResult result){
    
    }

    //WINS
    public void updatePlayerWins(){
        var request = new UpdatePlayerStatisticsRequest {
            Statistics = new List<StatisticUpdate>{
                new StatisticUpdate{
                    StatisticName = "Wins",
                    Value = userWins + 1     //New Wins value, change accordingly to needs
                }
            }
        };
        PlayFabClientAPI.UpdatePlayerStatistics(request, OnLeaderboardWinsUpdate, OnError);
    }

    //LOSSES
    void OnLeaderboardWinsUpdate(UpdatePlayerStatisticsResult result){
        userWins += 1;
        Debug.Log(userWins);
    }
    
    public void updatePlayerLosses(){
        var request = new UpdatePlayerStatisticsRequest {
            Statistics = new List<StatisticUpdate>{
                new StatisticUpdate{
                    StatisticName = "Losses",
                    Value = userLosses + 1     //New Losses Value, change accordingly to needs
                }
            }
        };
        PlayFabClientAPI.UpdatePlayerStatistics(request, OnLeaderboardLossesUpdate, OnError);
    }

    void OnLeaderboardLossesUpdate(UpdatePlayerStatisticsResult result){
        userLosses += 1;
        Debug.Log(userLosses);
    }

   void OnError(PlayFabError error) {
        Debug.Log(error);
        
    }

    private static Profile instance;
    public static Profile Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        UpdateProfile();
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
