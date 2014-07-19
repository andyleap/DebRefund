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
        public float DragNeededGreen;
        [Persistent]
        public float DragNeededYellow;

        public Settings()
        {
            DragNeededGreen = 90;
            DragNeededYellow = 70;
        }

        public void Load()
        {
            if (System.IO.File.Exists(filePath))
            {
                ConfigNode cnToLoad = ConfigNode.Load(filePath);
                ConfigNode.LoadObjectFromConfig(this, cnToLoad);
            }
        }

        public void Save()
        {
            ConfigNode cnTemp = ConfigNode.CreateConfigFromObject(this, new ConfigNode());
            cnTemp.Save(filePath);
        }
    }
}
