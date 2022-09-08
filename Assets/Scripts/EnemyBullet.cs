using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Bullet 
{
    public void SetupBullet(Vector2 fireDirect);
    public void UpdateBullet(float timePassed);
    public void Impact();
    public bool GetReadyToDie();

    

    public void DestroyBullet();
}
public class EnemyBullet : MonoBehaviour , Bullet
{
    public Vector2 bulletDirect = Vector2.zero;
    public Rigidbody2D rbody;
    public float velocity = 4.20f;
    public bool readyToDie = false;
    Bullet thisBullet;
    // Start is called before the first frame update
    void Start()
    {
           
    }
    
    void Bullet.DestroyBullet() { Destroy(gameObject); }
    void Bullet.SetupBullet(Vector2 fireDirect)
    {
        thisBullet = this;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, (Vector3)fireDirect);
        rbody = GetComponent<Rigidbody2D>();
        bulletDirect = fireDirect.normalized;
        MainScript.bullets.Add(this);
    }
    void Bullet.UpdateBullet(float timePassed)
    {
        float distToMove = velocity * timePassed;
        rbody.MovePosition((Vector2)transform.position + (distToMove * bulletDirect));
        rbody.velocity = Vector2.zero;
    }
    bool Bullet.GetReadyToDie() { return readyToDie; }
    void Bullet.Impact()
    {
        readyToDie = true;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.tag == "Player")
        {
            MainScript.CreateExplosion(collision.GetContact(0).point);
            Pooter p = collision.transform.GetComponent<Pooter>();
            Vector2 directToPooter = p.transform.position - transform.position;
            p.BounceOff(directToPooter.normalized);
            float pushSpeed = 10f;
            p.DealDamage();
            MainScript.ResetScoreMultiplier();
            p.AddVelocity(directToPooter.normalized * pushSpeed);
            CreateFanOfParticles();
        }
        
        thisBullet.Impact();
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
    // Update is called once per frame
    void Update()
    {
        
    }
}
