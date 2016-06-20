using UnityEngine;
using System.Collections;
using UnityStandardAssets.Utility;

public class PlayerController : MonoBehaviour {

    public enum AnimState
    {
        idle = 0,
        runForward,
        runBackward,
        runLeft,
        runRight,
    }

    private Transform m_cachedTransform = null;
    private NetworkView m_networkView = null;

    private Vector3 m_curPosition = Vector3.zero;
    private Quaternion m_curRotation = Quaternion.identity;

    private CharacterController m_charController = null;
    private Animation m_animation = null;

    public GameObject m_bullet = null;
    public Transform m_firePos = null;

    public AnimState m_animState = AnimState.idle;
    public AnimationClip[] m_animClips;

    // Use this for initialization
    void Awake ()
    {
        m_cachedTransform = this.gameObject.GetComponent<Transform>();        
        m_networkView = this.gameObject.GetComponent<NetworkView>();
        m_charController = this.gameObject.GetComponent<CharacterController>();
        m_animation = this.gameObject.GetComponentInChildren<Animation>();

        m_networkView.observed = this;

        if (m_networkView.isMine)
        {
            SmoothFollow follow = Camera.main.GetComponent<SmoothFollow>();
            follow.target = m_cachedTransform;
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (m_networkView.isMine == true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Fire();

                m_networkView.RPC("Fire", RPCMode.Others);
            }

            Vector3 localVelocity = m_cachedTransform.InverseTransformDirection(m_charController.velocity);
            Vector3 fowardDir = new Vector3(0.0f, 0.0f, localVelocity.z);
            Vector3 rightDir = new Vector3(localVelocity.x, 0.0f, 0.0f);

            if (fowardDir.z > 0.1f)
            {
                m_animState = AnimState.runForward;
            }
            else if (fowardDir.z < -0.1f)
            {
                m_animState = AnimState.runBackward;
            }
            else if (rightDir.x < -0.1f)
            {
                m_animState = AnimState.runLeft;
            }
            else if (rightDir.x > 0.1f)
            {
                m_animState = AnimState.runRight;
            }
            else
            {
                m_animState = AnimState.idle;
            }

            AnimationClip clip = m_animClips[(int)m_animState];
            m_animation.CrossFade(clip.name, 0.2f);
        }
        else
        {
            Vector3 pos = m_cachedTransform.position;
            m_cachedTransform.position = Vector3.Lerp(pos, m_curPosition, Time.deltaTime * 5.0f);

            Quaternion rotation = m_cachedTransform.rotation;
            m_cachedTransform.rotation = Quaternion.Lerp(rotation, m_curRotation, Time.deltaTime * 5.0f);

            AnimationClip clip = m_animClips[(int)m_animState];
            m_animation.CrossFade(clip.name, 0.2f);
        }
    }

    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        if (stream.isWriting)
        {
            Vector3 pos = m_cachedTransform.position;
            Quaternion rotation = m_cachedTransform.rotation;
            int animState = (int)m_animState;

            stream.Serialize(ref pos);
            stream.Serialize(ref rotation);
            stream.Serialize(ref animState);
        }
        else
        {
            Vector3 pos = Vector3.zero;
            Quaternion rotation = Quaternion.identity;
            int animState = 0;

            stream.Serialize(ref pos);
            stream.Serialize(ref rotation);
            stream.Serialize(ref animState);

            m_curPosition = pos;
            m_curRotation = rotation;
            m_animState = (AnimState)animState;
        }
    }

    [RPC]
    void Fire()
    {
        if (m_bullet == null ||
            m_firePos == null)
            return;

        GameObject.Instantiate(m_bullet, m_firePos.position, m_firePos.rotation);      
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Bullet")
        {
            Destroy(collider.gameObject);
        }
    }
}
