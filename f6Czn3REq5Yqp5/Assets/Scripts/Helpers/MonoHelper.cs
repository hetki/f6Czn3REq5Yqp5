using System.Collections.Generic;
using UnityEngine;

namespace Hetki.Helper
{
    public static class MonoHelper
    {
        private static readonly Dictionary<float, WaitForSeconds> waitDictionary = new Dictionary<float, WaitForSeconds>();
        private static readonly Dictionary<float, WaitForSecondsRealtime> waitRealtimeDictionary = new Dictionary<float, WaitForSecondsRealtime>();

        /// <summary>
        /// Get cached WaitForSeconds
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static WaitForSeconds GetWait(float time)
        {
            if (waitDictionary.TryGetValue(time, out var wait)) return wait;

            waitDictionary[time] = new WaitForSeconds(time);
            return waitDictionary[time];
        }

        /// <summary>
        /// Write console Log which adheres to DebugSettings
        /// </summary>
        /// <param name="content"></param>
        public static void Log(object content)
        {
            if (DebugSettings.LogState())
                Debug.Log(content);
        }

        /// <summary>
        /// Write console LogWarning which adheres to DebugSettings
        /// </summary>
        /// <param name="content"></param>
        public static void LogWarning(object content)
        {
            if (DebugSettings.LogState())
                Debug.LogWarning(content);
        }

        /// <summary>
        /// Write console LogError which adheres to DebugSettings
        /// </summary>
        /// <param name="content"></param>
        public static void LogError(object content)
        {
            if (DebugSettings.LogState())
                Debug.LogError(content);
        }

        /// <summary>
        /// Convert string to card layout
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static CVector2 StringToCardLayout(string value) 
        {
            string[] split = value.Split(",");
            return new CVector2(int.Parse(split[0]), int.Parse(split[1]));
        }

    }
}