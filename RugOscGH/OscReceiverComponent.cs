using Grasshopper.Kernel;
using System;
using System.Net;
using Rug.Osc;

namespace RugOscGH
{
    public class OscReceiverComponent : GH_Component
    {
        public OscReceiver receiver;
        public bool running = false;
        public string pdata = "";

        public OscReceiverComponent()
          : base("OscReceiver", "OscReceiver",
              "OSC Receiver",
              "RugOsc", "OSC")
        {
        }


        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Receive", "R", "Start receiving data.", GH_ParamAccess.item, false);
            pManager.AddBooleanParameter("Keep Last Data", "K", "Keep (Preserve) last received data.", GH_ParamAccess.item, false);
            pManager.AddTextParameter("IP Address", "IP", "IP address", GH_ParamAccess.item, "127.0.0.1");
            pManager.AddIntegerParameter("Port", "PN", "Port number.", GH_ParamAccess.item, 12345);
            pManager.AddIntegerParameter("Packet Size", "PS", "Packet size.", GH_ParamAccess.item, OscReceiver.DefaultPacketSize);
            pManager.AddIntegerParameter("Buffer Size", "BS", "Buffer size.", GH_ParamAccess.item, OscReceiver.DefaultMessageBufferSize);
            
        }


        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Data", "D", "Received data.", GH_ParamAccess.item);

        }


        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool run = false;
            bool keep = false;
            IPAddress ipaddress = null;
            string ipaddressStr = "";
            int port = 12345;
            int packetsize = OscReceiver.DefaultPacketSize;
            int buffersize = OscReceiver.DefaultMessageBufferSize;

            if (!DA.GetData<bool>(0, ref run)) return;
            if (!DA.GetData<bool>(1, ref keep)) return;
            if (!DA.GetData<string>(2, ref ipaddressStr)) return;
            IPAddress.TryParse(ipaddressStr, out ipaddress);
            if (ipaddress == null)
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "IP Address is not correct.");
                return;
            }

            if (!DA.GetData<int>(3, ref port)) return;
            if (!DA.GetData<int>(4, ref packetsize)) return;
            if (!DA.GetData<int>(5, ref buffersize)) return;


            if (run == true && running == false)
            {
                if (receiver != null)
                {
                    if (receiver.State == OscSocketState.Connected)
                    {
                        receiver.Close();
                    }
                }

                receiver = new OscReceiver(ipaddress, port, buffersize, packetsize);
                try
                {
                    receiver.Connect();
                    running = true;

                    this.ExpireSolution(true);
                }catch(Exception e)
                {
                    this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Could not connect.");
                }
            }
            else if (run == true && running == true)
            {
                if (receiver != null)
                {

                    if (receiver.State == OscSocketState.Connected)
                    {

                        OscPacket packet;
                        if (receiver.TryReceive(out packet))
                        {
                            pdata = packet.ToString();

                            if (!keep)
                            {
                                DA.SetData(0, pdata);
                            }
                        }

                    }

                    if (keep)
                    {
                        DA.SetData(0, pdata);
                    }


                    this.ExpireSolution(true);
                }
            }
            else
            {
                if (receiver != null)
                {
                    if (receiver.State == OscSocketState.Connected)
                    {
                        receiver.Close();
                    }
                }
                running = false;
            }
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.receive;
            }
        }


        public override Guid ComponentGuid
        {
            get { return new Guid("4874b01d-2c80-4a8a-b8e6-63842b5b01ae"); }
        }
    }
}
