using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMove : MonoBehaviour
{
    //Declare variable for AISpawner management
    private AISpawner m_AIManager;

    //Declare variables for moving nd turning
    //private bool m_isMoving = false;
    private bool m_hasTarget = false;
    private bool m_isTurning;

    //Variable for current waypoint
    private Vector3 m_wayPoint;
    private Vector3 m_lastWaypoint = new Vector3(0f, 0f, 0f);

    // Sets the animation speed
    private Animator m_animator;
    private float m_speed;
    //private float m_scale;

    private Collider m_collider;
    private RaycastHit m_hit;

    // Start is called before the first frame update
    void Start()
    {
        //Get AISpawner from Parent
        m_AIManager = transform.parent.GetComponentInParent<AISpawner>();
        m_animator = GetComponent<Animator>();

        SetUpNPC();
    }

    void SetUpNPC()
    {
        //Randomly Scale each NPC
        float m_scale = Random.Range(0f, 2f);
        transform.localScale += new Vector3(m_scale * 1.5f, m_scale, m_scale);
    }

    // Update is called once per frame
    void Update()
    {
        //Updates at every new waypoint found to move to
        if (!m_hasTarget)
        {
            m_hasTarget = CanFindTarget();
        }
        else
        {
            //Rotate NPC towards new waypoint
            RotateNPC(m_wayPoint, m_speed);
            //Moves NPC in straightline towards new point
            transform.position = Vector3.MoveTowards(transform.position, m_wayPoint, m_speed * Time.deltaTime);
        }
        //Reset Target if NPC reaches waypoint
        if (transform.position == m_wayPoint)
        {
            m_hasTarget = false;
        }

    }

    bool CanFindTarget(float start = 1f, float end = 7f)
    {
        m_wayPoint = m_AIManager.RandomWaypoint();

        //Avoids setting same waypoint twice
        if (m_lastWaypoint == m_wayPoint)
        {
            //Get new waypoint
            m_wayPoint = m_AIManager.RandomWaypoint();
            return false;
        }
        else
        {
            //Set waypoint to last waypoint
            m_lastWaypoint = m_wayPoint;
            //Get random speed for movement and animation
            m_speed = Random.Range(start, end);
            m_animator.speed = m_speed;
            //Bool sets to true to sshow new WP found
            return true;
        }
    }
    void RotateNPC(Vector3 waypoint, float currentSpeed)
    {
        //Gets random speed up for turning
        float TurnSpeed = currentSpeed * Random.Range(1f, 3f);

        //Get new direction for fish to look at 
        Vector3 LookAt = waypoint - transform.position;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(LookAt), TurnSpeed * Time.deltaTime);
    }
}
