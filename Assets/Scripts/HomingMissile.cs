using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMissile : MonoBehaviour, BadGuy
{
    bool readyToDie = false;
    float rotationSpeed = 25f;
    float currentSpeed = 0f;
    float accelerationRate = 0.5f;
    Rigidbody2D rbody;
    Counter debrisCounter;
    // Start is called before the first frame update
    void Start()
    {

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //MainScript.CreateExplosion(transform.position);
         MainScript.CreateBoomsplosion(transform.position);
        if(collision.transform.tag == "Player") 
        {
            Pooter p = collision.transform.GetComponent<Pooter>();
            Vector3 direct = (Pooter.pooterTransform.position - transform.position).normalized;
            p.BounceOff(direct);
            p.DealDamage();
            MainScript.ResetScoreMultiplier();
            p.extraVelocity += direct * Pooter.brickLength * 4.20f;
        }
        else { MainScript.AddToScore(1); }
        CreateFanOfParticles();
        readyToDie = true;
    }
    void CreateFanOfParticles()
    {
        float angleDiff = Mathf.PI / 4f;
        for (int i = 0; i < 16; i++)
        {
            float currentAngleMin = i * angleDiff;
            float randomVarianceFrom = Random.Range(0f, angleDiff);
            float currentAngle = currentAngleMin + randomVarianceFrom;
            Vector3 currentVector = new Vector3(Mathf.Sin(currentAngle), Mathf.Cos(currentAngle), 0f);
            MainScript.CreateRedDebris(transform.position, currentVector * Pooter.brickLength * 0.420f);
        }
    }
    void BadGuy.SetupBadGuy(){ rbody = GetComponent<Rigidbody2D>(); debrisCounter = new Counter(0.15f); }
    void BadGuy.UpdateBadGuy(float timePassed) 
    {
        if(transform.position.y <= Camera.main.transform.position.y - Screen.height * 0.0075f) { readyToDie = true; }
        float accelAmount = accelerationRate * timePassed * Pooter.brickLength;
        currentSpeed += accelAmount;
        Vector3 directToPlayer = Pooter.pooterTransform.position - transform.position;directToPlayer.z = 0;
        float rotationAmt = rotationSpeed * timePassed;
        float angleDiff = Vector3.Angle(directToPlayer, transform.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(Vector3.forward, directToPlayer * -1f), rotationAmt);
        if (angleDiff < rotationAmt) 
        {
            //transform.Rotate(new Vector3(0f, 0f, rotationAmt));
        }
        else
        {
            //transform.rotation = Quaternion.LookRotation(Vector3.forward, directToPlayer * -1f);
            //transform.Rotate(new Vector3(0f, 0f, angleDiff));
        }
        if(debrisCounter.hasfinished)
        {
            int randomInt = (int)Random.Range(2f, 5f);
            for(int i = 0; i < randomInt; i++)
            {
               MainScript.CreateRedDebris(Pooter.GetRandomNearbyPos(transform.position), transform.up * 1f * currentSpeed);
            }
            debrisCounter.ResetCounter();
        }
        else { debrisCounter.UpdateCounter(timePassed * (currentSpeed / (0.25f * Pooter.brickLength))); }
        rbody.MovePosition(transform.position + (transform.up * currentSpeed * -1f));
    }
    void BadGuy.KillBadGuy() { }
    bool BadGuy.GetReadyToDie() { return readyToDie; }

    void BadGuy.DestroyBadGuy() { Destroy(gameObject); }
    bool BadGuy.GetDead() { return false; }

    // Update is called once per frames
    void Update()
    {
        
    }
}
