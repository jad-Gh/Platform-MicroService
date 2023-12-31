﻿using PlatformService.Dtos;
using System.Text;
using System.Text.Json;

namespace PlatformService.SyncDataServices.Http
{
    public class HttpCommandDataclient : iCommandDataClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public HttpCommandDataclient(HttpClient httpClient,IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }
        public async Task SendPlatformToCommand(PlatformReadDto plat)
        {
            var httpContent = new StringContent(
               JsonSerializer.Serialize(plat),
               Encoding.UTF8,
               "application/json"
               );

            var response = await _httpClient.PostAsync($"{_configuration["CommandService"]}/api/c/Platforms/", httpContent);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("--> Sync POST to CommandService was OK!");
            }
            else {
                Console.WriteLine("--> Sync POST to CommandService was NOT OK!");
            }

        }
    }
}
