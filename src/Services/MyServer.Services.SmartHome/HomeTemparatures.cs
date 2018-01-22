namespace MyServer.Services.SmartHome
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Options;

    using MiHomeLib;
    using MiHomeLib.Devices;

    using MyServer.Services.SmartHome.Config;
    using MyServer.Services.SmartHome.Models;

    public class HomeTemparatures : IHomeTemparatures
    {
        private readonly List<TemperatureConfig> temperatureConfigs;

        private readonly List<Temperature> temperatures;

        public HomeTemparatures(IOptions<List<TemperatureConfig>> config)
        {
            this.temperatures = new List<Temperature>();
            this.temperatureConfigs = config.Value;
        }

        public IEnumerable<Temperature> GetTemeratures() => this.temperatures;

        public void Update()
        {
            using (var mi = new MiHome())
            {
                Task.Delay(5000).Wait();
                this.temperatures.Clear();

                foreach (var temperatureConfig in this.temperatureConfigs)
                {
                    var sensor = mi.GetDeviceBySid<ThSensor>(temperatureConfig.Sid);

                    if (sensor != null)
                    {
                        this.temperatures.Add(
                            new Temperature()
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