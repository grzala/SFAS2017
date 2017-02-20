using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PowerupHandler : MonoBehaviour {

    //this is a queue - buff at index 0 is first to be toggled off
    List<Buff> activePowerups = new List<Buff>();

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
            print(b.timeLeft);
        }
    }

    private void DeactivateInactivePowerups()
    {
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
        b.timeLeft = 5.0f;

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
        }

        if (toggle)
            AddBuff(b);
    }

    public void AddBuff(Buff b)
    {
        if (activePowerups.Count == 0)
        {
            activePowerups.Add(b);
            return;
        }

        //if there is already such a buff, reset it
        for (int i = 0; i < activePowerups.Count; i++)
        {
            if (activePowerups[i].type == b.type && activePowerups[i].target == b.target){
                activePowerups[i] = b;
                return;
            }
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
