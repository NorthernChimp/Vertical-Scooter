using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Advertisements;

public class MainScript : MonoBehaviour
{
    LevelType currentLevelType = LevelType.obstacleCourse;//indicates which form of level we are currently generating assets for. automatically set when game is started
    LevelType nextNonBasicLevelType = LevelType.tunnel;

    int basicLevel = 0;
    int tunnelLevel = 0;
    int obstacleLevel = 0;

    float startOfLevel;
    float distPerTrigger;
    float highestYPoint = 0f;
    float distanceMovedUpward = 0f;
    float distanceToNextLevelType;
    float lengthIntoCurrentLevel = 0f;
    float lengthOfCurrentLevel = 0f;
    float lastBrickYpos = 0f;
    int bricksBuilt = 0;
    int instancesBuilt = 0;
    float swingDist = 0f;
    float swingMultiplier = 1f;
    float laneWidth = 0f;
    bool builtEnemyLast = false;
    bool builtobstacleLast = false;
    bool builtWallLast = false;

    public static bool showingAd = false;

    public InterstitialAdLoader adLoader;
    static float currentScoreMultiplier = 1f;
    static Counter multiplierCounter;
    Counter slowMotionCounter;
    static Counter sparkleCounter;
    Counter topDebrisCounter;
    public List<HealthHeart> healthHearts;
    public Pooter pooter;
    public static bool trainingMode = false;
    float trainingTimer = 0f;
    public Transform trainingBlock;
    SpriteRenderer trainingBlockRender;
    int trainingPhase = 0;
    public ScrollingScreen scrollingScreen;
    public MainMenuScript mainMenu;

    List<RoundWall> roundWalls;
    List<Wall> walls;
    List<BadGuy> badguys;
    List<PowerUp> powerups;
    List<FlameWrangler> wranglers;
    public static List<Bullet> bullets;
    public static List<BackgroundVisualEffect> backgroundEffects;
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
    TutorialScreen tutorialScreen;
    Counter gameOverScreenCounter;
    float nextInstantiationPoint;
    int healthHeartDisplay = 3;//the health heart display. its the number of health hearts that are full or empty, it reacts to the static int from the pooterscript and updates when the game does;
    // Start is called before the first frame update
    void Start()
    {
        Screen.orientation = ScreenOrientation.Portrait;
        wranglers = new List<FlameWrangler>();
        roundWalls = new List<RoundWall>();
           slowMotionCounter = new Counter(8f); slowMotionCounter.UpdateCounter(8f);
        backgroundEffects = new List<BackgroundVisualEffect>();
        topDebrisCounter = new Counter(0.15f);   
        tutorialScreen = GetComponentInChildren<TutorialScreen>(); 
        float scaleWidth = (Screen.width * 0.005f)/0.32f;
        float scaleHeight = (Screen.height * 0.005f)/ 0.32f;
        Vector3 trainingScale = new Vector3(scaleWidth, scaleHeight, 1f);
        trainingBlock.localScale = trainingScale;
        trainingBlockRender = trainingBlock.GetComponent<SpriteRenderer>();
        trainingBlockRender.enabled = false;
        gameOverScreenCounter = new Counter(3.5f);
        Vector3 gameOverPos = gameOverScreen.position;gameOverPos.z = 5f;gameOverPos += Vector3.up * Screen.height * 0.01f; gameOverScreen.localPosition = gameOverPos;
        currentScoreMultiplierText.text = "X " + currentScoreMultiplier.ToString();
        currentScoreMultiplierText.fontSize = Screen.width / 22f;
        currentScoreMultiplierText.transform.position = new Vector3(Screen.width * 0.75f, Screen.height * 0.875f, 0f);
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
        currentScoreText.fontSize = Screen.width / 17.5f;
        currentScoreText.transform.position = new Vector3(Screen.width * 0.775f, Screen.height * 0.915f, 0f);
        mainMenu.SetupMenu();
        tutorialScreen.SetupTutorialScreen();
        float yPos = Screen.height * 0.005f;
        float firstBatch = 3f;
        float secondBatch = 5f;
        float thirdBatch = 7f;
        float fourthBatch = 11f;
        CreateBackgroundEffect("MildExplosion", new Vector3(GetRandomXPosOnScreen(),yPos,0f), 2f,firstBatch,Screen.height * 0.0045f,Color.white);
        yPos += Screen.height * 0.0015f;
        CreateBackgroundEffect("MildExplosion", new Vector3(GetRandomXPosOnScreen(), yPos, 0f), 2f,firstBatch,Screen.height * 0.0045f,Color.white);
        yPos += Screen.height * 0.0015f;
        CreateBackgroundEffect("MildExplosion", new Vector3(GetRandomXPosOnScreen(), yPos, 0f), 2f,secondBatch,Screen.height * 0.0085f,Color.gray);
        yPos += Screen.height * 0.0015f;
        CreateBackgroundEffect("MildExplosion", new Vector3(GetRandomXPosOnScreen(), yPos, 0f), 2f,secondBatch,Screen.height * 0.0085f,Color.gray);
        yPos += Screen.height * 0.0015f;
        CreateBackgroundEffect("SparkleSplosion", new Vector3(GetRandomXPosOnScreen(), yPos, 0f), 1.2f,thirdBatch,Screen.height * 0.0065f,Color.white);
        yPos += Screen.height * 0.0015f;
        CreateBackgroundEffect("SparkleSplosion", new Vector3(GetRandomXPosOnScreen(), yPos, 0f), 1.2f,thirdBatch,Screen.height * 0.0065f,Color.white);
        yPos += Screen.height * 0.0015f;
        CreateBackgroundEffect("SparkleSpin", new Vector3(GetRandomXPosOnScreen(), yPos, 0f), 1.25f, fourthBatch, Screen.height * 0.0065f, Color.white);
        yPos += Screen.height * 0.0015f;
        CreateBackgroundEffect("SparkleSpin", new Vector3(GetRandomXPosOnScreen(), yPos, 0f), 1.25f, fourthBatch, Screen.height * 0.0065f, Color.white);
        EnableHudElements(false);
        
    }
    void CreateWrangler(Vector3 pos,float height)
    {
        FlameWrangler w = Instantiate((GameObject)Resources.Load("prefabs/FlameWrangler"), pos, Quaternion.identity).GetComponent<FlameWrangler>();
        w.SetupWrangler(1f, height,obstacleLevel);
        wranglers.Add(w);
    }
    public void EnableHudElements(bool b)
    {
        foreach (HealthHeart h in healthHearts)
        {
            //h.enabled = b;
            h.SetVisible(b);
        }
        
        currentScoreMultiplierText.enabled = b;
        currentScoreText.enabled = b;
    }
    public List<BadGuy> GetBadGuysNearPointWithinDistance(Vector3 point,float withinDistance)
    {
        List<BadGuy> list = new List<BadGuy>();
        foreach(BadGuy b in badguys)
        {
            Vector3 pos = b.GetPosition();
            Vector3 diff = pos - point;
            diff.z = 0f;
            float dist = diff.magnitude;
            if(dist <= withinDistance)
            {
                list.Add(b);
            }
        }
        return list;
    }
    public void ActivateSlow()
    {
        slowMotionCounter.ResetCounter();
    }
    float GetRandomXPosOnScreen() { float randomXpos = Random.value * Screen.width * 0.01f; randomXpos -= Screen.width * 0.005f; return randomXpos; }
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
        Vector3 origin = Camera.main.transform.position + (Vector3.up * Screen.height * 0.0045f) + (Vector3.down * Pooter.brickLength * 3f) + (Vector3.left * Screen.width * 0.005f) + (Vector3.right * Pooter.brickLength * 3f);
        origin.z = -5f;
        for (int i = 0; i < healthHearts.Count; i++)
        {
            HealthHeart h = healthHearts[i];
            h.Setup();
            h.transform.position = origin + (Vector3.right * i * Pooter.brickLength * 3.15f);
            h.transform.localScale = Pooter.basicScale * 1.5f;
            h.SetOne();
        }
    }
    void CreateBackgroundEffect(string nameOfEffect,Vector3 point, float scaleMultiplier,float timeBetweenPlays,float baseSpeed,Color c)
    {
        point.z = -4f;
        GameObject g = (GameObject)Resources.Load(nameOfEffect);
        
        BackgroundVisualEffect b = Instantiate(g, point, Quaternion.identity).GetComponent<BackgroundVisualEffect>();
        b.SetupBackgroundVisualEffect(scaleMultiplier,  c,timeBetweenPlays,baseSpeed);
        backgroundEffects.Add(b);
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
        l.Setup(1f,false,Color.white);
        return (l.transform);
    }
    public static Transform CreateBigRedExplosion(Vector3 point)
    {
        point.z = -3f;
        GameObject g = (GameObject)Resources.Load("BigExplosion");
        g.transform.localScale = Pooter.basicScale * 2f;
        LimitedTimeVisualEffect l = Instantiate(g, point, Quaternion.identity).GetComponent<LimitedTimeVisualEffect>();
        MainScript.explosions.Add(l);
        l.rotates = true;
        l.rotateSpeed = 35f;
        l.Setup(1f, false, Color.red);
        return (l.transform);
    }
    public static PowerUp CreatePowerup(Vector3 point)
    {
        point.z = 1f;
        GameObject g = (GameObject)Resources.Load("powerup");
        g.transform.localScale = Pooter.basicScale * 1.25f;
        PowerUp p = Instantiate(g, point, Quaternion.identity).GetComponent<PowerUp>();
        float rando = Random.value;
        if (rando < 0.25f) { p.SetupPowerup(PowerupType.invulnerabl); } else if (rando < 0.5f) { p.SetupPowerup(PowerupType.speed); } else if (rando < 0.75f) { p.SetupPowerup(PowerupType.health); } else { p.SetupPowerup(PowerupType.timeSlow); }
        return p;
    }
    public static void CreateGreenDebris(Vector3 point, Vector3 moveDirect)
    {
        point.z = 1f;
        GameObject g = (GameObject)Resources.Load("greenDebris");
        g.transform.localScale = Pooter.basicScale * 0.525f;
        LimitedTimeVisualEffect l = Instantiate(g, point, Quaternion.identity).GetComponent<LimitedTimeVisualEffect>();
        MainScript.explosions.Add(l);
        l.moves = true;
        l.moveDirect = moveDirect;
        //l.rotates = true;
        //l.rotateSpeed = 35f;
        l.Setup(1f,false,Color.green);
    }
    public static void CreateColoredDebris(Vector3 point, Vector3 moveDirect,Color c, float scaleMultiplier)
    {
        point.z = 1f;
        GameObject g = (GameObject)Resources.Load("greenDebris");
        g.transform.localScale = Pooter.basicScale * scaleMultiplier;
        LimitedTimeVisualEffect l = Instantiate(g, point, Quaternion.identity).GetComponent<LimitedTimeVisualEffect>();
        MainScript.explosions.Add(l);
        l.moves = true;
        l.moveDirect = moveDirect;
        //l.rotates = true;
        //l.rotateSpeed = 35f;
        l.Setup(1f, false, c);
    }
    public static void CreateRedDebris(Vector3 point, Vector3 moveDirect)
    {
        point.z = 1f;
        GameObject g = (GameObject)Resources.Load("greenDebris");
        g.transform.localScale = Pooter.basicScale * 0.485f;
        LimitedTimeVisualEffect l = Instantiate(g, point, Quaternion.identity).GetComponent<LimitedTimeVisualEffect>();
        //l.GetComponent<SpriteRenderer>().color = Color.red;
        MainScript.explosions.Add(l);
        l.moves = true;
        l.moveDirect = moveDirect;
        //l.rotates = true;
        //l.rotateSpeed = 35f;
        l.Setup(0.35f, false, Color.red) ;
    }
    public static void CreateBlueDebris(Vector3 point, Vector3 moveDirect)
    {
        point.z = 1f;
        GameObject g = (GameObject)Resources.Load("greenDebris");
        g.transform.localScale = Pooter.basicScale * 0.8525f;
        LimitedTimeVisualEffect l = Instantiate(g, point, Quaternion.identity).GetComponent<LimitedTimeVisualEffect>();
        //l.GetComponent<SpriteRenderer>().color = Color.magenta;
        MainScript.explosions.Add(l);
        l.moves = true;
        l.moveDirect = moveDirect;
        //l.rotates = true;
        //l.rotateSpeed = 35f;
        l.Setup(0.35f,false,Color.magenta);
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
        l.Setup(1f,false,Color.white);
    }
    public static void CreateExplosionAtScale(Vector3 point, float scale)
    {
        point.z = -3f;
        GameObject g = (GameObject)Resources.Load("Explosion");
        g.transform.localScale = Pooter.basicScale * 2f * scale;
        LimitedTimeVisualEffect l = Instantiate(g, point, Quaternion.identity).GetComponent<LimitedTimeVisualEffect>();
        MainScript.explosions.Add(l);
        l.Setup(1f, false, Color.white);
    }
    public static void CreateExplosion(Vector3 point)
    {
        point.z = -3f;
        GameObject g = (GameObject)Resources.Load("Explosion");
        g.transform.localScale = Pooter.basicScale * 2f;
        LimitedTimeVisualEffect l = Instantiate(g, point, Quaternion.identity).GetComponent<LimitedTimeVisualEffect>();
        MainScript.explosions.Add(l);
        l.Setup(1f,false,Color.white);
    }
    public static void CreateTimesTwo(Vector3 point)
    {
        point.z = -3f;
        GameObject g = (GameObject)Resources.Load("TimesTwoSymbol");
        g.transform.localScale = Pooter.basicScale * 1f;
        LimitedTimeVisualEffect l = Instantiate(g, point, Quaternion.identity).GetComponent<LimitedTimeVisualEffect>();
        MainScript.explosions.Add(l);
        l.Setup(1f, false, Color.white);
    }
    public static void CreateWhiteCircle(Color c,Vector3 point,bool fades,float scaleMultiplier,float timeAlive)
    {
        point.z = 3f;
        GameObject g = (GameObject)Resources.Load("WhiteCirclePrefab");
        g.transform.localScale = Pooter.basicScale * scaleMultiplier;
        LimitedTimeVisualEffect l = Instantiate(g, point, Quaternion.identity).GetComponent<LimitedTimeVisualEffect>();
        MainScript.explosions.Add(l);
        l.Setup(timeAlive, fades, c);
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
            l.Setup(1f,false,Color.white);
        }
        
    }
    public static void CreateSparkleExplosion(Vector3 point)
    {
        
        point.z = -3f;
        GameObject g = (GameObject)Resources.Load("SparkleExplosion");
        g.transform.localScale = Pooter.basicScale * 2.5f;
        LimitedTimeVisualEffect l = Instantiate(g, point, Quaternion.identity).GetComponent<LimitedTimeVisualEffect>();
        MainScript.explosions.Add(l);
        l.Setup(1f,false,Color.white);
    }
    public void TriggerScrollScreen()//when the pooter has reached a certain height he triggers the scrollscreen to instantiate something above him
    {
        //Debug.Log("we have reached trigger at " + Time.time);
        float minAmt = 0.5f;
        if (playedEnemyLast) { minAmt += 0.25f; } else { minAmt -= 0.25f; }
        if (Random.value > minAmt)
        {
            playedEnemyLast = true;
            
            float difficultyPoint = (float)basicLevel;if (difficultyPoint > 4f) { difficultyPoint = 4f; }
            //difficultyPoint = 4F;
            
            int randomInt = (int)Random.Range(0f, difficultyPoint);
            //Debug.Log("random int is " + randomInt);
            if (MainScript.trainingMode) { randomInt = 0; }
            switch (randomInt)
            {
                case 0:
                    //badguys.Add(scrollingScreen.CreateEnemy((GameObject)Resources.Load("Prefabs/UFOBadGuyPrefab"), 2f));
                    badguys.Add(scrollingScreen.CreateEnemy((GameObject)Resources.Load("Prefabs/BadGuyPrefab"), 1.75f));
                    //badguys.Add(scrollingScreen.CreateEnemy((GameObject)Resources.Load("Prefabs/ProximityMine"), 0.5f));
                    break;
                case 1:
                    badguys.Add(scrollingScreen.CreateEnemy((GameObject)Resources.Load("Prefabs/homingMissile"), 1f));
                    break;
                case 2:
                    badguys.Add(scrollingScreen.CreateEnemy((GameObject)Resources.Load("Prefabs/UFOBadGuyPrefab"), 1f));
                    
                    break;
                case 3:
                    badguys.Add(scrollingScreen.CreateEnemy((GameObject)Resources.Load("Prefabs/CircleBadGuy"), 1f));
                    //scrollingScreen.CreateEnemy((GameObject)Resources.Load("Prefabs/BadGuy"), 2f);
                    break;
            }

        } else
        {
            playedEnemyLast = false;
            float difficultyPoint = currentDifficulty + 1f; if (difficultyPoint > 4f) { difficultyPoint = 4f; }
            if (trainingMode) { difficultyPoint = 1f; }
            float rando = Random.value;
            if (rando > 0.95f)
            {
                
                bool sameHeight = Random.value > 0.5f;
                float distApart = Random.Range(Screen.width * 0.002f, Screen.width * 0.0035f);
                float yMod = 0f; if (!sameHeight) { yMod = Random.value * Pooter.brickLength * 3f; }
                Vector3 pos = new Vector3(distApart * -1f, ScrollingScreen.GetYPosAboveScreen(), 0f); pos.y += yMod;
                float modifier = Mathf.Sign(Random.value - 0.5f);
                powerups.Add(CreatePowerup(pos + (Vector3.up * yMod * modifier)));
                powerups.Add(CreatePowerup(pos + (Vector3.right * distApart * 2f) + (Vector3.up * yMod* modifier * -1f)));
            }else if(rando > 0.85f && basicLevel > 2)
            {
                bool sameHeight = Random.value > 0.5f;
                float distApart = Random.Range(Screen.width * 0.002f, Screen.width * 0.0035f);
                float yMod = 0f;if (!sameHeight) {  yMod = Pooter.brickLength + (Random.value * Pooter.brickLength * 1f); }
                float modifier = Mathf.Sign(Random.value - 0.5f);
                badguys.Add(scrollingScreen.CreateEnemyAt((GameObject)Resources.Load("Prefabs/BarrelPrefab"), 1f,distApart,yMod * modifier));
                badguys.Add(scrollingScreen.CreateEnemyAt((GameObject)Resources.Load("Prefabs/BarrelPrefab"), 1f, distApart * -1f,yMod * -1f *modifier));
            }else if(rando > 0.65f && basicLevel > 5)
            {
                bool sameHeight = Random.value > 0.5f;
                float distApart = Random.Range(Screen.width * 0.002f, Screen.width * 0.0035f);
                float yMod = 0f; if (!sameHeight) { yMod = Pooter.brickLength + (Random.value * Pooter.brickLength * 1f); }
                float modifier = Mathf.Sign(Random.value - 0.5f);
                badguys.Add(scrollingScreen.CreateEnemyAt((GameObject)Resources.Load("Prefabs/ProximityMine"), 0.375f, distApart, yMod * modifier));
                badguys.Add(scrollingScreen.CreateEnemyAt((GameObject)Resources.Load("Prefabs/ProximityMine"), 0.375f, distApart * -1f, yMod * -1f * modifier));
                /*int numberOfMines = (int)Random.Range(2f, 4f);
                float distApart = (0.8f + (Random.value * 0.4f)) * Pooter.brickLength * 8f;
                float halfDist = (float)(numberOfMines - 1) * 0.5f * distApart;
                float originX = halfDist * -1f;
                for(int i = 0; i < numberOfMines; i++)
                {
                    badguys.Add(scrollingScreen.CreateEnemyAt((GameObject)Resources.Load("Prefabs/ProximityMine"),0.5f, originX + (i * distApart)));
                }*/
            }
            else
            {
                int numberOfWalls = (int)(Random.Range(1f, 1f + (basicLevel * 0.5f) ));
                if(numberOfWalls > 3) { numberOfWalls = 3; }
                //int numberOfWalls = (int)Random.Range(1f, difficultyPoint);
                float totalWidth = ((Screen.width * 0.0045f) - Pooter.brickLength) * 2f;
                Vector3 origin = Vector3.left * (totalWidth * 0.5f);
                origin.y = ScrollingScreen.GetYPosAboveScreen();
                float distBetween = (totalWidth / (numberOfWalls)) * 1f;
                //float xOffset = Random.Range(0f, randomXOffsetMax);
                bool hasAddedPowerup = false;
                bool hasAddedBarrel = false;
                bool hasAddedMine = false;
                for (int i = 0; i < numberOfWalls; i++)
                {
                    Vector3 randomY = new Vector3(0f, Random.value * Screen.width * 0.00025f, 0f);
                    Vector3 pos = origin + (Vector3.right * i * distBetween) + (Vector3.right * Random.value * distBetween * 0.95f) + randomY;
                    bool buildWall = true; ;
                    bool buildBarrel = false;
                    bool buildMine = false;
                    float randoValue = Random.value;
                    if (randoValue > 0.9f && !hasAddedPowerup) { buildWall = false; }else if(randoValue > 0.7f && !hasAddedBarrel && basicLevel > 2) { buildBarrel = true; }else if(randoValue > 0.55f && !hasAddedMine && basicLevel > 3) { buildMine = true; }
                    if (trainingMode) { buildWall = (Random.value > 0.25f); }
                    if (buildWall && !buildBarrel)
                    {
                        if (Random.value > 0.5f) { walls.Add(scrollingScreen.CreateWall(2f, 2f, 0f, pos)); } else { roundWalls.Add(scrollingScreen.CreateRoundWall(2f, 2f, 0f, pos)); }
                    }
                    else if (buildBarrel)
                    {
                        hasAddedBarrel = true;
                        badguys.Add(scrollingScreen.CreateEnemyAt((GameObject)Resources.Load("Prefabs/BarrelPrefab"), 1f,pos.x));
                    }else if (buildMine)
                    {
                        hasAddedMine = true;
                        badguys.Add(scrollingScreen.CreateEnemyAt((GameObject)Resources.Load("Prefabs/ProximityMine"), 0.25f,pos.x));
                    }
                    else
                    {
                        hasAddedPowerup = true;
                        powerups.Add(CreatePowerup(pos));
                    }
                }
            }
        }
    }
    public void TriggerScrollScreenObstacleCourse()//when the pooter has reached a certain height he triggers the scrollscreen to instantiate something above him
    {
        //Debug.Log("we have reached trigger at " + Time.time);
        float yPos = nextInstantiationPoint + (Screen.height * 0.0055f);
        if (!builtWallLast)
        {
            builtWallLast = true;
            //build walls
            float wallsHeight = Pooter.brickLength * (6f); ;
            nextInstantiationPoint += wallsHeight;

            float difficultyPoint = currentDifficulty + 1f;
            if (difficultyPoint > 4f) { difficultyPoint = 4f; }
            if (trainingMode) { difficultyPoint = 1f; }
            int numberOfWalls = (int)Random.Range(2f, difficultyPoint);
            float totalWidth = ((Screen.width * 0.00425f) - Pooter.brickLength) * 2f;
            float distBetweenEachWall = totalWidth / ((float)(numberOfWalls - 1));
            float distLeft = distBetweenEachWall * ((float)(numberOfWalls - 1)) * 0.5f;
            Vector3 origin = Vector3.left * distLeft;
            //origin.y = ScrollingScreen.GetYPosAboveScreen() + (wallsHeight * 0.5f);
            origin.y = yPos + (wallsHeight * 0.5f);
            //float distBetween = (totalWidth / (numberOfWalls)) * 1f;
            //Debug.DrawLine(Vector3.zero, Vector3.left * totalWidth * 0.5f);
            //float randomXOffsetMax = (totalWidth * 0.9f) / numberOfWalls;
            //float xOffset = Random.Range(0f, randomXOffsetMax);
            bool hasAddedPowerup = false;
            bool hasAddedBarrel = false;
            bool hasAddedMine = false;
            for (int i = 0; i < numberOfWalls; i++)
            {
                float amtRandom = Random.Range(-2f, 2f) * Pooter.brickLength;
                //Vector3 randomY = new Vector3(0f, amtRandom, 0f);
                Vector3 randomY = Vector3.zero ;
                Vector3 pos = origin + (Vector3.right * i * distBetweenEachWall) +  randomY;
                bool buildWall = true; ;
                bool buildBarrel = false;
                bool buildMine = false;
                float randoValue = Random.value;
                if (randoValue > 0.9f && !hasAddedPowerup) { buildWall = false; } else if (randoValue > 0.7f && !hasAddedBarrel) { buildBarrel = true; } else if (randoValue > 0.55f && !hasAddedMine) { buildMine = true; }
                if (trainingMode) { buildWall = (Random.value > 0.25f); }
                if (Random.value > 0.5f) { walls.Add(scrollingScreen.CreateWall(2f, 2f, 0f, pos)); } else { roundWalls.Add(scrollingScreen.CreateRoundWall(2f, 2f, 0f, pos)); }
                /*if (buildWall && !buildBarrel)
                {
                    
                }
                else if (buildBarrel)
                {
                    hasAddedBarrel = true;
                    badguys.Add(scrollingScreen.CreateEnemyAt((GameObject)Resources.Load("Prefabs/BarrelPrefab"), 1f, pos.x));
                }
                else if (buildMine)
                {
                    hasAddedMine = true;
                    badguys.Add(scrollingScreen.CreateEnemyAt((GameObject)Resources.Load("Prefabs/ProximityMine"), 0.25f, pos.x));
                }
                else
                {
                    hasAddedPowerup = true;
                    powerups.Add(CreatePowerup(pos));
                }*/
            }

        }
        else if (builtWallLast)
        {
            builtWallLast = false;
            if (builtEnemyLast)
            {
                builtobstacleLast = true;
                builtEnemyLast = false;
                float height = Random.Range(16f, 24f) * Pooter.brickLength;
                nextInstantiationPoint += height * 1f;
                Vector3 pos = new Vector3(0f, yPos + height * 0.5f, 0f);
                CreateWrangler(pos, height);
                //badguys.Add(scrollingScreen.CreateEnemyAt((GameObject)Resources.Load("Prefabs/BarrelPrefab"), pos, 1f));
                //badguys.Add(scrollingScreen.CreateEnemyAt((GameObject)Resources.Load("Prefabs/BarrelPrefab"), 1f, 0f,height * 0.5f));
                //build obstacle
            }
            else if (builtobstacleLast)
            {

                builtEnemyLast = true;
                builtobstacleLast = false;
                float height = Random.Range(4f, 6f) * Pooter.brickLength;
                nextInstantiationPoint += height;
                if (Random.value > 0.85f)
                {
                    Vector3 pos = new Vector3(0f, yPos + height * 0.5f, 0f);
                    powerups.Add(CreatePowerup(pos));
                }
                else
                {
                    Vector3 pos = new Vector3(0f, yPos + height * 0.5f, 0f);
                    //badguys.Add(scrollingScreen.CreateEnemyAt((GameObject)Resources.Load("Prefabs/BadGuyPrefab"), 1.5f, 0f, height * 0.5f));
                    badguys.Add(scrollingScreen.CreateEnemyAt((GameObject)Resources.Load("Prefabs/BadGuyPrefab"), pos, 1.5f));
                }
                //build enemy
            }
        }
        
        
        
        /*
        float minAmt = 0.5f;
        if (playedEnemyLast) { minAmt += 0.25f; } else { minAmt -= 0.25f; }
        if (Random.value > minAmt)
        {
            playedEnemyLast = true;
            float difficultyPoint = currentDifficulty; if (difficultyPoint > 4f) { difficultyPoint = 4f; }
            int randomInt = (int)Random.Range(0f, difficultyPoint);
            if (MainScript.trainingMode) { randomInt = 0; }
            switch (randomInt)
            {
                case 0:
                    //badguys.Add(scrollingScreen.CreateEnemy((GameObject)Resources.Load("Prefabs/UFOBadGuyPrefab"), 2f));
                    badguys.Add(scrollingScreen.CreateEnemy((GameObject)Resources.Load("Prefabs/BadGuyPrefab"), 1.75f));
                    //badguys.Add(scrollingScreen.CreateEnemy((GameObject)Resources.Load("Prefabs/ProximityMine"), 0.5f));
                    break;
                case 1:
                    badguys.Add(scrollingScreen.CreateEnemy((GameObject)Resources.Load("Prefabs/homingMissile"), 1f));
                    break;
                case 2:
                    badguys.Add(scrollingScreen.CreateEnemy((GameObject)Resources.Load("Prefabs/CircleBadGuy"), 1f));
                    break;
                case 3:
                    badguys.Add(scrollingScreen.CreateEnemy((GameObject)Resources.Load("Prefabs/UFOBadGuyPrefab"), 1f));
                    //scrollingScreen.CreateEnemy((GameObject)Resources.Load("Prefabs/BadGuy"), 2f);
                    break;
            }

        }
        else
        {
            playedEnemyLast = false;
            float difficultyPoint = currentDifficulty + 1f; if (difficultyPoint > 4f) { difficultyPoint = 4f; }
            if (trainingMode) { difficultyPoint = 1f; }
            float rando = Random.value;
            if (rando > 0.95f)
            {

                bool sameHeight = Random.value > 0.5f;
                float distApart = Random.Range(Screen.width * 0.002f, Screen.width * 0.0035f);
                float yMod = 0f; if (!sameHeight) { yMod = Random.value * Pooter.brickLength * 3f; }
                Vector3 pos = new Vector3(distApart * -1f, ScrollingScreen.GetYPosAboveScreen(), 0f); pos.y += yMod;
                float modifier = Mathf.Sign(Random.value - 0.5f);
                powerups.Add(CreatePowerup(pos + (Vector3.up * yMod * modifier)));
                powerups.Add(CreatePowerup(pos + (Vector3.right * distApart * 2f) + (Vector3.up * yMod * modifier * -1f)));
            }
            else if (rando > 0.85f)
            {
                bool sameHeight = Random.value > 0.5f;
                float distApart = Random.Range(Screen.width * 0.002f, Screen.width * 0.0035f);
                float yMod = 0f; if (!sameHeight) { yMod = Pooter.brickLength + (Random.value * Pooter.brickLength * 3f); }
                float modifier = Mathf.Sign(Random.value - 0.5f);
                badguys.Add(scrollingScreen.CreateEnemyAt((GameObject)Resources.Load("Prefabs/BarrelPrefab"), 1f, distApart, yMod * modifier));
                badguys.Add(scrollingScreen.CreateEnemyAt((GameObject)Resources.Load("Prefabs/BarrelPrefab"), 1f, distApart * -1f, yMod * -1f * modifier));
            }
            else if (rando > 0.65f)
            {
                bool sameHeight = Random.value > 0.5f;
                float distApart = Random.Range(Screen.width * 0.002f, Screen.width * 0.0035f);
                float yMod = 0f; if (!sameHeight) { yMod = Pooter.brickLength + (Random.value * Pooter.brickLength * 3f); }
                float modifier = Mathf.Sign(Random.value - 0.5f);
                badguys.Add(scrollingScreen.CreateEnemyAt((GameObject)Resources.Load("Prefabs/ProximityMine"), 0.375f, distApart, yMod * modifier));
                badguys.Add(scrollingScreen.CreateEnemyAt((GameObject)Resources.Load("Prefabs/ProximityMine"), 0.375f, distApart * -1f, yMod * -1f * modifier));
                /*int numberOfMines = (int)Random.Range(2f, 4f);
                float distApart = (0.8f + (Random.value * 0.4f)) * Pooter.brickLength * 8f;
                float halfDist = (float)(numberOfMines - 1) * 0.5f * distApart;
                float originX = halfDist * -1f;
                for(int i = 0; i < numberOfMines; i++)
                {
                    badguys.Add(scrollingScreen.CreateEnemyAt((GameObject)Resources.Load("Prefabs/ProximityMine"),0.5f, originX + (i * distApart)));
                }*/
                /*
            }
            else
            {
                int numberOfWalls = (int)Random.Range(1f, difficultyPoint);
                float totalWidth = ((Screen.width * 0.0045f) - Pooter.brickLength) * 2f;
                Vector3 origin = Vector3.left * (totalWidth * 0.5f);
                origin.y = ScrollingScreen.GetYPosAboveScreen();
                float distBetween = (totalWidth / (numberOfWalls)) * 1f;
                Debug.DrawLine(Vector3.zero, Vector3.left * totalWidth * 0.5f);
                float randomXOffsetMax = (totalWidth * 0.9f) / numberOfWalls;
                //float xOffset = Random.Range(0f, randomXOffsetMax);
                bool hasAddedPowerup = false;
                bool hasAddedBarrel = false;
                bool hasAddedMine = false;
                for (int i = 0; i < numberOfWalls; i++)
                {
                    Vector3 randomY = new Vector3(0f, Random.value * Screen.width * 0.0025f, 0f);
                    Vector3 pos = origin + (Vector3.right * i * distBetween) + (Vector3.right * Random.value * distBetween * 0.95f) + randomY;
                    bool buildWall = true; ;
                    bool buildBarrel = false;
                    bool buildMine = false;
                    float randoValue = Random.value;
                    if (randoValue > 0.9f && !hasAddedPowerup) { buildWall = false; } else if (randoValue > 0.7f && !hasAddedBarrel) { buildBarrel = true; } else if (randoValue > 0.55f && !hasAddedMine) { buildMine = true; }
                    if (trainingMode) { buildWall = (Random.value > 0.25f); }
                    if (buildWall && !buildBarrel)
                    {
                        if (Random.value > 0.5f) { walls.Add(scrollingScreen.CreateWall(2f, 2f, 0f, pos)); } else { roundWalls.Add(scrollingScreen.CreateRoundWall(2f, 2f, 0f, pos)); }
                    }
                    else if (buildBarrel)
                    {
                        hasAddedBarrel = true;
                        badguys.Add(scrollingScreen.CreateEnemyAt((GameObject)Resources.Load("Prefabs/BarrelPrefab"), 1f, pos.x));
                    }
                    else if (buildMine)
                    {
                        hasAddedMine = true;
                        badguys.Add(scrollingScreen.CreateEnemyAt((GameObject)Resources.Load("Prefabs/ProximityMine"), 0.25f, pos.x));
                    }
                    else
                    {
                        hasAddedPowerup = true;
                        powerups.Add(CreatePowerup(pos));
                    }
                }
            }
                
        }*/
    }
    public void ClearEverything()
    {
        while(badguys.Count > 0) { BadGuy b = badguys[0];b.DestroyBadGuy(); badguys.RemoveAt(0); }
        while(walls.Count > 0) { Wall w = walls[0]; Destroy(w.gameObject);walls.RemoveAt(0); }
        while (roundWalls.Count > 0) { RoundWall w = roundWalls[0]; Destroy(w.gameObject); roundWalls.RemoveAt(0); }
        while (explosions.Count > 0) { LimitedTimeVisualEffect e = explosions[0]; Destroy(e.gameObject); explosions.RemoveAt(0); }
        while(bullets.Count > 0) { Bullet b = bullets[0];b.DestroyBullet();bullets.RemoveAt(0); }
        while(powerups.Count > 0) { PowerUp p = powerups[0];powerups.Remove(p);Destroy(p.gameObject); }
        while(wranglers.Count > 0) { FlameWrangler f = wranglers[0];f.KillChildren(); wranglers.RemoveAt(0); Destroy(f.gameObject); }
        Pooter.currentHealth = 3;SetHealthHeartsToPooter();
    }
    public void StartGame(bool trainMode)
    {
        basicLevel = 1;
        tunnelLevel = 0;
        obstacleLevel = 0;
        mainMenu.CloseMenu();
        currentLevelType = LevelType.basic;
        nextNonBasicLevelType = LevelType.tunnel;
        trainingMode = trainMode;
        if (trainingMode) 
        {
            trainingPhase = 0;
            tutorialScreen.Activate(true);
            ChangeTrainingPhase();
        }
        else
        {
            EnableHudElements(true);
        }
        hasShownAd = false;
        pooter.inGame = true;
        gameStarted = true;
        gameEnded = false;
        transform.position = new Vector3(0f,0f,-10f);
        scrollingScreen.transform.position = new Vector3(0f, 0f, 4.99f);
        Vector3 pooterPos = Camera.main.transform.position; pooterPos.z = pooter.transform.position.z;
        pooter.transform.position = pooterPos;
        highestYPoint = pooter.transform.position.y;
        nextInstantiationPoint = highestYPoint + (distPerTrigger * Pooter.brickLength);
        distanceMovedUpward = 0f;
        lengthIntoCurrentLevel = 0f;
        //nextInstantiationPoint = lastBrickYpos += Pooter.brickLength * (distPerTrigger + (Random.value * Random.Range(-2f, 2f)));
        distPerTrigger = Screen.width * 0.0225f;
        distanceToNextLevelType = Screen.width * 0.075f;
        pooter.Revive();
        currentDifficulty = 1f;
        ResetScoreMultiplier();
        currentScore = 0;
        lengthOfCurrentLevel = Screen.width * (0.15f);
        bricksBuilt = 0;
        instancesBuilt = 0;
        Vector3 gameOverPos = gameOverScreen.localPosition; 
        gameOverPos.z = 5.5f; 
        gameOverPos += Vector3.up * Screen.height * 0.01f; 
        gameOverScreen.localPosition = Vector3.zero;
        gameOverScreen.localPosition += gameOverPos;
        gameOverScreen.localScale = Pooter.basicScale;
        gameOverScreenCounter.ResetCounter();
        Vector3 origin = new Vector3(Screen.width * -0.0025f, ScrollingScreen.GetYPosAboveScreen(), 0f);
        for(int i = 0; i < 3; i++)
        {
            //Vector3 pos = origin + (i * Vector3.right * Screen.width * 0.0025f);
           //badguys.Add(scrollingScreen.CreateEnemyAt(Resources.Load("prefabs/BarrelPrefab") as GameObject, 1f, pos.x));
        }
    }
    void LoadAdvertisement()
    {
        showingAd = true;
        adLoader.LoadAd();
        adLoader.ShowAd();
    }
    public void PauseGame()
    {
        gamePaused = true;
        //pooter.inGame = false;
        //gameStarted = false;
    }
    void ChangeTrainingPhase()
    {
        trainingPhase++;
        if(trainingPhase == 1)
        {
            tutorialScreen.SetImage(0);
            trainingBlockRender.enabled = true;
            Color c = Color.green;
            trainingBlockRender.color = new Color(c.r, c.g, c.b, 0.25f);
            //trainingBlock.localPosition = new Vector3(Screen.width * -0.0025f, Screen.height * -0.0025f, trainingBlock.localPosition.z);
            trainingBlock.localPosition = new Vector3(0f, Screen.height * -0.002f, trainingBlock.localPosition.z);
        }else if(trainingPhase == 2)
        {
            tutorialScreen.SetImage(1);
            Color c = Color.blue;
            trainingBlockRender.color = new Color(c.r, c.g, c.b, 0.25f);
            trainingBlock.localPosition = new Vector3(0f, Screen.height * -0.002f, trainingBlock.localPosition.z);
            //trainingBlock.localPosition = new Vector3(Screen.width * 0.0025f, Screen.height * -0.0025f, trainingBlock.localPosition.z);
        }
        else if(trainingPhase == 3)
        {
            Pooter.hasKilledSomebody = false;
            tutorialScreen.SetImage(2);
            trainingBlockRender.enabled = false;
        }else if(trainingPhase == 4)
        {
            tutorialScreen.SetImage(3);
        }
        else
        {
            gamePaused = true;
            ClearEverything();
            //Debug.Log("opening menu here");
            mainMenu.OpenMenu(true);
            EnableHudElements(false);
            if (trainingMode)
            {
                tutorialScreen.Activate(false);
                trainingBlockRender.enabled = false;
                trainingMode = false;
            }
        }
    }
    void UpdateTraining(float timePassed)
    {
        switch (trainingPhase)
        {
            case 1:
                Color c = trainingBlockRender.color;
                trainingTimer += timePassed;
                float alpha = 0.25f + (0.125f * Mathf.Sin(trainingTimer * Mathf.PI * 0.85f));
                trainingBlockRender.color = new Color(c.r, c.g, c.b, alpha);
                
                //if(Pooter.lastMoveDirect.magnitude > 0.75f) { ChangeTrainingPhase(); }
                break;
            case 2:
                Color c2 = trainingBlockRender.color;
                trainingTimer += timePassed;
                tutorialScreen.SetImage(1);
                float alpha2 = 0.25f + (0.125f * Mathf.Sin(trainingTimer * Mathf.PI * 0.85f));
                trainingBlockRender.color = new Color(c2.r, c2.g, c2.b, alpha2);
                //if (Pooter.pressingJump) { ChangeTrainingPhase(); }
                break;
            case 3:
                //if (Pooter.hasKilledSomebody){   ChangeTrainingPhase();}
                break;
        }
        if (tutorialScreen.GetTapped())
        {
            ChangeTrainingPhase();
            tutorialScreen.SetTapped(false);
        }
    }
    void UpdateGame(float timePassed)
    {
        //mainMenu.UpdateMenu(timePassed);
        if (!slowMotionCounter.hasfinished)
        {
            slowMotionCounter.UpdateCounter(timePassed);
            timePassed *= 0.5f;
        }
        scrollingScreen.UpdateScrollingScreen(timePassed);
        if (gameStarted && !gamePaused)
        {
            topDebrisCounter.UpdateCounter(timePassed);
            if (topDebrisCounter.hasfinished)
            {
                topDebrisCounter.ResetCounter();
                float randomXPos = Random.value * (Screen.width * 0.005f) * (Mathf.Sign(Random.value - 0.5f));
                Vector3 pos = new Vector3(randomXPos, transform.position.y + Screen.height * 0.005f, 0f);
                float speed = Screen.height * 0.0003f;
                Color c = Color.yellow;
                float randomValue = Random.value;
                if (randomValue > 0.8f)
                {
                    c = Color.magenta;
                }
                else if (randomValue > 0.3f)
                {
                    c = Color.white;
                }
                CreateColoredDebris(pos, Vector3.down * speed, c, 0.25f);
            }
            if (trainingMode)
            {
                UpdateTraining(timePassed);
            }
            multiplierCounter.UpdateCounter(timePassed); if (multiplierCounter.hasfinished && currentScoreMultiplier > 1f) { DecreaseScoreMultiplier();  }
            if (scoreMultiplierHasChanged) { scoreMultiplierHasChanged = false; currentScoreMultiplierText.text = "X " + currentScoreMultiplier.ToString(); }
            sparkleCounter.UpdateCounter(timePassed);
            pooter.UpdatePooter(timePassed);
            if (currentScore != weThinkScoreIs) { weThinkScoreIs = currentScore; currentScoreText.text = "Score : " + currentScore.ToString(); }
            currentDifficulty += timePassed * 0.025f;
            for(int i = 0; i < walls.Count; i++) { Wall w = walls[i]; if (w.CleanupThisWall()) { walls.Remove(w);i--;Destroy(w.gameObject); } }
            for(int i = 0; i < roundWalls.Count; i++) { RoundWall w = roundWalls[i]; if (w.CleanupThisWall()) { roundWalls.Remove(w);i--;Destroy(w.gameObject); } }
            for (int i = 0; i < badguys.Count; i++) { BadGuy b = badguys[i]; b.UpdateBadGuy(timePassed); if (b.GetReadyToDie()) { badguys.Remove(b); b.DestroyBadGuy(); } }
            for (int i = 0; i < bullets.Count; i++) { Bullet b = bullets[i]; b.UpdateBullet(timePassed); if (b.GetReadyToDie()) { bullets.Remove(b); b.DestroyBullet(); i--; } }
            for(int i = 0;i < backgroundEffects.Count; i++) { BackgroundVisualEffect b = backgroundEffects[i]; b.UpdateEffect(timePassed); }
            for(int i = 0;i < wranglers.Count; i++) { FlameWrangler w = wranglers[i];if (w.UpdateWrangler(timePassed)) { w.KillChildren(); wranglers.Remove(w);Destroy(w.gameObject);i--; } }
            for (int i = 0; i < explosions.Count; i++) 
            { 
                LimitedTimeVisualEffect e = explosions[i];
                e.UpdateEffect(timePassed);
                
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
                    float distMultiplier = directTowards.magnitude / (Pooter.brickLength * 4.20f);
                    distMoved *= distMultiplier;
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
        UpdateLevel();
    }
    void SwitchLevelType()
    {
        //Debug.Log("switching level " + currentDifficulty);
        lengthIntoCurrentLevel = 0f;
        lengthOfCurrentLevel = Screen.width * (0.075f);
        bricksBuilt = 0;
        instancesBuilt = 0;
        //Debug.Log("did the switch at " + Time.time + " and the length is " + lengthOfCurrentLevel);   
        distanceToNextLevelType += lengthOfCurrentLevel;
        if (currentLevelType == LevelType.basic) 
        { 
            if(nextNonBasicLevelType == LevelType.tunnel)
            {
                tunnelLevel++;
                currentLevelType = LevelType.tunnel;
                nextNonBasicLevelType = LevelType.obstacleCourse;
                //lastBrickYpos = ScrollingScreen.GetYPosAboveScreen();
                nextInstantiationPoint = pooter.transform.position.y + (Pooter.brickLength * 6f);
                lastBrickYpos = nextInstantiationPoint + (Screen.height * 0.0065f);
                //float swingDist = ((8f + currentDifficulty) * Pooter.brickLength);
                //laneWidth = Screen.width * 0.0075f; laneWidth -= (Random.value * Screen.width * 0.00025f)* (3f);
                laneWidth = Screen.width * 0.0075f; laneWidth -= (Random.value * Screen.width * 0.00125f);
                //swingDist = Screen.width * (0.001f + (0.00035f * difficultyMultiplier));
                swingDist = ((Screen.width * 0.005f) - laneWidth) * 0.45f;
                //swingDist = Screen.width * (0.0005f + (0.00025f * 3f));
                swingMultiplier = 8f;
                if (currentDifficulty > 3f) { swingMultiplier = 16f; }
                //if(currentDifficulty > 4f) { swingMultiplier = 6f; }else if (currentDifficulty > 2f) { swingMultiplier = 4f; }
                //if (swingDist > 4f * Pooter.brickLength) { swingMultiplier = 8f; } else if (swingDist > 2f * Pooter.brickLength) { swingMultiplier = 4f; } else { swingMultiplier = 2f; }
                //nextInstantiationPoint = lastBrickYpos + (Pooter.brickLength * 8f);
            }
            else
            {
                obstacleLevel++;
                builtobstacleLast = false;
                builtEnemyLast = true;
                builtWallLast = false;
                currentLevelType = LevelType.obstacleCourse;
                nextNonBasicLevelType = LevelType.tunnel;
                //nextInstantiationPoint = pooter.transform.position.y + (Pooter.brickLength * 8f);

                nextInstantiationPoint += (Pooter.brickLength * 8f);
                lastBrickYpos = nextInstantiationPoint + (Screen.height * 0.0065f);
            }

        } else 
        {
            basicLevel++;
            builtobstacleLast = false;
            builtEnemyLast = false;
            if (currentLevelType == LevelType.tunnel) 
            { 
               // nextInstantiationPoint = lastBrickYpos += Pooter.brickLength * (distPerTrigger + (Random.value * Random.Range(2f, 4f))); 
            } else
            {
                
            }
            //nextInstantiationPoint = pooter.transform.position.y + (Pooter.brickLength * f);
            nextInstantiationPoint += (Pooter.brickLength * 8f);
            currentLevelType = LevelType.basic;
            
            
            //nextInstantiationPoint = ScrollingScreen.GetYPosAboveScreen();
        }
        //Debug.Log(" this is the basic level now " + basicLevel + " and the tunnel " + tunnelLevel + " and the obstacle " + obstacleLevel);
        //Debug.Log("switching levelt type into " + currentLevelType);
        startOfLevel = nextInstantiationPoint;
    }
    void CreateTwoWallsApartFromCenter(float distFromCenter,float xMod,float yMod,float yPosition,bool makeRound)
    {
        Vector3 pos = new Vector3(0f - distFromCenter, yPosition, 0f);
        pos.x += xMod;
        if (makeRound)
        {
            roundWalls.Add(scrollingScreen.CreateRoundWall(2f, 2f, 0f, pos));
            pos.x += distFromCenter * 2f;
            roundWalls.Add(scrollingScreen.CreateRoundWall(2f, 2f, 0f, pos));
        }
        else
        {
            walls.Add(scrollingScreen.CreateWall(2f, 2f, 0f, pos));
            pos.x += distFromCenter * 2f;
            walls.Add(scrollingScreen.CreateWall(2f, 2f, 0f, pos));
        }


    }
    void UpdateLevel()
    {
        if (pooter.transform.position.y > highestYPoint)
        {
            float prevHighest = highestYPoint;
            highestYPoint = transform.position.y;
            float diff = highestYPoint - prevHighest;
            lengthIntoCurrentLevel += diff;
            distanceMovedUpward += diff;
            
        }
        if (currentLevelType == LevelType.basic)
        {
            if (distanceMovedUpward > nextInstantiationPoint)
            {
                nextInstantiationPoint += Pooter.brickLength * (distPerTrigger + (Random.value * Random.Range(-1f, 1f)));
                TriggerScrollScreen();
            }
            if (distanceMovedUpward >= distanceToNextLevelType && !trainingMode){SwitchLevelType();}
        }else if(currentLevelType == LevelType.tunnel)
        {
            float distBetweenBlocks = Pooter.brickLength * 2f;
            while(distanceMovedUpward > nextInstantiationPoint)
            {
                bricksBuilt++;
                float distanceUp = nextInstantiationPoint - startOfLevel;
                float percentDone = distanceUp/ lengthOfCurrentLevel;

                nextInstantiationPoint += distBetweenBlocks;
                if (percentDone < 0.05f)
                {
                    
                    float oneFifthDistance = lengthOfCurrentLevel * 0.05f;
                    //float percentageIntoBeginning = percentDone / 0.05f;
                    float percentageIntoBeginning = distanceUp / oneFifthDistance;
                    //float percentageIntoBeginning = percentDone;
                    float diff = Screen.width * 0.005f - (laneWidth * 0.5f);
                    float distFromCenter = (Screen.width * 0.005f) - (percentageIntoBeginning * diff);
                    lastBrickYpos += distBetweenBlocks ;
                    CreateTwoWallsApartFromCenter(distFromCenter, 0f, 0f, lastBrickYpos,true);

                }else if(percentDone < 0.95f)
                {
                    float percentageDoneCenterArea = (percentDone - 0.05f)/ 0.9f;
                    float center = Mathf.Sin(percentageDoneCenterArea * Mathf.PI * swingMultiplier) * swingDist;
                    float distFromCenter = laneWidth * 0.5f;
                    lastBrickYpos += distBetweenBlocks;
                    CreateTwoWallsApartFromCenter(distFromCenter, center, 0f, lastBrickYpos,true);
                }
                else if(percentDone < 1f)
                {
                    float remainingPerctent = percentDone - 0.95f;
                    float percentageIntoBeginning = remainingPerctent / 0.05f;
                    //float percentageIntoBeginning = percentDone;
                    float diff = Screen.width * 0.005f - (laneWidth * 0.5f);
                    float distFromCenter = (laneWidth * 0.5f) + (percentageIntoBeginning * diff);
                    //float distFromCenter = (Screen.width * 0.005f - (laneWidth * 0.5f)) + (percentageIntoBeginning * laneWidth * 0.5f);
                    lastBrickYpos += distBetweenBlocks;
                    CreateTwoWallsApartFromCenter(distFromCenter, 0f, 0f, lastBrickYpos,true);
                }
                else { SwitchLevelType(); }
            }
            int nextInstance = ((instancesBuilt + 1) * 7);
            if(bricksBuilt >= nextInstance)
            {
                //badguys.Add(scrollingScreen.CreateEnemy((GameObject)Resources.Load("Prefabs/UFOBadGuyPrefab"), 2f));
                /*badguys.Add(scrollingScreen.CreateEnemy((GameObject)Resources.Load("Prefabs/BadGuyPrefab"), 1.5f));
                badguys.Add(scrollingScreen.CreateEnemy((GameObject)Resources.Load("Prefabs/homingMissile"), 0.85f));
                badguys.Add(scrollingScreen.CreateEnemy((GameObject)Resources.Load("Prefabs/CircleBadGuy"), 2f));
                badguys.Add(scrollingScreen.CreateEnemy((GameObject)Resources.Load("Prefabs/UFOBadGuyPrefab"), 2f));*/
                //badguys.Add(scrollingScreen.CreateEnemyAt((GameObject)Resources.Load("Prefabs/BarrelPrefab"), 1f, distApart, yMod * modifier));
                float distanceUp = nextInstantiationPoint - startOfLevel;
                float percentDone = distanceUp / lengthOfCurrentLevel;
                float percentageDoneCenterArea = (percentDone - 0.05f) / 0.9f;
                float center = Mathf.Sin(percentageDoneCenterArea * Mathf.PI * swingMultiplier) * swingDist;
                float randomXPos = Random.Range(0f, laneWidth * 0.215f) * (Mathf.Sign(Random.value - 0.5f));randomXPos += center;
                if (builtEnemyLast)
                {
                    Vector3 pos = new Vector3(randomXPos, ScrollingScreen.GetYPosAboveScreen(), 0f);
                    float randoVal = Random.value;
                    if(randoVal > 0.4f)
                    {
                        bool buildTwo = false;
                        if (tunnelLevel > 2)
                        {
                            buildTwo = Random.value > 0.5f;
                            if (buildTwo)
                            {
                                float newXPos = Random.Range(Pooter.brickLength * 4f, laneWidth * 0.415f);
                                pos.x = newXPos;
                            }
                        }
                        if (Random.value > 0.5f) 
                        {
                            walls.Add(scrollingScreen.CreateWall(2f, 2f, 0f, pos)); 
                            if(buildTwo) 
                            {
                                pos.x *= -1f;
                                walls.Add(scrollingScreen.CreateWall(2f, 2f, 0f, pos));
                            }
                        } else 
                        { 
                            roundWalls.Add(scrollingScreen.CreateRoundWall(2f, 2f, 0f, pos));
                            if (buildTwo)
                            {
                                pos.x *= -1f;
                                roundWalls.Add(scrollingScreen.CreateRoundWall(2f, 2f, 0f, pos));
                            }
                        }
                    }
                    else if(randoVal > 0.2f)
                    {
                        powerups.Add(CreatePowerup(pos));
                    }
                    else
                    {
                        badguys.Add(scrollingScreen.CreateEnemyAt((GameObject)Resources.Load("Prefabs/BarrelPrefab"), 1f, randomXPos,0f));
                    }
                }
                else
                {
                    
                    float randomValue = (Random.value * (float)tunnelLevel);
                    
                    //float randomValue = (Random.value * currentDifficulty);
                    int randomInt = (int)randomValue;
                    if (randomInt > 4) {   randomInt = 4; }
                    if (tunnelLevel < 3 && Random.value > 0.75f) { randomInt = 3; }
                    switch (randomInt)
                    {
                        case 0:
                            badguys.Add(scrollingScreen.CreateEnemyAt((GameObject)Resources.Load("Prefabs/ProximityMine"), 0.45f, randomXPos));
                            break;
                        case 1:
                            badguys.Add(scrollingScreen.CreateEnemyAt((GameObject)Resources.Load("Prefabs/UFOBadGuyPrefab"), 0.85f, randomXPos));
                            break;
                        case 2:
                            badguys.Add(scrollingScreen.CreateEnemyAt((GameObject)Resources.Load("Prefabs/homingMissile"), 0.85f, 0f));
                            break;
                        case 3:
                            badguys.Add(scrollingScreen.CreateEnemyAt((GameObject)Resources.Load("Prefabs/BarrelPrefab"), 1f, randomXPos, 0f));
                            break;
                        case 4:
                            badguys.Add(scrollingScreen.CreateEnemyAt(Resources.Load("prefabs/CircleBadGuy") as GameObject, 1.25f, randomXPos));
                            break;
                    }
                    
                    
                }
                builtEnemyLast = !builtEnemyLast;
                
                instancesBuilt++;
            }
            //if(distanceMovedUpward > distanceToNextLevelType){SwitchLevelType();}
            
        }else if(currentLevelType == LevelType.obstacleCourse)
        {
            if (distanceMovedUpward > nextInstantiationPoint)
            {
                //nextInstantiationPoint += Pooter.brickLength * (distPerTrigger + (Random.value * Random.Range(-2f, 2f)));
                TriggerScrollScreenObstacleCourse();
            }
            if (distanceMovedUpward >= distanceToNextLevelType && !trainingMode) { SwitchLevelType(); }
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
        bool backButton = (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Q));
        if (backButton)
        {
            if (!mainMenu.menuEnabled) 
            { 
                gamePaused = true; 
                
                EnableHudElements(false);
                if (trainingMode)
                {
                    mainMenu.OpenMenu(true);
                    tutorialScreen.Activate(false);
                    trainingBlockRender.enabled = false;
                }
                else
                {
                    mainMenu.OpenMenu(false);
                }

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
                if (trainingMode)
                {
                    for(int i = 0; i < Input.touchCount; i++)
                    {
                        Touch t = Input.GetTouch(i);
                        if(t.phase == TouchPhase.Began)
                        {
                            if (tutorialScreen.DidThisTouchImpact(t))
                            {
                                tutorialScreen.SetTapped(true);
                            }
                        }
                    }
                }
            }
            else
            {
                if (pooter.hasInputTouch && gameOverScreenCounter.hasfinished && mainMenu.menuButtonCounter.hasfinished)
                {
                    if (!hasShownAd && pooter.hasInputTouch)
                    {
                        hasShownAd = true; LoadAdvertisement(); 
                    }
                    if (!mainMenu.menuEnabled)
                    {
                        gamePaused = true; 
                        if (trainingMode)
                        {
                            trainingBlockRender.enabled = false;   
                        }
                        mainMenu.OpenMenu(true);
                        EnableHudElements(false);
                        tutorialScreen.Activate(false);
                        Vector3 finalPos = mainMenu.transform.localPosition; finalPos.z = gameOverScreen.localPosition.z;
                        gameOverScreenCounter.EndCounter();
                        gameOverScreen.localPosition = finalPos;
                    }
                }
                //if (Input.GetKeyDown(KeyCode.Escape)) { if (mainMenu.enabled) { Application.Quit(); } }
            }
        }
    }
}
public enum LevelType { basic,tunnel,obstacleCourse}