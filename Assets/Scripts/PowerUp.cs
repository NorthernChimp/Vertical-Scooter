using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    // Start is called before the first frame update
    bool readyToDie = false;
    PowerupType powerType = PowerupType.speed;
    public Sprite speedSprite;
    public Sprite invulnerableSprite;
    public Sprite healthSprite;
    public Sprite timeSlowSprite;
    void Start()
    {
        
    }
    public void SetupPowerup(PowerupType typeToBecome)
    {
        powerType = typeToBecome;
        if(typeToBecome == PowerupType.speed) { GetComponent<SpriteRenderer>().sprite = speedSprite; }else if(typeToBecome == PowerupType.invulnerabl) { GetComponent<SpriteRenderer>().sprite = invulnerableSprite; }else if(typeToBecome == PowerupType.health) { GetComponent<SpriteRenderer>().sprite = healthSprite; }else if (typeToBecome == PowerupType.timeSlow)
        {
            GetComponent<SpriteRenderer>().sprite = timeSlowSprite;
        }
    }
    public bool GetReadyToDie()
    {
        if (transform.position.y < Camera.main.transform.position.y - (Screen.height * 0.01f))
        {
            readyToDie = true;
        }
        return readyToDie;

    }
    void CreateFanOfParticles()
    {
        float angleDiff = Mathf.PI / 4f;
        for (int i = 0; i < 16; i++)
        {
            float currentAngleMin = i * angleDiff;
            float randomVarianceFrom = Random.Range(0f, angleDiff);
            float currentAngle = currentAngleMin + randomVarianceFrom;
            Vector3 currentVector = new Vector3(Mathf.Sin(currentAngle), Mathf.Cos(currentAngle),0f);
            MainScript.CreateGreenDebris(transform.position, currentVector * Pooter.brickLength * 0.420f);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Pooter p = collision.transform.GetComponent<Pooter>();
            MainScript.CreateWhiteCircle(Color.green, transform.position, true, 2.20f, 0.25f);
            if (powerType == PowerupType.speed)
            {
                p.AddSettingsAffector(new PooterSettingsAffector(PooterSettingsAffectorType.changeSpeed, 15f, 4.2f));
                p.AddSettingsAffector(new PooterSettingsAffector(PooterSettingsAffectorType.changeAccel, 45f, 4.2f));
                p.AddSettingsAffector(new PooterSettingsAffector(PooterSettingsAffectorType.changeBounceAmt, -0.795f, 4.2f));
                
            }else if(powerType == PowerupType.invulnerabl)
            {
                p.AddSettingsAffector(new PooterSettingsAffector(PooterSettingsAffectorType.invulnerable, 4.2f));
            }else if(powerType == PowerupType.health)
            {
                Pooter.HealDamage();
            }else if (powerType == PowerupType.timeSlow)
            {
                //Debug.Log("we activate here");
                p.AddSettingsAffector(new PooterSettingsAffector(PooterSettingsAffectorType.changeSpeed, 1.35f, 8f));
                Camera.main.GetComponent<MainScript>().ActivateSlow();
            }
            
            readyToDie = true;
            CreateFanOfParticles();
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
public enum PowerupType { speed, invulnerabl ,health,timeSlow}