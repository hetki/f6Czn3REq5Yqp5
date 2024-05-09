using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Hetki.Helper
{
    public static class MonoHelper
    {
        private static readonly Dictionary<float, WaitForSeconds> waitDictionary = new Dictionary<float, WaitForSeconds>();
        private static readonly Dictionary<float, WaitForSecondsRealtime> waitRealtimeDictionary = new Dictionary<float, WaitForSecondsRealtime>();
        public static WaitForSeconds GetWait(float time)
        {
            if (waitDictionary.TryGetValue(time, out var wait)) return wait;

            waitDictionary[time] = new WaitForSeconds(time);
            return waitDictionary[time];
        }

        public static WaitForSecondsRealtime GetWaitRealtime(float time)
        {
            if (waitRealtimeDictionary.TryGetValue(time, out var wait)) return wait;

            waitRealtimeDictionary[time] = new WaitForSecondsRealtime(time);
            return waitRealtimeDictionary[time];
        }

        public static bool GetRandomElementFromList<T>(List<T> elements, out T element)
        {
            element = default(T);

            if (elements.Count > 0)
            {
                int randomNumber = UnityEngine.Random.Range(0, elements.Count);
                element = elements[randomNumber];
                return true;
            }

            return false;
        }

        private static Camera camera;
        public static Camera Camera
        {
            get
            {
                if (camera == null)
                    camera = Camera.main;

                return camera;
            }
        }

        public static void RefreshMainCamera()
        {
            camera = Camera.main;
        }

        public static void DeleteChildren(this Transform t)
        {
            foreach (Transform child in t) UnityEngine.Object.Destroy(child.gameObject);
        }

        public static string GenerateUID()
        {
            Guid uid = Guid.NewGuid();
            return uid.ToString();
        }

        public static void Log(object content, bool enabledDebugging)
        {
            if (enabledDebugging)
                Debug.Log(content);
        }

    }
}