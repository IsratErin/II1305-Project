using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GlobalLeaderboard : MonoBehaviour, IPointerClickHandler
{
    public GameObject UI;

    public GameObject TowerSelect;
    public GameObject LobbyUI;
    private void Start(){
        
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        // Perform your action here
        TowerSelect.gameObject.SetActive(false);
        UI.gameObject.SetActive(true);
        LobbyUI.gameObject.SetActive(false);

    }
}
