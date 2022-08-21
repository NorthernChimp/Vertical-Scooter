using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pooter : MonoBehaviour
{
    Vector2 moveDirect = Vector2.zero;
    Vector3 moveVelocity = Vector2.zero;
    public SpriteRenderer flameRender;
    Counter jetPackCounter;
    Counter jetPackAnimationCounter;
    public GameObject brick;
    public static Vector3 basicScale = Vector3.zero;
    MainScript m;
    public static float brickLength = 0f;
    public Transform background;
    public Rigidbody2D rbody;
    float highestYPoint = 0f;
    float distanceMovedUpward = 0f;
    float nextInstantiationPoint = 0f;
    float distPerTrigger = 7.5f;
    public static Transform pooterTransform;
    public PooterSettings defaultSettings;
    public PooterSettings currentSettings;
    public Vector3 extraVelocity = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        
        //SetupPooter();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log("colliding with " + collision.transform.name);
        if (collision.transform.tag == "Enemy")
        {
            BadGuy b = collision.transform.GetComponent<BadGuy>();
            if (!b.GetDead())
            {
                //Debug.Log("killing bad guy " + collision.transform.name);
                b.KillBadGuy();
                MainScript.CreateExplosion(collision.GetContact(0).point);
                Vector3 directFromBadGuy = transform.position - collision.transform.position;directFromBadGuy.z = 0f;
                BounceOff(directFromBadGuy.normalized);
            }
        }
        else
        {
            MainScript.CreateSparkleExplosion(collision.GetContact(0).point);
        }
    }
    public void SetupPooter()
    {
        
        defaultSettings = new PooterSettings();
        currentSettings = new PooterSettings();
        pooterTransform = transform;
        m = Camera.main.transform.GetComponent<MainScript>();
        highestYPoint = transform.position.y;
        float brickSize = (Screen.width / 30f) * 0.01f;//a brick should be one thirtieth of the screen width
        brickLength = brickSize;
        Vector3 origin = Vector3.left * brickSize * 14.5f;
        float scaleSize = brickSize / 0.32f;
        basicScale = new Vector3(scaleSize, scaleSize, 1f);
        transform.localScale = basicScale * 2f;
        background.localScale = basicScale * 1.7f;
        ScrollingScreen s = background.GetComponent<ScrollingScreen>();
        Camera.main.orthographicSize = Screen.height * 0.005f;
        jetPackCounter = new Counter(1f);
        jetPackAnimationCounter = new Counter(1f);
        jetPackAnimationCounter.hasfinished = true;
        nextInstantiationPoint = highestYPoint + (distPerTrigger * brickLength);
    }
    public void DisableJetPack()
    {
        flameRender.enabled = false;
        jetPackAnimationCounter.UpdateCounter(jetPackAnimationCounter.endTime);
    }
    void UpdateMoveDirect()
    {
        //moveDirect = Vector2.zero;
        moveDirect = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }
    public void UpdatePooter(float timePassed)
    {
        jetPackCounter.UpdateCounter(timePassed);
        UpdateMoveDirect();
        float accelRate = 20f * brickLength;
        Vector3 moveDirectThisFrame = (moveDirect.normalized * timePassed * accelRate);
        if (Input.GetKeyDown(KeyCode.Space) && jetPackCounter.hasfinished)
        {
            ActivateJetpack();
        }
        
        moveVelocity += moveDirectThisFrame;
        float maxVelocity = Screen.width * 0.005f;
        if(moveVelocity.magnitude > maxVelocity) { moveVelocity = moveVelocity.normalized * maxVelocity; }
        float yBoost = 0f;
        if (!jetPackAnimationCounter.hasfinished)
        {
            jetPackAnimationCounter.UpdateCounter(timePassed);
            if (jetPackAnimationCounter.hasfinished) { flameRender.enabled = false; }
            else
            {
                Vector3 packScale = new Vector3(1f, 1f, 1f) * 0.57f;
                packScale += new Vector3(Mathf.Sin(jetPackAnimationCounter.currentTime * Mathf.PI * 2f) + 1f, Mathf.Cos(jetPackAnimationCounter.currentTime * Mathf.PI * 2f) + 1f, 0f) * 0.85f;
                flameRender.transform.localScale = packScale;
                //float yBoost = Mathf.Sin(jetPackAnimationCounter.currentTime * Mathf.PI * 2f) + 1f;
                //float yBoost = Mathf.Pow(((jetPackAnimationCounter.currentTime - 1f) * 25f) ,2f) * 14.20f ;
                //Debug.Log("we in here");
                float boostSpeed = 10.20f;
                yBoost = boostSpeed * timePassed * jetPackAnimationCounter.GetPercentageDone();
                //moveDirectThisFrame.y += yBoost * timePassed;
            }
        }
        //transform.Translate((moveVelocity * timePassed) + (Vector3.up * yBoost));
        rbody.velocity = Vector2.zero;
        Vector3 extraVelocityMove = (extraVelocity * timePassed);
        //Debug.Log(extraVelocityMove);
        //rbody.MovePosition(transform.position + (moveVelocity * timePassed) + extraVelocityMove + (Vector3.up * yBoost));
        Vector3 posToMoveTo = transform.position + (moveVelocity * timePassed) + extraVelocityMove + (Vector3.up * yBoost);
        if(posToMoveTo.y < GetMaxYValue()) { posToMoveTo.y = GetMaxYValue(); }
        rbody.MovePosition(posToMoveTo);
        extraVelocity *= 0.975f;
        Vector3 camPos = Camera.main.transform.position;
        float extraDistFromCenter = 10f * brickLength;
        float yDiff = camPos.y - transform.position.y - (extraDistFromCenter);
        if(yDiff > 0f) { yDiff = 0f; }
        CheckIfWeAreWithinLeftRightScreen();
        Camera.main.transform.Translate(Vector3.down * yDiff * timePassed * 4.20f);
        if(transform.position.y > highestYPoint) 
        {
            float prevHighest = highestYPoint;
            highestYPoint = transform.position.y;
            float diff = highestYPoint - prevHighest;
            distanceMovedUpward += diff;
            if(distanceMovedUpward > nextInstantiationPoint)
            {
                nextInstantiationPoint += brickLength * distPerTrigger;
                m.TriggerScrollScreen();
            }
        }
    }
    public void AddVelocity(Vector3 velocityToAdd)
    {
        moveVelocity += velocityToAdd;
    }
    public void BounceOff(Vector3 normal)
    {
        rbody.velocity = Vector2.zero;
        moveVelocity = Vector3.Reflect(moveVelocity, normal);
        extraVelocity = Vector3.Reflect(extraVelocity, normal);
    }
    float GetMaxYValue() { return (Screen.width * 0.005f) - brickLength * 1f; }
    void CheckIfWeAreWithinLeftRightScreen()
    {
        float maxDist = GetMaxYValue();
        if (Mathf.Abs(transform.position.x) > maxDist) 
        { 
            Vector3 pos = transform.position;pos.x = maxDist * Mathf.Sign(transform.position.x);
            transform.position = pos;
            BounceOff(new Vector3(Mathf.Sign(transform.position.x) * 1f, 0f, 0f)); 
        }
        float maxHeightDist = Camera.main.transform.position.y - Screen.height* 0.005f; maxHeightDist += brickLength;
        if ((transform.position.y) < maxHeightDist)
        {
            Vector3 pos = transform.position; pos.y = maxHeightDist;
            transform.position = pos;
            BounceOff(new Vector3(0f, maxHeightDist, 0f));
        }
    }
    void ActivateJetpack()
    {
        flameRender.enabled = true;
        jetPackCounter.ResetCounter();
        jetPackAnimationCounter.ResetCounter();
    }
    // Update is called once per frame
    void Update()
    {
        UpdatePooter(Time.deltaTime);
    }
}
public class PooterSettings
{
    public bool controlled = true;
    public bool bounces = true;
    public float maxSpeed = 1f;
}