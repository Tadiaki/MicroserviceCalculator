﻿using CalculatorService.DTO_s;
using CalculatorService.Enums;
using EasyNetQ;
using OpenTelemetry.Context.Propagation;
using OpenTelemetry;
using System.Diagnostics;
using CalculatorService.Helpers;
using CalculatorService.Entities;
using CalculatorService.Data.Handlers;
using System;
using System.Threading.Tasks;
using System.Threading;



namespace CalculatorService.Services
{
    public class Calculator
    {
        private readonly CalculatorHandler _calcHandler;
        private readonly IHttpClientFactory _clientFactory;

        public Calculator(CalculatorHandler calcHandler, IHttpClientFactory clientFactory)
        {
            _calcHandler = calcHandler;
            _clientFactory = clientFactory;
        }

        internal async Task SendCalculationRequestAsync(CalculationRequestDTO calcReqDTO)
        {
            using (var activity = Monitoring.ActivitySource.StartActivity())
            {
                var bus = RabbitHutch.CreateBus("host=localhost;username=guest;password=guest");

                // pub
                var message = (calcReqDTO);

                var activityContext = activity?.Context ?? Activity.Current?.Context ?? default;
                var propagationContext = new PropagationContext(activityContext, Baggage.Current);
                var propagator = new TraceContextPropagator();
                propagator.Inject(propagationContext, message.Headers, (headers, key, value) => headers.Add(key, value));

                var topic = "";
                if (calcReqDTO.CalculationType == Enums.CalculationType.Addition)
                {
                    topic = "addition";
                }
                else
                {
                    topic = "subtraction";
                }

                await bus.PubSub.PublishAsync(message, typeof(CalculationRequestDTO), topic);
            }
        }


    }
}