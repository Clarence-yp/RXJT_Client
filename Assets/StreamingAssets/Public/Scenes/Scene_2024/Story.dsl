﻿story(1)
{ 
	local
	{
		@rnd(0);
	};
	onmessage("start")
	{
	  wait(100);
	  @rnd=rndfloat();
		loop(8)
	  {
	    createnpc(1001+$$,@rnd);
	  };
	  wait(1000);
	  setblockedshader(0x0000ff90,0.5,0,0xff000090,0.5,0);
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
	  loop(8)
	  {
	    createnpc(2001+$$);
	  };
	  wait(1000);
	  setblockedshader(0x0000ff90,0.5,0,0xff000090,0.5,0);
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
  	wait(1000);
	  setblockedshader(0x0000ff90,0.5,0,0xff000090,0.5,0);
	};
	onmessage("allnpckilled")
	{
    //camerayaw(-80,3100);
    //wait(500);
    //cameraheight(2.3,10);
	  //cameradistance(7.6,10);
	  lockframe(0.01);
    wait(500);
    lockframe(0.05);
    wait(1800);
    lockframe(0.08);
    wait(300);
    lockframe(0.2);
    wait(500);
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
