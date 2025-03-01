using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectScriptTower : MonoBehaviour
{

    [SerializeField] private Button selection0;
    [SerializeField] private Button selection1;
    [SerializeField] private Button selection2;
    [SerializeField] private Button selection3;

    void Start()
    {
        selection0.onClick.AddListener(changeToTower0);
        selection1.onClick.AddListener(changeToTower1);
        selection2.onClick.AddListener(changeToTower2);
        selection3.onClick.AddListener(changeToTower3);
    }

    private void changeToTower0 () { 
        GameManager.Instance.SelectedTower = GameManager.Instance.towerPrefab0;
    }

    private void changeToTower1 () { 
        GameManager.Instance.SelectedTower = GameManager.Instance.towerPrefab1;
    }

    private void changeToTower2 () { 
        GameManager.Instance.SelectedTower = GameManager.Instance.towerPrefab2;
    }

    private void changeToTower3 () { 
        GameManager.Instance.SelectedTower = GameManager.Instance.towerPrefab3;
    }

    
}
