using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UFOBadGuy : MonoBehaviour , BadGuy
{
    bool doubleNextPoints = false;
    Animator anim;
    public Sprite deadFace;
    Counter wanderCounter;
    bool hasBeenStruck = false;
    Vector3 directToMove = Vector3.zero;
    public Vector3 currentObjective = Vector3.zero;
    public float moveVelocity = 1.20f;
    public Rigidbody2D rbody;
    Counter fireCounter;
    public GameObject bulletPrefab;
    bool readyToDie = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void DoublePoints() { doubleNextPoints = true; }
    public bool DamagesPlayer() { return false; }
    bool BadGuy.GetDead() { return hasBeenStruck; }
    void BadGuy.DestroyBadGuy() { Destroy(gameObject); }//completely destroys the badguy. it has been set to "dead" and we are now in the "cleanup" phase removing the gameobject
    void BadGuy.KillBadGuy(Vector2 directToBadGuy)
    {
        int points = 3; if (doubleNextPoints){ points = 6; }
        MainScript.AddToScore(points);
        MainScript.IncreaseScoreMultiplier();
        hasBeenStruck = true;
        anim.Play("ufoDead");
        directToMove = (directToBadGuy).normalized;
        MainScript.CreateBigRedExplosion(transform.position);
        GetComponent<SpriteRenderer>().sprite = deadFace;
    }//sets it to "dead" does not remove or destroy it but marks it so the program knows to remove it
    bool BadGuy.GetReadyToDie() { return readyToDie; }
    void BadGuy.SetupBadGuy(float sizeMultiplier)
    {
        anim = GetComponent<Animator>();
        wanderCounter = new Counter(1f);
        fireCounter = new Counter(1.5f);
        float aspectRatio = (Screen.width * 0.0035f) / 1.28f;
        transform.localScale = new Vector3(aspectRatio, aspectRatio, 1f) * sizeMultiplier;
        currentObjective = new Vector3(Mathf.Sign(Random.value - 0.5f) * Screen.width * 0.0027f, transform.position.y, transform.position.z);
        rbody = GetComponent<Rigidbody2D>();
    }
    public Vector3 GetPosition() { return transform.position; }
    void FirePhaser()
    {
        Bullet e = Instantiate(bulletPrefab, transform.position, Quaternion.identity).GetComponent<EnemyBullet>();
        Vector2 directToPooter = Pooter.pooterTransform.position - transform.position;
        e.SetupBullet(directToPooter.normalized,GetComponent<Collider2D>());
    }
    void BadGuy.UpdateBadGuy(float timePassed)
    {
        if (transform.position.y <= Camera.main.transform.position.y - Screen.height * 0.0075f) { readyToDie = true; }
        rbody.velocity = Vector2.zero;
        if (!hasBeenStruck)
        {

            Vector2 directToPooter = Pooter.pooterTransform.position - transform.position;
            float distToMove = timePassed * moveVelocity * Pooter.brickLength;
            if (directToPooter.y > 0f) { directToPooter.y = 0f; }//does not move upward
            if (directToPooter.magnitude > Pooter.brickLength * 10f)
            {
                
                rbody.MovePosition(transform.position + (Vector3)(directToPooter.normalized * distToMove));
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
                float distToMove = timePassed * moveVelocity * Pooter.brickLength * 1.35f;
                transform.Rotate(new Vector3(0f, 0f, timePassed * Mathf.Sign(directToMove.x) * 45f));
                rbody.MovePosition(transform.position + (directToMove.normalized * distToMove));
            }
        }
        rbody.velocity = Vector2.zero;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
