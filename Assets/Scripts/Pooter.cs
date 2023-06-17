using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pooter : MonoBehaviour
{
    Vector2 moveDirect = Vector2.zero;
    Vector3 moveVelocity = Vector2.zero;
    Vector3 moveVelocityLastFrame = Vector3.zero;
    SpriteRenderer protectiveShielding;

    Vector3 currentVelocity = Vector3.zero;

    public Animator jetPack;
    Animator pooterAnim;

    public List<SpriteRenderer> renders;

    public bool hasInputTouch = false;
    bool hasTakenDamageThisFrame = false;

    Counter tookDamageCounter;
    bool flashingRedWhite = false;
    bool vulnerable = true;

    bool upwardArrowVisible = false;
    Transform upwardArrow;
    SpriteRenderer upwardArrowRender;

    public static bool pressingJump = false;
    public bool testingOnPC = false;
    float timeTouchEnded = 0f;
    Vector2 lastTouchPos = Vector2.zero;
    Counter circleCounter;
    Counter impactDebrisCounter;
    Counter blipCounter;
    Counter flashRedCounter;
    bool flashingRed = false;
    int timesFlashedRed = 0;
    Counter greenDebrisCounter;
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
    
    float nextInstantiationPoint = 0f;
    float distPerTrigger = 12.5f;
    public static int currentHealth = 3;
    enum FacingDirect { straight,left,right}
    FacingDirect directFace = FacingDirect.straight;
    Vector3 lastMove = Vector3.zero;
    public static Transform pooterTransform;
    public static Vector3 lastMoveDirect;
    public PooterSettings defaultSettings;
    public PooterSettings currentSettings;
    public Vector3 extraVelocity = Vector3.zero;
    public List<PooterSettingsAffector> settingsAffectors;
    public Collider2D coll;
    public float MaxVelocityPossible = 3.5f;
    Counter regenCounter;
    public static bool hasKilledSomebody = false;
    TouchInterface moveInterface;



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
                hasKilledSomebody = true;
                //Debug.Log("killing bad guy " + collision.transform.name);
                Vector3 directToBadGuy = collision.transform.position - transform.position;
                b.KillBadGuy(directToBadGuy);
                MainScript.CreateExplosion(collision.GetContact(0).point);
                //MainScript.CreateBigExplosion(collision.GetContact(0).point);
                if (b.DamagesPlayer())
                {

                }
                else
                {
                    Color c = new Color(0f, 0.5f, 0f, 0.5f);
                    MainScript.CreateWhiteCircle(c, transform.position, true, 2f, 1f);
                }
                
                Vector3 directFromBadGuy = collision.transform.position - transform.position; directFromBadGuy.z = 0f;
                BounceOff(directFromBadGuy.normalized);
            }
            if (impactDebrisCounter.hasfinished)
            {
                CreateFanOfGreenParticles();
                impactDebrisCounter.ResetCounter();
            }
            
        }
        else if(collision.transform.tag == "Wall")
        {
            if (impactDebrisCounter.hasfinished)
            {
                CreateFanOfGreenParticles();
                impactDebrisCounter.ResetCounter();
            }
        }
        else
        {
            if(collision.transform.tag != "EnemyBullet")
            {
                MainScript.CreateSparkleExplosion(collision.GetContact(0).point);
            }
            
        }
    } 
    void CreateFanOfGreenParticles()
    {
        //Debug.Log("createing a fanof green particles at " + Time.time);
        float angleDiff = Mathf.PI / 4f;
        for (int i = 0; i < 16; i++)
        {
            float currentAngleMin = i * angleDiff;
            float randomVarianceFrom = Random.Range(0f, angleDiff);
            float currentAngle = currentAngleMin + randomVarianceFrom;
            Vector3 currentVector = new Vector3(Mathf.Sin(currentAngle), Mathf.Cos(currentAngle), 0f);
            MainScript.CreateGreenDebris(transform.position, currentVector * Pooter.brickLength * 0.420f);
        }
    }
    public void AddSettingsAffector(PooterSettingsAffector aff){settingsAffectors.Add(aff);if (aff.affectType == PooterSettingsAffectorType.invulnerable) { protectiveShielding.enabled = true; vulnerable = false; } }
    public bool DealDamage()//assumes you're dealing 1 damage, returns true if the damage hits
    {
        if (currentSettings.vulnerable && !MainScript.trainingMode && !hasTakenDamageThisFrame)
        {
            hasTakenDamageThisFrame = true;
            tookDamageCounter.ResetCounter();
            Camera.main.GetComponent<MainScript>().pooter.regenCounter.ResetCounter();
            currentHealth--;
            if (currentHealth == 0) { } else { SetFlashingRed(); }
            ClampHealth();
            return true;
        }
        return false;
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
    void SetFlashingRed()
    {
        settingsAffectors.Add(new PooterSettingsAffector(PooterSettingsAffectorType.invulnerable, 0.75f));
        flashRedCounter.ResetCounter();
        foreach(SpriteRenderer r in renders)
        {
            r.color = Color.red;
        }
        flashingRed = true;
        flashingRedWhite = true;
        renders[0].color = Color.red;
        timesFlashedRed = 0;
    }
    void SetupFlashingRed()
    {
        tookDamageCounter.ResetCounter();
    }
    public void SetupPooter()
    {
        tookDamageCounter = new Counter(0.75f);
        impactDebrisCounter = new Counter(0.1f);
        circleCounter = new Counter(1f);
        flashRedCounter = new Counter(2f);
        SpriteRenderer[] re = GetComponentsInChildren<SpriteRenderer>();
        foreach(SpriteRenderer sr in re)
        {
            if (sr.name == "protectionShield") { protectiveShielding = sr; sr.enabled = false; }else if(sr.name == "UpwardArrow") { upwardArrow = sr.transform; upwardArrowRender = sr; SetArrowVisibility(false); }
        }
        greenDebrisCounter = new Counter(0.15f);
        blipCounter = new Counter(0.75f);
        blipCounter.ResetCounter();
        greenDebrisCounter.ResetCounter();
        moveInterface = new TouchInterface();
        pooterAnim = GetComponent<Animator>();
        pooterAnim.Play("idlePooter");
        jetPack.speed = 0f;
        regenCounter = new Counter(5f);
        settingsAffectors = new List<PooterSettingsAffector>();
        defaultSettings = new PooterSettings();
        currentSettings = new PooterSettings();
        pooterTransform = transform;
        m = Camera.main.transform.GetComponent<MainScript>();
        
        float brickSize = (Screen.width / 30f) * 0.01f;//a brick should be one thirtieth of the screen width
        brickLength = brickSize;
        Vector3 origin = Vector3.left * brickSize * 14.5f;
        float scaleSize = brickSize / 0.32f;
        basicScale = new Vector3(scaleSize, scaleSize, 1f);
        transform.localScale = basicScale * 1.25f;
        background.localScale = basicScale * 1.7f;
        upwardArrow.SetParent(null);
        upwardArrow.localScale = basicScale * 2f;
        ScrollingScreen s = background.GetComponent<ScrollingScreen>();
        Camera.main.orthographicSize = Screen.height * 0.005f;
        jetPackCounter = new Counter(1f);
        jetPackCounter.UpdateCounter(jetPackCounter.endTime);
        jetPackAnimationCounter = new Counter(1f);
        jetPackAnimationCounter.hasfinished = true;
        
        MaxVelocityPossible *= brickLength;
        
        rbody.velocity = Vector2.zero;
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
        moveDirect = Vector2.zero;
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
            a.endCounter.UpdateCounter(timepassed);if (a.endCounter.hasfinished) { i--;  settingsAffectors.Remove(a); } else
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
                        currentSettings.speedUp = true;
                        currentSettings.maxSpeed += a.amt;
                        break;
                    case PooterSettingsAffectorType.changeAccel:
                        currentSettings.accelRate += a.amt;
                        break;
                    case PooterSettingsAffectorType.invulnerable:
                        currentSettings.vulnerable = false;
                        break;
                    case PooterSettingsAffectorType.changeTime:
                        currentSettings.timeChange = a.amt;
                        break;
                    case PooterSettingsAffectorType.changeBounceAmt:
                        currentSettings.bounceAmt += a.amt;
                        break;
                }
            }
        }
        if(currentSettings.vulnerable != vulnerable) { protectiveShielding.enabled = !currentSettings.vulnerable; vulnerable = currentSettings.vulnerable; }
    }
    void PooterDies()
    {
        int i = PlayerPrefs.GetInt("HighScore");
        //Debug.Log("current high score is " + i);
        if (MainScript.currentScore > i) { PlayerPrefs.SetInt("HighScore", MainScript.currentScore); }
        //Debug.Log("now current high score is " + PlayerPrefs.GetInt("HighScore"));
        alive = false; 
        MainScript.CreateBigExplosion(transform.position).parent = transform; 
        //MainScript.CreateBigRedExplosion(transform.position).parent = transform; 
        MainScript.gameEnded = true;
        settingsAffectors = new List<PooterSettingsAffector>();
        coll.enabled = false;
        MakeInvisible(false);
    }
    public static Vector2 GetRandomVector()
    {
        float angle = Random.Range(0f, Mathf.PI * 2f);
        return (new Vector2(Mathf.Sin(angle), Mathf.Cos(angle)));
    }
    public static Vector3 GetRandomNearbyPos(Vector3 pos)
    {
        
        return pos + ((Vector3)GetRandomVector() * brickLength * 1.5f);
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
        hasTakenDamageThisFrame = false;
        impactDebrisCounter.UpdateCounter(timePassed);
        if (flashingRedWhite)
        {
            flashRedCounter.UpdateCounter(timePassed);
            if (flashRedCounter.hasfinished) { flashingRedWhite = false; renders[0].color = Color.white; } else
            {
                float percent = flashRedCounter.GetPercentageDone();
                int remainder = (int)(percent % 0.05f);
                if (remainder == 1 && flashingRed != false)
                {
                    flashingRed = false;
                    renders[0].color = Color.white;
                }
                else if (remainder == 0 && flashingRed != true)
                {
                    flashingRed = true;
                    renders[0].color = Color.red;
                }
            }
        }
        //float effectCounterMultiplier = 1f + (lastMove.magnitude / (brickLength * 0.15f));
        float effectCounterMultiplier = 1f;
        //Debug.Log(lastMove);
        if (flashingRed)
        {
            if (!flashRedCounter.hasfinished)
            {
                flashRedCounter.UpdateCounter(timePassed);
                float percent = flashRedCounter.GetPercentageDone();
                if (percent >= timesFlashedRed * 0.085f)
                {
                    timesFlashedRed++;
                    bool turningRed = timesFlashedRed % 2 > 0f;

                    for (int i = 0; i < renders.Count; i++)
                    {
                        SpriteRenderer r = renders[i];
                        if (turningRed)
                        {
                            r.color = Color.red;
                        }
                        else { r.color = Color.white; }
                    }

                }
            }
            else
            {
                flashingRed = false;
                for (int i = 0; i < renders.Count; i++)
                {
                    SpriteRenderer r = renders[i];
                    r.color = Color.white;
                }
            }
        }
        
        if (!jetPackAnimationCounter.hasfinished) { effectCounterMultiplier *= 1.5f; }
        float speedMultiplier = moveVelocity.magnitude / (brickLength * 6f);
        speedMultiplier = Mathf.Clamp(speedMultiplier, 1f, 8f);
        if (flashingRed) { speedMultiplier *= 1.5f; } else if (!jetPackAnimationCounter.hasfinished) { speedMultiplier *= 1f + (1.5f * jetPackAnimationCounter.GetPercentageDone()); }else if (currentSettings.speedUp) { speedMultiplier *= 1.5f; }
        //Debug.Log(speedMultiplier);
        if (circleCounter.hasfinished)
        {
            Vector3 pos = transform.position + (moveVelocity.normalized * -1f * brickLength * speedMultiplier * 0.45f);
            Color c = new Color(0.75f, 0.75f, 0.75f, 0.15f);//normal grayish color
            if (flashingRed)
            {
                c = new Color(0.85f, 0f, 0f, 0.65f);
            }else if (!jetPackAnimationCounter.hasfinished)
            {
                c = new Color(0f, 0f, 0.6f, 0.525f);
            }else if (currentSettings.speedUp)
            {
                speedMultiplier *= 0.825f;
                c = new Color(0f, 0.454f, 0f, 0.454f);
                
            }
            MainScript.CreateWhiteCircle(c, pos, true,speedMultiplier * 0.2f,0.125f * speedMultiplier);
            circleCounter.ResetCounter();
        }
        circleCounter.UpdateCounter(timePassed * speedMultiplier);
        if (blipCounter.hasfinished)
        {
            
            
            if(lastMove.magnitude > 0.5f * brickLength)
            {
                MainScript.CreateBlip(GetRandomNearbyPos(transform.position), lastMove * -1f * 0.35f);
            }
            blipCounter.ResetCounter();
        }
        else { blipCounter.UpdateCounter(timePassed * effectCounterMultiplier); }
        if (greenDebrisCounter.hasfinished)
        {
            if(lastMove.magnitude > 0.001f)
            {
                int amtOfDebris = (int)Random.Range(1f, 3f + lastMove.magnitude);
                if (!jetPackCounter.hasfinished) 
                { 
                    for(int i = 0; i < amtOfDebris; i++)
                    {
                        MainScript.CreateBlueDebris(GetRandomNearbyPos(transform.position), lastMove * -1f * 0.85f);
                    }
                    amtOfDebris *= 2;  
                }
                if (!jetPackAnimationCounter.hasfinished) { amtOfDebris += 2; }
                for (int i = 0; i < amtOfDebris; i++)
                {
                    MainScript.CreateGreenDebris(GetRandomNearbyPos(transform.position), lastMove * -1f * 0.5f);
                }
            }
            greenDebrisCounter.ResetCounter();
        }
        else { greenDebrisCounter.UpdateCounter(timePassed * effectCounterMultiplier); }
        
        if (regenCounter.hasfinished && alive) { HealDamage(); } else if(alive){ regenCounter.UpdateCounter(timePassed); }
        if(currentHealth == 0 && alive) { PooterDies(); }
        UpdateCurrentSettings(timePassed);
        timePassed *= currentSettings.timeChange;
        jetPackCounter.UpdateCounter(timePassed);
        //if (testingOnPC) { UpdateMoveDirect(); }
        float accelRate = currentSettings.accelRate * brickLength;
        Vector3 moveDirectThisFrame = (moveDirect.normalized * timePassed * accelRate);
        if (pressingJump)
        {
            if (jetPackCounter.hasfinished)
            {
                if (alive) { ActivateJetpack(); }
            }
            pressingJump = false; 
        }
        if (!jetPackCounter.hasfinished)
        {
            pooterAnim.Play("fistUpPooter");
        }
        else
        {
            if (moveDirectThisFrame.x == 0f)
            {
                if (directFace != FacingDirect.straight) { pooterAnim.Play("idlePooter"); directFace = FacingDirect.straight; }
            }
            else if (moveDirectThisFrame.x > 0f)
            {
                if (directFace != FacingDirect.right) { pooterAnim.Play("rightFacingPooter"); directFace = FacingDirect.right; }
            }
            else
            {
                if (directFace != FacingDirect.left) { pooterAnim.Play("leftFacingPooter"); directFace = FacingDirect.left; }
            }
        }
        
        
        if (moveDirect == Vector2.zero )
        {
            moveVelocity *= 0.95f;
        }
        else { if (Vector3.Dot(moveDirect.normalized, moveVelocity.normalized) < 0f) {} moveVelocity += moveDirectThisFrame; }
        
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
        //Vector3 extraVelocityMove = (extraVelocity * timePassed);
        //Vector3 moveVelocityFrameMoved = moveVelocity * timePassed; //if(moveVelocityFrameMoved.magnitude > MaxVelocityPossible) { moveVelocityFrameMoved = moveVelocityFrameMoved.normalized * MaxVelocityPossible; }
        Vector3 posToMoveTo = transform.position + ((moveVelocity * timePassed) + (Vector3.up * yBoost) );// + extraVelocityMove;
        if(posToMoveTo.y < GetMaxYValue()) { posToMoveTo.y = GetMaxYValue(); }
        //Debug.Log((moveVelocity * timePassed) + (Vector3.up * yBoost));
        //Debug.Log("pos to move to is " + posToMoveTo + " and transform pos is " + transform.position);
        lastMove = ((moveVelocity * timePassed) + (Vector3.up * yBoost));
        //Debug.Log("last move " + lastMove);
        //lastMoveDirect = posToMoveTo - transform.position;lastMoveDirect.z = 0f;
        lastMoveDirect = moveDirect;
        rbody.MovePosition(posToMoveTo);
        extraVelocity *= 0.975f;
        Vector3 camPos = Camera.main.transform.position; //camPos.y = transform.position.y;
        //Camera.main.transform.position = camPos;
        float extraDistFromCenter = 10f * brickLength;
        float yDiff = camPos.y - transform.position.y - (extraDistFromCenter);
        if(yDiff > 0f) { yDiff = 0f; }
        CheckIfWeAreWithinLeftRightScreen();
        Camera.main.transform.Translate(Vector3.down * yDiff * timePassed * 4.20f);
        UpdateArrow();
    }
    void SetArrowVisibility(bool b) { upwardArrowVisible = b;upwardArrowRender.enabled = b; }
    void UpdateArrow()
    {
        if(moveDirect == Vector2.zero)
        {
            if (upwardArrowVisible) { SetArrowVisibility(false); }
        }
        else 
        {
            if (!upwardArrowVisible) { SetArrowVisibility(true); }
            
            float multiplier = moveVelocity.magnitude / (brickLength * 8f);
            Vector3 newLocalPos = moveDirect.normalized * (brickLength * (1.5f + multiplier)); newLocalPos.z = -5f; newLocalPos += transform.position;
            upwardArrow.position = newLocalPos;
            upwardArrow.rotation = Quaternion.LookRotation(Vector3.forward, moveDirect.normalized);
        }
        
    }
    public void AddVelocity(Vector3 velocityToAdd)
    {
        moveVelocity += velocityToAdd;
    }
    public void BounceOff(Vector3 normal)
    {
        normal.z = 0f;
        rbody.velocity = Vector2.zero;
        if (currentSettings.bounces)
        {
            //Debug.Log("bounces " + normal);
            float bounceAmt = 0.925f;
            moveVelocity = Vector3.Reflect(moveVelocity, normal) * bounceAmt;
            extraVelocity = Vector3.Reflect(extraVelocity, normal) * bounceAmt;
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
            BounceOff(new Vector3(Mathf.Sign(transform.position.x) * 1f, 0f, 0f).normalized); 
        }
        float maxHeightDist = Camera.main.transform.position.y - Screen.height* 0.005f; maxHeightDist += brickLength;
        if ((transform.position.y) < maxHeightDist)
        {
            Vector3 pos = transform.position; pos.y = maxHeightDist;
            transform.position = pos;
            BounceOff(new Vector3(0f, maxHeightDist, 0f).normalized);
        }
    }
    public void Revive()
    {
        rbody.velocity = Vector2.zero;
        MakeInvisible(true);
        alive = true;
        //Debug.Log("reviving");
        moveDirect = Vector2.zero;
        coll.enabled = true;
    }
    void ActivateJetpack()
    {

        MainScript.CreateWhiteCircle(Color.green, transform.position, true, 2f, 0.75f);
        jetPack.Play("jetpackAnim");jetPack.speed = 1f;
        jetPackCounter.ResetCounter();
        jetPackAnimationCounter.ResetCounter();
    }
    void ActivateMoveInterface(int finger)
    {
        
        moveInterface.active = true; moveInterface.touchId = finger; moveInterface.currentDirect = Vector2.zero;
    }
    void DeActivateMoveInterface() {  moveInterface.active = false; }
    void TouchControls()
    {
        hasInputTouch = false;
        moveDirect = Vector2.zero;
        if (alive)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch t = Input.GetTouch(i);
                switch (t.phase)
                {
                    case TouchPhase.Began:
                        hasInputTouch = true;
                        //Debug.Log("this register "+ t.position + " and widht " + Screen.width);
                        if (!moveInterface.active)
                        {
                            if (Screen.width * 0.05f < t.position.x && t.position.x < Screen.width * 0.95f)
                            {
                                if (Screen.height * 0.05f < t.position.y && t.position.y < Screen.height * 0.95f)
                                {
                                    ActivateMoveInterface(t.fingerId);
                                    if (Time.time - timeTouchEnded <= 0.15f)
                                    {
                                        Vector2 differenceBetweenThisTouchAndLast = t.position - lastTouchPos;
                                        float dist = differenceBetweenThisTouchAndLast.magnitude;
                                        if (dist <= Screen.width * 0.1f) { pressingJump = true; }
                                    }
                                }
                            }
                        }
                        break;
                    case TouchPhase.Moved:
                    case TouchPhase.Stationary:
                        if (moveInterface.active) 
                        {

                            if (t.fingerId == moveInterface.touchId) 
                            {
                                moveInterface.UpdateInterface(t.deltaPosition);
                                float totalDist = moveInterface.currentDirect.magnitude;
                                float maxDist = Screen.width * 0.15f;
                                if(totalDist > maxDist)
                                {
                                    moveDirect = moveInterface.currentDirect.normalized;
                                }
                                else
                                {
                                    float percent = totalDist / maxDist;
                                    moveDirect = moveInterface.currentDirect.normalized * percent;
                                }
                                moveDirect = moveInterface.currentDirect.normalized; 
                            } 
                        }
                        //Debug.Log("we're in here");
                        break;
                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                        if (moveInterface.active) { if (t.fingerId == moveInterface.touchId) { DeActivateMoveInterface(); timeTouchEnded = Time.time; lastTouchPos = t.position; } }
                        break;
                }
            }
        }
        
        //if(moveDirect.magnitude > Screen.width * 0.15f) { moveDirect *= 0.925f; }
        if (testingOnPC)
        {
            if (Input.anyKeyDown) { hasInputTouch = true; }
        }
    }
    // Update is called once per frame
    void Update()
    {
        //UpdatePooter(Time.deltaTime);
        TouchControls();
        if (testingOnPC)
        {
            //pressingJump = Input.GetKey(KeyCode.Space);
        }
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
    public bool vulnerable = true;
    public bool controlled = true;
    public bool speedUp = false;
    public bool bounces = true;
    public float bounceAmt = 0.875f;
    public float maxSpeed = 30.5f;
    public float accelRate = 105f;
    public float timeChange = 1f;
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
class TouchInterface
{
    public Vector2 currentDirect = Vector2.zero;
    public int touchId;
    public bool active = false;
    public void UpdateInterface(Vector2 delta)
    {
        currentDirect += delta * 0.01f;
        if (currentDirect.magnitude > Pooter.brickLength * 2.5f)
        {
            currentDirect *= 0.985f;
            
        }
        //Debug.Log("dir " + currentDirect);
    }
    
}
public enum PooterSettingsAffectorType { disableBounce,disableControl,changeSpeed,changeAccel,invulnerable,changeTime,changeBounceAmt}