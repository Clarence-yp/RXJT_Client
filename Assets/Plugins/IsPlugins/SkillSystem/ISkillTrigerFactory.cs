using System;
using System.Collections.Generic;

namespace SkillSystem
{
    public interface ISkillTrigerFactory
    {
        ISkillTriger Create(ScriptableData.ISyntaxComponent trigerConfig);
    }
    public class SkillTrigerFactoryHelper<T> : ISkillTrigerFactory where T : ISkillTriger, new()
    {
        public ISkillTriger Create(ScriptableData.ISyntaxComponent trigerConfig)
        {
            T t = new T();
            t.Init(trigerConfig);
            return t;
        }
    }
}
