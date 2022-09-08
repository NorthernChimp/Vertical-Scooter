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
    void Start()
    {
        
    }
    public void SetupPowerup(PowerupType typeToBecome)
    {
        powerType = typeToBecome;
        if(typeToBecome == PowerupType.speed) { GetComponent<SpriteRenderer>().sprite = speedSprite; }else if(typeToBecome == PowerupType.invulnerabl) { GetComponent<SpriteRenderer>().sprite = invulnerableSprite; }
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
            if (powerType == PowerupType.speed)
            {
                p.AddSettingsAffector(new PooterSettingsAffector(PooterSettingsAffectorType.changeSpeed, 15f, 4.2f));
                p.AddSettingsAffector(new PooterSettingsAffector(PooterSettingsAffectorType.changeAccel, 45f, 4.2f));
            }else if(powerType == PowerupType.invulnerabl)
            {
                p.AddSettingsAffector(new PooterSettingsAffector(PooterSettingsAffectorType.invulnerable, 4.2f));
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
public enum PowerupType { speed, invulnerabl }