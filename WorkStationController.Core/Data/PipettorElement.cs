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
            throw new NotImplementedException();
        }
        public virtual string TypeName { get; set; }
        

        public virtual string SaveName
        {
            get { return TypeName; }
        }

        public virtual void DoExtraWork()
        {
            throw new NotImplementedException();
        }

        public virtual object Clone()
        {
            throw new NotImplementedException();
        }
    }
}
