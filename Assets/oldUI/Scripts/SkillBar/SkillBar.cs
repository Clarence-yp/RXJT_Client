using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using DashFire;
public class SkillBar : MonoBehaviour
{
    public delegate void OnCommonButtonDelegate();
    public delegate void OnButtonClickDelegate(SkillCategory skillType);
    public static OnButtonClickDelegate OnButtonClickedHandler;
    public static OnCommonButtonDelegate OnCommomButtonClickHandler;
    public GameObject goEffect = null;
    private GameObject m_RuntimeEffect = null;
    public GameObject CommonSkillGo = null;
    public UISprite[] spBright = new UISprite[c_SkillNum];
    public UISprite spFullEx = null;
    public UISprite spAshEx = null;
    public UISprite spAgeryValueEx = null;
    public float HighPecent = 0.2f;
    private List<object> m_EventList = new List<object>();


    // Use this for initialization
    void Awake()
    {
        object obj = LogicSystem.EventChannelForGfx.Subscribe<List<SkillInfo>>("ge_equiped_skills", "ui", InitSkillBar);
        if (obj != null) m_EventList.Add(obj);
        LogicSystem.PublishLogicEvent("ge_request_equiped_skills", "ui");
    }
    void Start()
    {
        if (null != CommonSkillGo)
            UIEventListener.Get(CommonSkillGo).onPress = OnButtonPress;
        object obj = LogicSystem.EventChannelForGfx.Subscribe<string, float>("ge_cast_skill_cd", "ui", CastCDByGroup);
        if (obj != null) m_EventList.Add(obj);
        obj = LogicSystem.EventChannelForGfx.Subscribe<SkillCannotCastType>("ge_skill_cannot_cast", "ui", HandleCastSkillResult);
        if (obj != null) m_EventList.Add(obj);
        obj = LogicSystem.EventChannelForGfx.Subscribe("ge_ui_unsubscribe", "ui", UnSubscribe);
        if (obj != null) m_EventList.Add(obj);

    }

    public void UnSubscribe()
    {
        try
        {
            foreach (object obj in m_EventList)
            {
                if (null != obj) LogicSystem.EventChannelForGfx.Unsubscribe(obj);
            }
            m_EventList.Clear();
        }
        catch (Exception ex)
        {
            DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
        }
    }
    // Update is called once per frame
    void Update()
    {
        UpdateAngryValue();
        if (m_TipsCD >= 0f)
        {
            m_TipsCD -= Time.deltaTime;
        }
        //按钮不可点击时停止普通攻击
        if (CommonSkillGo != null)
        {
            UIButton btn = CommonSkillGo.GetComponent<UIButton>();
            if (!btn.isEnabled) m_IsPressed = false;
        }
        if (m_IsPressed)
        {
            m_IsAttact = true;
            GfxModule.Skill.GfxSkillSystem.Instance.StartAttack(DashFire.LogicSystem.PlayerSelf, Vector3.zero);
        }
        else if (m_IsAttact == true)
        {
            m_IsAttact = false;
            GfxModule.Skill.GfxSkillSystem.Instance.StopAttack(DashFire.LogicSystem.PlayerSelf);
        }
        if (time > 0)
        {
            time -= RealTime.deltaTime;
            if (time <= 0)
            {
                foreach (BoxCollider collider in m_GoList)
                {
                    if (collider != null)
                    {
                        collider.enabled = true;
                    }
                }
                m_GoList.Clear();
            }
        }
        for (int index = 0; index < c_SkillNum; ++index)
        {
            if (remainCdTime[index] > 0)
            {
                string path = "Skill" + index.ToString() + "/skill0/CD";
                GameObject go = GetGoByPath(path);
                if (null != go)
                {
                    UISprite sp = go.GetComponent<UISprite>();
                    if (null != sp)
                    {
                        sp.fillAmount -= RealTime.deltaTime / GetCDTimeByIndex(index);
                        remainCdTime[index] = sp.fillAmount;
                        if (remainCdTime[index] <= 0)
                        {
                            IconFlashByIndex(index);
                        }
                    }
                }
            }
        }
    }
    void OnDisable()
    {
        if (m_RuntimeEffect != null)
        {
            Destroy(m_RuntimeEffect);
            m_RuntimeEffect = null;
        }
    }
    void OnDestroy()
    {
        if (m_RuntimeEffect != null)
        {
            Destroy(m_RuntimeEffect);
            m_RuntimeEffect = null;
        }
    }
    public void InitSkillBar(List<SkillInfo> skillInfos)
    {
        try
        {
            foreach (SkillCategory category in m_CategoryArray)
            {
                UnlockSkill(category, false);
            }
            if (skillInfos != null)
            {
                foreach (SkillInfo skill_info in skillInfos)
                {
                    SkillCategory category = skill_info.ConfigData.Category;
                    UnlockSkill(category, true, skill_info.ConfigData);
                }
            }
        }
        catch (Exception ex)
        {
            DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
        }
    }
    //发送普通攻击消息
    //press == true 表示按下
    //press == false 表示抬起
    void OnButtonPress(GameObject ob, bool press)
    {
        //Debug.Log("Common attack!" + press);
        LogicSystem.EventChannelForGfx.Publish("ge_attack", "game", press);
        m_IsPressed = press;
        if (OnCommomButtonClickHandler != null)
        {
            OnCommomButtonClickHandler();
        }
    }
    private void IconFlashByIndex(int index)
    {
        //Debug.Log("IconFlashByIndex:"+ index);
        string path = "Skill" + index.ToString() + "/skill0/bright";
        GameObject go = GetGoByPath(path);
        if (go == null)
            return;
        NGUITools.SetActive(go, true);
        TweenAlpha alpha = go.GetComponent<TweenAlpha>();
        if (null != alpha)
            Destroy(alpha);
        TweenAlpha tweenAlpha = go.AddComponent<TweenAlpha>();
        if (null == tweenAlpha)
            return;
        tweenAlpha.from = 0;
        tweenAlpha.to = 1;
        tweenAlpha.duration = 0.4f;
        tweenAlpha.animationCurve = tweenAnimation;
        GameObject goSkill = GetGoByPath("Skill" + index.ToString() + "/skill0");
        if (null != goSkill)
        {
            UIButton button = goSkill.GetComponent<UIButton>();
            if (button != null) button.isEnabled = true;
        }
    }

    public void SetSkillEnableByIndex(int index, int period, bool enable)
    {
        string childPath = "Skill" + index.ToString() + "/skill" + period.ToString() + "/CD";
        GameObject go = GetGoByPath(childPath);
        if (go == null)
            return;
        UISprite sp = go.GetComponent<UISprite>();
        if (null != sp)
        {
            if (enable == false)
                sp.fillAmount = 1f;
            else
            {
                sp.fillAmount = 0f;
            }

        }
    }

    public void CastCDByGroup(string group, float cdTime)
    {
        try
        {
            if (cdTime <= 0) return;
            int index = GetIndexByGroup(group);
            if (index == -1 || index >= c_SkillNum)
                return;
            skillsCDTime[index] = cdTime;
            string path = "Skill" + index.ToString() + "/skill0/CD";
            GameObject go = GetGoByPath(path);
            if (null != go)
            {
                UISprite sp = go.GetComponent<UISprite>();
                if (null != sp)
                {
                    sp.fillAmount = 1;
                    remainCdTime[index] = sp.fillAmount;
                }
            }
        }
        catch (Exception ex)
        {
            DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
        }
    }
    private GameObject GetGoByPath(string path)
    {
        GameObject go = null;
        Transform trans = this.transform.Find(path);
        if (null != trans)
        {
            go = trans.gameObject;
        }
        else
        {
            Debug.Log("Can not find " + path);
        }
        return go;
    }
    private GameObject GetGoByIndexAndName(int index, string name)
    {
        GameObject go = null;
        string path = "Skill" + index.ToString() + "/" + name;
        Transform trans = this.transform.Find(path);
        if (trans != null)
        {
            go = trans.gameObject;
        }
        return go;
    }
    public float GetCDTimeByIndex(int index)
    {
        float ret = 0;
        ret = skillsCDTime[index];
        return ret;
    }

    public void OnButtonClick()
    {
        if (UICamera.currentTouch != null)
        {
            GameObject go = UICamera.currentTouch.current;
            if (go == null)
                return;
            Transform trans = go.transform.parent;
            if (trans == null)
                return;
            GameObject parentGo = trans.gameObject;
            string name = parentGo.name;
            SkillCategory skillType;
            switch (name)
            {
                case "Skill0": skillType = SkillCategory.kSkillA; break;
                case "Skill1": skillType = SkillCategory.kSkillB; break;
                case "Skill2": skillType = SkillCategory.kSkillC; break;
                case "Skill3": skillType = SkillCategory.kSkillD; break;
                default: skillType = SkillCategory.kNone; break;
            }
            //向上层逻辑发送释放技能的消息
            //Debug.Log("!! Skill type is " + skillType);
            if (null != OnButtonClickedHandler)
            {
                OnButtonClickedHandler(skillType);
            }
            else
            {
                GfxModule.Skill.GfxSkillSystem.Instance.PushSkill(DashFire.LogicSystem.PlayerSelf, skillType, Vector3.zero);
            }
            //LogicSystem.EventChannelForGfx.Publish("ge_cast_skill", "game", skillType);
        }
    }

    public void UnlockSkill(SkillCategory category, bool isActive, SkillLogicData skillData = null)
    {
        try
        {
            int index = GetIndexByGroup(category);
            string goPath = "Skill" + index.ToString();
            //NGUIDebug.Log(this.name);
            Transform ts = this.transform.Find(goPath);
            if (null != ts)
            {
                GameObject go = ts.gameObject;
                if (skillData != null)
                {
                    ts = go.transform.Find("skill0");
                    if (ts != null)
                    {
                        UISprite sp = ts.gameObject.GetComponent<UISprite>();
                        UIButton btn = ts.GetComponent<UIButton>();
                        GameObject goAtlas = ResourceSystem.GetSharedResource(skillData.ShowAtlasPath) as GameObject;
                        if (goAtlas != null)
                        {
                            UIAtlas atlas = goAtlas.GetComponent<UIAtlas>();
                            if (atlas != null && sp != null)
                            {
                                sp.atlas = atlas;
                            }
                        }
                        else
                        {
                            Debug.LogError("!!!Load atlas failed.");
                        }
                        if (btn != null && sp != null)
                        {
                            btn.normalSprite = skillData.ShowIconName;
                            sp.spriteName = skillData.ShowIconName;
                        }
                    }
                    if (index < c_SkillNum && spBright[index] != null)
                    {
                        ;// spBright[index].spriteName = skillData.ShowIconName;
                    }
                }
                NGUITools.SetActive(go, isActive);
            }
            else
            {
                Debug.Log("!!can not find " + goPath);
            }
        }
        catch (Exception ex)
        {
            DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
        }
    }
    public void HandleCastSkillResult(SkillCannotCastType result)
    {
        try
        {
            float x = Screen.width / 2f;
            float y = Screen.height * HighPecent;
            Vector3 pos = UICamera.mainCamera.ScreenToWorldPoint(new Vector3(x, y, 0));
            string CHN = StrDictionaryProvider.Instance.GetDictString(12);
            switch (result)
            {
                case SkillCannotCastType.kInCD:
                    break;
                case SkillCannotCastType.kCostNotEnough:
                    CHN = StrDictionaryProvider.Instance.GetDictString(13);
                    break;
                case SkillCannotCastType.kUnknow:
                    CHN = StrDictionaryProvider.Instance.GetDictString(14);
                    break;
                default:
                    CHN = StrDictionaryProvider.Instance.GetDictString(14);
                    break;
            }
            if (m_TipsCD <= 0f)
            {
                LogicSystem.EventChannelForGfx.Publish("ge_screen_tip", "ui", pos.x, pos.y, 0, false, CHN);
                m_TipsCD = m_TipsDelta;
            }

        }
        catch (Exception ex)
        {
            DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
        }
    }

    //更新怒气值
    public void UpdateAngryValue()
    {
        SharedGameObjectInfo share_info = LogicSystem.PlayerSelfInfo;
        if (share_info != null)
        {
            float value = share_info.Rage / share_info.MaxRage;
            if (spAgeryValueEx != null) spAgeryValueEx.fillAmount = value;
            if (value >= 1 && spFullEx != null)
            {
                NGUITools.SetActive(spFullEx.gameObject, true);
                //播放特效
                if (goEffect != null && m_RuntimeEffect == null && spAshEx != null && NGUITools.GetActive(spAshEx.gameObject))
                {
                    m_RuntimeEffect = ResourceSystem.NewObject(goEffect) as GameObject;
                    if (m_RuntimeEffect != null) m_RuntimeEffect.transform.position = spFullEx.transform.position;
                }
            }
            else
            {
                if (spFullEx != null)
                {
                    NGUITools.SetActive(spFullEx.gameObject, false);
                    if (m_RuntimeEffect != null)
                    {
                        Destroy(m_RuntimeEffect);
                        m_RuntimeEffect = null;
                    }
                }
            }
        }
    }
    //
    public void OnExButtonClick()
    {
        GfxModule.Skill.GfxSkillSystem.Instance.PushSkill(DashFire.LogicSystem.PlayerSelf, SkillCategory.kEx, Vector3.zero);
    }

    public int GetIndexByGroup(SkillCategory category)
    {
        int ret = -1;
        switch (category)
        {
            case SkillCategory.kSkillA: ret = 0; break;
            case SkillCategory.kSkillB: ret = 1; break;
            case SkillCategory.kSkillC: ret = 2; break;
            case SkillCategory.kSkillD: ret = 3; break;
            default: ret = -1; break;
        }
        return ret;
    }

    public int GetIndexByGroup(string group)
    {
        int ret = -1;
        switch (group)
        {
            case "SkillA": ret = 0; break;
            case "SkillB": ret = 1; break;
            case "SkillC": ret = 2; break;
            case "SkillD": ret = 3; break;
            default: ret = -1; break;
        }
        return ret;
    }
    //当游戏结束时、按钮被隐藏时、需要调用该方法停止普通攻击
    public void StopAttack()
    {
        m_IsPressed = false;
    }
    //PVP模式下先隐藏Ex技能
    public void SetExButtonVisble(bool visible)
    {
        if (spAshEx != null)
            NGUITools.SetActive(spAshEx.gameObject, visible);
    }
    public float m_TipsCD = 0f;
    public float m_TipsDelta = 0.5f;
    public bool m_IsAttact = false;
    private bool m_IsPressed = false;
    private float time = 0.6f;
    public AnimationCurve tweenAnimation = null;
    public const float c_DisableScale = 1.2f;
    private const int c_SkillNum = 4;
    private float[] remainCdTime = new float[c_SkillNum];
    private Dictionary<int, Vector3> m_OriginalPos;
    private float[] skillsCDTime = new float[c_SkillNum];
    private List<BoxCollider> m_GoList = new List<BoxCollider>();
    private Vector3 m_SkillBarPos;
    private SkillCategory[] m_CategoryArray = new SkillCategory[4]{
    SkillCategory.kSkillA,
    SkillCategory.kSkillB,
    SkillCategory.kSkillC,
    SkillCategory.kSkillD
  };
}
