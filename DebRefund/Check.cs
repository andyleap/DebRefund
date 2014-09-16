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
        public static void CheckVersion(int ModID, Action<Version> action)
        {
            WebClient wc = new WebClient();

            wc.DownloadStringCompleted += (sender, e) =>
            {
                Dictionary<string, object> data = Json.Deserialize(e.Result) as Dictionary<string, object>;

                Version v = new Version { download_path = (string)data["download_path"], friendly_version = (string)data["friendly_version"], ksp_version = (string)data["ksp_version"], changelog = (string)data["changelog"] };

                action(v);
            };
            wc.DownloadStringAsync(new Uri("http://beta.kerbalstuff.com/api/mod/" + ModID.ToString() + "/latest"));
        }
    }
}
