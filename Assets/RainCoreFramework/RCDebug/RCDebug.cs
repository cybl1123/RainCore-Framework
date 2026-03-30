using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RainCoreFramework
{
    public static class RCDebug
    {
        public static int CurrentDebugLevel;
        public static void WARN(string info, int DebugLevel = 9)
        {
            if (DebugLevel > CurrentDebugLevel)
                Debug.LogWarning($"<b>[RAINCORE FRAMEWORK]</b> :: WARNING :: {info}");
        }
        public static void NOTICE(string info, int DebugLevel = 5)
        {
            if (DebugLevel > CurrentDebugLevel)
                Debug.Log($"<b>[RAINCORE FRAMEWORK]</b> :: NOTICE :: {info}");
        }
        public static void NOTICE(string objectName,string info, int DebugLevel = 5)
        {
            if (DebugLevel > CurrentDebugLevel)
                Debug.Log($"<b>[RAINCORE FRAMEWORK]</b> :: NOTICE :: {objectName} :: {info}");
        }
        public static void LOG(string info, int DebugLevel = 3)
        {
            if (DebugLevel > CurrentDebugLevel)
                Debug.Log($"<b>[RAINCORE FRAMEWORK]</b> :: LOG :: {info}");
        }
        public static void LOG(string objectName, string info, int DebugLevel = 3)
        {
            if (DebugLevel > CurrentDebugLevel)
                Debug.Log($"<b>[RAINCORE FRAMEWORK]</b> :: LOG :: {objectName} :: {info}");
        }
        public static void SUCCEED(string eventString, int DebugLevel = 5)
        {
            if (DebugLevel > CurrentDebugLevel)
                Debug.Log($"<b>[RAINCORE FRAMEWORK]</b> :: SUCCEED :: {eventString}");
        }
        public static void LOG_ASSERTIOIN(string infoString, int DebugLevel = 7)
        {
            if (DebugLevel > CurrentDebugLevel)
                Debug.LogAssertion($"<b>[RAINCORE FRAMEWORK]</b> :: ASSERTION :: {infoString}");
        }
        public static void Null_Exception(string objectString, int DebugLevel = 7)
        {
            if (DebugLevel > CurrentDebugLevel)
                Debug.Log($"<b>[RAINCORE FRAMEWORK]</b> :: NUll REFERENCE EXCEPTION :: {objectString}");
        }
        public static void Null_Exception(string objectName, string info, int DebugLevel = 7)
        {
            if (DebugLevel > CurrentDebugLevel)
                Debug.Log($"<b>[RAINCORE FRAMEWORK]</b> :: NUll REFERENCE EXCEPTION :: {objectName} :: {info}");
        }
        public static void CONTAINER_FINDMISSING(string info, int DebugLevel = 5)
        {
            if (DebugLevel > CurrentDebugLevel)
                Debug.Log($"<b>[RAINCORE FRAMEWORK]</b> :: CONTANIER FINDMISSING :: {info}");
        }
    }
}
