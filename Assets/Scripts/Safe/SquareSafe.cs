using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SquareSafe : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{

    private bool drawing = false;
    [SerializeField] private List<GameObject> waypoints;

    //

     [SerializeField] private AudioSource sendingTroops;

    private void Start()
    {
        waypoints = new List<GameObject>();
        waypoints.Add(this.gameObject);
    }

    public void AddWaypoint(GameObject waypoint)
    {
        waypoint.ApplyPathMaterial();
        waypoints.Add(waypoint);
    }

    public GameObject RemoveWaypoint()
    {
        GameObject lastWaypoint = waypoints[waypoints.Count - 1];
        waypoints.RemoveAt(waypoints.Count - 1);
        return lastWaypoint;
    }

    public GameObject LastWaypoint()
    {
        return waypoints[waypoints.Count - 1];
    }

    public List<GameObject> Waypoints
    {
        get { return waypoints; }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (GameManager.Instance.CurrentState == GameManager.GameState.Battle)
        {
            if (gameObject.GetComponent<SpriteRenderer>().sharedMaterial != GameManager.Instance.PathMaterial)
            {
                gameObject.ApplyCorrectMaterial();
            }
        }
        else
        {
            gameObject.ApplyIncorrectMaterial();

        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (gameObject.GetComponent<SpriteRenderer>().sharedMaterial != GameManager.Instance.PathMaterial)
        {
            gameObject.ApplyOriginalMaterial();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        /* 
            | Attack prepare stage |
            - On click change selected static square 
            - On click draw path from selected square with foreach over stack
            | Attack game stage |
            - On click spawn selected troop (maybe static) on the square
        */

        if (GameManager.Instance.CurrentState == GameManager.GameState.Battle)
        {
            //sendingTroops.Play();
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                
                if (gameObject == GameManager.Instance.SelectedSquareSafe)
                {
                    if (GameManager.Instance.SelectedTroop != null) {

                        int value = 0;
                    if (GameManager.Instance.SelectedTroop == GameManager.Instance.troopPrefab0){
                        value = 0;
                    } else if (GameManager.Instance.SelectedTroop == GameManager.Instance.troopPrefab1) {
                        value = 1;
                    } else if (GameManager.Instance.SelectedTroop == GameManager.Instance.troopPrefab2) {
                        value = 2;
                    } else if (GameManager.Instance.SelectedTroop == GameManager.Instance.troopPrefab3) {
                        value = 3;
                    } else {
                        value = -1;
                    }
                      
                    if (value == -1) {
                        return;
                    }

                    AddTroop(value);
                    }
                }
                else
                {
                    ColorPath();
                }
            }

            if (eventData.button == PointerEventData.InputButton.Right)
            {
                RebuildPath();
            }


            if (eventData.button == PointerEventData.InputButton.Middle)
            {
                if (GameManager.Instance.SelectedTroop == null)
                {
                    print("Troop selected");
                    GameManager.Instance.SelectedTroop = GameManager.Instance.TroopPrefab1;
                }
                else
                {
                    print("Troop deselected");
                    GameManager.Instance.SelectedTroop = null;
                }
            }
        }


    }

    private void ColorPath()
    {
        if (GameManager.Instance.SelectedSquareSafe != this && GameManager.Instance.SelectedSquareSafe != null)
        {
            foreach (GameObject waypoint in GameManager.Instance.SelectedSquareSafe.GetComponent<SquareSafe>().waypoints)
            {
                waypoint.ApplyOriginalMaterial();
            }
            GameManager.Instance.SelectedSquareSafe = this.gameObject;
            Drawing = true;
            foreach (GameObject waypoint in waypoints)
            {
                waypoint.ApplyPathMaterial();
            }
        }
        else if (GameManager.Instance.SelectedSquareSafe == null)
        {
            GameManager.Instance.SelectedSquareSafe = this.gameObject;
            Drawing = true;
            foreach (GameObject waypoint in waypoints)
            {
                waypoint.ApplyPathMaterial();
            }

        }
    }

    private void RebuildPath()
    {
        if (GameManager.Instance.SelectedSquareSafe != this && GameManager.Instance.SelectedSquareSafe != null)
        {
            foreach (GameObject waypoint in GameManager.Instance.SelectedSquareSafe.GetComponent<SquareSafe>().waypoints)
            {
                waypoint.ApplyOriginalMaterial();
            }
            GameManager.Instance.SelectedSquareSafe = this.gameObject;
            this.waypoints = new List<GameObject>();
            this.waypoints.Add(this.gameObject);
            Drawing = true;
            foreach (GameObject waypoint in waypoints)
            {
                waypoint.ApplyPathMaterial();
            }
        }
        else if (GameManager.Instance.SelectedSquareSafe == null)
        {
            GameManager.Instance.SelectedSquareSafe = this.gameObject;
            this.waypoints = new List<GameObject>();
            this.waypoints.Add(this.gameObject);
            Drawing = true;
            foreach (GameObject waypoint in waypoints)
            {
                waypoint.ApplyPathMaterial();
            }

        }
    }

    public bool Drawing
    {
        get { return drawing; }
        set { drawing = value; }
    }

    private void AddTroop(int type)
    {

        //
        //sendingTroops.Play();
        GameObject newTroop = Instantiate(GameManager.Instance.SelectedTroop, transform.position, Quaternion.identity);
        GameManager.Instance.AddTroop(newTroop);
        Troop troop = newTroop.GetComponent<Troop>();
        if (troop != null)
        {
            sendingTroops.Play();
            troop.Initialize(gameObject, type);
        }

    }

}
