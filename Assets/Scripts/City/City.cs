using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class City : MonoBehaviour
{

    public Health health;

    public void Initialize(float health)
    {
        this.health = GetComponent<Health>();
        this.health.Initialize(health);
    }

}
