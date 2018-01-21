using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MiHomeLib;
using MiHomeLib.Devices;
using MyServer.Services.SmartHome.Config;
using MyServer.Services.SmartHome.Models;

namespace MyServer.Services.SmartHome
{
    public class HomeTemparatures : IHomeTemparatures
    {
        private List<TemperatureConfig> temperatureConfigs;
        private List<Temperature> temperatures;

        public HomeTemparatures(IOptions<List<TemperatureConfig>> config)
        {
            this.temperatures = new List<Temperature>();
            this.temperatureConfigs = config.Value;
        }

        public IEnumerable<Temperature> GetTemeratures() => this.temperatures;

        public void Update()
        {
            using (var miHome = new MiHome("1hupuqxy0hy7adv8", "34ce00fa5dc9"))
            {
                Task.Delay(5000).Wait();
                this.temperatures.Clear();

                foreach (var temperatureConfig in temperatureConfigs)
                {
                    var sensor = miHome.GetDeviceBySid<ThSensor>(temperatureConfig.Sid);

                    if (sensor != null)
                    {
                        this.temperatures.Add(new Temperature()
                        {
                            NameBg = temperatureConfig.NameBg,
                            NameEn = temperatureConfig.NameEn,
                            Temerature = sensor.Temperature.Value,
                            Humidity = sensor.Humidity.Value
                        });
                    }
                }
            }
        }
    }
}
