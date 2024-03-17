using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Axis.Native
{
#if UNITY_ANDROID && !UNITY_EDITOR
    public class AxisRuntimeNative
    {


        private AndroidJavaObject axisRuntime = null;
        private AndroidJavaObject activityContext = null;


        public void Start()
        {
            if (activityContext == null)
            {
                using (AndroidJavaClass activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                {
                    activityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
                }

                
            }
          
            Debug.Assert(activityContext != null, "Cannot find Activity Context");

            if(axisRuntime==null)
            {
                using (AndroidJavaClass plugin = new AndroidJavaClass("com.refract.axisruntime.AxisRuntimeBridge"))
                {
                    if (plugin != null)
                    {
                        axisRuntime = plugin.CallStatic<AndroidJavaObject>("instance");
                    }
                }
            }
            
            Debug.Assert(axisRuntime != null, "Cannot find Axis Runtime");
            axisRuntime.Call("start", activityContext);

        }
        public void Stop()
        {
            if (activityContext == null)
            {
                using (AndroidJavaClass activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                {
                    activityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
                }            
            }
            Debug.Assert(activityContext == null, "Cannot find Activity Context");

            if(axisRuntime==null)
            {
                using (AndroidJavaClass plugin = new AndroidJavaClass("com.refract.axisruntime.AxisRuntimeBridge"))
                {
                    if (plugin != null)
                    {
                        axisRuntime = plugin.CallStatic<AndroidJavaObject>("instance");
                    }            
                }
            }
            Debug.Assert(axisRuntime == null, "Cannot find Axis Runtime");
            axisRuntime.Call("stop", activityContext);
        }
    }

#else
    public class AxisRuntimeNative
    {
        public void Start()
        {
            Debug.Log("AxisRuntime Native Start() can only being calling in Android");
        }
        public void Stop()
        {
            Debug.Log("AxisRuntime Native Stop() can only being calling in Android");
        }
    }
#endif
}
