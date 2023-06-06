using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameWrangler : MonoBehaviour
{
    public GameObject flamePrefab;
    WranglerType typeOfWrangler;
    float rotationSpeed = 0f;
    float moveSpeed = 0f;
    List<Transform> flames;
    List<float> flameBaseAngles;
    List<float> flameDistFromCenter;
    float totalTimePassed = 0f;
    float diameter = 0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void KillChildren()
    {
        while(flames.Count > 0)
        {
            GameObject g = flames[0].gameObject;
            flames.RemoveAt(0);
            Destroy(g);
        }
    }
    public void SetupWrangler(float scaleMultiplier,float availableHeight,int currentObstacleLevel) 
    {
        flameDistFromCenter = new List<float>() { 0f};
        flameBaseAngles = new List<float>() { 0f};
        transform.localScale = Pooter.basicScale;
        flames = new List<Transform>() { flamePrefab.transform };
        float rando = Random.value;
        flamePrefab.transform.SetParent(null);
        //rando = 0.35f;
        if(rando > 0.67f)
        {
            typeOfWrangler = WranglerType.circular;
            int numberOfArms = currentObstacleLevel; if(numberOfArms > 4) { numberOfArms = 4; }
            rotationSpeed = (Mathf.PI *  1.5f) - ((numberOfArms - 1) * Mathf.PI * 0.35f);
            float angleDiff = (Mathf.PI * 2f) / (float)numberOfArms;
            for (int i = 0; i < numberOfArms; i++)
            {
                float currentAngle = i * angleDiff;
                Vector3 direct = new Vector3(Mathf.Sin(currentAngle), Mathf.Cos(currentAngle), 0f);
                float distApart = availableHeight * 0.125f;
                for(int o = 1;o < 4; o++)
                {
                    Vector3 offset = o * direct * distApart;
                    Vector3 pos = transform.position + offset;
                    Transform t = Instantiate(flamePrefab, pos, Quaternion.identity).transform;
                    flames.Add(t);
                    flameBaseAngles.Add(currentAngle);
                    flameDistFromCenter.Add(offset.magnitude);
                }
            }

        }else if(rando > 0.33f)
        {
            typeOfWrangler = WranglerType.horizontalMoving;
            int numberOfFlames = (int)(availableHeight / (Pooter.brickLength * 4f));
            float leftOfScreen = Screen.width * -0.0055f;
            float distBetween = Pooter.brickLength * 4f;
            float topFlameYPos = transform.position.y + ((float)numberOfFlames * 0.5f * distBetween);
            Vector3 origin = new Vector3(leftOfScreen, topFlameYPos, 0f);
            flames[0].Translate(Vector3.down * distBetween * 1f);
            flames[0].position = origin;
            float moveMultiplier = (float)currentObstacleLevel;if(moveMultiplier > 4f) { moveMultiplier = 4f; }
            moveSpeed = (Screen.width * 0.0025f) + (Screen.width * 0.00025f * moveMultiplier); 
            if(Random.value > 0.5f) { moveSpeed *= -1f; }
            for(int i = 1; i < numberOfFlames; i++)
            {
                Vector3 pos = origin + (new Vector3(i * -0.55f * distBetween,i * -1f * distBetween,0f));
                Transform t = Instantiate(flamePrefab, pos, Quaternion.identity).transform;
                flames.Add(t);
            }
        }
        else
        {
            typeOfWrangler = WranglerType.pathFollowing;
            int numberOfFlames = (int)(availableHeight / (Pooter.brickLength * 4f));
            //Debug.Log(numberOfFlames);
            float totalDist = Mathf.PI * 4f;//two full rotations
            float distBetweenFlames = totalDist / (float)numberOfFlames;
            diameter = Screen.width * 0.00225f;
            float moveSpeedMultiplier = (float)currentObstacleLevel; if(moveSpeedMultiplier > 1.75f) { moveSpeedMultiplier = 1.75f; }
            moveSpeed = Mathf.PI * moveSpeedMultiplier;
            flameDistFromCenter.Add(0f);
            for(int i = 1; i < numberOfFlames; i++)
            {
                float currentPos = i * distBetweenFlames;
                float currentDist = i * distBetweenFlames;
                float circlePosModifier = Mathf.Sign((Mathf.PI * 2f) - currentDist);
                Vector3 center = transform.position + (Vector3.right * diameter * circlePosModifier);
                Vector3 directFromCenter = new Vector3(Mathf.Cos(currentDist), Mathf.Sin(currentDist), 0f) * diameter;
                Vector3 pos = center + directFromCenter;
                Transform t = Instantiate(flamePrefab, pos, Quaternion.identity).transform;
                flames.Add(t);
                flameDistFromCenter.Add(currentDist);
            }
        }
    }
    public bool UpdateWrangler(float timePassed)
    {
        totalTimePassed += timePassed;
        
        
        switch (typeOfWrangler)
        {
            case WranglerType.circular:
                float angleChange = rotationSpeed * timePassed;
                Vector3 pos = transform.position;
                pos.x = Mathf.Sin(totalTimePassed * Mathf.PI * 0.5f) * Pooter.brickLength * (Screen.width * 0.003f);
                transform.position = pos;
                for (int i = 0; i < flames.Count;i++)
                {
                    Transform t = flames[i];
                    float baseAngle = flameBaseAngles[i];
                    float currentAngle = baseAngle + (totalTimePassed * rotationSpeed);
                    float baseDist = flameDistFromCenter[i];
                    Vector3 directTo = t.position - transform.position;directTo.z = 0f;
                    Vector3 currentDirectFromCenter = new Vector3(Mathf.Cos(currentAngle), Mathf.Sin(currentAngle), 0f);
                    //Vector3 newdirectTo = Quaternion.Euler(0f, 0f, angleChange) * directTo;
                    Vector3 newdirectTo = currentDirectFromCenter * baseDist;
                    t.position = transform.position + newdirectTo;
                }
                break;
            case WranglerType.horizontalMoving:
                float moveDist = timePassed * moveSpeed;
                
                for (int i = 0; i < flames.Count; i++)
                {
                    Transform t = flames[i];
                    t.Translate(Vector3.right * moveDist);
                    float xOffset = Mathf.Sin((totalTimePassed + (i * Mathf.PI * 0.025f))* Mathf.PI) * Pooter.brickLength * 0.15f;
                    moveDist += xOffset;
                    Vector3 posit = t.position;
                    if (Mathf.Abs(posit.x) > Screen.width * 0.0055f && Mathf.Sign(moveSpeed) == Mathf.Sign(posit.x)) { posit.x *= -1f;t.position = posit; }
                }
                    break;
            case WranglerType.pathFollowing:
                float pathDist = timePassed * moveSpeed;
                for (int i = 0; i < flames.Count; i++)
                {
                    Transform t = flames[i];
                    flameDistFromCenter[i] += pathDist; if (flameDistFromCenter[i] > Mathf.PI * 4f) { flameDistFromCenter[i] -= Mathf.PI * 4f; }
                    float currentPath = flameDistFromCenter[i];
                    float circlePosModifier = Mathf.Sign(currentPath - (Mathf.PI * 2f));
                    Vector3 center = transform.position + (Vector3.right * diameter * circlePosModifier);
                    Vector3 directFromCenter = new Vector3(Mathf.Cos(currentPath) , Mathf.Sin(currentPath), 0f) * diameter;
                    if(circlePosModifier > 0f) { directFromCenter.x *= -1f; }
                    t.position = center + directFromCenter;
                }
                    break;
        }
        float yDiff = Camera.main.transform.position.y - transform.position.y;
        bool killWrangler = (yDiff > Screen.height * 0.0065f);
        return killWrangler;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
public enum WranglerType { circular,horizontalMoving,pathFollowing}