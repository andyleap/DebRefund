//The MIT License (MIT)
//
//Copyright (c) 2014 Andrew Leap
//
//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in
//all copies or substantial portions of the Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
/*
namespace DebRefund
{
    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    class DebRefundSettings : MonoBehaviour
    {
        void Awake()
        {
            //Register for the Ready event
            GameEvents.onGUIApplicationLauncherReady.Add(OnGUIAppLauncherReady);
            GameEvents.onGameSceneLoadRequested.Add(OnGameSceneLoadRequested);
            MinDrag = Settings.Instance.DragNeededYellow.ToString();
            RecDrag = Settings.Instance.DragNeededGreen.ToString();
        }
        void OnDestroy()
        {
            //Clean up
            GameEvents.onGUIApplicationLauncherReady.Remove(OnGUIAppLauncherReady);
            if (appButton != null)
                ApplicationLauncher.Instance.RemoveModApplication(appButton);
        }

        ApplicationLauncherButton appButton = null;
        void OnGUIAppLauncherReady()
        {
            if (ApplicationLauncher.Ready)
            {
                appButton = ApplicationLauncher.Instance.AddModApplication(
                    onAppLaunchToggleOn,
                    onAppLaunchToggleOff,
                    onAppLaunchHoverOn,
                    onAppLaunchHoverOff,
                    onAppLaunchEnable,
                    onAppLaunchDisable,
                    ApplicationLauncher.AppScenes.SPACECENTER,
                    (Texture)GameDatabase.Instance.GetTexture("DebRefund/AppIcon.png", false)
                );
            }
        }

        void OnGameSceneLoadRequested(GameScenes scene)
        {
            ApplicationLauncher.Instance.RemoveModApplication(appButton);
        }

        bool guiVisible = false;
        bool guiHover = false;

        void onAppLaunchToggleOn() { guiVisible = true; }
        void onAppLaunchToggleOff() { guiVisible = false; Settings.Instance.Save(); }
        void onAppLaunchHoverOn() { guiHover = true; }
        void onAppLaunchHoverOff() { guiHover = false; Settings.Instance.Save(); }
        void onAppLaunchEnable() {  }
        void onAppLaunchDisable() {  }

        string MinDrag;
        string RecDrag;

        void OnGUI()
        {
            if (guiVisible || guiHover)
            {
                Rect pos = new Rect(Screen.width - 220, 60, 200, 150);
                
                GUILayout.BeginArea(pos, GUI.skin.box);
                GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
                GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                GUILayout.Label("Minimum Drag Ratio");
                MinDrag = GUILayout.TextField(MinDrag, GUILayout.ExpandWidth(true));
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                GUILayout.Label("Recommended Drag Ratio");
                RecDrag = GUILayout.TextField(RecDrag, GUILayout.ExpandWidth(true));
                GUILayout.EndHorizontal();


                float.TryParse(MinDrag, out Settings.Instance.DragNeededYellow);
                float.TryParse(RecDrag, out Settings.Instance.DragNeededGreen);
            }
        }
    }
}
*/