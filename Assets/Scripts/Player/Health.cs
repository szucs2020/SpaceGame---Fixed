/*
 * Health.cs
 * Authors: Christian, Lorant
 * Description: This is the class that supports player health, damage and death.
 */
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Health : NetworkBehaviour {

    public float maxHealth;
    private Image HealthBar;
    private bool regen = false;
    public float regenTime = 6f;
    private float regenStarTime = 0f;

    [SyncVar(hook = "UpdateHealthBar")]
    private float health;
    [SyncVar]
    private float damage;

    public void Start() {
        HealthBar = transform.FindChild("HealthCanvas").FindChild("HealthBG").FindChild("Health").GetComponent<Image>();
        health = maxHealth;
        UpdateHealthBar(health);
    }

    void Update() {

        if (!isServer) {
            return;
        }

        if (Time.time - regenStarTime > regenTime && regen) {
            regenHealth();
        }
    }

    public void Damage(float damage) {

        //damage should only be done on the server
        if (!isServer) {
            return;
        }

        this.damage += damage;
        this.health -= damage;

        //player death
        if (this.health <= 0) {
            Die();
        }

        regenStarTime = Time.time;
        regen = true;
    }

    public void Kill() {
        if (!isServer) {
            return;
        }
        Die();
    }

    //updates character health bar
    private void UpdateHealthBar(float curHealth) {
        bool b = HealthBar == null;
        if (HealthBar != null) {
            HealthBar.fillAmount = (curHealth / this.maxHealth);
        }
    }

    //kill player and attempt respawn
    private void Die() {

        Player p = GetComponent<Player>();
        string pname = p.playerName;
        int pslot = p.playerSlot;

        bool isAI = p.getIsAI();

        if (isAI) {
            GameObject.Find("GameSettings").GetComponent<GameController>().AttemptSpawnAI(pslot, pname);
        } else {
            GameObject.Find("GameSettings").GetComponent<GameController>().AttemptSpawnPlayer(this.connectionToClient, this.playerControllerId, pslot, pname);
        }
        p.Die();
    }

    private void regenHealth() {
        health += Time.deltaTime * 10;

        if (health >= maxHealth)
            regen = false;
    }

    public float getHealth() {
        return health;
    }
}
