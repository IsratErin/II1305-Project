using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SquareBase : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{

    [SerializeField] private int towerId;
    private GameObject tower;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (GameManager.Instance.CurrentState == GameManager.GameState.Base) {
            gameObject.ApplyCorrectMaterial();
        } else if (GameManager.Instance.CurrentState == GameManager.GameState.Battle) {
            if (GameManager.Instance.SelectedSquareSafe != null) {
                GameManager.Instance.SelectedSquareSafe.GetComponent<SquareSafe>().Drawing = false;
            }
        } else {
            gameObject.ApplyIncorrectMaterial();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        gameObject.ApplyOriginalMaterial();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        /* Place selected tower (maybe tracked in static) as child of this cube */
        /* Update tower id once tower is placed*/

        if (GameManager.Instance.CurrentState == GameManager.GameState.Base)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                /* Add tower create object*/
                /* Set tower id */

                if (GameManager.Instance.towerCounter < 8) {
                    int value = 0;
                    if (GameManager.Instance.SelectedTower == GameManager.Instance.towerPrefab0){
                        value = 1;
                    } else if (GameManager.Instance.SelectedTower == GameManager.Instance.towerPrefab1) {
                        value = 2;
                    } else if (GameManager.Instance.SelectedTower == GameManager.Instance.towerPrefab2) {
                        value = 3;
                    } else if (GameManager.Instance.SelectedTower == GameManager.Instance.towerPrefab3) {
                        value = 4;
                    } else {
                        value = 0;
                    }
                      
                    if (value == 0) {
                        return;
                    }
                    AddTower(value);
                }
            } else if (eventData.button == PointerEventData.InputButton.Right)
            {
                /* Remove tower destroy object*/
                /* Set tower id to null*/
                RemoveTower();
            }
        }



    }

    public void AddTower(int newTowerId)
    {
        if (!HasTower()) {
            GameObject newTower = (GameObject)Instantiate(GameManager.Instance.SelectedTower, gameObject.transform.position, Quaternion.identity);
            newTower.transform.parent = gameObject.transform;
            TowerId = newTowerId;
            tower = newTower;
            GameManager.Instance.AddTower(newTower);
            Tower towerComponent = newTower.GetComponent<Tower>();
            if (towerComponent != null)
            {
                towerComponent.Initialize(100, newTowerId);
            }
            GameManager.Instance.towerCounter++;
        }
    }

    public void RemoveTower()
    {
        if (tower) {
            if (GameManager.Instance.SpawnedTowers.Contains(tower)) {
                GameManager.Instance.RemoveTower(tower);
                Destroy(gameObject.transform.GetChild(0).gameObject);
                TowerId = 0;
                GameManager.Instance.towerCounter--;
            }
            
        }
    }

    public bool HasTower()
    {
        return towerId > 0;
    }

    public int TowerId
    {
        get { return towerId; }
        set { towerId = value; }
    }
}

