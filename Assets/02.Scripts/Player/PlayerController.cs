using UnityEngine;
using System.Collections;
using UnityStandardAssets.Utility;

public class PlayerController : MonoBehaviour {

    private Transform m_cachedTransform = null;
    private NetworkView m_networkView = null;

    private Vector3 m_curPosition = Vector3.zero;
    private Quaternion m_curRotation = Quaternion.identity;

    // Use this for initialization
    void Awake ()
    {
        m_cachedTransform = this.gameObject.GetComponent<Transform>();        
        m_networkView = this.gameObject.GetComponent<NetworkView>();

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
            return;

        Vector3 pos = m_cachedTransform.position;
        m_cachedTransform.position = Vector3.Lerp(pos, m_curPosition, Time.deltaTime * 5.0f);

        Quaternion rotation = m_cachedTransform.rotation;
        m_cachedTransform.rotation = Quaternion.Lerp(rotation, m_curRotation, Time.deltaTime * 5.0f);
    }

    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        if (stream.isWriting)
        {
            Vector3 pos = m_cachedTransform.position;
            Quaternion rotation = m_cachedTransform.rotation;

            stream.Serialize(ref pos);
            stream.Serialize(ref rotation);
        }
        else
        {
            Vector3 pos = Vector3.zero;
            Quaternion rotation = Quaternion.identity;

            stream.Serialize(ref pos);
            stream.Serialize(ref rotation);

            m_curPosition = pos;
            m_curRotation = rotation;
        }
    }
}
