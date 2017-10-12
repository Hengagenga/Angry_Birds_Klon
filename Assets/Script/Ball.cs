using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour {
    public bool swotch = false;
    public bool countdown = false;
    public float life = 2f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Debug.Log(swotch);
		if (countdown)
        {
            life -= Time.deltaTime;
        }
        if (life <= 0)
        {
            Destroy(gameObject);
        }
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            swotch = true;
            countdown = true;
        }
    }
}
