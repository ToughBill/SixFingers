using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkstationController.Core.Data
{
    public interface IPipettorCommand
    {
        string Name { get; set; }
    }
}
