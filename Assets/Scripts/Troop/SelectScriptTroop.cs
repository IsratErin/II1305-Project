using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectScriptTroop : MonoBehaviour
{

    [SerializeField] private Button selection0;
    [SerializeField] private Button selection1;
    [SerializeField] private Button selection2;
    [SerializeField] private Button selection3;

    void Start()
    {
        selection0.onClick.AddListener(changeToTroop0);
        selection1.onClick.AddListener(changeToTroop1);
        selection2.onClick.AddListener(changeToTroop2);
        selection3.onClick.AddListener(changeToTroop3);
    }

    private void changeToTroop0 () { 
        GameManager.Instance.SelectedTroop = GameManager.Instance.troopPrefab0;
    }

    private void changeToTroop1 () { 
        GameManager.Instance.SelectedTroop = GameManager.Instance.troopPrefab1;
    }

    private void changeToTroop2 () { 
        GameManager.Instance.SelectedTroop = GameManager.Instance.troopPrefab2;
    }

    private void changeToTroop3 () { 
        GameManager.Instance.SelectedTroop = GameManager.Instance.troopPrefab3;
    }

    
}
