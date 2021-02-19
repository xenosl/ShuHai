using System.Linq;
using UnityEngine.LowLevel;

namespace ShuHai.Unity
{
    public static class PlayerLoopSystemUtil
    {
        public static void AddSubSystem(ref PlayerLoopSystem system, PlayerLoopSystem subSystem)
        {
            var list = system.subSystemList.ToList();
            list.Add(subSystem);
            system.subSystemList = list.ToArray();
        }

        public static void InsertSubSystem(ref PlayerLoopSystem system, int index, PlayerLoopSystem subSystem)
        {
            var list = system.subSystemList.ToList();
            list.Insert(index, subSystem);
            system.subSystemList = list.ToArray();
        }
    }
}
