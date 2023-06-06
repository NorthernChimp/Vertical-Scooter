using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    // Start is called before the first frame update
    public BoxCollider2D coll;
    public List<Sprite> possibleSprites;
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
            //Debug.DrawRay((Vector3)collision.ClosestPoint(p.transform.position), directToPlayer,Color.green,3f);
            float halfWidth = (GetWidth() * 0.5f);
            float halfHeight = (GetHeight() * 0.5f);
            //float xDist = Mathf.Abs(directToPlayer.x);
            //float yDist = Mathf.Abs(directToPlayer.y);
            //float clampedX = Mathf.Clamp(p.transform.position.x, transform.position.x -halfWidth, transform.position.x + halfWidth);
            float clampedX = Mathf.Clamp(directToPlayer.x, -halfWidth, halfWidth);
            float clampedY = Mathf.Clamp(directToPlayer.y, -halfHeight, halfHeight);
            //float clampedY = Mathf.Clamp(p.transform.position.y, transform.position.y -halfHeight, transform.position.y + halfHeight);
            Vector3 clampedPos = new Vector3(clampedX, clampedY, 0f);
            //Vector3 clampedPosWorld = clampedPos + transform.position;
            //Vector3 normal = p.transform.position - (transform.position + clampedPos);
            Vector3 normal = clampedPos - transform.position;normal.z = 0f;
            p.BounceOff(normal.normalized);
            p.rbody.velocity = Vector2.zero;
            //Debug.DrawRay(transform.position + clampedPos, normal.normalized,Color.green,3f);
        }else if (collision.transform.tag == "EnemyBullet")
        {
            Bullet b = collision.transform.GetComponent<Bullet>();
            b.Impact();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Pooter p = collision.GetComponent<Pooter>();
            Vector3 directToPlayer = p.transform.position - transform.position;
            //Debug.DrawRay((Vector3)collision.ClosestPoint(p.transform.position), directToPlayer,Color.green,3f);
            float halfWidth = (GetWidth() * 0.5f);
            float halfHeight = (GetHeight() * 0.5f);
            //float xDist = Mathf.Abs(directToPlayer.x);
            //float yDist = Mathf.Abs(directToPlayer.y);
            //float clampedX = Mathf.Clamp(p.transform.position.x, transform.position.x -halfWidth, transform.position.x + halfWidth);
            float clampedX = Mathf.Clamp(directToPlayer.x, -halfWidth, halfWidth);
            float clampedY = Mathf.Clamp(directToPlayer.y, -halfHeight, halfHeight);
            //float clampedY = Mathf.Clamp(p.transform.position.y, transform.position.y -halfHeight, transform.position.y + halfHeight);
            Vector3 clampedPos = new Vector3(clampedX, clampedY, 0f);
            Vector3 clampedPosWorld = clampedPos + transform.position;
            Vector3 normal = p.transform.position - (transform.position + clampedPos);
            p.transform.position = clampedPosWorld + (normal.normalized * 2f * Pooter.brickLength);
            p.AddVelocity(normal.normalized * Pooter.brickLength * 1f);
            p.BounceOff(normal.normalized);
            //Debug.DrawRay(transform.position + clampedPos, normal.normalized,Color.green,3f);
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Pooter p = collision.GetComponent<Pooter>();
            Vector3 directToPlayer = p.transform.position - transform.position;
            //Debug.DrawRay((Vector3)collision.ClosestPoint(p.transform.position), directToPlayer,Color.green,3f);
            float halfWidth = (GetWidth() * 0.5f);
            float halfHeight = (GetHeight() * 0.5f);
            //float xDist = Mathf.Abs(directToPlayer.x);
            //float yDist = Mathf.Abs(directToPlayer.y);
            //float clampedX = Mathf.Clamp(p.transform.position.x, transform.position.x -halfWidth, transform.position.x + halfWidth);
            float clampedX = Mathf.Clamp(directToPlayer.x, -halfWidth, halfWidth);
            float clampedY = Mathf.Clamp(directToPlayer.y, -halfHeight, halfHeight);
            //float clampedY = Mathf.Clamp(p.transform.position.y, transform.position.y -halfHeight, transform.position.y + halfHeight);
            Vector3 clampedPos = new Vector3(clampedX, clampedY, 0f);
            Vector3 clampedPosWorld = clampedPos + transform.position;
            Vector3 normal = p.transform.position - (transform.position + clampedPos);
            p.transform.position = clampedPosWorld + (normal.normalized * 2f * Pooter.brickLength);
            p.AddVelocity(normal.normalized * Pooter.brickLength * 1f);
            p.BounceOff(normal.normalized);
            //Debug.DrawRay(transform.position + clampedPos, normal.normalized,Color.green,3f);
        }
    }
    float GetWidth()
    {
        float percent = transform.localScale.x / Pooter.basicScale.x;
        return percent * Pooter.brickLength;
        //return coll.size.x * transform.localScale.x;
    }
    float GetHeight()
    {
        float percent = transform.localScale.y / Pooter.basicScale.y;
        return percent * Pooter.brickLength;
        //return coll.size.y * transform.localScale.y;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
