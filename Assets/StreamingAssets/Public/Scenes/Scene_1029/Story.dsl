﻿story(1)
{ 
	local
	{
		@Count(0);
	};
	onmessage("start")
	{
	  wait(10);
	  inc(@Count);
	  wait(100);
	  publishgfxevent("ge_pve_fightinfo","ui",1,1,1,5);
	  wait(10);
	  loop(3)
	  {
	  	createnpc(1001+$$);
	  	wait(300);
	  };
	  wait(3500);
	  loop(3)
	  {
	  	createnpc(1004+$$);
	  	wait(300);
	  };
	  wait(1000);
	  loop(2)
	  {
	  	createnpc(1007+$$);
	  	wait(300);
	  };
	  wait(1000);
	  setblockedshader(0x0000ff90,0.5,0,0xff000090,0.5,0);
	};
	onmessage("allnpckilled")
	{
		wait(10);
		inc(@Count);
	  wait(2000);
	  publishgfxevent("ge_pve_fightinfo","ui",1,@Count,@Count,5);
	  wait(10);
	  if(2==@Count)
	  {
		  loop(3)
		  {
		  	createnpc(2001+$$);
		  	wait(300);
		  };
		  wait(2000);
		  loop(3)
		  {
		  	createnpc(2004+$$);
		  	wait(300);
		  };
		  wait(1500);
		  loop(2)
		  {
		  	createnpc(2007+$$);
		  	wait(300);
		  };
		};
		if(3==@Count)
	  {
		  loop(3)
		  {
		  	createnpc(3001+$$);
		  	wait(300);
		  };
		  wait(1800);
		  loop(3)
		  {
		  	createnpc(3004+$$);
		  	wait(300);
		  };
		  wait(1800);
		  loop(3)
		  {
		  	createnpc(3007+$$);
		  	wait(300);
		  };
		};
		if(4==@Count)
	  {
		  loop(3)
		  {
		  	createnpc(4001+$$);
		  	wait(300);
		  };
		  wait(1800);
		  loop(3)
		  {
		  	createnpc(4004+$$);
		  	wait(300);
		  };
		  wait(1800);
		  loop(3)
		  {
		  	createnpc(4007+$$);
		  	wait(300);
		  };
		};
		if(5==@Count)
	  {
	  	loop(3)
		  {
		  	createnpc(5001+$$);
		  	wait(300);
		  };
		  wait(2800);
		  loop(3)
		  {
		  	createnpc(5004+$$);
		  	wait(300);
		  };
		  wait(2800);
		  loop(4)
		  {
		  	createnpc(5007+$$);
		  	wait(300);
		  	startstory(2);
				terminate();
			};
		};
		wait(1000);
		setblockedshader(0x0000ff90,0.5,0,0xff000090,0.5,0);
	};
	onmessage("missionfailed")
  {
    missionfailed(1010);
    terminate();
  };
};
story(2)
{
  local
  {
    @reconnectCount(0);
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
