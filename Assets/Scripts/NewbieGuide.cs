using UnityEngine;
using DashFire;
using System.Collections;
using StoryDlg;

public class NewbieGuide : MonoBehaviour
{

    // Use this for initialization
    public Vector3 m_NpcPos;
    public GameObject m_HintEffect;
    public float m_DetectRange = 2.0f;

    public SkillCategory m_GuideCategory;
    public float m_WarriorSkillLockFrameDelay = 1.8f;
    public int m_WarriorFirstSkill = 160201;
    public int m_WarriorSecondSkill = 160202;
    public float m_AssassinSkillLockFrameDelay = 1.8f;
    public int m_AssassinFirstSkill = 160201;
    public int m_AssassinSecondSkill = 160202;
    private float m_SkillStartTime = 0;
    private bool m_HasLockFrame = false;
    private bool m_HasSkillTeached = false;

    private RoleInfo m_RoleInfo;
    private GameObject m_RuntimeEffect;
    private bool m_IsDetect = false;
    private bool m_EnableGUI = false;
    //配置小手的位置
    private const int C_OffsetL = 120;
    private const int C_OffsetB = 120;
    public const int C_TransOffset = 60;
    private int m_TransFlag = 1;
    private int m_TransOffset = C_TransOffset;
    private bool m_IsCommonSkillGuid = false;
    private const float m_Epsilon = 0.00001f;
    private enum StoryEndEvent
    {
        CREATE_NPC = 1,
        ATTACK_NPC = 2,
        SKILL_NPC = 3,
        RETURN_CITY = 4,
    }
    private enum HeroIdEnum
    {
        WARRIOR = 1,
        ASSASSIN = 2,
    }
    void Start()
    {
        m_RoleInfo = LobbyClient.Instance.CurrentRole;
        if (m_RoleInfo.SceneInfo.Count == 0)
        {
            JoyStickInputProvider.JoyStickEnable = false;
            LogicSystem.StartStory(2);
            SkillBar.OnButtonClickedHandler += HandleSkillBarClicked;
            SkillBar.OnCommomButtonClickHandler += HandlerOnSkillCommonButtonClick;
        }
        else
        {
            LogicSystem.StartStory(3);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_IsDetect)
        {
            GameObject player = LogicSystem.PlayerSelf;
            if (null != player)
            {
                if (Vector3.Distance(player.transform.position, m_NpcPos) < m_DetectRange)
                {
                    OnReachEnemy();
                    m_IsDetect = false;
                }
            }
        }
        UpdateSkillLockFrame();
    }

    void OnDestroy()
    {
        StoryDlgManager.Instance.ClearListener();
        UIBeginnerGuide.Instance.ClearHandler();
        SkillBar.OnButtonClickedHandler = null;
        SkillBar.OnCommomButtonClickHandler = null;
        GfxModule.Skill.GfxSkillSystem.OnGfxShillStart = null;
    }

    private void OnStoryEnd(int id)
    {
        //Debug.LogError("OnStoryEnd " + id);
        JoyStickInputProvider.SetActive(true);
        UIBeginnerGuide.Instance.SetSkillBarActive(true);
        switch (id)
        {
            case (int)StoryEndEvent.CREATE_NPC:
                StartCoroutine(OnCreateNomalAttackNpc());
                break;
            case (int)StoryEndEvent.ATTACK_NPC:
                OnStartAttackNpc();
                break;
            case (int)StoryEndEvent.SKILL_NPC:
                OnShowSkill();
                break;
            case (int)StoryEndEvent.RETURN_CITY:
                StartCoroutine(OnShowReturnCity());
                break;
            default:
                break;
        }
    }
    private void OnStoryStart()
    {
        UIBeginnerGuide.Instance.SetSkillBarActive(false);
    }

    private IEnumerator OnCreateNomalAttackNpc()
    {
        if (null != m_HintEffect)
        {
            m_RuntimeEffect = ResourceSystem.NewObject(m_HintEffect) as GameObject;
            if (null != m_RuntimeEffect)
            {
                m_RuntimeEffect.transform.position = m_NpcPos;
                m_IsDetect = true;
            }
        }
        yield return new WaitForSeconds(0.2f);
        SetGUIEnable(true);
        //UIBeginnerGuide.Instance.ShowHandInControl();
    }
    void OnGUI()
    {
        if (m_EnableGUI)
        {
            if (Application.isEditor && !Application.isPlaying)
            {
                VirtualScreen.ComputeVirtualScreen();
                VirtualScreen.SetGuiScaleMatrix();
            }
            GUI.depth = 0;
            Texture handTex = ResourceSystem.GetSharedResource("UI/BeginnerGuide/shouzhi") as Texture;
            if (handTex != null)
            {
                Rect rect = VirtualScreen.GetRealRect(new Rect(C_OffsetL + m_TransOffset, C_OffsetB, 100, 100));
                UIBeginnerGuide.Instance.ShowGuideDlgInControl(rect.center, rect.height);
                rect.y = Screen.height - rect.y;

                GUI.DrawTexture(rect, handTex);
                if (m_TransOffset == 0) m_TransFlag = 1;
                if (m_TransOffset == C_TransOffset) m_TransFlag = -1;
                m_TransOffset += m_TransFlag;
            }
            else
            {
                Debug.Log("HandTex is null");
            }
        }
    }
    private void OnReachEnemy()
    {
        SetGUIEnable(false);
        if (null != m_RuntimeEffect)
        {
            ResourceSystem.RecycleObject(m_RuntimeEffect);
        }
        LogicSystem.SendStoryMessage("reachenemy");
    }

    private void OnStartAttackNpc()
    {
    }

    private void OnShowSkill()
    {
        UIBeginnerGuide.Instance.TransHandInFirstSkill();
    }

    private IEnumerator OnShowReturnCity()
    {
        yield return new WaitForSeconds(1.5f);
        UIBeginnerGuide.Instance.ShowReturnButton();
    }
    private void SetGUIEnable(bool enable)
    {
        m_EnableGUI = enable;
    }
    private IEnumerator DelayOnSkillStartLockFrame(GameObject obj, float time)
    {
        yield return new WaitForSeconds(time);
        //Time.timeScale = 0;
    }
    private void HandleSkillBarClicked(SkillCategory category)
    {
        if (!m_HasSkillTeached)
        {
            if (m_GuideCategory == category)
            {
                if (m_SkillStartTime < m_Epsilon)
                {
                    m_SkillStartTime = Time.time;
                    EnableController(false);
                    LogicSystem.NotifyGfxForceStartSkill(LogicSystem.PlayerSelf, GetFirstSkill());
                    LogicSystem.NotifyGfxStartSkill(LogicSystem.PlayerSelf, category, Vector3.zero);
                    OnSkillAAttack(2);
                }
                else
                {
                    if (Time.time - m_SkillStartTime > GetLockFrameDelay() && m_HasLockFrame)
                    {
                        Time.timeScale = 1.0f;
                        LogicSystem.NotifyGfxForceStartSkill(LogicSystem.PlayerSelf, GetSecondSkill());
                        m_HasSkillTeached = true;
                        HideAllGuide();
                        EnableController(true);
                    }
                }
            }
        }
        else
        {
            LogicSystem.NotifyGfxStartSkill(LogicSystem.PlayerSelf, category, Vector3.zero);
        }
    }
    private void EnableController(bool active)
    {
        JoyStickInputProvider.SetActive(active);
        UIBeginnerGuide.Instance.SetCommonSkillBtnActive(active);
    }
    private int GetFirstSkill()
    {
        RoleInfo role_info = LobbyClient.Instance.CurrentRole;
        if (null != role_info)
        {
            if (role_info.HeroId == (int)HeroIdEnum.WARRIOR)
            {
                return m_WarriorFirstSkill;
            }
            else if (role_info.HeroId == (int)HeroIdEnum.ASSASSIN)
            {
                return m_AssassinFirstSkill;
            }
        }
        return 0;
    }
    private int GetSecondSkill()
    {
        RoleInfo role_info = LobbyClient.Instance.CurrentRole;
        if (null != role_info)
        {
            if (role_info.HeroId == (int)HeroIdEnum.WARRIOR)
            {
                return m_WarriorSecondSkill;
            }
            else if (role_info.HeroId == (int)HeroIdEnum.ASSASSIN)
            {
                return m_AssassinSecondSkill;
            }
        }
        return 0;
    }
    private float GetLockFrameDelay()
    {
        RoleInfo role_info = LobbyClient.Instance.CurrentRole;
        if (null != role_info)
        {
            if (role_info.HeroId == (int)HeroIdEnum.WARRIOR)
            {
                return m_WarriorSkillLockFrameDelay;
            }
            else if (role_info.HeroId == (int)HeroIdEnum.ASSASSIN)
            {
                return m_AssassinSkillLockFrameDelay;
            }
        }
        return 0;
    }
    private void UpdateSkillLockFrame()
    {
        if (m_SkillStartTime > m_Epsilon && !m_HasLockFrame)
        {
            if (Time.time - m_SkillStartTime > GetLockFrameDelay())
            {
                m_HasLockFrame = true;
                Time.timeScale = 0.0f;
            }
        }
    }
    public void OnAllNpcDead(int index)
    {
        UIBeginnerGuide.Instance.TransHandInFirstSkill();
    }
    public void OnCloseController()
    {
        JoyStickInputProvider.JoyStickEnable = false;
    }
    public void OnStartGuide()
    {
        EasyJoystick.On_JoystickMoveStart += On_JoystickMoveStart;
        //新手关卡不允许直接退出
        UIManager.Instance.HideWindowByName("Tuichu");
        //JoyStickInputProvider.JoyStickEnable = true;
        SetGUIEnable(true);
    }
    public void OnCommonAttack(int index)
    {
        m_IsCommonSkillGuid = true;
        UIBeginnerGuide.Instance.TransHandInCommonAttact(index);
        UIBeginnerGuide.Instance.ShowGuideDlgAboveCommon(index);
    }
    public void OnSkillAAttack(int index)
    {
        UIBeginnerGuide.Instance.TransHandInFirstSkill();
        UIBeginnerGuide.Instance.ShowGuideDlgAboveSkillA(index);
    }
    public void HideAllGuide()
    {
        UIManager.Instance.HideWindowByName("GuideHand");
        UIManager.Instance.HideWindowByName("GuideDlgRight");
    }
    private void On_JoystickMoveStart(MovingJoystick move)
    {
        UIManager.Instance.HideWindowByName("GuideDlg");
        SetGUIEnable(false);
        EasyJoystick.On_JoystickMoveStart -= On_JoystickMoveStart;
    }
    private void HandlerOnSkillCommonButtonClick()
    {
        if (m_IsCommonSkillGuid)
        {
            HideAllGuide();
            m_IsCommonSkillGuid = false;
        }
    }
}
