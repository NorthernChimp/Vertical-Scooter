using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelBadGuy : MonoBehaviour,BadGuy
{

    Animator anim;
    Counter debrisCounter;
    Counter whiteCircleCounter;
    Counter endCounter;
    bool readyToDie;
    bool dead = false;
    bool struck = false;
    Vector3 directToMove = Vector3.zero;
    Rigidbody2D rbody;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Enemy" || collision.collider.tag == "Enemy Bullet")
        {
            if (!dead) { Explode(); }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
    }
    public Vector3 GetPosition() { return transform.position; }
    void Explode()
    {
        //MainScript.CreateBigRedExplosion(transform.position);
        struck = true;
        dead = true;
        anim.speed = 1f;
        rbody.velocity = Vector2.zero;
        MainScript.CreateExplosionAtScale(transform.position,2.5f);
        MainScript m = Camera.main.transform.GetComponent<MainScript>();
        List<BadGuy> badGuysWithinDistance = m.GetBadGuysNearPointWithinDistance(transform.position, Pooter.brickLength * 12f);
        foreach(BadGuy b in badGuysWithinDistance)
        {
            if (!b.GetDead() && (Object)b != this)
            {
                if (b is BarrelBadGuy)
                {

                    b.KillBadGuy(Vector2.zero);

                }
                else
                {
                    Vector3 pos = b.GetPosition();
                    Vector3 direct = (pos - transform.position).normalized;
                    MainScript.CreateTimesTwo(transform.position + (direct.normalized * Pooter.brickLength * 4.35f));
                    b.DoublePoints();
                    b.KillBadGuy(direct);
                }
            }
        }
    }
    public void DoublePoints() { }
    bool BadGuy.DamagesPlayer()
    {
        return false;
    }
    void BadGuy.SetupBadGuy(float sizeMultiplier)
    {
        endCounter = new Counter(1f);
        rbody = GetComponent<Rigidbody2D>(); anim = GetComponent<Animator>(); 
        debrisCounter = new Counter(0.1f);
        float aspectRatio = (Screen.width * 0.00125f) / 0.64f;
        anim.speed = 0f;
        transform.localScale = new Vector3(aspectRatio, aspectRatio, 1f) * sizeMultiplier;
        whiteCircleCounter = new Counter(0.65f);
    }
    void BadGuy.DestroyBadGuy() { Destroy(gameObject); }
    void BadGuy.KillBadGuy(Vector2 directToBadGuy) 
    { 
        if(directToBadGuy != Vector2.zero)
        {
            struck = true;
            Vector2 newDirect = new Vector2(directToBadGuy.x, directToBadGuy.y * 2f).normalized;
            directToMove = newDirect;
            endCounter = new Counter(0.75f);
        }
        else
        {
            Explode();
        }
    }
    bool BadGuy.GetDead() { return struck; }
    bool BadGuy.GetReadyToDie() { return readyToDie; }
    void BadGuy.UpdateBadGuy(float timePassed)
    {
        if (struck)
        {
            float speed = Pooter.brickLength * 19.20f * timePassed;
            rbody.MovePosition(transform.position + (directToMove * speed));
        }
        if (dead)
        {
            rbody.velocity = Vector2.zero;
            if (!readyToDie)
            {
                endCounter.UpdateCounter(timePassed);
                if (endCounter.hasfinished) { readyToDie = true; }
            }
            
        }
        if (transform.position.y <= Camera.main.transform.position.y - Screen.height * 0.0075f || Mathf.Abs(transform.position.x) >= Screen.width * 0.006f ) { readyToDie = true; }
    }
        // Update is called once per frame
        void Update()
    {
        
    }
}
