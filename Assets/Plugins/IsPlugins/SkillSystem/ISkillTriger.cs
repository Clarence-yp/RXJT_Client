using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkillSystem
{
    public interface ISkillTriger
    {
        ISkillTriger Clone();//克隆触发器，触发器只会从DSL实例一次，之后都通过克隆产生新实例
        long GetStartTime();
        void Init(ScriptableData.ISyntaxComponent config);//从DSL语言初始化触发器实例
        void Reset();//复位触发器到初始状态
        bool Execute(object sender, SkillInstance instance, long delta, long curSectionTime);//执行触发器，返回false表示触发器结束，下一tick不再执行
        void Analyze(object sender, SkillInstance instance);//语义分析，配合上下文sender与instance进行语义分析，在执行前收集必要的信息
    }
    public abstract class AbstractSkillTriger : ISkillTriger
    {
        public virtual long GetStartTime()
        {
            return m_StartTime;
        }

        public void Init(ScriptableData.ISyntaxComponent config)
        {
            ScriptableData.CallData callData = config as ScriptableData.CallData;
            if (null != callData)
            {
                Load(callData);
            }
            else
            {
                ScriptableData.FunctionData funcData = config as ScriptableData.FunctionData;
                if (null != funcData)
                {
                    Load(funcData);
                }
                else
                {
                    ScriptableData.StatementData statementData = config as ScriptableData.StatementData;
                    if (null != statementData)
                    {
                        //是否支持语句类型的触发器语法？
                        Load(statementData);
                    }
                    else
                    {
                        //error
                    }
                }
            }
        }
        public abstract ISkillTriger Clone();
        public virtual void Reset() { }
        public virtual bool Execute(object sender, SkillInstance instance, long delta, long curSectionTime) { return false; }
        public virtual void Analyze(object sender, SkillInstance instance) { }

        protected virtual void Load(ScriptableData.CallData callData)
        {
        }
        protected virtual void Load(ScriptableData.FunctionData funcData)
        {
        }
        protected virtual void Load(ScriptableData.StatementData statementData)
        {
        }

        protected long m_StartTime = 0;
    }
}
