using Crestron.SimplSharp;
using Crestron.SimplSharpPro;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot;
using static System.Net.Mime.MediaTypeNames;
using Crestron.SimplSharp.Net;
using System;
using File = System.IO.File;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Net;
using Newtonsoft.Json.Linq;

namespace varbot
{
    public class ControlSystem : CrestronControlSystem
    {
        TelegramBotClient botClient;
        CancellationTokenSource cts = new ();
        HttpClient client = new HttpClient();
        Urls urls;
        HttpClient http;

        public ControlSystem()
        {
            botClient = new TelegramBotClient("");
            urls = new Urls();
            http = new HttpClient();

            try
            {
                Crestron.SimplSharpPro.CrestronThread.Thread.MaxNumberOfUserThreads = 20;
                //Subscribe to the controller events (System, Program, and Ethernet)
                CrestronEnvironment.ProgramStatusEventHandler += new ProgramStatusEventHandler(ControllerProgramEventHandler);

                CrestronConsole.AddNewConsoleCommand(AddMagicUrl, "addmagicurl", "Add cws URL", ConsoleAccessLevelEnum.AccessAdministrator);
                CrestronConsole.AddNewConsoleCommand(ListMagicUrl, "listmagicurls", "List cws URLs added to processor", ConsoleAccessLevelEnum.AccessAdministrator);
            }
            catch (Exception e)
            {
                ErrorLog.Error("Error in the constructor: {0}", e.Message);
            }
        }

        public override void InitializeSystem()
        {
            try
            {
                // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
                ReceiverOptions receiverOptions = new()
                {
                    AllowedUpdates = Array.Empty<UpdateType>() // receive all update types except ChatMember related updates
                };

                botClient.StartReceiving(
                    updateHandler: HandleUpdateAsync,
                    pollingErrorHandler: HandlePollingErrorAsync,
                    receiverOptions: receiverOptions,
                    cancellationToken: cts.Token
                );

                client.DefaultRequestHeaders.Add("Connection", "close");
                client.DefaultRequestHeaders.Add("Accept", "*/*");

            }
            catch (Exception e)
            {
                ErrorLog.Error("Error in InitializeSystem: {0}", e.Message);
            }
        }

        private void AddMagicUrl(string cmdParameters)
        {
            if (cmdParameters == null) 
            {
                CrestronConsole.ConsoleCommandResponse("Use addmagicurl <url>");
                return;
            };

            if (cmdParameters.Contains(" "))
            {
                CrestronConsole.ConsoleCommandResponse("You should only add one argument to the addmagicurl command\nUse addmagicurl <url>");
                return;
            };

            urls.AddUrl(cmdParameters);

            CrestronConsole.ConsoleCommandResponse("Url has been added");
        }

        private void ListMagicUrl(string cmdParameters)
        {
            CrestronConsole.ConsoleCommandResponse("List magic urls:\n");

            foreach(Url url in urls) 
            {
                CrestronConsole.ConsoleCommandResponse($"Magic Url: {url.url}\n");
            }
        }

        async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            // Only process Message updates: https://core.telegram.org/bots/api#message
            if (update.Message is not { } message)
                return;
            // Only process text messages
            if (message.Text is not { } messageText)
                return;

            var chatId = message.Chat.Id;

            CrestronConsole.PrintLine($"Received a '{messageText}' message in chat {chatId}.");

            Message sentMessage = await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: processMessage(messageText),
                cancellationToken: cancellationToken);
        }

        string processMessage(string messageText)
        {
            string responseText = new string("test");
            //String[] messageTextArguments = messageText.Split(' ');

            if (messageText == "/getlist")
            {
                return GetListVariables();
            }

            return responseText;
        }

        private string GetListVariables()
        {
            string variableList = new string("List of variables:\n");
            
            foreach (Url url in urls) 
            {
                HttpResponseMessage response = http.Send(new HttpRequestMessage(HttpMethod.Get, url.url));
                JObject jsonObject = JObject.Parse(response.Content.ReadAsStringAsync().GetAwaiter().GetResult());

                foreach (var variableItem in jsonObject)
                {
                    variableList += variableItem.Key;
                    foreach (var variableInformation in JObject.Parse(variableItem.Value.ToString()))
                    {
                        if (variableInformation.Key == "variableValue")
                            variableList += " : " + variableInformation.Value + "\n";
                    }
                }
            }

            return variableList;
        }

        Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            ErrorLog.Error($"Polling error: {ErrorMessage}");
            return Task.CompletedTask;
        }


        void ControllerProgramEventHandler(eProgramStatusEventType programStatusEventType)
        {
            switch (programStatusEventType)
            {
                case (eProgramStatusEventType.Paused):
                    //The program has been paused.  Pause all user threads/timers as needed.
                    break;
                case (eProgramStatusEventType.Resumed):
                    //The program has been resumed. Resume all the user threads/timers as needed.
                    break;
                case (eProgramStatusEventType.Stopping):
                    //The program has been stopped.
                    //Close all threads. 
                    //Shutdown all Client/Servers in the system.
                    //General cleanup.
                    //Unsubscribe to all System Monitor events
                    // Send cancellation request to stop bot
                    cts.Cancel();
                    break;
            }

        }
    }
}
