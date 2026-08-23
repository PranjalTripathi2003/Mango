using Azure.Messaging.ServiceBus;
using Mango.Services.EmailAPI.Model.Dto;
using System.Text;
using Newtonsoft.Json;
using Mango.Services.EmailAPI.Services;
using Mango.Services.EmailAPI.Message;

namespace Mango.Services.EmailAPI.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {

        private readonly IConfiguration _configuration;
        private readonly string serviceBusConnectionString;
        private readonly string emailCartQueue;

        private readonly string registerUserQueue;

        private readonly EmailService _emailService;

        private readonly string orderCreatedTopic;
        private readonly string orderCreated_Email_Subscription;

       

        private ServiceBusProcessor _emailCartProccessor;
        private ServiceBusProcessor _registerUserProcessor;

        private readonly ServiceBusProcessor _emailOrderPlacedProcessor;

        public AzureServiceBusConsumer(IConfiguration configuration, EmailService emailService)
        {
            _configuration = configuration;
            serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
            emailCartQueue = _configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue");
            registerUserQueue = _configuration.GetValue<string>("TopicAndQueueNames:RegisterUserQueue");
            orderCreatedTopic = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic");
            orderCreated_Email_Subscription = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreated_Email_Subscription");
            var client = new ServiceBusClient(serviceBusConnectionString);
            _emailCartProccessor = client.CreateProcessor(emailCartQueue);
            _registerUserProcessor = client.CreateProcessor(registerUserQueue);
            _emailOrderPlacedProcessor = client.CreateProcessor(orderCreatedTopic, orderCreated_Email_Subscription);
            _emailService = emailService;
        }
        private async Task OnEmailCartRequestReceived(ProcessMessageEventArgs args)
        {

            var message = args.Message;

            var body = Encoding.UTF8.GetString(message.Body);

            CartDto objMessage = JsonConvert.DeserializeObject<CartDto>(body);

            try
            {
                // try to log the mail
                await _emailService.EmailCartAndLog(objMessage);
                await args.CompleteMessageAsync(args.Message);

            }
            catch (Exception ex)
            {
                throw;

            }

        }
        private async Task OnRegisterUserRequestReceived(ProcessMessageEventArgs args)
        {

            var message = args.Message;

            var body = Encoding.UTF8.GetString(message.Body);

            string email = JsonConvert.DeserializeObject<string>(body);

            try
            {
               
                await _emailService.RegisterUserEmailAndLog(email);
                await args.CompleteMessageAsync(args.Message);

            }
            catch (Exception ex)
            {
                throw;

            }

        }

        private async Task OnOrderPlaceRequestReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;

            var body = Encoding.UTF8.GetString(message.Body);

            RewardMessage objMessage = JsonConvert.DeserializeObject<RewardMessage>(body);

            try
            {
                // try to log the mail
                await _emailService.LogOrderPlaced(objMessage);
                await args.CompleteMessageAsync(args.Message);

            }
            catch (Exception ex)
            {
                throw;

            }
        }


        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.Message);
            return Task.CompletedTask;
        }
        public async Task Start()
        {
            _emailCartProccessor.ProcessMessageAsync += OnEmailCartRequestReceived;
            _emailCartProccessor.ProcessErrorAsync += ErrorHandler;
            await _emailCartProccessor.StartProcessingAsync();

            _registerUserProcessor.ProcessMessageAsync += OnRegisterUserRequestReceived;
            _registerUserProcessor.ProcessErrorAsync += ErrorHandler;               
            await _registerUserProcessor.StartProcessingAsync();

            _emailOrderPlacedProcessor.ProcessMessageAsync += OnOrderPlaceRequestReceived;
            _emailOrderPlacedProcessor.ProcessErrorAsync += ErrorHandler;
            await _emailOrderPlacedProcessor.StartProcessingAsync();

        }

      

        public async Task Stop()
        {
            await _emailCartProccessor.StopProcessingAsync();
            await _emailCartProccessor.DisposeAsync();

            await _registerUserProcessor.StopProcessingAsync();
            await _registerUserProcessor.DisposeAsync();

            await _emailOrderPlacedProcessor.StopProcessingAsync();
            await _emailOrderPlacedProcessor.DisposeAsync();
        }
    }
}

