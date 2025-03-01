using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    [SerializeField] private Image healthBar;
    private float healthAmt = 100;
    private float maxHealth = 100;

    private void Update() {
        if (healthAmt <= 0) {
            if (GameManager.Instance.SpawnedTroops.Contains(gameObject)) {
                GameManager.Instance.RemoveTroop(gameObject);
            } else if (GameManager.Instance.SpawnedTowers.Contains(gameObject)) {
                GameManager.Instance.RemoveTower(gameObject);
            }
            Destroy(gameObject);
        }
        
    }

    public void Initialize(float health)
    {
        healthAmt = health;
        maxHealth = health;
    }

    public void RecieveDamage(float Damage)
    {
        healthAmt -= Damage;
        healthBar.fillAmount = healthAmt / maxHealth;
    }

    public void Healing(float healPoints)
    {
        healthAmt += healPoints;
        healthAmt = Mathf.Clamp(healthAmt, 0, maxHealth);
        healthBar.fillAmount = healthAmt / maxHealth;
    }

    public float HealthAmt
    {
        get { return healthAmt; }
        set { healthAmt = value; }
    }

}

