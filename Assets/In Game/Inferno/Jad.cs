using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jad : MonoBehaviour
{
    Enemy enemyScript;
    NPC npcScript;
    Combat combatScript;

    public Sprite mageProjectile;
    public Sprite rangeProjectile;
    public Color mageProjectileColor;
    public Color rangeProjectileColor;

    public Sprite protectFromRange;
    public Sprite protectFromMage;

    public SpriteRenderer signalSprite;

    int currentStyle;

    private void Start()
    {
        enemyScript = GetComponent<Enemy>();
        npcScript = GetComponent<NPC>();
        combatScript = GetComponent<Combat>();
        combatScript.projectileSpawnsImmediately = true;

        signalSprite.enabled = false;

        TickManager.afterTick += AfterTick;
    }

    void AfterTick()
    {
        if (combatScript.attackCooldown == enemyScript.attackSpeed || combatScript.attackCooldown <= 0)
        {
            signalSprite.enabled = false;
        }

        if (combatScript.attackCooldown == 3)
        {
            currentStyle = Random.Range(0, 2);
            signalSprite.enabled = true;
            if (currentStyle == 0)
            {
                signalSprite.sprite = protectFromRange;
                enemyScript.attackStyle = "Ranged";
                enemyScript.customProjectile = rangeProjectile;
                enemyScript.projectileColor = rangeProjectileColor;
            }
            else
            {
                signalSprite.sprite = protectFromMage;
                enemyScript.attackStyle = "Magic";
                enemyScript.customProjectile = mageProjectile;
                enemyScript.projectileColor = mageProjectileColor;
            }
        }
    }

}
