using UnityEngine;

namespace ASGA.DS
{
    public partial class SpawnInfoData : BaseInfoData
    {
        public SpawnInfoData() { }

        public override void Dispose()
        {
            Debug.LogError($"@@ SpawnInfoData Dispose [{dataID}, {dataName}]");
        }
    }
}

