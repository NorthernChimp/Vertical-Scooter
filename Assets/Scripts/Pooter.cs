using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pooter : MonoBehaviour
{
    Vector2 moveDirect = Vector2.zero;
    Vector3 moveVelocity = Vector2.zero;

    public Animator jetPack;
    Animator pooterAnim;

    public List<SpriteRenderer> renders;

    public bool pressingJump = false;

    public bool inGame = false;
    public bool alive = true;
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
    float distPerTrigger = 12.5f;
    public static int currentHealth = 3;
    enum FacingDirect { straight,left,right}
    FacingDirect directFace = FacingDirect.straight;
    public static Transform pooterTransform;
    public PooterSettings defaultSettings;
    public PooterSettings currentSettings;
    public Vector3 extraVelocity = Vector3.zero;
    public List<PooterSettingsAffector> settingsAffectors;
    public Collider2D coll;
    Counter regenCounter;
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
                //MainScript.CreateBigExplosion(collision.GetContact(0).point);
                Vector3 directFromBadGuy = transform.position - collision.transform.position;directFromBadGuy.z = 0f;
                BounceOff(directFromBadGuy.normalized);
            }
        }
        else{MainScript.CreateSparkleExplosion(collision.GetContact(0).point);}
    }
    public static void DealDamage()//assumes you're dealing 1 damage
    {
        Camera.main.GetComponent<MainScript>().pooter.regenCounter.ResetCounter();
        if (currentHealth == 0) { }//the game ends...
        currentHealth--;
        ClampHealth();
    }
    public static void HealDamage()//assumes you're healing 1 damage
    {
        Camera.main.GetComponent<MainScript>().pooter.regenCounter.ResetCounter();
        currentHealth++;
        ClampHealth();
    }
    public static void ClampHealth()
    {
        currentHealth = (int)Mathf.Clamp((float)currentHealth, 0f, 3f);
        //Debug.Log("after clamping " + currentHealth);
    }
    public void SetupPooter()
    {
        pooterAnim = GetComponent<Animator>();
        pooterAnim.Play("idlePooter");
        jetPack.speed = 0f;
        regenCounter = new Counter(5f);
        settingsAffectors = new List<PooterSettingsAffector>();
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
        transform.localScale = basicScale * 1.25f;
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
        //flameRender.enabled = false;
        //jetPack.Play("jetpackAnim");
        //jetPack.Play("jetpackAnim", 0, 0f);
        jetPack.speed = 0f;
        jetPackAnimationCounter.UpdateCounter(jetPackAnimationCounter.endTime);
    }
    void UpdateMoveDirect()
    {
        //moveDirect = Vector2.zero;
        if (alive) { moveDirect = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")); } else
        {
            moveDirect = Vector2.zero;
        }
        
    }
    void UpdateCurrentSettings(float timepassed)
    {
        currentSettings = PooterSettings.CopySettings(defaultSettings);
        for(int i = 0; i < settingsAffectors.Count; i++)
        {
            PooterSettingsAffector a = settingsAffectors[i];
            a.endCounter.UpdateCounter(timepassed);if (a.endCounter.hasfinished) { i--; settingsAffectors.Remove(a); } else
            {
                switch (a.affectType)
                {
                    case PooterSettingsAffectorType.disableControl:
                        currentSettings.controlled = false;
                        break;
                    case PooterSettingsAffectorType.disableBounce:
                        currentSettings.bounces = false;
                        break;
                    case PooterSettingsAffectorType.changeSpeed:
                        currentSettings.maxSpeed += a.amt;
                        break;
                    case PooterSettingsAffectorType.changeAccel:
                        currentSettings.accelRate += a.amt;
                        break;
                        
                }
            }
        }
    }
    void PooterDies()
    {
        alive = false; MainScript.CreateBigExplosion(transform.position).parent = transform; MainScript.gameEnded = true;
        settingsAffectors = new List<PooterSettingsAffector>();
        coll.enabled = false;
        MakeInvisible(false);
    }
    void MakeInvisible(bool invis)
    {
        for(int i = 0;i < renders.Count; i++)
        {
            SpriteRenderer r = renders[i];
            r.enabled = invis;
        }
    }
    public void UpdatePooter(float timePassed)
    {
        if (regenCounter.hasfinished && alive) { HealDamage(); } else if(alive){ regenCounter.UpdateCounter(timePassed); }
        if(currentHealth == 0 && alive) { PooterDies(); }
        UpdateCurrentSettings(timePassed);
        jetPackCounter.UpdateCounter(timePassed);
        UpdateMoveDirect();
        float accelRate = currentSettings.accelRate * brickLength;
        Vector3 moveDirectThisFrame = (moveDirect.normalized * timePassed * accelRate);
        if (pressingJump && jetPackCounter.hasfinished)
        {
            if (alive) { ActivateJetpack(); }
        }
        if(moveDirectThisFrame.x == 0f)
        {
            if(directFace != FacingDirect.straight) { pooterAnim.Play("idlePooter"); directFace = FacingDirect.straight; }
        }else if(moveDirectThisFrame.x > 0f)
        {
            if(directFace != FacingDirect.right) { pooterAnim.Play("rightFacingPooter");directFace = FacingDirect.right; }
        }
        else
        {
            if(directFace != FacingDirect.left) { pooterAnim.Play("leftFacingPooter");directFace = FacingDirect.left; }
        }
        if(moveDirect == Vector2.zero)
        {
            moveVelocity *= 0.95f;
        }
        else { moveVelocity += moveDirectThisFrame; }
        
        float maxVelocity = currentSettings.maxSpeed * brickLength;
        if(moveVelocity.magnitude > maxVelocity) { moveVelocity = moveVelocity.normalized * maxVelocity;  }
        float yBoost = 0f;
        if (!jetPackAnimationCounter.hasfinished)
        {
            jetPackAnimationCounter.UpdateCounter(timePassed);
            if (jetPackAnimationCounter.hasfinished) 
            {
                jetPack.Play("jetpackAnim", 0, 0f);
                //jetPack.playbackTime = 0f;
                jetPack.speed = 0f;
            }
            else
            {
                float boostSpeed = 14.20f;
                yBoost = boostSpeed * timePassed * jetPackAnimationCounter.GetPercentageDone();
            }
        }
        rbody.velocity = Vector2.zero;
        Vector3 extraVelocityMove = (extraVelocity * timePassed);
        Vector3 posToMoveTo = transform.position + (moveVelocity * timePassed) + (Vector3.up * yBoost) + extraVelocityMove;
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
                nextInstantiationPoint += brickLength * (distPerTrigger + (Random.value * Random.Range(-2f,2f)));
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
        if (currentSettings.bounces)
        {
            moveVelocity = Vector3.Reflect(moveVelocity, normal);
            extraVelocity = Vector3.Reflect(extraVelocity, normal);
        }
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
    public void Revive()
    {
        MakeInvisible(true);
        alive = true;
        coll.enabled = true;
    }
    void ActivateJetpack()
    {
        jetPack.Play("jetpackAnim");jetPack.speed = 1f;
        jetPackCounter.ResetCounter();
        jetPackAnimationCounter.ResetCounter();
    }
    // Update is called once per frame
    void Update()
    {
        //UpdatePooter(Time.deltaTime);
        pressingJump = Input.GetKey(KeyCode.Space);
    }
}
public class PooterSettings
{
    public static PooterSettings CopySettings(PooterSettings settingsToCopy)
    {
        PooterSettings s = new PooterSettings();
        s.bounces = settingsToCopy.bounces;
        s.controlled = settingsToCopy.controlled;
        s.maxSpeed = settingsToCopy.maxSpeed;
        return s;
    }
    public bool controlled = true;
    public bool bounces = true;
    public float maxSpeed = 15f;
    public float accelRate = 45f;
}
public class PooterSettingsAffector
{
    public Counter endCounter;
    public PooterSettingsAffectorType affectType;
    public float amt = 0f;
    public PooterSettingsAffector(PooterSettingsAffectorType ty, float endTime)
    {
        endCounter = new Counter(endTime);
        affectType = ty;
    }
    public PooterSettingsAffector(PooterSettingsAffectorType ty, float amount,float endTime)
    {
        amt = amount;
        endCounter = new Counter(endTime);
        affectType = ty;
    }
}
public enum PooterSettingsAffectorType { disableBounce,disableControl,changeSpeed,changeAccel}