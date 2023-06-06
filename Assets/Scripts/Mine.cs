using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour , BadGuy
{
    bool doublePoints = false;
    bool readyToDie = false;
    bool dead = false;
    Animator anim;
    Rigidbody2D rbody;
    Counter endCounter;
    Counter debrisCounter;
    Counter debrisCounter2;
    Counter scanCounter;
    bool hasActivated = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        string tag = collision.transform.tag;
        switch (tag)
        {
            case "Player":
                Pooter p = collision.transform.GetComponent<Pooter>();
                if (p.DealDamage()) { MainScript.ResetScoreMultiplier(); } else
                {
                    int points = 2;
                    if (doublePoints) { points *= 2; }
                    MainScript.AddToScore(points);
                }
                Vector3 directTo = collision.transform.position - transform.position;
                p.BounceOff(directTo.normalized);
                Explode();
                break;
            case "Enemy":
            case "EnemyBullet":
                
                break;
        }
        
    }
    void Explode()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        dead = true;
        hasActivated = true;
        GetComponent<Collider2D>().enabled = false;
        //anim.Play("alarmMode", 0, 1f);
        MainScript.CreateExplosion(transform.position);
    }
    public void SetupBadGuy(float sizeMultiplier)
    {
        anim = GetComponent<Animator>();
        anim.Play("NewScan");
        rbody = GetComponent<Rigidbody2D>();
        
        endCounter = new Counter(1f);
        rbody = GetComponent<Rigidbody2D>(); anim = GetComponent<Animator>();
        debrisCounter = new Counter(0.25f);
        debrisCounter2 = new Counter(0.125f);
        float aspectRatio = (Screen.width * 0.00125f) / 0.64f;
        //anim.speed = 0f;
        transform.localScale = new Vector3(aspectRatio, aspectRatio, 1f) * sizeMultiplier;
        scanCounter = new Counter(1.75f);
    }
    public void UpdateBadGuy(float timePassed)
    {
        if (!hasActivated)
        {
            scanCounter.UpdateCounter(timePassed);
            if (scanCounter.hasfinished)
            {
                scanCounter.ResetCounter();
                Vector3 playerPos = Pooter.pooterTransform.position;
                Vector3 difference = transform.position - playerPos;
                float dist = difference.magnitude;
                if (dist < Pooter.brickLength * 16f)
                {
                    hasActivated = true;
                    anim.Play("alarmMode", 0, 1f);
                    MainScript.CreateWhiteCircle(Color.red, transform.position, true, 1.35f, 1f);
                }
                else
                {
                    MainScript.CreateWhiteCircle(Color.green, transform.position, true, 1f, 1f);
                    anim.Play("NewScan");
                    //anim.speed = 0.5f;
                }
            }
        }
        else if (!dead)
        {
            Vector3 playerPos = Pooter.pooterTransform.position;
            Vector3 difference = playerPos - transform.position;
            debrisCounter2.UpdateCounter(timePassed);
            if (debrisCounter2.hasfinished)
            {
                debrisCounter2.ResetCounter();
                MainScript.CreateRedDebris(transform.position, difference.normalized);
            }
            debrisCounter.UpdateCounter(timePassed);
            if (debrisCounter.hasfinished)
            {
                debrisCounter.ResetCounter();
                MainScript.CreateWhiteCircle(Color.red, transform.position, true, 1.35f, 1f);
            }
            
            float speed = Pooter.brickLength * 12f;
            rbody.MovePosition(transform.position + (difference.normalized * speed * timePassed));
        }
        if (dead && !readyToDie) 
        {
            endCounter.UpdateCounter(timePassed);
            if (endCounter.hasfinished) { readyToDie = true; }
        }
        if (transform.position.y <= Camera.main.transform.position.y - Screen.height * 0.0075f) { readyToDie = true; }
    }
    public bool GetDead() { return dead; }
    public Vector3 GetPosition() { return transform.position; }
    public bool GetReadyToDie() { return readyToDie; }
    public void DestroyBadGuy() { Destroy(gameObject); }
    public void KillBadGuy(Vector2 direct) { Explode(); }
    public void DoublePoints() { doublePoints = true; }
    public bool DamagesPlayer(){return true;}
    // Update is called once per frame
    void Update()
    {
        
    }
}
