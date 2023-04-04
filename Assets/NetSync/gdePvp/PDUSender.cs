using UnityEngine;
using System.Collections;

public class PDUSender : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

	public virtual bool syncSkill(uint myID, int skillID)
	{
		return false;
	}
    public virtual bool syncAttack(int iCurAttackStep)
    {
        return false;
    }
    public virtual bool sendCreatePlayer()
    {
        return false;
    }

    public virtual  bool sendPDU(PDURunner.PDU pdu)
    {
        return false;
    }

    public virtual bool sendInjuryBloodByAttack(bool bBigBlood,Vector3 pot,Vector3 dir)
    {
        return false;
    }

    public virtual bool sendInjuryBloodBySkill(int iEffetID)
    {
        return false;
    }
	// Update is called once per frame
	void Update () {
	
	}
}
