using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car_Player : MonoBehaviour
{
    public Rigidbody rb;

    public float maxForwardAccel = 18, maxReverseAccel = 9, turnStrength = 30, gravityForce = 10, dragOnGround = 3, delayAmount = 0.3f, maxSpeed;

    [HideInInspector] public float forwardAccelBuildUp, reverseAccelBuildUp;

    private float speedInput, turnInput, accelDelay, decelDelay, boostDelay, driftInput;

    private bool grounded;

    public bool isOffTrack = false, isBoosted = false;
    public LayerMask whatIsGround;
    public float groundRayLength = 5;
    public Transform groundRayPoint;

    public Transform leftFrontWheel, rightFrontWheel;
    public float maxWheelTurn = 25;

    //public Car_Player_Collision playerCollision;

    //public GameManager gameManager;

    
    // Start is called before the first frame update
    void Start()
    {
        //rb.transform.parent = null;
    }

    // Update is called once per frame

    void Update()
    {
        VerticalInput();
        TurnInput();

        //Debug.Log(forwardAccelBuildUp);
    }

    void FixedUpdate()
    {
        GroundCheck();
        ApplyForce();

        //Debug.Log(maxSpeed);
    }





    void VerticalInput()
    {
        //Debug.Log(isBoosted);

        speedInput = 0f;

        accelDelay += Time.deltaTime;
        decelDelay += Time.deltaTime;

        if (!isOffTrack)
        {
            if (isBoosted)
            {
                boostDelay += Time.deltaTime;

                if (boostDelay >= 3)
                {

                    forwardAccelBuildUp = maxForwardAccel;

                    boostDelay = 0;

                    transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, driftInput * (forwardAccelBuildUp / 10) * (turnStrength * 2) * Time.deltaTime * Input.GetAxis("Vertical"), 0f));


                    isBoosted = false;
                }
                else
                {
                    rb.AddForce(transform.forward * 25000);
                    transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, turnInput * 1 * turnStrength * Time.deltaTime * Input.GetAxis("Vertical"), 0f));
                }

            }
            else
            {

            }


        }
        else
        {
            //maxForwardAccel = maxSpeed;
            //maxReverseAccel = 2;

            //isBoosted = false;
        }

        if (!isBoosted)
        {
            if (Input.GetAxis("Vertical") > 0)
            {

                if (reverseAccelBuildUp <= 0)
                {
                    if (accelDelay >= delayAmount && forwardAccelBuildUp < maxForwardAccel /* + playerCollision.coinCount*/ && forwardAccelBuildUp != maxForwardAccel + 10)
                    {
                        forwardAccelBuildUp++;

                        accelDelay = 0;
                    }

                    speedInput = Input.GetAxis("Vertical") * forwardAccelBuildUp * 1000f;
                }
                else
                {
                    if (decelDelay >= delayAmount && reverseAccelBuildUp > 0)
                    {
                        reverseAccelBuildUp--;
                    }
                }
            }
            else if (Input.GetAxis("Vertical") < 0)
            {

                if (forwardAccelBuildUp <= 0)
                {
                    if (accelDelay >= delayAmount && reverseAccelBuildUp < maxReverseAccel/* + playerCollision.coinCount*/)
                    {
                        reverseAccelBuildUp++;

                        accelDelay = 0;
                    }

                    speedInput = Input.GetAxis("Vertical") * reverseAccelBuildUp * 1000f;
                }
                else
                {
                    if (decelDelay >= delayAmount && forwardAccelBuildUp > 0)
                    {
                        forwardAccelBuildUp--;
                    }


                }

            }
            else
            {

                if (decelDelay >= delayAmount && forwardAccelBuildUp > 0)
                {
                    forwardAccelBuildUp--;
                    reverseAccelBuildUp--;

                    decelDelay = 0;
                }

                
            }

        }

        else
        {
            forwardAccelBuildUp--;
            reverseAccelBuildUp--;

            decelDelay = 0;
        }

        if (Input.GetAxis("Vertical") == 0)
        {
            if(accelDelay < delayAmount)
            {
                forwardAccelBuildUp--;
                //Debug.Log("Input.GetAxis('vertical') = 0");
            }
            

        }

        if (forwardAccelBuildUp <= 0)
        {
            forwardAccelBuildUp = 0;
        }
        else if (reverseAccelBuildUp <= 0)
        {
            reverseAccelBuildUp = 0;
        }



    }






    void TurnInput()
    {


       turnInput = Input.GetAxis("Horizontal");

        if (grounded)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift) && Input.GetAxis("Vertical") > 0)
            {
                
                if (turnInput > 0)
                {
                    driftInput = 1;
                }
                else if (turnInput < 0)
                {
                    driftInput = -1;
                }    
            }
            else if (Input.GetKey(KeyCode.LeftShift) && Input.GetAxis("Vertical") > 0)
            {

                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, driftInput * (forwardAccelBuildUp / 10) * (turnStrength * 2) * Time.deltaTime * Input.GetAxis("Vertical"), 0f));


            }
            else
            {

                if (forwardAccelBuildUp > 0)
                {
                    transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, turnInput * (forwardAccelBuildUp / 10) * turnStrength * Time.deltaTime * Input.GetAxis("Vertical"), 0f));
                }
                else if (reverseAccelBuildUp > 0)
                {
                    transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, turnInput * turnStrength * Time.deltaTime * Input.GetAxis("Vertical"), 0f));
                }

                leftFrontWheel.localRotation = Quaternion.Euler(leftFrontWheel.localRotation.eulerAngles.x, (turnInput * maxWheelTurn) - 180, leftFrontWheel.localRotation.eulerAngles.z);
                rightFrontWheel.localRotation = Quaternion.Euler(rightFrontWheel.localRotation.eulerAngles.x, turnInput * maxWheelTurn, rightFrontWheel.localRotation.eulerAngles.z);
            }
        }

        

        transform.position = rb.transform.position;


    }





    void GroundCheck()
    {


        
        RaycastHit hit;

        if (Physics.Raycast(groundRayPoint.position, -transform.up, out hit, groundRayLength, whatIsGround))
        {
            grounded = true;

            Quaternion smoothtransition = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;

            transform.rotation = Quaternion.Lerp(transform.rotation, smoothtransition, Time.deltaTime * 10);
        }
        else
        {
            grounded = false;
        }

    }





    void ApplyForce()
    {


        if (grounded)
        {
            rb.drag = dragOnGround;

            if (Mathf.Abs(speedInput) > 0)
            {
                rb.AddForce(transform.forward * speedInput);
            }
        }
        else
        {
            rb.drag = 0.1f;

            rb.AddForce(Vector3.up * -gravityForce * 500f);
        }


    }



    

}
