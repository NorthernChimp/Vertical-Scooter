using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UFOBadGuy : MonoBehaviour , BadGuy
{
    Animator anim;
    public Sprite deadFace;
    Counter wanderCounter;
    bool hasBeenStruck = false;
    Vector3 directToMove = Vector3.zero;
    public Vector3 currentObjective = Vector3.zero;
    public float velocity = 4.20f;
    public float moveVelocity = 1.20f;
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
    void BadGuy.KillBadGuy()
    {
        MainScript.AddToScore(3);
        hasBeenStruck = true;
        anim.Play("ufoDead");
        directToMove = (transform.position - Camera.main.GetComponent<MainScript>().pooter.transform.position).normalized;
        GetComponent<SpriteRenderer>().sprite = deadFace;
    }//sets it to "dead" does not remove or destroy it but marks it so the program knows to remove it
    bool BadGuy.GetReadyToDie() { return readyToDie; }
    void BadGuy.SetupBadGuy()
    {
        anim = GetComponent<Animator>();
        wanderCounter = new Counter(1f);
        fireCounter = new Counter(0.65f);
        currentObjective = new Vector3(Mathf.Sign(Random.value - 0.5f) * Screen.width * 0.0027f, transform.position.y, transform.position.z);
        rbody = GetComponent<Rigidbody2D>();
    }
    void FirePhaser()
    {
        Bullet e = Instantiate(bulletPrefab, transform.position, Quaternion.identity).GetComponent<EnemyBullet>();
        Vector2 directToPooter = Pooter.pooterTransform.position - transform.position;
        e.SetupBullet(directToPooter.normalized);
    }
    void BadGuy.UpdateBadGuy(float timePassed)
    {
        if (transform.position.y <= Camera.main.transform.position.y - Screen.height * 0.0075f) { readyToDie = true; }
        rbody.velocity = Vector2.zero;
        if (!hasBeenStruck)
        {

            Vector2 directToPooter = Pooter.pooterTransform.position - transform.position;
            float distToMove = timePassed * moveVelocity;
            if (directToPooter.y > 0f) { directToPooter.y = 0f; }//does not move upward
            if (directToPooter.magnitude > Pooter.brickLength * 10f)
            {
                
                rbody.MovePosition(transform.position + (Vector3)(directToPooter * distToMove));
            }
            else { 
            }
            fireCounter.UpdateCounter(timePassed);
            if (fireCounter.hasfinished) { FirePhaser(); fireCounter.ResetCounter(); }
        }
        else
        {
            wanderCounter.UpdateCounter(timePassed);
            if (wanderCounter.hasfinished) { readyToDie = true; }
            else
            {
                transform.Rotate(new Vector3(0f, 0f, timePassed * Mathf.Sign(directToMove.x) * 45f));
                rbody.MovePosition(transform.position + (directToMove * 4.20f * timePassed));
            }
        }
        rbody.velocity = Vector2.zero;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
