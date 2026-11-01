using System.Collections;
using System.Threading;
using UnityEngine;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

public class TargetBehavior : MonoBehaviour
{
    public float rotationSpeed = 0f;
    public float accelerationSpeed = 300f;
    public float decelerationSpeed = 450f;
    public float maxSpeed = 1500f;
    public float accelerationTime = 3f;
    public float decelerationTime = 5f;
    public bool slowingDown = false;

    void Start()
    {
        InvokeRepeating("Spinning", .5f, 2f);

        Invoke("Decelerate", 8f);
    }



    // Update is called once per frame
    void Update()
    {

        transform.Rotate(rotationSpeed * Time.deltaTime, 0, 0);


    }

    //transform.Rotate(0, 0, 15 * Time.deltaTime);
   void Spinning() 
    {
        rotationSpeed += accelerationSpeed;

        if(rotationSpeed >= maxSpeed)
        {
            rotationSpeed = maxSpeed;
            CancelInvoke("Spinning");
        }
    }

    void slowDownSpin()
    {
        CancelInvoke("Spiining");
        InvokeRepeating("Decelerate", 0.1f, 1f);
           
    }

    void Decelerate()
    {
        rotationSpeed -= decelerationSpeed;

        if (rotationSpeed <= 0f)
        {
            rotationSpeed = 0f;
            CancelInvoke("Decelerate");
        }
    }







}