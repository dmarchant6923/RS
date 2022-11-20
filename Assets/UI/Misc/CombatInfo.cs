using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatInfo : MonoBehaviour
{
    public Text playerAttack;
    public Text enemyDefense;
    public Text playerAttackResult;
    public Text enemyName;
    public Text enemyAttack;
    public Text playerDefense;
    public Text enemyAttackResult;
    public static CombatInfo instance;

    private void Start()
    {
        instance = this;
        playerAttack.text = "";
        enemyDefense.text = "";
        playerAttackResult.text = "";
        enemyAttack.text = "";
        playerDefense.text = "";
        enemyAttackResult.text = "";
    }

    public static void PlayerAttack(string attackStyle, int attackRoll)
    {
        instance.playerAttack.text =
        "Player attack style: " + attackStyle + "\n" +
        "Player max attack roll: " + attackRoll;
    }

    public static void EnemyDefense(int enemyDefLvl, int enemyStyleBonus, int defRoll, bool magic, string playerAttackStyle)
    {
        string level = "defense";
        if (magic)
        {
            level = "magic";
        }
        instance.enemyDefense.text =
        "Enemy " + level + " level: " + enemyDefLvl + "\n" +
        "Enemy " + playerAttackStyle + " defense bonus: " + enemyStyleBonus + "\n" +
        "Enemy max defense roll: " + defRoll;
    }
    public static void PlayerAttackResult(float hitChance, int maxHit, float dps)
    {
        instance.playerAttackResult.text =
        "Player hit chance: " + hitChance * 100 + "%\n" +
        "Player max hit: " + maxHit + "\n" +
        "Damage per second: " + dps;

    }
    public static void EnemyAttack(string name, string attackStyle, int styleLevel, int attackBonus, int attackRoll)
    {
        instance.enemyName.text = "DEFENDING (" + name + ")";

        string skill = "attack";
        if (attackStyle == AttackStyles.rangedStyle)
        {
            skill = "ranged";
        }
        else if (attackStyle == AttackStyles.magicStyle)
        {
            skill = "magic";
        }

        instance.enemyAttack.text =
        "Enemy attack style: " + attackStyle + "\n" +
        "Enemy " + skill + " level: " + styleLevel + "\n" +
        "Enemy " + attackStyle.ToLower() + " bonus: " + attackBonus + "\n" +
        "Enemy max attack roll: " + attackRoll;
    }
    public static void PlayerDefense(int playerDefBonus, int defRoll, string enemyAttackStyle)
    {
        instance.playerDefense.text =
        "Player " + enemyAttackStyle + " defense bonus: " + playerDefBonus + "\n" +
        "Player max defense roll: " + defRoll;

    }
    public static void EnemyAttackResult(float hitChance, int maxHit, float dps)
    {
        instance.enemyAttackResult.text =
        "Enemy hit chance: " + hitChance * 100 + "%\n" +
        "Enemy max hit: " + maxHit + "\n" +
        "Damage per second: " + dps;
    }
}
