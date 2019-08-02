using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VRMOD.Controls
{
    public interface IShortcut : IDisposable
    {
        void Evaluate();
    }
}
