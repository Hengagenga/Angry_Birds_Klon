using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    public float wind = 0;
    public bool BadWindOnTrajectory = true;
    public bool addforceWind = true;

 int segmentCount = 20;
    public GameObject block;
    public GameObject Ball;
    public GameObject Launchbegin;
    public Collider2D StartLaunchArea;          // In diesem Gebiet wird der Launch gestartet
    public float FireSpeed = 5.0f;

    public SlingshotState slingshotState;

    public enum SlingshotState
    {
        Idle,
        UserPulling,
        BirdFlying
    }


    private GameObject currentBall;


    public LineRenderer TrajectoryLineRenderer;


    void Start()
    {


        slingshotState = SlingshotState.Idle;

    }

    void SetTrajectoryLineRenderesActive(bool active)
    {
        TrajectoryLineRenderer.enabled = active;
    }

    void DisplayTrajectoryLineRenderer2(float distance)
    {
        SetTrajectoryLineRenderesActive(true);
        Vector3 v2 = Launchbegin.transform.position - currentBall.transform.position;
       
        Vector2[] segments = new Vector2[segmentCount];


        segments[0] = currentBall.transform.position;


        Vector2 segVelocity = new Vector2(v2.x, v2.y) * FireSpeed * distance;

        for (int i = 1; i < segmentCount; i++)
        {

            float time2 = i * Time.fixedDeltaTime * 5;
            segments[i] = segments[0] + segVelocity * time2 + 0.5f * Physics2D.gravity * Mathf.Pow(time2, 2);

            
        }

        for (int i = segmentCount-2; i >=0; i--)
        {
                
         RaycastHit2D hit = Physics2D.Linecast(segments[i], segments[i + 1]);
            Debug.DrawLine(segments[i], segments[i+1]);
            if (hit) {
                if (hit.collider.gameObject.tag == "Ground")
                {
                    block.transform.position = hit.point;

                }
            }
        }

        TrajectoryLineRenderer.SetVertexCount(segmentCount);
        for (int i = 0; i < segmentCount; i++)
        {

           

            TrajectoryLineRenderer.SetPosition(i, segments[i]);



        }
    }
    private void Throw(float distance)
    {

        Vector3 velocity = Launchbegin.transform.position - currentBall.transform.position;

        currentBall.GetComponent<Rigidbody2D>().velocity = new Vector2(velocity.x, velocity.y) * FireSpeed * distance;
        

    }

    void Update()
    {
        
        switch (slingshotState)
        {
            case SlingshotState.Idle:


                if (Input.GetMouseButtonDown(0))
                {

                    Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    if (StartLaunchArea.OverlapPoint(worldPos))
                    {
                        currentBall = GameObject.Instantiate(Ball);
                        currentBall.transform.rotation = Launchbegin.transform.rotation;
                        currentBall.SetActive(true);


                        slingshotState = SlingshotState.UserPulling;
                    }

                    Vector3 location = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                    if (Ball.GetComponent<CircleCollider2D>() == Physics2D.OverlapPoint(location))
                    {
                        slingshotState = SlingshotState.UserPulling;
                    }
                }
                break;
            case SlingshotState.UserPulling:

                if (Input.GetMouseButton(0))
                {
                   
                    Vector3 location = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    location.z = 0;
                    if (Vector3.Distance(location, Launchbegin.transform.position) > 2f)
                    {
                        var maxPosition = (location - Launchbegin.transform.position).normalized * 2f + Launchbegin.transform.position;
                        currentBall.transform.position = maxPosition;
                    }
                    else
                    {
                        currentBall.transform.position = location;
                    }
                    float distance = Vector3.Distance(Launchbegin.transform.position, currentBall.transform.position);
                    if (BadWindOnTrajectory) {
                        distance = distance - wind;
                    }
                    DisplayTrajectoryLineRenderer2(distance);
                }
                else 
                {
                    currentBall.transform.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                    SetTrajectoryLineRenderesActive(false);

                    float distance = Vector3.Distance(Launchbegin.transform.position, currentBall.transform.position);
                    distance = distance - wind;
                    slingshotState = SlingshotState.BirdFlying;
                      Throw(distance);
                    

                }
                break;
            case SlingshotState.BirdFlying:
                if (addforceWind)
                {
                    currentBall.GetComponent<Rigidbody2D>().AddForce(new Vector2(-wind, 0));
                } else { slingshotState = SlingshotState.Idle; }
                if (currentBall.GetComponent<Ball>().swotch) {
                    slingshotState = SlingshotState.Idle;
                }

                break;
            default:
                break;
        }

    }

}





