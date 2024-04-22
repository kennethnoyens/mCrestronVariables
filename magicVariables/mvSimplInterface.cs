//-----------------------------------------------------------------------
// <copyright file="mVariable.cs">
//     Copyright (c) 2020 Kenneth Noyens
//     You may use, distribute and modify this code under the
//     terms of the LGPLv3 license.
//     You should have received a copy of the GNU LGPLv3 license with
//     this file. If not, please visit https://github.com/kennethnoyens/mCrestronVariables
// </copyright>
//-----------------------------------------------------------------------
using Crestron.SimplSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace magicVariables
{
    /// <summary>
    /// Class used to interface between the variables (mVariable) and SIMPL Windows.
    /// </summary>
    public class MvSimplInterface
    {
        public MagicVariable mv = null;
        public delegate void DelegateVariableChanged(SimplSharpString newValue);
        public DelegateVariableChanged VariableChanged
        {
            get;
            set;
        }

        public MvSimplInterface()
        {
            VariableChanged = null;
        }

        public void Name(string _name)
        {
            mv = MvMain.GetMagicVariable(_name);
            mv.VariableChanged += new MagicVariable.mVariableChangeEventHandler(mv_VariableChanged);
        }

        void mv_VariableChanged(object source, EventArgs args)
        {
            if (VariableChanged != null)
                VariableChanged(mv.variableValue);
        }

        public void setVariableType(string type)
        {
            if (mv != null)
                mv.variableType = type;
        }

        public void setVariableOptions(string options)
        {
            if (mv != null)
                mv.variableOptions = options;
        }

        public void setValue(string value)
        {
            if (mv != null)
                mv.setVariableValue(value);
        }
    }
}