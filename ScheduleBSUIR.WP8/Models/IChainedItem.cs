using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBSUIR.Models
{
    public interface IChainedItem
    {
        IChainedItem Next();
        IChainedItem Previous();
    }
}
