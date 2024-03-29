using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jad : MonoBehaviour
{
    [HideInInspector] public Enemy enemyScript;
    [HideInInspector] public NPC npcScript;
    Combat combatScript;

    public Sprite mageProjectile;
    public Sprite rangeProjectile;
    public Color mageProjectileColor;
    public Color rangeProjectileColor;

    public Sprite protectFromRange;
    public Sprite protectFromMage;

    public SpriteRenderer signalSprite;

    int currentStyle;

    public GameObject healerPrefab;
    bool healersSpawned = false;
    public int numberOfHealers = 3;

    public AudioClip rangeSound;
    public AudioClip mageSound;
    AudioSource audioSource;

    private void Start()
    {
        enemyScript = GetComponent<Enemy>();
        npcScript = GetComponent<NPC>();
        combatScript = GetComponent<Combat>();
        combatScript.projectileSpawnDelay = 0;

        signalSprite.enabled = false;

        TickManager.afterTick += AfterTick;

        audioSource = GetComponent<AudioSource>();
    }

    void AfterTick()
    {
        if (combatScript.attackCooldown > 3 || enemyScript.death)
        {
            signalSprite.enabled = false;
        }
        else
        {
            signalSprite.enabled = true;
        }

        if (combatScript.attackCooldown == 3)
        {
            currentStyle = Random.Range(0, 2);
            if (currentStyle == 0)
            {
                signalSprite.sprite = protectFromRange;
                enemyScript.attackStyle = "Ranged";
                enemyScript.customProjectile = rangeProjectile;
                enemyScript.projectileColor = rangeProjectileColor;
                audioSource.clip = rangeSound;
            }
            else
            {
                signalSprite.sprite = protectFromMage;
                enemyScript.attackStyle = "Magic";
                enemyScript.customProjectile = mageProjectile;
                enemyScript.projectileColor = mageProjectileColor;
                audioSource.clip = mageSound;
            }

            audioSource.Play();
        }

        if (healersSpawned == false && (float)enemyScript.hitpoints <= (float)enemyScript.initialHitpoints / 2)
        {
            SpawnHealers();
        }
    }

    void SpawnHealers()
    {
        healersSpawned = true;
        if (numberOfHealers == 3)
        {
            for (int i = 0; i < 3; i++)
            {
                Vector2 position = npcScript.trueTile + Vector2.up * npcScript.tileSize;
                if (i == 1)
                {
                    position += Vector2.up;
                }
                if (i == 2)
                {
                    position += Vector2.left;
                }
                GameObject newHealer = Instantiate(healerPrefab, position, Quaternion.identity);
                newHealer.name = healerPrefab.name;
                newHealer.GetComponent<JadHealer>().jadScript = this;
            }
        }
    }

    private void OnDestroy()
    {
        TickManager.afterTick -= AfterTick;
    }

}
