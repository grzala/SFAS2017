using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/* I did not edit the code in case I want to implement new features that need it
 * However, I only use typeTwo prefab and only yhe attract magnet type */

[RequireComponent(typeof(Rigidbody))]
public class MagnetizedByPlayer : NetworkBehaviour
{
    public enum Type { Attract, Repel }

    [SerializeField]
    private float RepelForce = 1000.0f;

    [SerializeField]
    private float MinimumDistance = 1.0f;

    [SerializeField]
    private Type MagnetizeType = Type.Repel;

    private Player mPlayer;
    private Rigidbody mBody;

    void Awake()
    {
        mPlayer = FindObjectOfType<Player>();
        mBody = GetComponent<Rigidbody>();
    }

	void Update()
    {

	}


    public void UpdateMagnet(List<Player> players)
	{
		float distance = float.MaxValue;
		Player closestPlayer = null;

		foreach (Player p in players) 
		{
			float temp = Vector3.Distance(p.transform.position, transform.position);
			if (temp < distance)
			{
				closestPlayer = p;
				distance = temp;
			}
		}

		mPlayer = closestPlayer;

		if (mPlayer != null)
		{
			Vector3 difference = MagnetizeType == Type.Repel ? transform.position - mPlayer.transform.position : mPlayer.transform.position - transform.position;
			if( difference.magnitude <= MinimumDistance )
			{
				mBody.AddForce(difference * RepelForce * Time.deltaTime);
			}
		}	
	}

    public Player GetClosestPlayer(List<Player> plist)
    {
        float distance = float.MaxValue;
        Player closestPlayer = null;

        foreach (Player p in plist) 
        {
            float temp = Vector3.Distance(p.transform.position, transform.position);
            if (temp < distance)
            {
                closestPlayer = p;
                distance = temp;
            }
        }

        return closestPlayer;
    }

    public void MagnetizeByPlayer(Player p)
    {
        Vector3 difference = MagnetizeType == Type.Repel ? transform.position - mPlayer.transform.position : mPlayer.transform.position - transform.position;
        if( difference.magnitude <= MinimumDistance )
        {
            mBody.AddForce(difference * RepelForce * Time.deltaTime);
        }
    }

    public void MagnetizeByClosestPlayer(List<Player> plist)
    {
        Player p = GetClosestPlayer(plist);
        MagnetizeByPlayer(p);
    }

    public float GetMagnetRange()
    {
        return MinimumDistance;
    }

    public Type getMagnetType()
    {
        return MagnetizeType;
    }
}
