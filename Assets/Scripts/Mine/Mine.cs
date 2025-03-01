using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour
{

    private float mineDamage = 0.5f; 
    private float radius = 10f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        List<GameObject> ObjectsToDamange = FindObjectsWithinRadius();

        foreach (GameObject troopToDamage in ObjectsToDamange) {
            troopToDamage.TakeDamage(50f);
        }
       
        Destroy(gameObject);
    }

    private List<GameObject> FindObjectsWithinRadius()
    {
        List<GameObject> objectsWithinRadius = new List<GameObject>();

        foreach (GameObject troop in GameManager.Instance.SpawnedTroops)
        {
            // Calculate the distance between this.gameObject and the current object in the list
            float distance = Vector2.Distance(transform.position, troop.transform.position);

            // Check if the object is within the specified radius
            if (distance <= radius)
            {
                // Add the object to the list of objects within the radius
                objectsWithinRadius.Add(troop);
            }
        }

        return objectsWithinRadius;
    } 
}
