using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameStatebuttons : MonoBehaviour
{

    [SerializeField] private Button ButtonBase;
    [SerializeField] private Button ButtonMine;
    [SerializeField] private Button ButtonPath;
    [SerializeField] private Button ButtonTroop;



    void Start()
    {
        ButtonBase.onClick.AddListener(Base);
        ButtonMine.onClick.AddListener(Mine);
        ButtonPath.onClick.AddListener(Path);
        ButtonTroop.onClick.AddListener(Troop);

    }

    private void Base()
    {
        GameManager.Instance.CurrentState = GameManager.GameState.Base;
    }

    private void Mine()
    {
        GameManager.Instance.CurrentState = GameManager.GameState.Mine;
    }

    private void Path()
    {
        GameManager.Instance.CurrentState = GameManager.GameState.Battle;
        
    }

    private void Troop()
    {
        GameManager.Instance.CurrentState = GameManager.GameState.Minesweeper;
    }

}
