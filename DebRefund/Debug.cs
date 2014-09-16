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

#if DEBUG
//This will kick us into the save called default and set the first vessel active
[KSPAddon(KSPAddon.Startup.MainMenu, false)]
public class Debug_AutoLoadPersistentSaveOnStartup : MonoBehaviour
{
    //use this variable for first run to avoid the issue with when this is true and multiple addons use it
    public static bool first = true;
    public void Start()
    {
        //only do it on the first entry to the menu
        if (first)
        {
            first = false;
            HighLogic.SaveFolder = "default";
            Game game = GamePersistence.LoadGame("persistent", HighLogic.SaveFolder, true, false);
            
            if (game != null && game.flightState != null && game.compatible)
            {
                game.Start();
                /*Int32 FirstVessel;
                Boolean blnFoundVessel = false;
                for (FirstVessel = 0; FirstVessel < game.flightState.protoVessels.Count; FirstVessel++)
                {
                    //This logic finds the first non-asteroid vessel
                    if (game.flightState.protoVessels[FirstVessel].vesselType != VesselType.SpaceObject &&
                        game.flightState.protoVessels[FirstVessel].vesselType != VesselType.Unknown)
                    {
                        ////////////////////////////////////////////////////
                        //PUT ANY OTHER LOGIC YOU WANT IN HERE//
                        ////////////////////////////////////////////////////
                        blnFoundVessel = true;
                        break;
                    }
                }
                if (!blnFoundVessel)
                    FirstVessel = 0;
                FlightDriver.StartAndFocusVessel(game, FirstVessel);*/
            }

            //CheatOptions.InfiniteFuel = true;
        }
    }
}
#endif