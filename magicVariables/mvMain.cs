//-----------------------------------------------------------------------
// <copyright file="mVariable.cs">
//     Copyright (c) 2020 Kenneth Noyens
//     You may use, distribute and modify this code under the
//     terms of the LGPLv3 license.
//     You should have received a copy of the GNU LGPLv3 license with
//     this file. If not, please visit https://github.com/kennethnoyens/mCrestronVariables
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Text;
using System.Collections.Generic;
using Crestron.SimplSharp;
using Newtonsoft.Json;

namespace magicVariables
{
    static public class MvMain
    {
        static private Dictionary<string, MagicVariable> variables = new Dictionary<string, MagicVariable>();
        static private CCriticalSection cs = new CCriticalSection();

        static public string getMagicVariablesSerialized()
        {
            cs.Enter();
            string jsonResult = JsonConvert.SerializeObject(MvMain.variables);
            cs.Leave();
            return jsonResult;
        }

        static public string getMagicVariablesSerialized(String search)
        {
            Dictionary<string, MagicVariable> searchResult = new Dictionary<string, MagicVariable>();

            cs.Enter();
            
            foreach (KeyValuePair<string, MagicVariable> variable in variables)
            {
                if (variable.Key.ToString().Contains(search))
                {
                    searchResult.Add(variable.Key, variable.Value);
                }
            }
            cs.Leave();

            string jsonResult = JsonConvert.SerializeObject(searchResult);
            return jsonResult;
        }

        static public MagicVariable GetMagicVariable(string name)
        {
            MagicVariable variable;

            cs.Enter();

            if (!variables.TryGetValue(name, out variable))
            {
                variable = new MagicVariable();
                variables.Add(name, variable);
            }

            cs.Leave();

            return variable;
        }

        static public bool CheckMagicVariableExists(string name)
        {
            return variables.ContainsKey(name);
        }
    }
}
