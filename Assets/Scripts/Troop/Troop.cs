using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Troop : MonoBehaviour
{
    private float moveSpeed;

    private int cost = 0;
    private int type;
    private float range = 20;
    private Health health;
    public GameObject closestTower;
    private List<GameObject> troopPath;
    public int waypointIndex = 0;
    private GameObject bulletType;
    private float interval = 0.3f;
    private float timer = 0.3f;
    float closestDistance = Mathf.Infinity;

    public void Initialize(GameObject startingSquare, int type)
    {
        troopPath = new List<GameObject>(startingSquare.GetComponent<SquareSafe>().Waypoints);
        transform.position = troopPath[0].transform.position;
        this.health = GetComponent<Health>();
        this.health.Initialize(100);
        this.type = type;


        switch (type)
        {
            case 0:
                moveSpeed = 15f;
                cost = 1;
                range = 1;
                bulletType = GameManager.Instance.bulletTroopPrefab0;
                interval = 0.3f;
                timer = interval;
                break;
            case 1:
                moveSpeed = 3f;
                cost = 2;
                range = 1;
                bulletType = GameManager.Instance.bulletTroopPrefab1;
                interval = 0.3f;
                timer = interval;
                break;
            case 2:
                moveSpeed = 5f;
                cost = 5;
                range = 40;
                interval = 0.3f;
                timer = interval;
                bulletType = GameManager.Instance.bulletTroopPrefab2;
                break;
            case 3:
                moveSpeed = 5f;
                cost = 10;
                range = 35;
                interval = 0.3f;
                timer = interval;
                bulletType = GameManager.Instance.bulletTroopPrefab3;
                break;
            default:
                Destroy(gameObject);
                GameManager.Instance.troopCounter--;
                break;
        }
        GameManager.Instance.troopCounter = GameManager.Instance.troopCounter + cost;

    }

    private void Update()

    {
        timer += Time.deltaTime;
        Move();
        // Ensure there are objects in the list and a valid target object
        if (GameManager.Instance.SpawnedTowers.Count == 0)
        {
            if (GameManager.Instance.City != null) {
                closestTower = GameManager.Instance.City;
                closestDistance = Vector2.Distance(GameManager.Instance.City.transform.position, transform.position);
                if (closestDistance <= range) {
                    if (timer >= interval)
                        {
                            GameObject bullet = Instantiate(bulletType, transform.position, Quaternion.identity);
                            bullet.GetComponent<Bullet>().Initialize(GameManager.Instance.City, type);
                            timer = 0.0f;
                        }
                }
                return;
            } else {
                if (!GameManager.Instance.gameOver) {
                    GameManager.Instance.gameOver = true;
                    GameManager.Instance.GameOver();
                    closestTower = null;
                    return;
                }

            }
            
        }

        closestDistance = Mathf.Infinity;

        foreach (GameObject tower in GameManager.Instance.SpawnedTowers)
        {
            // Get the position of the target object and the current object in the list
            Vector2 troopPosition = gameObject.transform.position;
            Vector2 towerPosition = tower.transform.parent.position;

            // Calculate the distance between the target position and the current object position
            float distance = Vector2.Distance(troopPosition, towerPosition);

            // Check if the current object is closer than the previously closest object
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTower = tower;
            }
        }

        if (timer >= interval && closestDistance <= range)
        {
            GameObject bullet = Instantiate(bulletType, transform.position, Quaternion.identity);
            bullet.GetComponent<Bullet>().Initialize(closestTower, type);
            timer = 0.0f;
        }

        
    
    }

    private void Move()
    {

        if (GameManager.Instance.City == null || Vector2.Distance(GameManager.Instance.City.transform.position, transform.position) < range) {
            return;
        }

        if (closestDistance <= range) {
            return;
        }
        
        // Move towards the next waypoint if there are more waypoints remaining
        if (waypointIndex < troopPath.Count)
        {
            transform.position = Vector2.MoveTowards(transform.position, troopPath[waypointIndex].transform.position, moveSpeed * Time.deltaTime);

            // If the troop reaches the current waypoint, move to the next waypoint
            if (transform.position == troopPath[waypointIndex].transform.position)
            {
                waypointIndex++;
            }
        } else {
            if (closestTower != null) {
                transform.position = Vector2.MoveTowards(transform.position, closestTower.transform.position, moveSpeed * Time.deltaTime);
            } 
        }
    }

  
    public void pushBack() {
        Vector3 currentPos = transform.position;
        transform.position = new Vector3(currentPos.x + 1, currentPos.y, currentPos.y);
    }

}
