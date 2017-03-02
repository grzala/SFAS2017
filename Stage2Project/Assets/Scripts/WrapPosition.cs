using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* I Don't Use this, However I left this in a project */
public class WrapPosition : MonoBehaviour
{


	void Awake()
    {
		
	}
	
	void Update ()
    {
        Vector3 position = transform.position;

        if (position.x < Arena.Width * -0.5f)
        {
            position.x += Arena.Width;
        }
        else if (position.x > Arena.Width * 0.5f)
        {
            position.x -= Arena.Width;
        }

        if (position.z < Arena.Height * -0.5f)
        {
            position.z += Arena.Height;
        }
        else if (position.z > Arena.Height * 0.5f)
        {
            position.z -= Arena.Height;
        }

        transform.position = position;
    }
}
