using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface BadGuy 
{
    public void SetupBadGuy();
    public void UpdateBadGuy(float timePassed);
    public bool GetReadyToDie();
    public void KillBadGuy();
    public void DestroyBadGuy();
    public bool GetDead();
}

public class StrafingBadGuy : MonoBehaviour , BadGuy 
{
    Animator anim;
    public Sprite deadFace;
    Counter wanderCounter;
    bool hasBeenStruck = false;
    Vector3 directToMove = Vector3.zero;
    public Vector3 currentObjective = Vector3.zero;
    public float velocity = 4.20f;
    public Rigidbody2D rbody;
    Counter fireCounter;
    public GameObject bulletPrefab;
    bool readyToDie = false;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }
    bool BadGuy.GetDead() { return hasBeenStruck; }
    void BadGuy.DestroyBadGuy() { Destroy(gameObject); }//completely destroys the badguy. it has been set to "dead" and we are now in the "cleanup" phase removing the gameobject
    void BadGuy.KillBadGuy() {
        MainScript.AddToScore(3);
        MainScript.IncreaseScoreMultiplier();
        hasBeenStruck = true;
        anim.Play("damagedAnim");
        directToMove = (transform.position - Camera.main.GetComponent<MainScript>().pooter.transform.position).normalized;
        GetComponent<SpriteRenderer>().sprite = deadFace;
    }//sets it to "dead" does not remove or destroy it but marks it so the program knows to remove it
    bool BadGuy.GetReadyToDie(){return readyToDie;}
    void BadGuy.SetupBadGuy()
    {
        anim = GetComponent<Animator>();
        wanderCounter = new Counter(1f);
        fireCounter = new Counter(0.2f);
        currentObjective = new Vector3(Mathf.Sign(Random.value - 0.5f) * Screen.width * 0.0047f,transform.position.y,transform.position.z);
        rbody = GetComponent<Rigidbody2D>();
    }
    void FirePhaser()
    {
        Bullet e = Instantiate(bulletPrefab, transform.position, Quaternion.identity).GetComponent<EnemyBullet>();
        e.SetupBullet(Vector2.down);
    }
    void BadGuy.UpdateBadGuy(float timePassed)
    {
        if (transform.position.y <= Camera.main.transform.position.y - Screen.height * 0.0075f) { readyToDie = true; }
        rbody.velocity = Vector2.zero;
        if (!hasBeenStruck)
        {
            fireCounter.UpdateCounter(timePassed);
            if (fireCounter.hasfinished) { FirePhaser(); fireCounter.ResetCounter(); }
            float distance = (currentObjective - transform.position).magnitude;
            float distToMove = timePassed * velocity;
            if (distToMove > distance)
            {
                transform.position = currentObjective;
                distToMove -= distance;
                currentObjective.x = currentObjective.x * -1f;
            }
            Vector3 directToObjective = (currentObjective - transform.position).normalized;
            rbody.MovePosition(transform.position + (directToObjective * distToMove));
        }
        else
        {
            wanderCounter.UpdateCounter(timePassed);
            if (wanderCounter.hasfinished) { readyToDie = true; } else
            {
                transform.Rotate(new Vector3(0f, 0f, timePassed * Mathf.Sign(directToMove.x) * 45f));
                rbody.MovePosition(transform.position + (directToMove * 4.20f * timePassed));
            }   
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
