using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class timg : MonoBehaviour {

    private RectTransform recttransform;
    [HideInInspector] public Transform m_Targets;
    // Use this for initialization
    void Start () {
        recttransform = GetComponent<RectTransform>();
        m_Targets = GameObject.Find("Main Camera").transform;
    }
	
	// Update is called once per frame
	void Update () {
        float turn = 0f;
        if (m_Targets.forward.x >= 0) turn = Mathf.Atan(m_Targets.forward.z/ m_Targets.forward.x);
        if (m_Targets.forward.x < 0) turn = Mathf.PI + Mathf.Atan(m_Targets.forward.z / m_Targets.forward.x);
        turn *= (180f / Mathf.PI);
        Quaternion turnRotation = Quaternion.Euler(0f, 0f, turn-90.0f);
        recttransform.rotation = turnRotation;
        //recttransform.LookAt(new Vector3(recttransform.position.x + m_Targets.forward.x, recttransform.position.y - m_Targets.forward.z,0f));
    }
}
