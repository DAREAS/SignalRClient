using System;
using System.IO.Pipes;
using System.Threading.Tasks;
using AnalisysQueueAndData.Model;
using EventStore;
using Microsoft.AspNet.SignalR.Client;
using Newtonsoft.Json;

namespace AnalisysQueueAndData
{
    class Program
    {
        // constants 
        private const string Url = "http://eventstore.azurewebsites.net/eventstore/v1";
        private const string ServiceId = "svc_WNNuM4H1";
        private const string QueueId = "queue_letwrcB6";
        private const string Username = "usr_zjEhrIQ6";
        private const string Password = "pass_E97fLvuRP";

        static void Main(string[] args)
        {
            Console.WriteLine("Starting client  http://localhost:8089");

            var connection = new HubConnection("http://localhost:8089/");

            IHubProxy proxyHub = connection.CreateHubProxy("EventStoreHub");
            
            //proxyHub.On<string, string, string, string>("addMessage", (elementKey, elementName, elementmessage, elementBody) => Console.Write($"Add Message to Element {elementKey} with message {elementmessage} \n"));
            proxyHub.On<string, string, string>("addElement", (elementKey, elementName, elementBody) => Console.Write($"Add Element {elementKey} with name {elementName} \n"));

            connection.Start().Wait();

            //RunAsync(proxyHub).GetAwaiter();

            while (true)
            {
                string key = Console.ReadLine();

                if (key.ToUpper() == "W")
                {
                    proxyHub.Invoke("addElement", "123345678911000", "Element Name", JsonConvert.SerializeObject(new Element()).ToString()).ContinueWith(task =>
                    {
                        if (task.IsFaulted)
                        {
                            Console.WriteLine("!!! There was an error opening the connection:{0} \n", task.Exception.GetBaseException());
                        }

                        Console.WriteLine("");

                    }).Wait();
                }

                if (key.ToUpper() == "C")
                {
                    break;
                }
            }
        }

        private static async Task RunAsync(IHubProxy proxyHub)
        {
            SDK.Client = new EventStoreConfiguration(Url).SetAuthentication(Username, Password).SetService(ServiceId, "v1").Create();
            SDK.Client.Received += (sender, argsReceived) =>
            {
                var data = argsReceived.Message;
                var x = argsReceived.Message.GetData<object>();

                proxyHub.Invoke("addMessage", "client message", " sent from console client").ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        Console.WriteLine("!!! There was an error opening the connection:{0} \n", task.Exception.GetBaseException());
                    }

                    Console.WriteLine();

                }).Wait();
            };

            await SDK.Client.StartAsync(ServiceId, QueueId);
        }
    }
}
