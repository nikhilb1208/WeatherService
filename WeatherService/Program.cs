using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace WeatherService
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Enter ZipCode: ");
                string query = Console.ReadLine();
                //610acf4c1d203448cd6f671955c5e8aa
                string key = "610acf4c1d203448cd6f671955c5e8aa";

                var result = GetData(key, query).GetAwaiter().GetResult();
                if (result.current != null)
                {
                    RulesCheck(result);
                }
                Console.ReadLine();
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static async Task<WeatherObject> GetData(string key, string query)
        {
            var http = new HttpClient();

            var url = string.Format("http://api.weatherstack.com/current?access_key={0}&query={1}", key, query);

            var response = await http.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var stringResult = await response.Content.ReadAsStringAsync();

            var data = JsonConvert.DeserializeObject<WeatherObject>(stringResult);

            return data;

        }

        static void RulesCheck(WeatherObject currentWeather)
        {
            Console.WriteLine("Should I go outside - {0}, {1}", Enum.IsDefined(typeof(RainEnum), currentWeather.current.weather_code) ? "No" : "Yes", currentWeather.current.weather_descriptions.FirstOrDefault());

            Console.WriteLine("Should I wear sunscreen? - {0}", currentWeather.current.uv_index > 3 ? "Yes" : "No");

            Console.WriteLine("Can I fly kite? - {0}", !Enum.IsDefined(typeof(RainEnum), currentWeather.current.weather_code) && currentWeather.current.wind_speed > 15 ? "Yes" : "No");
        }

        public enum RainEnum
        {
            Patchy_rain_possible = 176,
            Patchy_light_rain = 293,
            Light_rain = 296,
            Moderate_rain_at_times = 299,
            Moderate_rain = 302,
            Heavy_rain_at_times = 305,
            Heavy_rain = 308,
            Light_freezing_rain = 311
        }

        public class WeatherObject
        {
            public Current current { get; set; }
        }

        public class Current
        {
            public int weather_code { get; set; }
            public List<string> weather_descriptions { get; set; }
            public int wind_speed { get; set; }
            public int uv_index { get; set; }
        }
    }
}
