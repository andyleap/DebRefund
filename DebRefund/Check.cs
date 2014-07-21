using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using MiniJSON;

namespace KSVersionCheck
{
    public struct Version
    {
        public string download_path;
        public string friendly_version;
        public string ksp_version;
        public string changelog;
    }
    public class Check
    {
        public static Version CheckVersion(int ModID)
        {
            WebClient wc = new WebClient();

            string versionjson = wc.DownloadString("http://beta.kerbalstuff.com/api/mod/" + ModID.ToString() + "/latest_version");

            Dictionary<string, object> data = Json.Deserialize(versionjson) as Dictionary<string, object>;

            return new Version {download_path = (string)data["download_path"], friendly_version = (string)data["friendly_version"], ksp_version = (string)data["ksp_version"], changelog = (string)data["changelog"]};
        }
    }
}
