﻿story(1)
{
};
story(2)
{ 
    local
    {
        @npcbatch(0);
    };
    onmessage("start")
    {
        showdlg(1);
    };
    onmessage("reachenemy")
    {
        showdlg(2);
        createnpc(1001);
    };
    onmessage("allnpckilled")
    {
        if(0==@npcbatch)
        {
		
            showdlg(3);
            loop(3)
            {
                createnpc(1002+$$);
            };
		cameralookat(33.70038,150.1627,36.11686);
		wait(1500);
		camerafollow();	
        };
        if(1==@npcbatch)
        {
            showdlg(4);
        };

        @npcbatch=1;
    };
    onmessage("returncity")
    {
        wait(1000);
        missioncompleted(1000);
        terminate();
    };
    onmessage("missionfailed")
    {
        missionfailed(1000);
        terminate();
    };
};
story(3)
{ 
    onmessage("start")
    {
        loop(3)
        {
            createnpc(1001+$$);
        };
    };
    onmessage("allnpckilled")
    {
        lockframe(0.1);
        wait(3000);
        lockframe(1.0);
        wait(1000);
        missioncompleted(1000);
        terminate();
    };
};
