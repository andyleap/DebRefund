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

namespace DebRefund
{
    public class Settings
    {
        public static Settings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Settings();
                    _instance.Load();
                }
                return _instance;
            }
        }
        private static Settings _instance;
        protected String filePath = KSPUtil.ApplicationRootPath + "GameData/DebRefund/Config.txt";
        [Persistent]
        public float MinimumSpeedGreen;
        [Persistent]
        public float MinimumSpeedYellow;
        [Persistent]
        public float SafeRecoveryPercent;
        [Persistent]
        public float YellowMaxPercent;
        [Persistent]
        public float YellowMinPercent;
        [Persistent]
        public bool UpdateCheck;
        [Persistent]
        public List<string> IgnoredParts;

        public Settings()
        {
            MinimumSpeedGreen = 6;
            MinimumSpeedYellow = 10;
            SafeRecoveryPercent = 90;
            YellowMaxPercent = 90;
            YellowMinPercent = 40;
            UpdateCheck = false;
            IgnoredParts = new List<string> { "launchClamp1" };
        }

        public void Load()
        {
            if (System.IO.File.Exists(filePath))
            {
                ConfigNode cnToLoad = ConfigNode.Load(filePath);
                ConfigNode.LoadObjectFromConfig(this, cnToLoad);
            }
            this.Save();
        }

        public void Save()
        {
            ConfigNode cnTemp = ConfigNode.CreateConfigFromObject(this, new ConfigNode());
            cnTemp.Save(filePath);
        }
    }
}
