//-----------------------------------------------------------------------
// <copyright file="mvVariable.cs">
//     Copyright (c) 2020 Kenneth Noyens
//     You may use, distribute and modify this code under the
//     terms of the LGPLv3 license.
//     You should have received a copy of the GNU LGPLv3 license with
//     this file. If not, please visit https://github.com/kennethnoyens/mCrestronVariables
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace magicVariables
{
    public class MagicVariable
    {
        //public string name;
        public bool autoSave;
        public string variableType;
        public string variableOptions;
        private string _variableValue;

        public string variableValue
        {
            get { return _variableValue; }
            set
            {
                _variableValue = value; 
                //OnVariableChanged();
            }
        }

        public delegate void mVariableChangeEventHandler(object source, EventArgs args);
        public event mVariableChangeEventHandler VariableChanged;

        public void Save()
        {
        }

        public void setVariableValue(string _value)
        {
            variableValue = _value;
        }

        public void setVariableValue(string _value, bool sendToSimpl)
        {
            variableValue = _value;
            if(sendToSimpl)
                OnVariableChanged();
        }

        protected virtual void OnVariableChanged()
        {
            if (VariableChanged != null)
                VariableChanged(this, EventArgs.Empty);
        }
    }
}