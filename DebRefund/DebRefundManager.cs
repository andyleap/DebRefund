using System;
using UnityEngine;
using System.Linq;

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


                float drag = 0;
                float mass = 0;
                float cost = 0;
                if (!v.packed)
                    foreach (Part p in v.Parts)
                        p.Pack();
                foreach (ProtoPartSnapshot p in v.protoVessel.protoPartSnapshots)
                {
                    mass += p.mass;
                    cost += p.partInfo.cost;
                    print("DebRefund: " + p.partName);
                    ProtoPartModuleSnapshot pm = p.modules.FirstOrDefault(pms => pms.moduleName == "ModuleParachute");
                    if (pm != null)
                    {
                        ModuleParachute mp = (ModuleParachute)pm.moduleRef;
                        mp.Load(pm.moduleValues);
                        drag += mp.fullyDeployedDrag * p.mass;
                        print("DebRefund: " + " " + mp.fullyDeployedDrag + " " + mp.fullyDeployedDrag * p.mass);
                    }
                    foreach (ProtoPartResourceSnapshot pr in p.resources)
                    {
                        if (pr.resourceValues.HasValue("amount"))
                        {
                            float amt = float.Parse(pr.resourceValues.GetValue("amount"));
                            cost += (float)amt * pr.resourceRef.info.unitCost;
                            mass += (float)amt * pr.resourceRef.info.density;
                        }

                    }
                }

                print("DebRefund: " + drag + " " + mass);

                if (drag > mass * 70)
                {
                    float recFactor = CalculateRecoveryFactor(v);
                    if (drag > mass * 90)
                    {
                        Funding.Instance.Funds += recFactor * cost;
                        MessageSystem.Message m = new MessageSystem.Message(
                            "Debris landed safely",
                            String.Format("Debris was landed safely, {0} refunded", recFactor * cost),
                            MessageSystemButton.MessageButtonColor.GREEN,
                            MessageSystemButton.ButtonIcons.MESSAGE);
                        MessageSystem.Instance.AddMessage(m);
                    }
                    else
                    {
                        float damageFactor = Mathf.Lerp(0.9f, 0.5f, Mathf.InverseLerp(mass * 90, mass * 70, drag));
                        Funding.Instance.Funds += recFactor * cost * damageFactor;
                        MessageSystem.Message m = new MessageSystem.Message(
                            "Debris landed",
                            String.Format("Debris was landed with some damage, {0} refunded", recFactor * cost * damageFactor),
                            MessageSystemButton.MessageButtonColor.YELLOW,
                            MessageSystemButton.ButtonIcons.ALERT);
                        MessageSystem.Instance.AddMessage(m);
                    }

                }
                else
                {
                    MessageSystem.Message m = new MessageSystem.Message(
                        "Debris hit HARD",
                        String.Format("Debris hit hard, sorry, nothing to be salvaged"),
                        MessageSystemButton.MessageButtonColor.RED,
                        MessageSystemButton.ButtonIcons.ALERT);
                    MessageSystem.Instance.AddMessage(m);
                }
            }
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

