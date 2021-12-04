using GithubSearchApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GithubSearchApi.Services
{
    public class GithubSearchService
    {
        private const string URL = @"https://api.github.com/search/repositories";
        public string GetRepos(string q)
        {
            string res = string.Empty;
            //set querystring parameters
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("?q={0}", q);


            string urlParameters = sb.ToString();
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(URL);
                    client.DefaultRequestHeaders.Accept.Clear();
                    // Add an Accept header for JSON format.
                    client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                    //Add user-agent for Reading "Forbidden" Web Pages
                    client.DefaultRequestHeaders.Add("user-agent",
                           "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");
                    // List data response.
                    HttpResponseMessage response = client.GetAsync(urlParameters).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        res = response.Content.ReadAsStringAsync().Result;
                        //string jsonString = File.ReadAllText(fileName);
                        //weatherForecast = JsonSerializer.Deserialize<WeatherForecast>(jsonString);
                        ReposFull repos = JsonConvert.DeserializeObject<ReposFull>(res);
                    }
                    else
                    {
                        Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
                    }

                }

            }
            catch (Exception ex)
            {
                //TODO
                string msg = ex.Message;
            }
            return res;

        }
    }
}
