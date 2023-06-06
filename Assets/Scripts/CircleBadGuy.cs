using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleBadGuy : MonoBehaviour, BadGuy
{
    bool doubleNextPoints = false;
    public Transform flame1;
    public Transform flame2;
    bool dead = false;
    bool readyToDie = false;
    float currentRotation = 0f;
    float distFromCenter = 0f;
    Rigidbody2D rbody;
    Vector3 directToMove;
    Counter endCounter;
    Counter debrisCounter;
    Counter whiteCircleCounter;
    Animator anim;
    public float moveSpeed = 2.420f;
    // Start is called before the first frame update
    void Start()
    {

    }
    public void DoublePoints() { doubleNextPoints = true; }
    public Vector3 GetPosition() { return transform.position; }
    public bool DamagesPlayer() { return false; }
    void BadGuy.SetupBadGuy(float sizeMultiplier)
    { 
        distFromCenter = flame1.localPosition.magnitude; 
        rbody = GetComponent<Rigidbody2D>(); anim = GetComponent<Animator>(); 
        debrisCounter = new Counter(0.1f); 
        float aspectRatio = (Screen.width * 0.00125f) / 0.64f;
        transform.localScale = new Vector3(aspectRatio, aspectRatio, 1f) * sizeMultiplier;
        whiteCircleCounter = new Counter(0.65f);
    }
    void BadGuy.DestroyBadGuy() { Destroy(gameObject); }
    void BadGuy.KillBadGuy(Vector2 directToBadGuy) { dead = true; directToMove = (directToBadGuy).normalized; endCounter = new Counter(0.75f); anim.Play("damagedCircleBad"); int score = 5; if (doubleNextPoints) { score = 10; } MainScript.AddToScore(score); MainScript.IncreaseScoreMultiplier(); }
    bool BadGuy.GetDead() { return dead; }
    bool BadGuy.GetReadyToDie() { return readyToDie; }
    void BadGuy.UpdateBadGuy(float timePassed)
    {
        float moveThisFrame = moveSpeed * timePassed * Pooter.brickLength;
        if (!dead)
        {
            whiteCircleCounter.UpdateCounter(timePassed);
            Vector3 directToPlayer = (Pooter.pooterTransform.position - transform.position).normalized;
            directToPlayer.z = 0f;
            rbody.MovePosition(transform.position + (directToPlayer.normalized * moveThisFrame));
            if (debrisCounter.hasfinished)
            {
                debrisCounter.ResetCounter();
                int randomInt = (int)Random.Range(2f, 5f);
                for (int i = 0; i < randomInt; i++)
                {
                    MainScript.CreateRedDebris(Pooter.GetRandomNearbyPos(transform.position), directToPlayer * -0.01f * Pooter.brickLength);
                }
            }
            else { debrisCounter.UpdateCounter(timePassed); }
        }
        else
        {
            whiteCircleCounter.UpdateCounter(timePassed * 4f);
            endCounter.UpdateCounter(timePassed);
            if (endCounter.hasfinished) { readyToDie = true; }
            rbody.MovePosition(transform.position + (directToMove.normalized * moveThisFrame * 3.25f ));
        }
        
        if (whiteCircleCounter.hasfinished)
        {
            whiteCircleCounter.ResetCounter();
            Color c = new Color(1f, 0f, 0f, 0.5f);
            MainScript.CreateWhiteCircle(c, transform.position, true, 2f, 0.5f);
        }
        rbody.velocity = Vector2.zero;
        float rotationSpeed = Mathf.PI * (0.25f + (0.1f * MainScript.currentDifficulty));
        float currentFrameRotation = rotationSpeed * timePassed * 0.75f;
        currentRotation += currentFrameRotation;
        Vector3 currentPos = new Vector3(Mathf.Sin(currentRotation), Mathf.Cos(currentRotation), 0f) * distFromCenter;
        flame1.localPosition = currentPos;
        flame2.localPosition = currentPos * -1f;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
