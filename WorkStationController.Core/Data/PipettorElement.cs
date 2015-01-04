using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkstationController.Core.Utility;

namespace WorkstationController.Core
{
    public abstract class PipettorElement : BindableBase, ISerialization, ISaveName, IDeserializationEx, ICloneable
    {
        public virtual void Serialize(string toXmlFile)
        {
            // Do nothing
            return;
        }
        public virtual string TypeName { get; set; }
        

        public virtual string SaveName
        {
            get { return TypeName; }
        }

        public virtual void DoExtraWork()
        {
            // Do nothing
            return;
        }

        public virtual object Clone()
        {
            // Do nothing
            return null;
        }
    }
}
