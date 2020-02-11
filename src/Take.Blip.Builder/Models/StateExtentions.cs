﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Take.Blip.Builder.Models
{
    /// <summary>
    /// Extension methods of <see cref="State"/>
    /// </summary>
    public static class StateExtentions
    {
        public static bool InputExpirationTimeEnabled(this State state)
        {
            return state?.Input != null && state.Input?.IsExpirationTimeEnabled() == true;
        }
    }
}
