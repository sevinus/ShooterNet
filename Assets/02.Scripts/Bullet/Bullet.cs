using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

    private float m_lifeTime;
    private Rigidbody m_cachedRigidbody;

	// Use this for initialization
	void Start ()
    {
        m_lifeTime = 0.5f;

        m_cachedRigidbody = this.gameObject.GetComponent<Rigidbody>();
        m_cachedRigidbody.velocity = this.gameObject.transform.forward * 10.0f;

        Destroy(this.gameObject, m_lifeTime);
    }
	
	// Update is called once per frame
	void Update ()
    {	
	}
}
