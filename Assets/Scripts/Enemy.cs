using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    #region Movement_variables
    public float movespeed;
    #endregion

    #region Physics_components
    Rigidbody2D EnemyRB;
    #endregion

    #region Targeting_variables
    public Transform player;
    #endregion

    #region Attack_variables
    public float explosionDamage;
    public float explosionRadius;
    public GameObject explosionObj;
    #endregion

    #region Health_variables
    public float maxHealth;
    float currHealth;        
    #endregion

    #region Unity_functions

    // runs once on creation
    private void Awake() {
        EnemyRB = GetComponent<Rigidbody2D>();

        currHealth = maxHealth;
    }

    // runs once every frame
    private void Update() {
        // check to see if we know where player is
        if (player == null) {
            return;
        }

        Move();
    }
    #endregion

    #region Movement_functions

    // move directly at player
    private void Move() {
        // calculate movement vector player position - enemy position = direction of player relative to enemy
        Vector2 direction = player.position - transform.position;

        EnemyRB.velocity = direction.normalized * movespeed;
    }
    #endregion

    // Raycasts box for player, causes damage, spawns explosion prefab
    #region Attack_functions
    private void Explode() {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, explosionRadius, Vector2.zero);

        foreach (RaycastHit2D hit in hits) {
            if (hit.transform.CompareTag("Player"))
            {
                // Cause damage
                Debug.Log("Hit player with explosion");

                // Spawn explosion prefab
                Instantiate(explosionObj, transform.position, transform.rotation);
                hit.transform.GetComponent<PlayerController>().TakeDamage(explosionDamage);
                Destroy(this.gameObject);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.transform.CompareTag("Player")) {
            Explode();
        }
    }
        
    #endregion
    #region Health_functions
    // Enemy takes damage based on value param
    public void TakeDamage(float value) {
        // Decrement health
        currHealth -= value;
        Debug.Log("Health is now " + currHealth.ToString());

        // Check for death
        if (currHealth <= 0) {
            Die();
        }
    }

    // Destroys game object
    private void Die() {
        Destroy(this.gameObject);
    }
        
    #endregion
}
