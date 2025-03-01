using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{

    private Health health;
    public GameObject closestTroop;
    private GameObject bulletType;

    private int type;
    private float range;
    private float interval = 0.3f;
    private float timer = 0.3f;

    public void Initialize(float health, int type)
    {
        this.type = type;
        this.health = GetComponent<Health>();


        switch (type)
        {
            case 1:
                health = 100;
                range = 50f;
                bulletType = GameManager.Instance.bulletTowerPrefab0;
                interval = 0.3f;
                timer = interval;
                break;
            case 2:
                health = 100;
                range = 50f;
                bulletType = GameManager.Instance.bulletTowerPrefab1Foot;
                interval = 0.5f;
                timer = interval;
                break;
            case 3:
                health = 100;
                range = 50f;
                interval = 0.3f;
                timer = interval;
                bulletType = GameManager.Instance.bulletTowerPrefab2;
                break;
            case 4:
                health = 100;
                range = 50f;
                interval = 0.3f;
                timer = interval;
                bulletType = GameManager.Instance.bulletTowerPrefab3;
                break;
            default:
                Destroy(gameObject);
                GameManager.Instance.towerCounter--;
                break;
        }

        this.health.Initialize(health);
    }

    private void Update()
    {
        timer += Time.deltaTime;
        // Ensure there are objects in the list and a valid target object
        if (GameManager.Instance.SpawnedTroops.Count == 0)
        {
            closestTroop = null;
            return;
        }

        float closestDistance = Mathf.Infinity;

        foreach (GameObject troop in GameManager.Instance.SpawnedTroops)
        {
            // Get the position of the target object and the current object in the list
            Vector2 towerPosition = gameObject.transform.position;
            Vector2 troopPosition = troop.transform.position;

            // Calculate the distance between the target position and the current object position
            float distance = Vector2.Distance(towerPosition, troopPosition);

            // Check if the current object is closer than the previously closest object
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTroop = troop;
            }
        }

        if (timer >= interval && closestDistance < range)
        {
            if (type == 2){
                switch(Random.Range(0, 3)) {
                    case 0:
                    bulletType = GameManager.Instance.bulletTowerPrefab1Basket;
                    break;
                    case 1: 
                    bulletType = GameManager.Instance.bulletTowerPrefab1Beach;
                    break;
                    case 2: 
                    bulletType = GameManager.Instance.bulletTowerPrefab1Foot;
                    break;
                    default:
                    bulletType = GameManager.Instance.bulletTowerPrefab1Basket;
                    break;
                }
            }
            GameObject bullet = Instantiate(bulletType, transform.position, Quaternion.identity);
            bullet.GetComponent<Bullet>().Initialize(closestTroop, type);
            timer = 0.0f;
        }


    }

}
