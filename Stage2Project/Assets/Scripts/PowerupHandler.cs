using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PowerupHandler : MonoBehaviour {

    //this is a queue - buff at index 0 is first to be toggled off
    List<Buff> activePowerups = new List<Buff>();

    Dictionary<Powerup.Type, float> durations = new Dictionary<Powerup.Type, float>();

    //buff is an abstract representation of an effect of a powerup
    //holds target and type
    //buff is applied directly to a player
    public struct Buff {
        public Powerup.Type type;
        public Player target;
        public float timeLeft;
    }

	// Use this for initialization
	void Start () {
        PopulateDurations();
	}

    private void PopulateDurations()
    {
        //if 
        durations.Add(Powerup.Type.SPEED_UP, 5.0f);
        durations.Add(Powerup.Type.SLOW_DOWN, 5.0f);
        durations.Add(Powerup.Type.INVERSE, 3.0f);
        durations.Add(Powerup.Type.DOUBLE_CUBES, 0.1f);
        durations.Add(Powerup.Type.HALVE_CUBES, 0.1f);
    }
	
	// Update is called once per frame
	void Update () {
        updateTimes();

        DeactivateInactivePowerups();
		
	}

    private void updateTimes()
    {
        for (int i = 0; i < activePowerups.Count; i++)
        {
            Buff b = activePowerups[i];
            b.timeLeft -= Time.deltaTime;
            activePowerups[i] = b;
            //print(activePowerups[i].timeLeft);
        }
    }

    private void DeactivateInactivePowerups()
    {
        //while first in queue has less than 0 senconds left, delete from queue
        while (activePowerups.Count > 0 && activePowerups[0].timeLeft <= 0)
        {
            TogglePowerup(activePowerups[0], false);
            activePowerups.RemoveAt(0);
        }
    }

    public void ActivatePowerup(Powerup.Type type, Powerup.Target target, Player player) 
    {
        List<Player> targets = new List<Player>();

        if (target == Powerup.Target.SELF) //player passed as argument
        {
            targets = new List<Player>();
            targets.Add(player);
        }
        else if (target == Powerup.Target.REST) //everyone except player as argument
        {
            targets = new List<Player>(GetComponent<GameManager>().players);
            targets.Remove(player);
        }

        foreach (Player t in targets)
        {
            Buff b = CreateBuff(type, t);
            TogglePowerup(b, true);
            AddBuff(b);
        }
    }

    public Buff CreateBuff(Powerup.Type type, Player target)
    {
        Buff b;
        b.target = target;
        b.type = type;
        b.timeLeft = durations[b.type];

        return b;

    }

    public void TogglePowerup(Buff b, bool toggle)
    {

        switch (b.type)
        {
            case Powerup.Type.SPEED_UP:
                b.target.speedBuff = toggle;
                break;
            case Powerup.Type.SLOW_DOWN:
                b.target.slowBuff = toggle;
                break;
            case Powerup.Type.INVERSE:
                b.target.inverted = toggle;
                break;

            case Powerup.Type.DOUBLE_CUBES:
                if (toggle)
                {
                    GetComponent<GameManager>().DoubleCubes(b.target);
                }
                break;
            case Powerup.Type.HALVE_CUBES:
                if (toggle)
                {
                    GetComponent<GameManager>().HalveCubes(b.target);
                }
                break;
                
            default:
                print("WARNING!!!!!!!!: POWERUP NOT YET IMPLEMENTED");
                break;
        }

    }

    public void AddBuff(Buff b)
    {
        //if there is already such a buff, remove it, and then add later at appropriate index
        int toRemove = -1;
        for (int i = 0; i < activePowerups.Count; i++)
        {
            if (activePowerups[i].type == b.type && activePowerups[i].target.netId == b.target.netId)
            {
                toRemove = i;
                break;
            }
        }
        if (toRemove >= 0)
        {
            activePowerups.RemoveAt(toRemove);
        }

        if (activePowerups.Count == 0)
        {
            activePowerups.Add(b);
            return;
        }

        //find suitable place to insert the buff
        int index = 0;

        while (index <= activePowerups.Count)
        {
            
            if (index == activePowerups.Count || activePowerups[index].timeLeft > b.timeLeft)
                break;

            index++;
        }

        if (index == activePowerups.Count)
        {
            activePowerups.Add(b);
        }
        else
        {
            activePowerups.Insert(index, b);
        }
    }
}
