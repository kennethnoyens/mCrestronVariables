//-----------------------------------------------------------------------
// <copyright file="mVariable.cs">
//     Copyright (c) 2020-2021 Kenneth Noyens
//     You may use, distribute and modify this code under the
//     terms of the LGPLv3 license.
//     You should have received a copy of the GNU LGPLv3 license with
//     this file. If not, please visit https://github.com/kennethnoyens/mCrestronVariables
// </copyright>
//-----------------------------------------------------------------------
using Crestron.SimplSharp;
using Crestron.SimplSharp.CrestronIO;
using Crestron.SimplSharp.WebScripting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace magicVariables
{
    public class MvCwsInterface
    {
        private HttpCwsServer Server;

        public MvCwsInterface()
        {
            Server = new HttpCwsServer("/mv");
            Server.HttpRequestHandler = new MvServerHandler(Server);
            Server.Routes.Add(new HttpCwsRoute("vars") { Name = "VARS.LIST" });
            Server.Routes.Add(new HttpCwsRoute("vars/{name}") { Name = "VARS.NAME" });
            Server.Register();
        }
    }

    internal class MvServerHandler : IHttpCwsHandler
    {
        private HttpCwsServer Server;
        public MvServerHandler(HttpCwsServer _Server)
        {
            this.Server = _Server;
        }

        void IHttpCwsHandler.ProcessRequest(HttpCwsContext context)
        {
            try
            {
                if (context.Request.RouteData == null)
                {
                    return;
                }
                else
                {
                    switch (context.Request.HttpMethod + context.Request.RouteData.Route.Name.ToUpper())
                    {
                        case "GETVARS.LIST":
                            context.Response.ContentType = "application/json";
                            context.Response.Write(MvMain.getMagicVariablesSerialized(), true);
                            break;
                        case "GETVARS.NAME":
                            String findName = Convert.ToString(context.Request.RouteData.Values["name"]);
                            context.Response.ContentType = "application/json";
                            context.Response.Write(MvMain.getMagicVariablesSerialized(findName), true);
                            break;
                        case "PUTVARS.NAME":
                            if (context.Request.InputStream != null && context.Request.InputStream.Length > 0)
                            {
                                String name = Convert.ToString(context.Request.RouteData.Values["name"]);
                                Byte[] rawNewValue = new Byte[(int)context.Request.InputStream.Length];
                                context.Request.InputStream.Read(rawNewValue, 0, (int)context.Request.InputStream.Length);
                                MvMain.GetMagicVariable(name).setVariableValue(System.Text.Encoding.UTF8.GetString(rawNewValue, 0, (int)context.Request.InputStream.Length), true);
                                //CrestronConsole.PrintLine("MV PUTVARS " + name + ": " + MvMain.GetMagicVariable(name).variableValue);
                            }
                            else
                            {
                                context.Response.StatusCode = 411;
                                context.Response.Write("No content found", true);
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                context.Response.ContentType = "text/html";
                context.Response.StatusCode = 500;
                context.Response.Write("<HTML><HEAD>ERROR</HEAD><BODY>Internal server error</BODY></HEAD></HTML>", true);
                CrestronConsole.PrintLine("Error while processing http cws request: " + ex.Message);
            }
        }
    }
}