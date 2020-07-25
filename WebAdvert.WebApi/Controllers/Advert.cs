using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using AdvertApi.Models;
using Amazon.DynamoDBv2;
using Amazon.Runtime.SharedInterfaces;
using Amazon.SimpleNotificationService;
using AutoMapper.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAdvert.WebApi.Services;
using Microsoft.Extensions.Configuration;
using AdvertApi.Models.Messages;
using Amazon.SimpleNotificationService.Model;
using Newtonsoft.Json;

namespace WebAdvert.WebApi.Controllers
{
    [Route("adverts/v1")]
    [ApiController]
    public class Advert : ControllerBase
    {
        private readonly IAdvertStorageService _advertStorageService;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;
        public Advert(IAdvertStorageService advertStorageService, Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            _advertStorageService = advertStorageService;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("Create")]
        [ProducesResponseType(400)]
        [ProducesResponseType(200,Type=typeof(CreateAdvertResponse))]
        public async Task<IActionResult> Create(AdvertModel model)
        {
            string recordId;
            try
            {
                recordId = await _advertStorageService.Add(model);
            }
            catch(KeyNotFoundException)
            {
                return new NotFoundResult();
            }
            catch (Exception exception)
            {
                return StatusCode(500, exception.Message);
            }

            return StatusCode(201, new CreateAdvertResponse { Id = recordId });
        }

        [HttpPut]
        [Route("Confirm")]
        [ProducesResponseType(400)]
        [ProducesResponseType(200,Type=typeof(CreateAdvertResponse))]
        public async Task<IActionResult> Confirm(ConfirmAdvertModel model)
        {
            try
            {
                await _advertStorageService.Confirm(model);
                await RaiseAdvertConfirmedMessage(model);
            }
            catch (KeyNotFoundException)
            {
                return new NotFoundResult();
            }
            catch (Exception exception)
            {
                return StatusCode(500, exception.Message);
            }

            return new OkResult();
        }

        private async Task RaiseAdvertConfirmedMessage(ConfirmAdvertModel model)
        {
            var topicArn = _configuration.GetValue<string>("TopicArn");

            using (var client = new AmazonSimpleNotificationServiceClient())
            {
                var message = new AdvertConfirmMessage
                {
                    Id = model.Id,
                    Title = "Title Example" //burada dbden alınır. Hızlı geçmek amacıyla alınmadı.
                };

                string serializedMessage = JsonConvert.SerializeObject(message);

                //await client.PublishAsync(topicArn, serializedMessage);
                
                // elastich search service cannot created in AWS so I dont save adverts to elastic search !

            }
        }
    }
}
