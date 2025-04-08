using System;
using UnityEngine;

/// <summary>
/// 데이터 클래스
/// </summary>
namespace ASGA.DS
{
    public partial class BaseInfoData : IDisposable
    {
        public BaseInfoData() { }

        public virtual void Dispose()
        {

        }
        public int dataID { get; set; }
        public string dataName { get; set; }
        public int objectType { get; set; }
    }

    public partial class SkillInfoData : BaseInfoData
    {
        public SkillInfoData() { }

        public override void Dispose()
        {

        }

    }

    public partial class CharacterInfoData : BaseInfoData
    {
        public CharacterInfoData() { }

        public override void Dispose()
        {

        }

    }
}



