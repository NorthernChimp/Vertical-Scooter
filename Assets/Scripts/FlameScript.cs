using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.tag == "Player")
        {
            Pooter p = collision.GetComponent<Pooter>();
            p.DisableJetPack();
            Vector3 direct = (transform.position - p.transform.position).normalized;
            p.BounceOff(direct);
            p.AddVelocity(direct * Pooter.brickLength * -14.20f);
            //p.extraVelocity += direct * Pooter.brickLength * -14.20f;
            MainScript.CreateSparkleExplosion(collision.transform.position);
            //this.enabled = false;
            GetComponent<Collider2D>().enabled = false;
            GetComponent<SpriteRenderer>().enabled = false;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
