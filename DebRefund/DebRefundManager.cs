using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace DebRefund
{
    [KSPAddon(KSPAddon.Startup.SpaceCentre, true)]
    public class DebRefundManager : MonoBehaviour
    {
        public void Awake()
        {
            DontDestroyOnLoad(this);

            print("DebRefund Awake");
            GameEvents.onVesselDestroy.Add(this.onVesselDestroy);
            
        }

        public void OnDestroy()
        {
            print("DebRefund Destroy");
            GameEvents.onVesselDestroy.Remove(this.onVesselDestroy);
        }

        public void onVesselDestroy(Vessel v)
        {
            print("DebRefund Vessel Destroyed");

            if (HighLogic.CurrentGame.Mode != Game.Modes.CAREER)
            {
                return;
            }

            bool nonAtmoKill = false;
            if (FlightGlobals.ActiveVessel != null)
            {
                Vector3d activePos = FlightGlobals.ActiveVessel.GetWorldPos3D();
                Vector3d pos = v.GetWorldPos3D();
                if ((activePos - pos).magnitude < 2000)
                {
                    nonAtmoKill = true;
                }
            }

            if (!HighLogic.LoadedSceneIsEditor && !v.isActiveVessel && (v.situation == Vessel.Situations.FLYING || v.situation == Vessel.Situations.SUB_ORBITAL) && v.orbit.referenceBody.bodyName.Equals("Kerbin") && (v.mainBody.GetAltitude(v.CoM) - (v.terrainAltitude < 0 ? 0 : v.terrainAltitude) > 10) && !nonAtmoKill)
            {
                bool RealChutes = AssemblyLoader.loadedAssemblies.Any(a => a.name.Contains("RealChute"));
                object MatLibraryInstance = null;
                Type matLibraryType = null;
                System.Reflection.MethodInfo matMethod = null;
                System.Reflection.PropertyInfo matDragProp = null;
                if (RealChutes)
                {
                    matLibraryType = AssemblyLoader.loadedAssemblies
                        .SelectMany(a => a.assembly.GetExportedTypes())
                        .SingleOrDefault(t => t.FullName == "RealChute.Libraries.MaterialsLibrary");
                    matMethod = matLibraryType.GetMethod("GetMaterial", new Type[] { typeof(string) });
                    MatLibraryInstance = matLibraryType.GetProperty("instance").GetValue(null, null);
                    
                    Type matDefType = AssemblyLoader.loadedAssemblies
                        .SelectMany(a => a.assembly.GetExportedTypes())
                        .SingleOrDefault(t => t.FullName == "RealChute.Libraries.MaterialDefinition");
                    matDragProp = matDefType.GetProperty("dragCoefficient");
                }

                float drag = 0;
                float mass = 0;
                float cost = 0;

                Dictionary<string, float> Resources = new Dictionary<string, float>();
                Dictionary<string, float> ResourceCosts = new Dictionary<string, float>();
                Dictionary<string, int> Parts = new Dictionary<string, int>();
                Dictionary<string, float> PartCosts = new Dictionary<string, float>();
                
                if (!v.packed)
                    foreach (Part p in v.Parts)
                        p.Pack();
                foreach (ProtoPartSnapshot p in v.protoVessel.protoPartSnapshots)
                {
                    mass += p.mass;
                    float dryCost;
                    float fuelCost;
                    ShipConstruction.GetPartCosts(p, p.partInfo, out dryCost, out fuelCost);
                    cost += dryCost;

                    if (!PartCosts.ContainsKey(p.partInfo.title))
                    {
                        PartCosts.Add(p.partInfo.title, dryCost);
                        Parts.Add(p.partInfo.title, 0);
                    }
                    Parts[p.partInfo.title] += 1;

                    print("DebRefund: " + p.partName);

                    if (RealChutes)
                    {
                        ProtoPartModuleSnapshot pm = p.modules.FirstOrDefault(pms => pms.moduleName == "RealChuteModule");
                        if (pm != null)
                        {
                            ConfigNode[] parachutes = pm.moduleValues.GetNodes("PARACHUTE");
                            foreach (ConfigNode chute in parachutes)
                            {
                                if (float.Parse(chute.GetValue("cutAlt")) < 0)
                                {
                                    float d = float.Parse(chute.GetValue("deployedDiameter"));
                                    float area = Mathf.PI * Mathf.Pow(d / 2, 2);
                                    string mat = chute.GetValue("material");
                                    object matObj = matMethod.Invoke(MatLibraryInstance, new object[] { mat });
                                    float dragC = (float)matDragProp.GetValue(matObj, null);

                                    drag += dragC * area;
                                }
                            }
                        }
                    }
                    else
                    {
                        ProtoPartModuleSnapshot pm = p.modules.FirstOrDefault(pms => pms.moduleName == "ModuleParachute");
                        if (pm != null)
                        {
                            ModuleParachute mp = (ModuleParachute)pm.moduleRef;
                            mp.Load(pm.moduleValues);
                            drag += mp.fullyDeployedDrag * p.mass;
                            print("DebRefund: " + " " + mp.fullyDeployedDrag + " " + mp.fullyDeployedDrag * p.mass);
                        }
                    }
                    foreach (ProtoPartResourceSnapshot pr in p.resources)
                    {
                        if (pr.resourceValues.HasValue("amount"))
                        {
                            float amt = float.Parse(pr.resourceValues.GetValue("amount"));
                            cost += (float)amt * pr.resourceRef.info.unitCost;
                            mass += (float)amt * pr.resourceRef.info.density;
                            if (!ResourceCosts.ContainsKey(pr.resourceName))
                            {
                                ResourceCosts.Add(pr.resourceName, pr.resourceRef.info.unitCost);
                                Resources.Add(pr.resourceName, 0);
                            }
                            Resources[pr.resourceName] += amt;
                        }

                    }
                }

                print("DebRefund: " + drag + " " + mass);


                StringBuilder partlist = new StringBuilder();

                partlist.AppendLine("Parts:");
                foreach(var kvp in Parts)
                {
                    partlist.AppendFormat("{0} x {1} @ {2} = {3}", kvp.Value, kvp.Key, PartCosts[kvp.Key], PartCosts[kvp.Key] * kvp.Value);
                    partlist.AppendLine();
                }

                if (Resources.Any(r => r.Value > 0))
                {
                    partlist.AppendLine("Resources:");
                    foreach (var kvp in Resources.Where(r => r.Value > 0))
                    {
                        partlist.AppendFormat("{0} x {1} @ {2} = {3}", kvp.Value, kvp.Key, ResourceCosts[kvp.Key], ResourceCosts[kvp.Key] * kvp.Value);
                        partlist.AppendLine();
                    }
                }


                if (drag > mass * Settings.Instance.DragNeededYellow)
                {
                    List<String> crew = RecoverKerbals(v);
                    float recFactor = CalculateRecoveryFactor(v);
                    if (drag > mass * Settings.Instance.DragNeededGreen)
                    {
                        float Science = RecoverScience(v, 0.95f);
                        StringBuilder Message = new StringBuilder();
                        Message.AppendLine("Debris was landed safely");
                        Message.Append(partlist.ToString());
                        Message.AppendFormat("{0} refunded({1:P2})", recFactor * cost, recFactor);
                        Message.AppendLine();
                        Message.AppendFormat("Science Recovered: {0}(95%)", Science);
                        Message.AppendLine();
                        foreach (string member in crew)
                        {
                            Message.AppendLine(member + " Recovered");
                        }
                        Funding.Instance.Funds += recFactor * cost;
                        MessageSystem.Message m = new MessageSystem.Message(
                            "Debris landed safely",
                            Message.ToString(),
                            MessageSystemButton.MessageButtonColor.GREEN,
                            MessageSystemButton.ButtonIcons.MESSAGE);
                        MessageSystem.Instance.AddMessage(m);
                    }
                    else
                    {
                        float damageFactor = Mathf.Lerp(0.9f, 0.5f, Mathf.InverseLerp(mass * 90, mass * 70, drag));
                        float Science = RecoverScience(v, damageFactor);
                        StringBuilder Message = new StringBuilder();
                        Message.AppendLine("Debris was landed with some damage");
                        Message.Append(partlist.ToString());
                        Message.AppendFormat("{0} refunded({1:P2})", recFactor * cost * damageFactor, recFactor * damageFactor);
                        Funding.Instance.Funds += recFactor * cost * damageFactor;
                        Message.AppendLine();
                        Message.AppendFormat("Science Recovered: {0}({1:P2})", Science, damageFactor);
                        Message.AppendLine();
                        foreach (string member in crew)
                        {
                            Message.AppendLine(member + " Recovered");
                        }
                        MessageSystem.Message m = new MessageSystem.Message(
                            "Debris landed",
                            Message.ToString(),
                            MessageSystemButton.MessageButtonColor.YELLOW,
                            MessageSystemButton.ButtonIcons.ALERT);
                        MessageSystem.Instance.AddMessage(m);
                    }
                    
                }
                else
                {
                    StringBuilder Message = new StringBuilder();
                    Message.AppendLine("Debris hit hard, sorry, nothing to be salvaged");
                    Message.Append(partlist.ToString());
                    MessageSystem.Message m = new MessageSystem.Message(
                        "Debris hit HARD",
                        Message.ToString(),
                        MessageSystemButton.MessageButtonColor.RED,
                        MessageSystemButton.ButtonIcons.ALERT);
                    MessageSystem.Instance.AddMessage(m);
                }
            }
        }

        public List<String> RecoverKerbals(Vessel v)
        {
            List<String> crew = new List<string>();
            foreach (ProtoCrewMember pcm in v.protoVessel.GetVesselCrew())
            {
                pcm.rosterStatus = ProtoCrewMember.RosterStatus.Available;
                crew.Add(pcm.KerbalRef.crewMemberName);
            }
            return crew;
        }

        public float RecoverScience(Vessel v, float Damage)
        {
            float SciRecovered = 0f;
            foreach (ProtoPartSnapshot p in v.protoVessel.protoPartSnapshots)
            {
                foreach (ProtoPartModuleSnapshot pm in p.modules)
                {
                    Debug.Log("DebRefund: " + pm.moduleValues.HasNode("ScienceData"));
                    foreach (ConfigNode sciNode in pm.moduleValues.GetNodes("ScienceData"))
                    {
                        ScienceData sci = new ScienceData(sciNode);

                        ScienceSubject sciSub = ResearchAndDevelopment.GetSubjectByID(sci.subjectID);

                        SciRecovered += ResearchAndDevelopment.Instance.SubmitScienceData(sci.dataAmount, sciSub, Damage);
                    }
                }
            }
            return SciRecovered;
        }

        public float CalculateRecoveryFactor(Vessel v)
        {
            var cb = SpaceCenter.Instance.cb;
            double dist = SpaceCenter.Instance.GreatCircleDistance(cb.GetRelSurfaceNVector(v.latitude, v.longitude));
            double max = cb.Radius * Math.PI;

            return Mathf.Lerp(0.88f, 0.9f, (float)(dist / max));

        }
    }
}

