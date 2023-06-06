using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundWall : MonoBehaviour
{
    public CircleCollider2D coll;
    public List<Sprite> possibleSprites;
    // Start is called before the first frame update
    void Start()
    {
        int randomInt = (int)(Random.value * possibleSprites.Count);
        GetComponent<SpriteRenderer>().sprite = possibleSprites[randomInt];
        int randomDirectInt = (int)(Random.value * 4f);
        float randomAngle = Mathf.PI * 0.5f * randomDirectInt;
        Vector3 upDirect = new Vector3(Mathf.Sin(randomAngle), Mathf.Cos(randomAngle), 0f);
        transform.rotation = Quaternion.LookRotation(Vector3.forward, upDirect);
    }
    public bool CleanupThisWall()//returns true if its ready for cleanup
    {
        return transform.position.y < (Camera.main.transform.position.y) - (Screen.height * 0.01f);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player")
        {
            Pooter p = collision.collider.GetComponent<Pooter>();
            Vector3 directToPlayer = p.transform.position - transform.position;
            p.BounceOff(directToPlayer.normalized);
        }
        else if (collision.collider.tag == "EnemyBullet")
        {
            Bullet b = collision.transform.GetComponent<Bullet>();
            
            b.Impact();
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
