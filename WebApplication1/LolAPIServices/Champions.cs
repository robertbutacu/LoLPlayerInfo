﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using WebApplication1.Models;

namespace WebApplication1.LolAPIServices
{
    public class Champions
    {
        public ChampionsListDto GetAll()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://eun1.api.riotgames.com");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = client.GetAsync($"/lol/static-data/v3/champions?locale=en_US&dataById=false&api_key={Globals.API_KEY}").Result;
            if (response.IsSuccessStatusCode)
            {
                string responseString = response.Content.ReadAsStringAsync().Result;
                var modelObject = response.Content.ReadAsAsync<ChampionsListDto>().Result;
                /*foreach(var champion in modelObject.data)
                {
                    System.Diagnostics.Debug.WriteLine("test");
                    System.Diagnostics.Debug.WriteLine(champion.Value.name);
                }*/
                System.Diagnostics.Debug.WriteLine("test");
                return modelObject;
            }
            else
                return null;
        }
    }
}