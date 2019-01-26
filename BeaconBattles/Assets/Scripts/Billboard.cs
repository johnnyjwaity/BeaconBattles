using UnityEngine;
using System.Collections;

public class Billboard : MonoBehaviour
{

    Camera m_Camera;

    void Start()
    {
        m_Camera = Camera.main;
    }

    void Update()
    {
        //look at the camera so the text is always aligned
        transform.LookAt(transform.position + m_Camera.transform.rotation * Vector3.forward,
                         m_Camera.transform.rotation * Vector3.up);
    }
}