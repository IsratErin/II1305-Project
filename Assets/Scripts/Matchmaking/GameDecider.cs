using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameDecider : MonoBehaviour
{

    public Button MainMenu;

    public TMP_Text banner;

    public TMP_Text netTrophiesGained;

    public TMP_Text totalTrophies;
    // Start is called before the first frame update
    private int TROPHIES_WIN = 30;
    private int TROPHIES_LOSS = -30;

    private string queue = Matchmaking.Queue;

    private bool gameWon = GameManager.gameWon;
    void Start()
    {
        MainMenu.onClick.AddListener(GoToMenu);
        if(gameWon){
            if(queue == "Competitive"){
                Profile.Instance.updateTrophies(TROPHIES_WIN);
                Profile.Instance.updatePlayerWins();
                netTrophiesGained.SetText("+"+TROPHIES_WIN);
                totalTrophies.SetText((Profile.userTrophies + TROPHIES_WIN).ToString());
                banner.SetText("You Boomed the Opponent");
            }
        }else{
            if(queue == "Competitive"){
                Profile.Instance.updateTrophies(TROPHIES_LOSS);
                Profile.Instance.updatePlayerLosses();
                netTrophiesGained.SetText(""+TROPHIES_LOSS);
                totalTrophies.SetText((Profile.userTrophies + TROPHIES_LOSS).ToString());
            }
            banner.SetText("Busted");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void GoToMenu(){
        SceneManager.LoadScene("GameBoardScene");
    }
}
