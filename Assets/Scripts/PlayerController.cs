using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Movement_variables
    public float movespeed;
    float x_input;
    float y_input;
    #endregion

    #region Physics_components
    Rigidbody2D PlayerRB;
    #endregion

    #region Attack_variables
    public float Damage;
    public float attackspeed = 1;
    float attackTimer;
    public float hitboxTiming;
    public float endanimationTiming;
    bool isAttacking;
    Vector2 currDirection;
    #endregion

    #region Animation_components;
    Animator anim;
    #endregion

    #region Health_variables
    public float maxHealth;
    float currHealth;
    #endregion

    #region Unity_functions
    private void Awake() {
        PlayerRB = GetComponent<Rigidbody2D>();

        attackTimer = 0;

        anim = GetComponent<Animator>();

        currHealth = maxHealth;
    }

    private void Update() {
        if (isAttacking) {
            return;
        }
        x_input = Input.GetAxisRaw("Horizontal");
        y_input = Input.GetAxisRaw("Vertical");

        Move();

        if (Input.GetKeyDown(KeyCode.J) && attackTimer <= 0) {
            Attack();
        } else {
            attackTimer -= Time.deltaTime;
        }
    }
    #endregion

    #region Movement_functions
    private void Move() {
        anim.SetBool("Moving", true);

        if (x_input > 0) {
            PlayerRB.velocity = Vector2.right * movespeed;
            currDirection = Vector2.right;

        } else if (x_input < 0) {
            PlayerRB.velocity = Vector2.left * movespeed;
            currDirection = Vector2.left;
        } else if (y_input > 0) {
            PlayerRB.velocity = Vector2.up * movespeed;
            currDirection = Vector2.up;
        } else if (y_input < 0) {
            PlayerRB.velocity = Vector2.down * movespeed;
            currDirection = Vector2.down;
        } else {
            PlayerRB.velocity = Vector2.zero;
            anim.SetBool("Moving", false);
        }

        anim.SetFloat("DirX", currDirection.x);
        anim.SetFloat("DirY", currDirection.y);
    }
    #endregion

    #region Attack_Functions
    private void Attack() {
        Debug.Log("Attacking now");
        Debug.Log(currDirection);
        attackTimer = attackspeed;
        // handles animations and calculates hitboxes
        StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine() {
        isAttacking = true;
        PlayerRB.velocity = Vector2.zero;

        anim.SetTrigger("Attacktrig");
        yield return new WaitForSeconds(hitboxTiming);
        Debug.Log("Casting hitbox now");
        RaycastHit2D[] hits = Physics2D.BoxCastAll(PlayerRB.position + currDirection, Vector2.one, 0f, Vector2.zero);

        foreach (RaycastHit2D hit in hits) {
            Debug.Log(hit.transform.name);
            if (hit.transform.CompareTag("Enemy")) {
                Debug.Log("Tons of damage");
                hit.transform.GetComponent<Enemy>().TakeDamage(Damage);
            }
        }
        yield return new WaitForSeconds(hitboxTiming);
        isAttacking = false;

        yield return null;
    }
    #endregion

    #region Health_functions

    // Take damage based on value param passed in by caller
    public void TakeDamage(float value) {
        // Decrement health
        currHealth -= value;
        Debug.Log("Health is now " + currHealth.ToString());

        // Change UI

        // Check if dead
        if (currHealth <= 0) {
            Die();
        }
    }

    // Heals player based on value param passed in by caller
    public void Heal(float value) {
        // Increment health by value
        currHealth += value;
        currHealth = Mathf.Min(currHealth, maxHealth);
        Debug.Log("Health is now " + currHealth.ToString());
    }

    // Destroys player object and triggers end scene stuff
    private void Die() {
        // Destroy this object
        Destroy(this.gameObject);

        // Trigger anything to end the game, find GameManager and Lose game
    }
        
    #endregion
}