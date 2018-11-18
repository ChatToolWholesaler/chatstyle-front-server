using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {

    public float m_MinDis = 8f;
    public float m_MaxDis = 24f;
    public float m_CurDis = 16f;
    public float m_ScrollSpeed;
    public float m_Angle = Mathf.PI / 6;
    public float m_AngleSpeed;
    public float m_MaxAngle;
    public float m_MinAngle;
    public bool is_Third;
    public float m_speed;  //move speed
    public float m_TurnSpeed;
    [HideInInspector] public Transform m_Targets;
     public bool m_AngleControl;
     public bool m_TurnControl;
     public bool m_MoveControl;

    private string m_MouseX;
    private string m_MouseY;
    private string m_MouseScrollWheel;
    private float m_MouseInputValue;
    private RaycastHit hit;

    public void EnableAngleControl()
    {
        m_AngleControl = true;
    }

    public void DisableAngleControl()
    {
        m_AngleControl = false;
    }

    public void EnableTurnControl()
    {
        m_TurnControl = true;
    }

    public void DisableTurnControl()
    {
        m_TurnControl = false;
    }

    public void DisableMoveControl()
    {
        m_MoveControl = false;
    }

    public void EnableMoveControl()
    {
        m_MoveControl = true;
    }

    private void Awake()
    {
        is_Third = true;
        //m_Targets = GameObject.Find("role").transform;
    }

    // Use this for initialization
    void Start()
    {
        m_MouseX = "Mouse X";
        m_MouseY = "Mouse Y";
        m_MouseScrollWheel = "Mouse ScrollWheel";
    }

    // Update is called once per frame
    void Update()
    {
        if (m_AngleControl)AngleControl();
        /*if (is_Third)
        {
            Vector3 tar = m_Targets.position - m_Targets.forward * Mathf.Cos(m_Angle) * m_CurDis + new Vector3(0.0f, Mathf.Sin(m_Angle) * m_CurDis, 0.0f);
            Vector3 direction = tar - m_Targets.position;
            int layermask = 1 << 9;//只检测与场景物体的碰撞
            if (Physics.Raycast(m_Targets.position, direction, out hit, m_CurDis, layermask))
            {
                tar = hit.point;
            }
            transform.position = tar;
            Vector3 dif = (m_Targets.position - transform.position);
            float factor = Mathf.Sqrt(dif.x * dif.x + dif.z * dif.z);
            Vector3 inc = new Vector3(dif.z * m_CurDis / factor / m_CurDis, 0f, -dif.x * m_CurDis / factor / m_CurDis);
            Vector3 upo = new Vector3(-(dif.y * dif.x) / factor / m_CurDis, factor / m_CurDis, -(dif.y * dif.z) / factor / m_CurDis);
            transform.LookAt(m_Targets.position + inc + upo, new Vector3(0f, 1f, 0f));
        }
        else
        {*/
        //transform.position = m_Targets.position;
        Vector3 forward = transform.forward;
        if (m_TurnControl)
        {
            forward = Turn();
            transform.LookAt(transform.position + forward * Mathf.Cos(m_Angle) - new Vector3(0.0f, forward.magnitude*Mathf.Sin(m_Angle), 0.0f), new Vector3(0f, 1f, 0f));
        }
        if (m_MoveControl)
        {
            Move();
        }
        
        //}
    }

    private Vector3 Turn()
    {
        m_MouseInputValue = Input.GetAxis(m_MouseX);
        float turn = -m_MouseInputValue * m_TurnSpeed * Time.deltaTime;
        turn = turn * Mathf.PI / 180f;
        Vector3 forward = new Vector3(transform.forward.x * Mathf.Cos(turn) - transform.forward.z * Mathf.Sin(turn), 0f, transform.forward.z * Mathf.Cos(turn) + transform.forward.x * Mathf.Sin(turn));
        return forward;
    }

    private void AngleControl()
    {
        m_MouseInputValue = -Input.GetAxis(m_MouseY);
        float turn = m_MouseInputValue * m_AngleSpeed * Time.deltaTime;
        m_Angle += turn;
        if (m_Angle > m_MaxAngle) m_Angle = m_MaxAngle;
        if (m_Angle < m_MinAngle) m_Angle = m_MinAngle;

        m_MouseInputValue = -Input.GetAxis(m_MouseScrollWheel);
        float scroll = m_MouseInputValue * m_ScrollSpeed * Time.deltaTime;
        m_CurDis += scroll;
        if (m_CurDis > m_MaxDis) m_CurDis = m_MaxDis;
        if (m_CurDis < m_MinDis) m_CurDis = m_MinDis;
    }

    private void Move()
    {
        /*if (Input.GetButton(m_forward))
        {
            Vector3 movement = transform.forward * m_speed * Time.deltaTime;
            m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
        }
        if (Input.GetButton(m_backforward))
        {
            Vector3 movement = -transform.forward * m_speed * Time.deltaTime;
            m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
        }
        if (Input.GetButton(m_left))
        {
            Vector3 movement = new Vector3(-transform.forward.z, 0f, transform.forward.x) * m_speed * Time.deltaTime;
            m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
        }
        if (Input.GetButton(m_right))
        {
            Vector3 movement = new Vector3(transform.forward.z, 0f, -transform.forward.x) * m_speed * Time.deltaTime;
            m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
        }*/
        float InputValuex = Input.GetAxis("Vertical"); 
         Vector3 movement = transform.forward * InputValuex * m_speed * Time.deltaTime;
        transform.position += movement;
        float InputValuey = Input.GetAxis("Horizontal");
        movement =  transform.right *(InputValuey) * m_speed * Time.deltaTime;
        //movement = new Vector3(-transform.forward.z, 0f, transform.forward.x) * (-InputValuey) * m_speed * Time.deltaTime;
        transform.position += movement;
        /*if (((InputValuex != 0 || InputValuey != 0) && m_Rigidbody.velocity.y <= 0.01f && m_Rigidbody.velocity.y >= -0.01f) && !step_effect.GetComponent<AudioSource>().isPlaying)//若水平方向有速度且不在空中则播放走路音效
        {
            step_effect.GetComponent<AudioSource>().Play();
        }
        else if(((InputValuex == 0 && InputValuey == 0) || (m_Rigidbody.velocity.y > 0.01f || m_Rigidbody.velocity.y < -0.01f)) && step_effect.GetComponent<AudioSource>().isPlaying)//若水平方向无速度或者在空中则停止播放走路音效
        {
            step_effect.GetComponent<AudioSource>().Stop();
        }*/

    }
}
