using Grasshopper.Kernel;
using System;
using System.Net;
using System.Collections.Generic;
using Rug.Osc;
using Grasshopper.Kernel.Types;

namespace RugOscGH
{
    public class OscSenderComponent : GH_Component
    {
        public OscSender sender;
        public bool running = false;

        public OscSenderComponent()
          : base("OscSender", "OscSender",
              "OSC Sender",
              "RugOsc", "OSC")
        {
        }


        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Send", "S", "Start sending data.", GH_ParamAccess.item, false);
            pManager.AddTextParameter("Local IP Address", "LIP", "IP address", GH_ParamAccess.item, "127.0.0.1");
            pManager.AddIntegerParameter("Local Port", "LP", "Port number.", GH_ParamAccess.item, 10000);
            pManager.AddTextParameter("Remote IP Address", "RIP", "Remote IP Address", GH_ParamAccess.item, "127.0.0.1");
            pManager.AddIntegerParameter("Remote Port", "RP", "Port number.", GH_ParamAccess.item, 12345);
            pManager.AddGenericParameter("Data", "D", "Sending data.", GH_ParamAccess.list);
            pManager.AddTextParameter("Tag", "T", "Data tag.", GH_ParamAccess.item, "data");
            pManager.AddIntegerParameter("Packet Size", "PS", "Packet size.", GH_ParamAccess.item, OscReceiver.DefaultPacketSize);
            pManager.AddIntegerParameter("Buffer Size", "BS", "Buffer size.", GH_ParamAccess.item, OscReceiver.DefaultMessageBufferSize);
        }


        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Data", "D", "Data", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool run = false;
            IPAddress localipaddress = null;
            IPAddress remoteipaddress = null;
            string localipaddressStr = "";
            string remoteipaddressStr = "";
            int remoteport = 12345;
            int localport = 10000;
            int packetsize = OscReceiver.DefaultPacketSize;
            int buffersize = OscReceiver.DefaultMessageBufferSize;
            List<object> sdatas = new List<object>();
            string tag = "";

            if (!DA.GetData<bool>(0, ref run)) return;
            if (!DA.GetData<string>(1, ref localipaddressStr)) return;
            IPAddress.TryParse(localipaddressStr, out localipaddress);
            if (localipaddress == null)
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Local IP Address is not correct.");
                return;
            }

            if (!DA.GetData<int>(2, ref localport)) return;
            if (!DA.GetData<string>(3, ref remoteipaddressStr)) return;
            IPAddress.TryParse(remoteipaddressStr, out remoteipaddress);
            if (remoteipaddress == null)
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Remote IP Address is not correct.");
                return;
            }
            if (!DA.GetData<int>(4, ref remoteport)) return;
            if (!DA.GetDataList<object>(5, sdatas)) sdatas = new List<object>();
            if (!DA.GetData<string>(6, ref tag)) return;
            if (!DA.GetData<int>(7, ref packetsize)) return;
            if (!DA.GetData<int>(8, ref buffersize)) return;


            if (run == true) {
                
                using (OscSender sender = new OscSender(localipaddress, localport, remoteipaddress, remoteport, OscSender.DefaultMulticastTimeToLive, buffersize, packetsize))
                {
                    sender.Connect();

                    List<object> datas = new List<object>();
                    foreach (var obj in sdatas)
                    {

                        if (
                            (obj is int) ||
                            (obj is long) ||
                            (obj is float) ||
                            (obj is double) ||
                            (obj is string) ||
                            (obj is bool) ||
                            (obj is OscNull) ||
                            (obj is OscColor) ||
                            (obj is OscSymbol) ||
                            (obj is OscTimeTag) ||
                            (obj is OscMidiMessage) ||
                            (obj is OscImpulse) ||
                            (obj is byte) ||
                            (obj is byte[]))
                        {
                            datas.Add(obj);
                        }
                        else if (obj is GH_Number)
                        {
                            datas.Add(((GH_Number)obj).Value);
                        }
                        else if (obj is GH_String)
                        {
                            datas.Add(((GH_String)obj).Value);
                        }
                        else if (obj is GH_Integer)
                        {
                            datas.Add(((GH_Integer)obj).Value);
                        }
                        else if (obj is GH_Boolean)
                        {
                            datas.Add(((GH_Boolean)obj).Value);
                        }
                    }

                    try
                    {
                        sender.Send(new OscMessage("/" + tag, datas.ToArray()));
                        DA.SetDataList(0, datas);
                    }
                    catch (Exception e)
                    {
                        this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, e.ToString());
                    }
                }

            }
        }


        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.send;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("41edf393-194e-4b86-9fb5-f8d615616ff8"); }
        }
    }
}