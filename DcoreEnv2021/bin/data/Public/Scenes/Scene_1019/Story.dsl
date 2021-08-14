﻿story(1)
{
	local
  {
    @trg(0);
  };
	onmessage("start")
	{	
		sendgfxmessage("GfxGameRoot","EnableBloom");
		wait(10);
		sendgfxmessage("Main Camera","DimScreen",10);
		wait(10);
		showui(false);
		wait(10);
		sendgfxmessage("Main Camera","LightScreen",2000);
		wait(100);
	  publishlogicevent("ge_change_player_movemode","game",1);
	  publishlogicevent("ge_change_npc_movemode","game",unitid2objid(10001),1);
	  wait(100);
	  npcmovewithwaypoints(10001,vector2list("41.6441 22.82592 47.167 21.05215 50.8255 22.21408"));
	  playerselfmovewithwaypoints(vector2list("43.30661 24.02605 48.82951 22.25227 49.6871 23.91948"));
	  wait(100);
	  showdlg(101901);
	  wait(1000);
	  setblockedshader(0x0000ff90,0.5,0,0xff000090,0.5,0);
	};
	onmessage("dialogover",101901)
	{
		inc(@trg);
		if(2==@trg)
	  {
	    localmessage("st2");
	  };
	};
	onmessage("playerselfarrived")
	{
	  inc(@trg);
		wait(10);
	  publishlogicevent("ge_change_npc_movemode","game",unitid2objid(10002),2);
	  wait(10);
	  npcmove(10002,vector3(51.31762,172.9803,26.06957));
		if(2==@trg)
	  {
	    localmessage("st2");
	  };
	};
	onmessage("st2")
	{
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
	local
  {
    @MCount(0);
  };
	onmessage("npcarrived",10002)
	{
		wait(100);
		inc(@MCount);
	  if(2==@MCount)
	  {
	    destroynpc(10002);
	  }
	  else
	  {
	  	showdlg(101902);
	  };
	};
	onmessage("dialogover",101902)
	{
		wait(10);
		npcmove(10002,vector3(34.14244,172.9803,25.66005));
		wait(10);
		showdlg(101903);
		wait(10);
		restartareamonitor(1);
	};
	onmessage("dialogover",101903)
	{
		wait(10);
	  publishlogicevent("ge_change_player_movemode","game",2);
	  wait(10);
	  sendgfxmessage("Main Camera","DimScreen",300);
	  wait(300);
	  sendgfxmessage("Main Camera","LightScreen",2800);
	  wait(500);
	  showui(true);
	  wait(10);
	};
	onmessage("anyuserenterarea",1)
	{
		sendgfxmessage("Main Camera","DimScreen",10);
		wait(10);
		showui(false);
		wait(10);
		sendgfxmessage("StoryObj","SetPlayerselfPosition",62.47073,178.9233,46.59911); 
		wait(10);
		publishlogicevent("ge_change_player_movemode","game",1);
		showwall("BDoor",true);
		sendgfxmessage("Main Camera","LightScreen",1800);
		wait(800);
		playerselfmove(vector3(59.8275,178.9233,48.22662));		
	};
	onmessage("playerselfarrived")
	{
	  playerselfface(-1.0188903);
	  wait(10);
	  showdlg(101904);
	};
	onmessage("dialogover",101904)
	{
		publishlogicevent("ge_change_npc_movemode","game",unitid2objid(2001),1);
		wait(10);
		npcmove(2001,vector3(56.97326,178.9233,49.28206));
	};
	onmessage("npcarrived",2001)
	{
		showdlg(101905);
		wait(10);
		publishlogicevent("ge_change_npc_movemode","game",unitid2objid(2001),2);
		wait(10);
		publishlogicevent("ge_change_player_movemode","game",2);
	};
	onmessage("dialogover",101905)
	{
		sendgfxmessage("Main Camera","DimScreen",300);
		wait(300);
		sendgfxmessage("Main Camera","LightScreen",1800);
		wait(800);
		setcamp(2001,2);
		wait(10);
		sendgfxmessage("GfxGameRoot","DisableBloom");
		showui(true);
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