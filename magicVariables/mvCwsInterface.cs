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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Crestron.SimplSharp.WebScripting;
using Newtonsoft.Json;

namespace magicVariables
{
    public class MvCwsInterface
    {
        private HttpCwsServer Server;

        public MvCwsInterface()
        {
            Server = new HttpCwsServer("/mv");    
            Server.HttpRequestHandler = new MvServerHandler(Server);
            Server.Routes.Add(new HttpCwsRoute("variables") { Name = "VARIABLES" });
            Server.Register();
        }
    }

    class MvServerHandler : IHttpCwsHandler
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
                    string action = context.Request.HttpMethod + context.Request.RouteData.Route.Name.ToUpper();
                    switch (action)
                    {
                        case "GETVARIABLES":
                            context.Response.ContentType = "application/json";
                            context.Response.Write(MvMain.getMagicVariablesSerialized(), true);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                context.Response.ContentType = "text/html";
                context.Response.StatusCode = 500;
                context.Response.Write("<HTML><HEAD>ERROR</HEAD><BODY>Internal server error</BODY></HEAD></HTML>", true);
            }
        }
    }
}