﻿story(1)
{ 
	onmessage("start")
	{
	  wait(100);
		loop(6)
	  {
	    createnpc(1001+$$);
	  };
	};
	onmessage("allnpckilled")
	{
		wait(2000);
		restartareamonitor(2);
		wait(100);
		showwall("AtoB",false);
	};
	onmessage("anyuserenterarea",2)
	{
		showwall("BDoor",true);
		startstory(2);
		terminate();	  
	};
  onmessage("missionfailed")
  {
    missionfailed(1010);
    terminate();
  };
};
story(2)
{
	onmessage("start")
	{
		wait(100);
	  loop(7)
	  {
	    createnpc(2001+$$);
	  };
	};
	onmessage("allnpckilled")
	{
		wait(2000);
		restartareamonitor(3);
		wait(100);
		showwall("BtoC",false);
	};
	onmessage("anyuserenterarea",3)
	{
		showwall("CDoor",true);
		startstory(3);
		terminate();
	};
  onmessage("missionfailed")
  {
    missionfailed(1010);
    terminate();
  };
};
story(3)
{
  local
  {
    @reconnectCount(0);
  };
	onmessage("start")
	{	
		wait(100);
	  loop(9)
	  {
  	  createnpc(3001+$$);
  	};
	};
	onmessage("allnpckilled")
	{
    //camerayaw(-80,3100);
    //wait(500);
    //cameraheight(2.3,10);
	  //cameradistance(7.6,10);
	  wait(10);
    lockframe(0.2);
    wait(500);
    lockframe(0.08);
    wait(450);
    lockframe(0.05);
    wait(50);
    lockframe(0.1);
    wait(2000);
    lockframe(1.0);
		wait(300);
		//camerayaw(0,100);
	  //cameraheight(-1,100);
	  //cameradistance(-1,100);
	  wait(3000);
		//检测网络状态
		while(!islobbyconnected() && @reconnectCount<10)
		{
		  reconnectlobby();
		  wait(3000);
		  inc(@reconnectCount);
		};
		if(islobbyconnected())
		{
		  missioncompleted(1010);
		  terminate();
		} else {
		  missionfailed(1010);
		};
	};
  onmessage("missionfailed")
  {
    missionfailed(1010);
    terminate();
  };
};
