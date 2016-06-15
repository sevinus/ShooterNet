using UnityEngine;
using System.Collections;

public class MoveController : MonoBehaviour {

    private Transform m_cachedTransform = null;
    private CharacterController m_controller = null;
    private NetworkView m_networkView = null;

    private float m_xAxis = 0.0f;
    private float m_yAxis = 0.0f;

    public float m_moveSpeed = 5.0f;
    public float m_rotateSpeed = 50.0f;

	// Use this for initialization
	void Start ()
    {
        m_cachedTransform = this.gameObject.GetComponent<Transform>();
        m_controller = this.gameObject.GetComponent<CharacterController>();
        m_networkView = this.gameObject.GetComponent<NetworkView>();

        this.enabled = m_networkView.isMine;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (m_cachedTransform == null ||
            m_controller == null)
            return;

        m_xAxis = Input.GetAxis("Horizontal");
        m_yAxis = Input.GetAxis("Vertical");
        float mouseX = Input.GetAxis("Mouse X");

        float rotateAngle = mouseX * m_rotateSpeed;
        Vector3 rorate = Vector3.up * rotateAngle;
        m_cachedTransform.Rotate(rorate * Time.deltaTime, Space.Self);

        Vector3 moveDir = (m_cachedTransform.right * m_xAxis) + (m_cachedTransform.forward * m_yAxis);
        moveDir.y -= 20.0f * Time.deltaTime;
        m_controller.Move(moveDir * m_moveSpeed * Time.deltaTime);
	}
}
