using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MainScript : MonoBehaviour
{
    public List<HealthHeart> healthHearts;
    public Pooter pooter;
    public ScrollingScreen scrollingScreen;
    public static List<Wall> walls;
    public static List<BadGuy> badguys;
    public static List<Bullet> bullets;
    public static List<LimitedTimeVisualEffect> explosions;
    public static int currentScore = 0;
    int weThinkScoreIs = 0;//this page has what it thinks the score is and the actual score. the score is added to statically and in the next update checks it against this value to update teh current score text if need be
    public static float currentDifficulty = 1f;
    public TextMeshProUGUI currentScoreText;
    bool playedEnemyLast = true;
    int healthHeartDisplay = 3;//the health heart display. its the number of health hearts that are full or empty, it reacts to the static int from the pooterscript and updates when the game does;
    // Start is called before the first frame update
    void Start()
    {
        bullets = new List<Bullet>();
        badguys = new List<BadGuy>();
        explosions = new List<LimitedTimeVisualEffect>();
        walls = new List<Wall>();
        pooter.SetupPooter();
        SetupHealthHearts();
        currentScore = 0;
        currentScoreText.text = "Score : " + currentScore.ToString(); 
        currentScoreText.fontSize = Screen.width / 15f;
        currentScoreText.transform.position = new Vector3(Screen.width * 0.75f, Screen.height * 0.95f, 0f);
    }
    public static void AddToScore(int amt){currentScore += amt; }
    void SetupHealthHearts()
    {
        Vector3 origin = Camera.main.transform.position + (Vector3.up * Screen.height * 0.005f) + (Vector3.down * Pooter.brickLength * 3f) + (Vector3.left * Screen.width * 0.005f) + (Vector3.right * Pooter.brickLength * 3f);
        origin.z = -5f;
        for (int i = 0; i < healthHearts.Count; i++)
        {
            HealthHeart h = healthHearts[i];

            h.transform.position = origin + (Vector3.right * i * Pooter.brickLength * 3f);
            h.transform.localScale = Pooter.basicScale;
            h.SetOne();
        }
    }
    public static void CreateExplosion(Vector3 point)
    {
        point.z = -3f;
        GameObject g = (GameObject)Resources.Load("Explosion");
        g.transform.localScale = Pooter.basicScale * 2f;
        LimitedTimeVisualEffect l = Instantiate(g, point, Quaternion.identity).GetComponent<LimitedTimeVisualEffect>();
        MainScript.explosions.Add(l);
        l.Setup(1f);
    }
    public static void CreateBoomsplosion(Vector3 point)
    {
        point.z = -3f;
        GameObject g = (GameObject)Resources.Load("BoomSplosion");
        g.transform.localScale = Pooter.basicScale * 2.5f;
        LimitedTimeVisualEffect l = Instantiate(g, point, Quaternion.identity).GetComponent<LimitedTimeVisualEffect>();
        MainScript.explosions.Add(l);
        l.Setup(1f);
    }
    public static void CreateSparkleExplosion(Vector3 point)
    {
        point.z = -3f;
        GameObject g = (GameObject)Resources.Load("SparkleExplosion");
        g.transform.localScale = Pooter.basicScale * 2.5f;
        LimitedTimeVisualEffect l = Instantiate(g, point, Quaternion.identity).GetComponent<LimitedTimeVisualEffect>();
        MainScript.explosions.Add(l);
        l.Setup(1f);
    }
    public void TriggerScrollScreen()//when the pooter has reached a certain height he triggers the scrollscreen to instantiate something above him
    {
        float minAmt = 0.5f;
        if (playedEnemyLast) { minAmt += 0.25f; } else { minAmt -= 0.25f; }
        if (Random.value > minAmt)
        {
            playedEnemyLast = true;
            float difficultyPoint = currentDifficulty; if (difficultyPoint > 4f) { difficultyPoint = 4f; }
            int randomInt = (int)Random.Range(0f, difficultyPoint);
            switch (randomInt)
            {
                case 0:
                    scrollingScreen.CreateEnemy((GameObject)Resources.Load("Prefabs/UFOBadGuyPrefab"), 2f);
                    //scrollingScreen.CreateEnemy((GameObject)Resources.Load("Prefabs/BadGuyPrefab"), 1.5f);
                    break;
                case 1:
                    scrollingScreen.CreateEnemy((GameObject)Resources.Load("Prefabs/homingMissile"), 0.85f);
                    break;
                case 2:
                    scrollingScreen.CreateEnemy((GameObject)Resources.Load("Prefabs/CircleBadGuy"), 2f);
                    break;
                case 3:
                    scrollingScreen.CreateEnemy((GameObject)Resources.Load("Prefabs/UFOBadGuyPrefab"), 2f);
                    //scrollingScreen.CreateEnemy((GameObject)Resources.Load("Prefabs/BadGuy"), 2f);
                    break;
            }

        } else
        {
            playedEnemyLast = false;
            float difficultyPoint = currentDifficulty + 1f; if (difficultyPoint > 4f) { difficultyPoint = 4f; }
            int numberOfWalls = (int)Random.Range(1f, difficultyPoint);
            float totalWidth = ((Screen.width * 0.0045f) - Pooter.brickLength) * 2f;
            Vector3 origin = Vector3.left * (totalWidth * 0.5f);
            origin.y = ScrollingScreen.GetYPosAboveScreen();
            float distBetween = totalWidth / (numberOfWalls);
            Debug.DrawLine(Vector3.zero, Vector3.left * totalWidth * 0.5f);
            float randomXOffsetMax = (totalWidth * 0.9f) / numberOfWalls;
            //float xOffset = Random.Range(0f, randomXOffsetMax);
            for (int i = 0; i < numberOfWalls; i++)
            {
                Vector3 pos = origin + (Vector3.right * i * distBetween) + (Vector3.right * Random.value * distBetween);
                scrollingScreen.CreateWall(2f, 2f, 0f, pos);
            }
        }
    }
    void UpdateGame(float timePassed)
    {
        if(currentScore != weThinkScoreIs){weThinkScoreIs = currentScore;currentScoreText.text = "Score : " + currentScore.ToString();}
        currentDifficulty += timePassed * 0.075f;
        pooter.UpdatePooter(timePassed);
        scrollingScreen.UpdateScrollingScreen(timePassed);
        for (int i = 0; i < badguys.Count; i++) { BadGuy b = badguys[i]; b.UpdateBadGuy(timePassed); if (b.GetReadyToDie()) { badguys.Remove(b); b.DestroyBadGuy(); } }
        for (int i = 0; i < bullets.Count; i++) { Bullet b = bullets[i]; b.UpdateBullet(timePassed); if (b.GetReadyToDie()) { bullets.Remove(b); b.DestroyBullet(); i--; } }
        for (int i = 0; i < explosions.Count; i++) { LimitedTimeVisualEffect e = explosions[i]; e.endCounter.UpdateCounter(timePassed); if (e.endCounter.hasfinished) { explosions.Remove(e); i--; e.Destroy(); } }
        if (Pooter.currentHealth != healthHeartDisplay) { SetHealthHeartsToPooter(); }
    }
    void SetHealthHeartsToPooter()
    {
        healthHeartDisplay = Pooter.currentHealth;
        for(int i = 0; i < healthHearts.Count; i++)
        {
            HealthHeart h = healthHearts[i];
            bool isFull = i < healthHeartDisplay;
            if (isFull && !h.isFull)
            {
                h.SetOne();
            }
            else if(!isFull && h.isFull)
            {
                h.SetZero();
            }
        }
    }
    private void FixedUpdate()
    {
        UpdateGame(Time.fixedDeltaTime);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
