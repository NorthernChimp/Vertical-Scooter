using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MainScript : MonoBehaviour
{
    public InterstitialAdLoader adLoader;
    static float currentScoreMultiplier = 1f;
    static Counter multiplierCounter;
    static Counter sparkleCounter;
    public List<HealthHeart> healthHearts;
    public Pooter pooter;
    public ScrollingScreen scrollingScreen;
    public MainMenuScript mainMenu;
    List<Wall> walls;
    List<BadGuy> badguys;
    List<PowerUp> powerups;
    public static List<Bullet> bullets;
    public static List<LimitedTimeVisualEffect> explosions;
    public static bool gamePaused = false;
    public static int currentScore = 0;
    int weThinkScoreIs = 0;//this page has what it thinks the score is and the actual score. the score is added to statically and in the next update checks it against this value to update teh current score text if need be
    static bool scoreMultiplierHasChanged = false;
    public static float currentDifficulty = 1f;
    public Transform gameOverScreen;
    public TextMeshProUGUI currentScoreText;
    public TextMeshProUGUI currentScoreMultiplierText;
    bool playedEnemyLast = true;
    public static bool gameStarted = false;
    public static bool gameEnded = false;
    bool hasShownAd = false;
    Counter gameOverScreenCounter;
    int healthHeartDisplay = 3;//the health heart display. its the number of health hearts that are full or empty, it reacts to the static int from the pooterscript and updates when the game does;
    // Start is called before the first frame update
    void Start()
    {
        gameOverScreenCounter = new Counter(1.25f);
        Vector3 gameOverPos = gameOverScreen.position;gameOverPos.z = 5f;gameOverPos += Vector3.up * Screen.height * 0.01f; gameOverScreen.localPosition = gameOverPos;
        currentScoreMultiplierText.text = "X " + currentScoreMultiplier.ToString();
        currentScoreMultiplierText.fontSize = Screen.width / 20f;
        currentScoreMultiplierText.transform.position = new Vector3(Screen.width * 0.75f, Screen.height * 0.9f, 0f);
        multiplierCounter = new Counter(2.5f);
        multiplierCounter.UpdateCounter(1f);//starts out completed, resets when multiplier increases
        sparkleCounter = new Counter(0.420f);
        bullets = new List<Bullet>();
        badguys = new List<BadGuy>();
        explosions = new List<LimitedTimeVisualEffect>();
        walls = new List<Wall>();
        pooter.SetupPooter();
        pooter.transform.position = new Vector3(0f, 0f, pooter.transform.position.z);
        SetupHealthHearts();
        currentScore = 0;
        powerups = new List<PowerUp>();
        currentScoreText.text = "Score : " + currentScore.ToString(); 
        currentScoreText.fontSize = Screen.width / 15f;
        currentScoreText.transform.position = new Vector3(Screen.width * 0.75f, Screen.height * 0.95f, 0f);
        mainMenu.SetupMenu();
    }
    public static void IncreaseScoreMultiplier()
    {
        currentScoreMultiplier += 1f;
        multiplierCounter.ResetCounter();
        scoreMultiplierHasChanged = true;
    }
    public static void DecreaseScoreMultiplier()
    {
        currentScoreMultiplier -= 1f;
        multiplierCounter.ResetCounter();
        scoreMultiplierHasChanged = true;
    }
    public static void ResetScoreMultiplier()
    {
        currentScoreMultiplier = 1f;
        multiplierCounter.ResetCounter();
        scoreMultiplierHasChanged = true;
    }
    public static void AddToScore(int amt){currentScore += amt * (int)currentScoreMultiplier; }
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
    public static Transform CreateBigExplosion(Vector3 point)
    {
        point.z = -3f;
        GameObject g = (GameObject)Resources.Load("BigExplosion");
        g.transform.localScale = Pooter.basicScale * 2f;
        LimitedTimeVisualEffect l = Instantiate(g, point, Quaternion.identity).GetComponent<LimitedTimeVisualEffect>();
        MainScript.explosions.Add(l);
        l.rotates = true;
        l.rotateSpeed = 35f;
        l.Setup(1f);
        return (l.transform);
    }
    public static PowerUp CreatePowerup(Vector3 point)
    {
        point.z = 1f;
        GameObject g = (GameObject)Resources.Load("powerup");
        g.transform.localScale = Pooter.basicScale * 1.25f;
        PowerUp p = Instantiate(g, point, Quaternion.identity).GetComponent<PowerUp>();
        if(Random.value > 0.5f){p.SetupPowerup(PowerupType.invulnerabl);}else { p.SetupPowerup(PowerupType.speed); }
        return p;
    }
    public static void CreateGreenDebris(Vector3 point, Vector3 moveDirect)
    {
        point.z = 1f;
        GameObject g = (GameObject)Resources.Load("greenDebris");
        g.transform.localScale = Pooter.basicScale * 0.375f;
        LimitedTimeVisualEffect l = Instantiate(g, point, Quaternion.identity).GetComponent<LimitedTimeVisualEffect>();
        MainScript.explosions.Add(l);
        l.moves = true;
        l.moveDirect = moveDirect;
        //l.rotates = true;
        //l.rotateSpeed = 35f;
        l.Setup(1f);
    }
    public static void CreateRedDebris(Vector3 point, Vector3 moveDirect)
    {
        point.z = 1f;
        GameObject g = (GameObject)Resources.Load("greenDebris");
        g.transform.localScale = Pooter.basicScale * 0.185f;
        LimitedTimeVisualEffect l = Instantiate(g, point, Quaternion.identity).GetComponent<LimitedTimeVisualEffect>();
        l.GetComponent<SpriteRenderer>().color = Color.red;
        MainScript.explosions.Add(l);
        l.moves = true;
        l.moveDirect = moveDirect;
        //l.rotates = true;
        //l.rotateSpeed = 35f;
        l.Setup(0.35f);
    }
    public static void CreateBlip(Vector3 point,Vector3 moveDirect)
    {
        point.z = 0f;
        GameObject g = (GameObject)Resources.Load("blipSprinkle");
        g.transform.localScale = Pooter.basicScale * 1f;
        LimitedTimeVisualEffect l = Instantiate(g, point, Quaternion.identity).GetComponent<LimitedTimeVisualEffect>();
        MainScript.explosions.Add(l);
        l.moves = true;
        l.moveDirect = moveDirect;
        l.rotates = true;
        l.rotateSpeed = 35f;
        l.Setup(1f);
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
        if (sparkleCounter.hasfinished)
        {
            sparkleCounter.ResetCounter();
            point.z = -3f;
            GameObject g = (GameObject)Resources.Load("BoomSplosion");
            g.transform.localScale = Pooter.basicScale * 2.5f;
            LimitedTimeVisualEffect l = Instantiate(g, point, Quaternion.identity).GetComponent<LimitedTimeVisualEffect>();
            MainScript.explosions.Add(l);
            l.Setup(1f);
        }
        
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
                    //scrollingScreen.CreateEnemy((GameObject)Resources.Load("Prefabs/UFOBadGuyPrefab"), 2f);
                    badguys.Add(scrollingScreen.CreateEnemy((GameObject)Resources.Load("Prefabs/BadGuyPrefab"), 1.5f));
                    break;
                case 1:
                    badguys.Add(scrollingScreen.CreateEnemy((GameObject)Resources.Load("Prefabs/homingMissile"), 0.85f));
                    break;
                case 2:
                    badguys.Add(scrollingScreen.CreateEnemy((GameObject)Resources.Load("Prefabs/CircleBadGuy"), 2f));
                    break;
                case 3:
                    badguys.Add(scrollingScreen.CreateEnemy((GameObject)Resources.Load("Prefabs/UFOBadGuyPrefab"), 2f));
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
            float distBetween = (totalWidth / (numberOfWalls)) * 1f;
            Debug.DrawLine(Vector3.zero, Vector3.left * totalWidth * 0.5f);
            float randomXOffsetMax = (totalWidth * 0.9f) / numberOfWalls;
            //float xOffset = Random.Range(0f, randomXOffsetMax);
            bool hasAddedPowerup = false;
            for (int i = 0; i < numberOfWalls; i++)
            {
                Vector3 pos = origin + (Vector3.right * i * distBetween) + (Vector3.right * Random.value * distBetween * 0.9f);
                bool buildWall = true; ;
                if(Random.value > 0.9f && !hasAddedPowerup) { buildWall = false; }
                if(buildWall)
                {
                    walls.Add(scrollingScreen.CreateWall(2f, 2f, 0f, pos));
                }
                else 
                {
                    hasAddedPowerup = true;
                    powerups.Add(CreatePowerup(pos));
                }
            }
        }
    }
    public void ClearEverything()
    {
        while(badguys.Count > 0) { BadGuy b = badguys[0];b.DestroyBadGuy(); badguys.RemoveAt(0); }
        while(walls.Count > 0) { Wall w = walls[0]; Destroy(w.gameObject);walls.RemoveAt(0); }
        while(explosions.Count > 0) { LimitedTimeVisualEffect e = explosions[0]; Destroy(e.gameObject); explosions.RemoveAt(0); }
        while(bullets.Count > 0) { Bullet b = bullets[0];b.DestroyBullet();bullets.RemoveAt(0); }
        while(powerups.Count > 0) { PowerUp p = powerups[0];powerups.Remove(p);Destroy(p.gameObject); }
        Pooter.currentHealth = 3;SetHealthHeartsToPooter();
    }
    public void StartGame()
    {
        hasShownAd = false;
        pooter.inGame = true;
        gameStarted = true;
        gameEnded = false;
        Vector3 pooterPos = Camera.main.transform.position; pooterPos.z = pooter.transform.position.z;
        pooter.transform.position = pooterPos;
        pooter.Revive();
        currentDifficulty = 1f;
        ResetScoreMultiplier();
        currentScore = 0;
        Vector3 gameOverPos = gameOverScreen.position; gameOverPos.z = 5.5f; gameOverPos += Vector3.up * Screen.height * 0.01f; gameOverScreen.localPosition = gameOverPos;
        gameOverScreenCounter.ResetCounter();
    }
    void LoadAdvertisement()
    {
        adLoader.LoadAd();
        adLoader.ShowAd();
    }
    public void PauseGame()
    {
        gamePaused = true;
        //pooter.inGame = false;
        //gameStarted = false;
    }
    void UpdateGame(float timePassed)
    {
        //mainMenu.UpdateMenu(timePassed);
        scrollingScreen.UpdateScrollingScreen(timePassed);

        if (gameStarted && !gamePaused)
        {
            multiplierCounter.UpdateCounter(timePassed); if (multiplierCounter.hasfinished && currentScoreMultiplier > 1f) { DecreaseScoreMultiplier();  }
            if (scoreMultiplierHasChanged) { scoreMultiplierHasChanged = false; currentScoreMultiplierText.text = "X " + currentScoreMultiplier.ToString(); }
            sparkleCounter.UpdateCounter(timePassed);
            pooter.UpdatePooter(timePassed);
            if (currentScore != weThinkScoreIs) { weThinkScoreIs = currentScore; currentScoreText.text = "Score : " + currentScore.ToString(); }
            currentDifficulty += timePassed * 0.025f;
            for(int i = 0; i < walls.Count; i++) { Wall w = walls[i]; if (w.CleanupThisWall()) { walls.Remove(w);i--;Destroy(w.gameObject); } }
            for (int i = 0; i < badguys.Count; i++) { BadGuy b = badguys[i]; b.UpdateBadGuy(timePassed); if (b.GetReadyToDie()) { badguys.Remove(b); b.DestroyBadGuy(); } }
            for (int i = 0; i < bullets.Count; i++) { Bullet b = bullets[i]; b.UpdateBullet(timePassed); if (b.GetReadyToDie()) { bullets.Remove(b); b.DestroyBullet(); i--; } }
            for (int i = 0; i < explosions.Count; i++) 
            { 
                LimitedTimeVisualEffect e = explosions[i]; 
                e.endCounter.UpdateCounter(timePassed); 
                if (e.rotates) { e.transform.Rotate(Vector3.forward * e.rotateSpeed * timePassed); }
                if (e.moves) { e.transform.Translate(e.moveDirect); }
                if (e.endCounter.hasfinished) { explosions.Remove(e); i--; e.Destroy(); } }
            for(int i = 0; i < powerups.Count; i++) { PowerUp p = powerups[i]; if (p.GetReadyToDie()) { powerups.Remove(p);i--;Destroy(p.gameObject); } }
            if (Pooter.currentHealth != healthHeartDisplay) { SetHealthHeartsToPooter(); }
            if (gameEnded)
            {
                if (gameOverScreenCounter.hasfinished)
                {
                    Vector2 objectivePos = mainMenu.transform.localPosition;
                    float distMoved = Screen.height * timePassed * 0.0125f;
                    Vector2 directTowards = objectivePos - (Vector2)gameOverScreen.localPosition;
                    if(directTowards.magnitude < distMoved) 
                    {
                        Vector3 finalPos = mainMenu.transform.localPosition;finalPos.z = gameOverScreen.localPosition.z;
                        gameOverScreen.localPosition = finalPos;
                    } else 
                    { gameOverScreen.transform.Translate(Vector3.down * distMoved); }
                }
                else { gameOverScreenCounter.UpdateCounter(timePassed); }
            }
        }
        
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
        UpdateGame(Time.fixedDeltaTime * 1f);
    }
    // Update is called once per frame
    void Update()
    {

        bool backButton = Input.GetKeyDown(KeyCode.Escape);
        if (backButton)
        {
            if (!mainMenu.menuEnabled) 
            { 
                gamePaused = true; 
                mainMenu.OpenMenu(false); 
            } else
            {
                Application.Quit();
            }
        }
        if ( !gamePaused) 
        {
            //Debug.Log("in here");
            if (pooter.alive)
            {
                
            }
            else
            {
                if (pooter.hasInputTouch && gameOverScreenCounter.hasfinished && mainMenu.menuButtonCounter.hasfinished)
                {
                    if (!hasShownAd && pooter.hasInputTouch)
                    {
                        hasShownAd = true; LoadAdvertisement(); 
                    }
                    if (!mainMenu.menuEnabled){ gamePaused = true; mainMenu.OpenMenu(false); }
                }
                //if (Input.GetKeyDown(KeyCode.Escape)) { if (mainMenu.enabled) { Application.Quit(); } }
            }
            
            
        }
    }
}
