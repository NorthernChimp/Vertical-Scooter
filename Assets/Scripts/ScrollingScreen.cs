using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingScreen : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject wall;
    public GameObject roundWall;
    public GameObject badguy;
    public GameObject homingMissile;
    public Transform topHalfPosition;
    float heightOfSecondHalf = 0f;
    void Start()
    {
        
    }
    public Wall CreateWall(float width, float height, float rotateSpeed,Vector3 pos)
    {
        pos.z = -2f;
        Transform t = Instantiate(wall, pos, Quaternion.identity).transform;
        t.localScale = new Vector3(Pooter.basicScale.x * width, Pooter.basicScale.y * height,1f);
        Wall w = t.GetComponent<Wall>();
        return w;
    }
    public RoundWall CreateRoundWall(float width, float height, float rotateSpeed, Vector3 pos)
    {
        pos.z = -2f;
        Transform t = Instantiate(roundWall, pos, Quaternion.identity).transform;
        t.localScale = new Vector3(Pooter.basicScale.x * width, Pooter.basicScale.y * height, 1f);
        RoundWall w = t.GetComponent<RoundWall>();
        return w;
    }
    public BadGuy CreateEnemyAt(GameObject prefab, Vector3 pos,float scaleMultiplier)
    {
        //GameObject prefab = badguy;
        //if(Random.value > 0.5f) { prefab = homingMissile; }
        
        Transform t = Instantiate(prefab, pos, Quaternion.identity).transform;
        //t.localScale = Pooter.basicScale * scaleMultiplier;
        //t.localScale = Pooter.basicScale;
        //t.parent = transform;
        BadGuy b = t.GetComponent<BadGuy>();
        b.SetupBadGuy(scaleMultiplier);
        return b;
    }
    public BadGuy CreateEnemyAt(GameObject prefab, float scaleMultiplier,float xPos,float yModifier)
    {
        //GameObject prefab = badguy;
        //if(Random.value > 0.5f) { prefab = homingMissile; }
        Vector3 pos = GetPointAboveScreen();pos.x = xPos;
        pos.y += yModifier;
        Transform t = Instantiate(prefab, pos, Quaternion.identity).transform;
        //t.localScale = Pooter.basicScale * scaleMultiplier;
        //t.localScale = Pooter.basicScale;
        //t.parent = transform;
        BadGuy b = t.GetComponent<BadGuy>();
        b.SetupBadGuy(scaleMultiplier);
        return b;
    }
    public BadGuy CreateEnemyAt(GameObject prefab, float scaleMultiplier, float xPos)
    {
        //GameObject prefab = badguy;
        //if(Random.value > 0.5f) { prefab = homingMissile; }
        Vector3 pos = GetPointAboveScreen(); pos.x = xPos;
        Transform t = Instantiate(prefab, pos, Quaternion.identity).transform;
        //t.localScale = Pooter.basicScale * scaleMultiplier;
        //t.localScale = Pooter.basicScale;
        //t.parent = transform;
        BadGuy b = t.GetComponent<BadGuy>();
        b.SetupBadGuy(scaleMultiplier);
        return b;
    }
    public BadGuy CreateEnemy(GameObject prefab, float scaleMultiplier)
    {
        //GameObject prefab = badguy;
        //if(Random.value > 0.5f) { prefab = homingMissile; }
        Transform t = Instantiate(prefab, GetPointAboveScreen(), Quaternion.identity).transform;
        //t.localScale = Pooter.basicScale * scaleMultiplier;
        //t.localScale = Pooter.basicScale;
        //t.parent = transform;
        BadGuy b = t.GetComponent<BadGuy>();
        b.SetupBadGuy(scaleMultiplier);
        return b;
    }
    public void UpdateScrollingScreen(float timePassed)
    {
        heightOfSecondHalf = topHalfPosition.localPosition.x;
        float currentScrollSpeed = 1f; //eventually increase it as difficulty increases
        float multiplier = 0.87f;
        float distanceToScroll = currentScrollSpeed * timePassed* multiplier;
        transform.Translate(Vector3.down * distanceToScroll,Space.World);
        float tooFar = heightOfSecondHalf * 1f;
        float yDiff = Camera.main.transform.position.y - transform.position.y;
        //Debug.Log(yDiff);
        if(yDiff > tooFar) { Vector3 newPos = transform.TransformPoint(topHalfPosition.localPosition); transform.position = newPos; }
    }
    public static float GetYPosAboveScreen()
    {
        float minDist = Screen.height * 0.0055f;
        float maxDist = minDist + Pooter.brickLength;
        //float maxDist = Screen.height * 0.0065f;
        float minXVal = Screen.width * 0.00485f;
        float randomPoint = Random.Range(minDist, maxDist);
        return Camera.main.transform.position.y + randomPoint;
    }
    Vector3 GetPointAboveScreen()
    {
        float minDist = Screen.height * 0.0055f;
        float maxDist = minDist + Pooter.brickLength;
        //float maxDist = Screen.height * 0.0065f;
        float minXVal = Screen.width * 0.00485f;
        float randomPoint = Random.Range(minDist, maxDist);
        float randomXVal = Random.Range(-minXVal, minXVal);
        Vector3 pos = new Vector3(randomXVal, Camera.main.transform.position.y + randomPoint, 0f);
        return pos;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
