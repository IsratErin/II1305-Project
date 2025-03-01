using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PlayFab.ClientModels;
using PlayFab;

public class LeaderboardUI : MonoBehaviour
{

    public GameObject UI;
    public Button closeLeaderboardBtn;
    public Button rightBtn;
    public Button leftBtn;

    public GameObject personalProfile;
    public GameObject rowPrefab;

    public GameObject lobbyUI;
    public GameObject TowerSelect;
    //public GameObject lobbyUI;


    public Transform rowsParent;

    private int page = 0;

    private int totalLeaderboardEntries = 0;
    // Start is called before the first frame update

    void Start()
    {
        closeLeaderboardBtn.onClick.AddListener(closeUI);
        rightBtn.onClick.AddListener(right);
        leftBtn.onClick.AddListener(left);

        getTrophies();
        getPrivateTrophies();
    }


    private void closeUI(){
        TowerSelect.gameObject.SetActive(true);
        lobbyUI.gameObject.SetActive(true);
        UI.gameObject.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    private void right()
    {
        Debug.Log(totalLeaderboardEntries);
        page+=10;
        getTrophies();
    }
    private void left()
    {
        page-= 10;
        if(page < 0){
            page = 0;
        }
        getTrophies();
    }

    private void getPrivateTrophies(){
        var request = new GetLeaderboardAroundPlayerRequest{
        StatisticName = "Trophies",    //id for any player, MasterID for the user playing on the device
        MaxResultsCount = 1
    };
    PlayFabClientAPI.GetLeaderboardAroundPlayer(request, OnPersonalLeaderboardGet, OnError);
    }

    private void OnPersonalLeaderboardGet(GetLeaderboardAroundPlayerResult result){
        string id = result.Leaderboard[0].PlayFabId;
        Image [] boxes = personalProfile.GetComponentsInChildren<Image>();
        boxes[0].GetComponentInChildren<TMP_Text>().SetText((result.Leaderboard[0].Position + 1).ToString());
        boxes[4].GetComponentInChildren<TMP_Text>().SetText(result.Leaderboard[0].StatValue.ToString());
        GetAccountInfoRequest req = new GetAccountInfoRequest(){
            PlayFabId = id
        };
        PlayFabClientAPI.GetAccountInfo(req, res =>
            {
                boxes[2].GetComponentInChildren<TMP_Text>().SetText(res.AccountInfo.Username.ToString());
            }, error => Debug.LogError(error.GenerateErrorReport()));
            
        string wins = "0";
        string losses = "0";
        var request = new GetLeaderboardAroundPlayerRequest{
            StatisticName = "Wins",
            MaxResultsCount = 1,
        };
        PlayFabClientAPI.GetLeaderboardAroundPlayer(request, res1 =>
            {
                wins = res1.Leaderboard[0].StatValue.ToString();
                request = new GetLeaderboardAroundPlayerRequest{
                    StatisticName = "Losses",
                    MaxResultsCount = 1,
                };
                PlayFabClientAPI.GetLeaderboardAroundPlayer(request, res2 =>
                {
                    losses = res2.Leaderboard[0].StatValue.ToString();
                    Debug.Log(wins+"/"+losses);
                    boxes[3].GetComponentInChildren<TMP_Text>().SetText(wins+"/"+losses);
                }, error => Debug.LogError(error.GenerateErrorReport()));
            }, error => Debug.LogError(error.GenerateErrorReport()));

    }
    public void getTrophies(){
    var request = new GetLeaderboardRequest{
        StatisticName = "Trophies",
        StartPosition = page,
        MaxResultsCount = 10
    };
    
    PlayFabClientAPI.GetLeaderboard(request, OnLeaderboardGet, OnError);
   } 

   void OnLeaderboardGet(GetLeaderboardResult result){
        foreach(Transform item in rowsParent){
            Destroy(item.gameObject);
        }

        foreach(var item in result.Leaderboard){
            GameObject newGo = Instantiate(rowPrefab, rowsParent);
            Image row = newGo.GetComponentInChildren<Image>();
            Image [] boxes = row.GetComponentsInChildren<Image>();
            boxes[0].GetComponentInChildren<TMP_Text>().SetText((item.Position +1).ToString());
            boxes[4].GetComponentInChildren<TMP_Text>().SetText(item.StatValue.ToString());
            GetAccountInfoRequest req = new GetAccountInfoRequest(){
                    PlayFabId = item.PlayFabId
                };
            PlayFabClientAPI.GetAccountInfo(req, result =>
                {
                    boxes[2].GetComponentInChildren<TMP_Text>().SetText(result.AccountInfo.Username.ToString());
                }, error => Debug.LogError(error.GenerateErrorReport()));
            
            string wins = "0";
            string losses = "0";
            var request = new GetLeaderboardAroundPlayerRequest{
                StatisticName = "Wins",
                MaxResultsCount = 1,
                PlayFabId = item.PlayFabId
            };
            PlayFabClientAPI.GetLeaderboardAroundPlayer(request, res =>
            {
                wins = res.Leaderboard[0].StatValue.ToString();
                request = new GetLeaderboardAroundPlayerRequest{
                    StatisticName = "Losses",
                    MaxResultsCount = 1,
                    PlayFabId = item.PlayFabId
                };
                PlayFabClientAPI.GetLeaderboardAroundPlayer(request, res2 =>
                {
                    losses = res2.Leaderboard[0].StatValue.ToString();
                    boxes[3].GetComponentInChildren<TMP_Text>().SetText(wins+"/"+losses);
                }, error => Debug.LogError(error.GenerateErrorReport()));
            }, error => Debug.LogError(error.GenerateErrorReport()));
        }
   }

   void OnError(PlayFabError error) {
        Debug.Log(error);
    }
}
