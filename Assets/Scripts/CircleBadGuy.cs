using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleBadGuy : MonoBehaviour, BadGuy
{
    public Transform flame1;
    public Transform flame2;
    bool dead = false;
    bool readyToDie = false;
    float currentRotation = 0f;
    float distFromCenter = 0f;
    Rigidbody2D rbody;
    Vector3 directToMove;
    Counter endCounter;
    Animator anim;
    // Start is called before the first frame update
    void Start()
    {

    }
    void BadGuy.SetupBadGuy(){ distFromCenter = flame1.localPosition.magnitude; rbody = GetComponent<Rigidbody2D>(); anim = GetComponent<Animator>(); }
    void BadGuy.DestroyBadGuy() { Destroy(gameObject); }
    void BadGuy.KillBadGuy() { dead = true; directToMove = (transform.position - Camera.main.GetComponent<MainScript>().pooter.transform.position).normalized; endCounter = new Counter(0.75f); anim.Play("damagedCircleBad"); MainScript.AddToScore(5); }
    bool BadGuy.GetDead() { return dead; }
    bool BadGuy.GetReadyToDie() { return readyToDie; }
    void BadGuy.UpdateBadGuy(float timePassed)
    {
        float moveSpeed = 2.420f;
        float moveThisFrame = moveSpeed * timePassed;
        if (!dead)
        {
            Vector3 directToPlayer = (Pooter.pooterTransform.position - transform.position).normalized;
            directToPlayer.z = 0f;
            rbody.MovePosition(transform.position + (directToPlayer.normalized * moveThisFrame));
        }
        else
        {
            endCounter.UpdateCounter(timePassed);
            if (endCounter.hasfinished) { readyToDie = true; }
            rbody.MovePosition(transform.position + (directToMove * moveThisFrame * 1.25f));
        }
        float rotationSpeed = Mathf.PI * (0.25f + (0.1f * MainScript.currentDifficulty));
        float currentFrameRotation = rotationSpeed * timePassed;
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
