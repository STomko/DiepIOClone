using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using TMPro;
using UnityEngine;

public class UI : MonoBehaviour
{
    public TankManager player;

    public TMP_Text scoreText;
    public TMP_Text timerText;
    public TMP_Text upgradeScoreText;

    TimeSpan playTimer;
    bool timerRunning = true;
    float elapsedTime;
    float upgradeInterval;
    float currentInterval;

    public GameObject healthBar;
    public GameObject healthBackground;

    public GameController gc;

    float healthXScale = 2.9f;
    float healthYScale = 0.4f;
    float healthZScale = 1.0f;

    bool playerGameOver = false;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<TankManager>();
        elapsedTime = 0.0f;
        upgradeInterval = 30.0f;
        currentInterval = upgradeInterval;
        StartCoroutine(updateTimer());
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerGameOver)
        {
            updateScoreText();
            updateHealthBar();
            updateUpgradeScoreText();
        }
    }

    void updateScoreText()
    {
        scoreText.text = "Score: " + player.score.ToString();
    }

    void updateUpgradeScoreText()
    {
        upgradeScoreText.text = "Upgrade Points: " + player.upgradePoints.ToString();
    }

    private IEnumerator updateTimer()
    {
        while (timerRunning)
        {
            elapsedTime += Time.deltaTime;
            playTimer = TimeSpan.FromSeconds(elapsedTime);
            timerText.text = "Time: " + playTimer.ToString("mm':'ss'.'ff");
            if (elapsedTime >= currentInterval)
            {
                currentInterval += upgradeInterval;
                gc.giveUpgradePoint();
            }

            yield return null;
        }
    }

    public void gameOverDisplay()
    {
        timerRunning = false;
        scoreText.fontSize *= 2;
        timerText.fontSize *= 2;
        Destroy(healthBar);
        Destroy(healthBackground);
    }

    /*Updates the user's health bar on the screen.*/
    void updateHealthBar()
    {
        if (player.currentHealthPercent() > 0)
        {
            healthBar.transform.localScale = new Vector3(healthXScale * player.currentHealthPercent(), healthYScale, healthZScale);
        }
        else
        {
            healthBar.transform.localScale = new Vector3(0, healthYScale, healthZScale);
            playerGameOver = true;
        }
    }
}
