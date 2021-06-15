using Grasshopper.Kernel;
using System;
using System.Drawing;

namespace RugOscGH
{
    public class RugOscGHInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "RugOscGH";
            }
        }
        public override Bitmap Icon
        {
            get
            {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return null;
            }
        }
        public override string Description
        {
            get
            {
                //Return a short string describing the purpose of this GHA library.
                return "";
            }
        }
        public override Guid Id
        {
            get
            {
                return new Guid("58b4de6c-8b14-46fb-91dd-b0b01a567c3a");
            }
        }

        public override string AuthorName
        {
            get
            {
                //Return a string identifying you or your company.
                return "";
            }
        }
        public override string AuthorContact
        {
            get
            {
                //Return a string representing your preferred contact details.
                return "";
            }
        }
    }
}
