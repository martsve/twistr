using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TwistrSolver.Interface
{
    interface ITwistDivider 
    {
        Outcome Invoke();
        
        bool HasChanged();
    }
}