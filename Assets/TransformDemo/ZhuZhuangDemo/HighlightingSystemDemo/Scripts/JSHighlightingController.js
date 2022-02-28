#pragma strict

protected var ho : HighlightableObject;

function Awake ()
{
	ho = gameObject.AddComponent(HighlightableObject);
}

function Update ()
{
	// Fade in/out constant highlighting with 'Tab' button
	if (Input.GetKeyDown(KeyCode.Tab)) 
	{
		ho.ConstantSwitch();
	}
	// Turn on/off constant highlighting with 'Q' button
	else if (Input.GetKeyDown(KeyCode.Q))
	{
		ho.ConstantSwitchImmediate();
	}
	
	// Turn off all highlighting modes with 'Z' button
	if (Input.GetKeyDown(KeyCode.Z)) 
	{
		ho.Off();
	}
}