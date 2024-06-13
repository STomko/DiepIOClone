using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    private static GameController instance = null;

    bool isPaused;
    bool isGameOver;
    public bool hasPlayerRespawned = false;

    public TankManager player;
    public TankManager enemy;

    public GameObject enemyPrefab;
    public GameObject tempObj;

    public Canvas UI;

    public CameraMovement camMovement;

    public Sprite customTankSprite;
    public Sprite customWeaponSprite;

    /*Scores needed to upgrade.*/
    public int tier1Score = 100;
    public int tier2Score = 500;
    public int tier3Score = 2500;

    public static int currentUpgradeLevel = 0;

    /*Stat increases:*/
    public static float speedIncrease = 0.25f;
    public float regenIncrease = -0.1f;
    public float projectileSpeedIncrease = 0.5f;
    public int projectilePenetrationIncrease = 1;
    public float reloadIncrease = -0.05f;

    /*Stat increase buttons:*/
    public Button regenButton;
    public Button projSpeedButton;
    public Button projPenetrationButton;
    public Button reloadButton;

    /*Player upgrade counts.
     *When the player becomes a new tank, the stats are reset, this is used to reapply them.*/
    public int regenerationCount = 0;
    public int projectileSpeedCount = 0;
    public int projectilePenetrationCount = 0;
    public int reloadSpeedCount = 0;

    /*Player upgrades go here*/
    public GameObject t1TwinPlayer;
    public GameObject t1SniperPlayer;
    public GameObject t1FlankPlayer;
    public GameObject t1MachinePlayer;
    public GameObject t1CannonPlayer;

    public GameObject t2TwinFlankPlayer;
    public GameObject t2TwinQuadPlayer;
    public GameObject t2TwinTriplePlayer;

    public GameObject t3TwinFlankTriplePlayer;
    public GameObject t3TwinQuadOctoPlayer;
    public GameObject t3TwinTriplePentaPlayer;
    public GameObject t3TwinTripleSpreadPlayer;
    public GameObject t3TwinTripleTripletPlayer;

    /*Enemy upgrades go here*/
    public GameObject t1TwinEnemy;
    public GameObject t1SniperEnemy;
    public GameObject t1FlankEnemy;
    public GameObject t1MachineEnemy;
    public GameObject t1CannonEnemy;

    public GameObject t2TwinFlankEnemy;
    public GameObject t2TwinQuadEnemy;
    public GameObject t2TwinTripleEnemy;

    public GameObject t3TwinFlankTripleEnemy;
    public GameObject t3TwinQuadOctoEnemy;
    public GameObject t3TwinTriplePentaEnemy;
    public GameObject t3TwinTripleSpreadEnemy;
    public GameObject t3TwinTripleTripletEnemy;

    public static GameController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameController();
            }
            return instance;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        isPaused = false;
        isGameOver = false;
        hideButtons();
        player = GameObject.Find("Player").GetComponent<TankManager>();
        enemy = GameObject.Find("Enemy").GetComponent<TankManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isGameOver)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                pausePressed();
            }
            checkForEnemyUpgrade();
            checkForGameOver();
            debug();
        }  
    }

    /*Stops everything moving when the Escape key is pressed.
     *Resumes when pressed again.*/
    void pausePressed()
    {
        isPaused = !isPaused;
        if (isPaused)
        {
            showButtons();
            Time.timeScale = 0.0f;
        }
        else
        {
            hideButtons();
            Time.timeScale = 1.0f;
        }
    }

    public void respawnEnemy()
    {
        GameObject newEnemy = Instantiate(enemyPrefab, new Vector3(0,0,-1), Quaternion.identity);
        newEnemy.name = "Enemy";
        newEnemy.transform.position = newEnemy.GetComponent<EnemyMovement>().newTargetPos();
        player.setEnemy(newEnemy);
        enemy = newEnemy.GetComponent<TankManager>();
        enemy.gainPoints(player.score);

        /*When different prefabs and upgrades etc. are added, this should be randomly selected based on the player score.
         *Or, give the enemy a score equal to that of the player? Then upgrading will be automatic.*/
    }

    void checkForEnemyUpgrade()
    {
        if (enemy.tier == 0)
        {
            if (enemy.score >= tier1Score)
            {
                enemy.score -= tier1Score;
                GameObject[] t1Options = { t1TwinEnemy, t1SniperEnemy, t1FlankEnemy, t1MachineEnemy, t1CannonEnemy };
                updateEnemy(t1Options[Random.Range(0, t1Options.Length)]);
                enemy.tier = 1;
            }
        }
        if (enemy.tier == 1)
        {
            if (enemy.score >= tier2Score)
            {
                enemy.score -= tier2Score;
                GameObject[] t2Options = { t2TwinFlankEnemy, t2TwinQuadEnemy, t2TwinTripleEnemy };
                updateEnemy(t2Options[Random.Range(0, t2Options.Length)]);
                enemy.tier = 2;
            }
        }
        if (enemy.tier == 2)
        {
            if (enemy.score >= tier3Score)
            {
                enemy.score -= tier3Score;
                GameObject[] t3Options = { t3TwinFlankTripleEnemy, t3TwinQuadOctoEnemy, t3TwinTriplePentaEnemy, t3TwinTripleSpreadEnemy, t3TwinTripleTripletEnemy };
                updateEnemy(t3Options[Random.Range(0, t3Options.Length)]);
                enemy.tier = 3;
            }
        }
        //OTHER TIERS GO HERE
    }

    void checkForGameOver()
    {
        if (player == null)
        {
            isGameOver = true;
            UI.GetComponent<UI>().gameOverDisplay();
        }
    }

    /*This gives both the player and the enemy an upgrade point to change their stats.
     *Enemy point is spent straight away randomly.*/
    public void giveUpgradePoint()
    {
        player.gainUpgradePoint();
        giveEnemyUpgrade();
        currentUpgradeLevel += 1;
    }

    /*Looks for any button in the UI and displays them.
     *This way, you don't need to manually set up every button.
     *Any new buttons you add to the canvas will automatically show and hide when Esc is pressed.*/
    public void showButtons()
    {
        foreach (Transform child in UI.transform)
        {
            if (child.gameObject.GetComponent<Button>())
            {
                child.gameObject.SetActive(true);
            }
        }
    }

    public void hideButtons()
    {
        foreach (Transform child in UI.transform)
        {
            if (child.gameObject.GetComponent<Button>())
            {
                child.gameObject.SetActive(false);
            }
        }
    }

    /***********NEW TANK BUTTONS***********/
    public void onT1TwinPress()
    {
        if (attemptToBuy(tier1Score))
        {
            updatePlayer(t1TwinPlayer);
        }
    }

    public void onT1SniperPress()
    {
        if (attemptToBuy(tier1Score))
        {
            updatePlayer(t1SniperPlayer);
        }
    }

    public void onT1FlankPress()
    {
        if (attemptToBuy(tier1Score))
        {
            updatePlayer(t1FlankPlayer);
        }
    }

    public void onT1MachinePress()
    {
        if (attemptToBuy(tier1Score))
        {
            updatePlayer(t1MachinePlayer);
        }
    }

    public void onT1CannonPress()
    {
        if (attemptToBuy(tier1Score))
        {
            updatePlayer(t1CannonPlayer);
        }
    }

    public void onT2TwinFlankPress()
    {
        if (attemptToBuy(tier2Score))
        {
            updatePlayer(t2TwinFlankPlayer);
        }
    }

    public void onT2TwinQuadPress()
    {
        if (attemptToBuy(tier2Score))
        {
            updatePlayer(t2TwinQuadPlayer);
        }
    }

    public void onT2TwinTriplePress()
    {
        if (attemptToBuy(tier2Score))
        {
            updatePlayer(t2TwinTriplePlayer);
        }
    }

    public void onT3TwinFlankTriplePress()
    {
        if (attemptToBuy(tier3Score))
        {
            updatePlayer(t3TwinFlankTriplePlayer);
        }
    }

    public void onT3TwinQuadOctoPress()
    {
        if (attemptToBuy(tier3Score))
        {
            updatePlayer(t3TwinQuadOctoPlayer);
        }
    }

    public void onT3TwinTriplePentaPress()
    {
        if (attemptToBuy(tier3Score))
        {
            updatePlayer(t3TwinTriplePentaPlayer);
        }
    }

    public void onT3TwinTripleSpreadPress()
    {
        if (attemptToBuy(tier3Score))
        {
            updatePlayer(t3TwinTripleSpreadPlayer);
        }
    }

    public void onT3TwinTripleTripletPress()
    {
        if (attemptToBuy(tier3Score))
        {
            updatePlayer(t3TwinTripleTripletPlayer);
        }
    }
    /***********NEW TANK BUTTONS***********/

    /***********UPGRADE BUTTONS***********/
    public void onRegenerationPress()
    {
        if (player.spendUpgradePoint())
        {
            regenerationCount++;
            regenButton.GetComponentInChildren<TMP_Text>().text = "Regeneration: Level " + regenerationCount;
            player.upgradeRegen(regenIncrease);
        }
    }

    public void onProjSpeedPress()
    {
        if (player.spendUpgradePoint())
        {
            projectileSpeedCount++;
            projSpeedButton.GetComponentInChildren<TMP_Text>().text = "Projectile Speed: Level " + projectileSpeedCount;
            player.upgradeProjSpeed(projectileSpeedIncrease);
        }
    }

    public void onProjPenetrationPress()
    {
        if (player.spendUpgradePoint())
        {
            projectilePenetrationCount++;
            projPenetrationButton.GetComponentInChildren<TMP_Text>().text = "Projectile Penetration: Level " + projectilePenetrationCount;
            player.upgradeProjPenetration(projectilePenetrationIncrease);
        }
    }

    public void onReloadPress()
    {
        if (player.spendUpgradePoint())
        {
            reloadSpeedCount++;
            reloadButton.GetComponentInChildren<TMP_Text>().text = "Reload Speed: Level " + reloadSpeedCount;
            player.upgradeReloadSpeed(reloadIncrease);
        }
    }
    /***********UPGRADE BUTTONS***********/

    bool attemptToBuy(int toSpend)
    {
        if (player.score >= toSpend)
        {
            player.score -= toSpend;
            return true;
        }
        return false;
    }

    void updatePlayer(GameObject newPrefab)
    {
        Vector3 playerPos = player.transform.position;
        int currentScore = player.score;
        int currentUpgrades = player.upgradePoints;
        Destroy(player.gameObject);
        GameObject newPlayer = Instantiate(newPrefab, playerPos, Quaternion.identity);
        newPlayer.name = "Player";

        player.score = currentScore;
        player.upgradePoints = currentUpgrades;
        player = newPlayer.GetComponent<TankManager>();
        UI.GetComponent<UI>().player = player;
        camMovement.player = newPlayer;
        enemy.enemy = newPlayer;
        reapplyPlayerUpgrades();
        pausePressed();
    }

    void updateEnemy(GameObject newPrefab)
    {
        Vector3 enemyPos = enemy.transform.position;
        int currentScore = enemy.score;

        GameObject oldEnemy = enemy.gameObject;
        GameObject newEnemy = Instantiate(newPrefab, enemyPos, Quaternion.identity);
        newEnemy.name = "Enemy";

        enemy = newEnemy.GetComponent<TankManager>();
        enemy.score = currentScore;
        player.enemy = newEnemy;
        reapplyEnemyUpgrades();
        
        Destroy(oldEnemy.gameObject);
    }

    void reapplyPlayerUpgrades()
    {
        for (int i = 0; i < regenerationCount; i++)
        {
            player.upgradeRegen(regenIncrease);
        }
        for (int i = 0; i < projectileSpeedCount; i++)
        {
            player.upgradeProjSpeed(projectileSpeedIncrease);
        }
        for (int i = 0; i < projectilePenetrationCount; i++)
        {
            player.upgradeProjPenetration(projectilePenetrationIncrease);
        }
        for (int i = 0; i < reloadSpeedCount; i++)
        {
            player.upgradeReloadSpeed(reloadIncrease);
        }
    }

    void reapplyEnemyUpgrades()
    {
        for (int i = 0; i < currentUpgradeLevel; i++)
        {
            giveEnemyUpgrade();
        }
    }

    /*When the enemy respawns, they randomly apply the upgrades.*/
    void giveEnemyUpgrade()
    {
        int choice = Random.Range(0, 4);
        switch (choice)
        {
            case 0:
                enemy.upgradeRegen(regenIncrease);
                break;
            case 1:
                enemy.upgradeProjSpeed(projectilePenetrationIncrease);
                break;
            case 2:
                enemy.upgradeProjPenetration(projectilePenetrationIncrease);
                break;
            default:
                enemy.upgradeReloadSpeed(reloadIncrease);
                break;

        }
    }

    /*Debug purposes.*/
    void debug()
    {
        if (Input.GetKey("l"))
        {
            updatePlayer(tempObj);
        }
    }
}
