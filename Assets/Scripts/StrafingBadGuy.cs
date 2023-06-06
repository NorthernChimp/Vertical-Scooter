using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface BadGuy 
{
    public void SetupBadGuy(float sizeMultiplier);
    public void UpdateBadGuy(float timePassed);
    public bool GetReadyToDie();
    public void KillBadGuy(Vector2 directToBadGuy);
    public void DestroyBadGuy();
    public bool GetDead();
    public bool DamagesPlayer();
    public Vector3 GetPosition();
    public void DoublePoints();
}

public class StrafingBadGuy : MonoBehaviour , BadGuy 
{
    bool doubleNextPoints = false;
    Animator anim;
    public Sprite deadFace;
    Counter debrisCounter;
    Counter wanderCounter;
    bool hasBeenStruck = false;
    Vector3 directToMove = Vector3.zero;
    public Vector3 currentObjective = Vector3.zero;
    public float velocity = 24.20f;
    Vector3 baseScale = Vector3.zero;
    public Rigidbody2D rbody;
    Counter fireCounter;
    public GameObject bulletPrefab;
    bool readyToDie = false;
    float timeAlive = 0f;
    public bool DamagesPlayer() { return false; }
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void DoublePoints() { doubleNextPoints = true; }
    public Vector3 GetPosition() { return transform.position; }
    bool BadGuy.GetDead() { return hasBeenStruck; }
    void BadGuy.DestroyBadGuy() { Destroy(gameObject); }//completely destroys the badguy. it has been set to "dead" and we are now in the "cleanup" phase removing the gameobject
    void BadGuy.KillBadGuy(Vector2 directToBadGuy) {
        //baseScale = transform.localScale;
        int score = 3; if (doubleNextPoints) { score = 6; }
        MainScript.AddToScore(score);
        MainScript.IncreaseScoreMultiplier();
        hasBeenStruck = true;
        anim.Play("damagedAnim");
        directToMove = (directToBadGuy).normalized;
        GetComponent<SpriteRenderer>().sprite = deadFace;
    }//sets it to "dead" does not remove or destroy it but marks it so the program knows to remove it
    bool BadGuy.GetReadyToDie(){return readyToDie;}
    void BadGuy.SetupBadGuy(float sizeMultiplier)
    {
        //float aspectRatio = 1.28f / (Screen.width * 0.001f);
        //float aspectRatio = (Screen.width * 0.001f) / 1.28f  ;
        float aspectRatio = (Screen.width * 0.0015f) / 1.28f;
        transform.localScale = new Vector3(aspectRatio, aspectRatio, 1f) * sizeMultiplier;
        baseScale = transform.localScale;
        debrisCounter = new Counter(0.25f);
        anim = GetComponent<Animator>();
        wanderCounter = new Counter(1f);
        fireCounter = new Counter(2.25f);
        currentObjective = new Vector3(Mathf.Sign(Random.value - 0.5f) * Screen.width * 0.0047f,transform.position.y,transform.position.z);
        rbody = GetComponent<Rigidbody2D>();
    }
    void FirePhaser()
    {
        Bullet e = Instantiate(bulletPrefab, transform.position, Quaternion.identity).GetComponent<EnemyBullet>();
        e.SetupBullet(Vector2.down,GetComponent<Collider2D>());
    }
    void BadGuy.UpdateBadGuy(float timePassed)
    {
        timeAlive += timePassed;
        if (transform.position.y <= Camera.main.transform.position.y - Screen.height * 0.0075f) { readyToDie = true; }
        rbody.velocity = Vector2.zero;
        if (!hasBeenStruck)
        {
            fireCounter.UpdateCounter(timePassed);
            if (fireCounter.hasfinished) { FirePhaser(); fireCounter.ResetCounter(); }
            float distance = (currentObjective - transform.position).magnitude;
            float distToMove = timePassed * velocity * Pooter.brickLength;
            if (distToMove > distance)
            {
                transform.position = currentObjective;
                distToMove -= distance;
                currentObjective.x = currentObjective.x * -1f;
            }
            Vector3 directToObjective = (currentObjective - transform.position).normalized;
            rbody.MovePosition(transform.position + (directToObjective * distToMove));
            if (debrisCounter.hasfinished)
            {
                debrisCounter.ResetCounter();
                int randomInt = (int)Random.Range(2f, 4f);
                Vector2 moveDirect = new Vector3(currentObjective.x - transform.position.x, 0f, 0f).normalized;
                for (int i = 0; i < randomInt; i++)
                {
                    MainScript.CreateRedDebris(Pooter.GetRandomNearbyPos(transform.position), moveDirect * -0.07f * Pooter.brickLength); ;
                }
            }
            else { debrisCounter.UpdateCounter(timePassed); }
            //wanderCounter.UpdateCounter(timePassed);
            float multiplier = Mathf.Sin(timeAlive * Mathf.PI * 2f);
            transform.localScale = baseScale * (1f + (multiplier * 0.15f));
        }
        else
        {
            wanderCounter.UpdateCounter(timePassed);
            float multiplier = Mathf.Sin(timeAlive * Mathf.PI * 4f);
            transform.localScale = baseScale * (1.1f + (multiplier * 0.5f));
            //Debug.Log(transform.localScale);
            if (wanderCounter.hasfinished) { readyToDie = true; } else
            {
                transform.Rotate(new Vector3(0f, 0f, timePassed * Mathf.Sign(directToMove.x) * 45f));
                rbody.MovePosition(transform.position + (directToMove * 17.20f * timePassed * Pooter.brickLength));
            }   
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
