using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainScript : MonoBehaviour
{
    public Pooter pooter;
    public ScrollingScreen scrollingScreen;
    public static List<Wall> walls;
    public static List<BadGuy> badguys;
    public static List<Bullet> bullets;
    public static List<LimitedTimeVisualEffect> explosions;
    public static float currentDifficulty = 1f;
    bool playedEnemyLast = true;
    // Start is called before the first frame update
    void Start()
    {
        bullets = new List<Bullet>();
        badguys = new List<BadGuy>();
        explosions = new List<LimitedTimeVisualEffect>();
        walls = new List<Wall>();
        pooter.SetupPooter();
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
            float difficultyPoint = currentDifficulty; if(difficultyPoint > 4f) { difficultyPoint = 4f; }
            int randomInt = (int)Random.Range(0f, difficultyPoint);
            switch (randomInt)
            {
                case 0:
                    scrollingScreen.CreateEnemy((GameObject)Resources.Load("Prefabs/BadGuyPrefab"), 1.5f);
                    break;
                case 1:
                    scrollingScreen.CreateEnemy((GameObject)Resources.Load("Prefabs/homingMissile"), 0.85f);
                    break;
                case 2:
                    scrollingScreen.CreateEnemy((GameObject)Resources.Load("Prefabs/CircleBadGuy"), 2f);
                    break;
                case 3:
                    scrollingScreen.CreateEnemy((GameObject)Resources.Load("Prefabs/BadGuyPrefab"), 2f);
                    //scrollingScreen.CreateEnemy((GameObject)Resources.Load("Prefabs/BadGuy"), 2f);
                    break;
            }
            
        }else
        {
            playedEnemyLast = false;
            float difficultyPoint = currentDifficulty + 1f; if (difficultyPoint > 4f) { difficultyPoint = 4f; }
            int numberOfWalls = (int)Random.Range(1f, difficultyPoint);
            float totalWidth = ((Screen.width * 0.0045f) -Pooter.brickLength) * 2f;
            Vector3 origin = Vector3.left * (totalWidth * 0.5f);
            origin.y = ScrollingScreen.GetYPosAboveScreen();
            float distBetween = totalWidth / (numberOfWalls);
            Debug.DrawLine(Vector3.zero, Vector3.left * totalWidth * 0.5f);
            float randomXOffsetMax = (totalWidth * 0.9f) / numberOfWalls;
            //float xOffset = Random.Range(0f, randomXOffsetMax);
            for (int i = 0; i < numberOfWalls; i++)
            {
                Vector3 pos = origin + (Vector3.right * i * distBetween) + (Vector3.right * Random.value * distBetween) ;
                scrollingScreen.CreateWall(2f, 2f, 0f,pos);
            }
        }
    }
    void UpdateGame(float timePassed)
    {
        currentDifficulty += timePassed * 0.075f;
        pooter.UpdatePooter(timePassed);
        scrollingScreen.UpdateScrollingScreen(timePassed);
        for(int i = 0; i < badguys.Count; i++){BadGuy b = badguys[i];b.UpdateBadGuy(timePassed);if (b.GetReadyToDie()){ badguys.Remove(b); b.DestroyBadGuy(); } }
        for(int i = 0; i < bullets.Count; i++) { Bullet b = bullets[i]; b.UpdateBullet(timePassed); if (b.GetReadyToDie()) { bullets.Remove(b); b.DestroyBullet(); i--; } }
        for (int i = 0; i < explosions.Count; i++) { LimitedTimeVisualEffect e = explosions[i]; e.endCounter.UpdateCounter(timePassed); if (e.endCounter.hasfinished) { explosions.Remove(e); i--; e.Destroy(); } }
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
