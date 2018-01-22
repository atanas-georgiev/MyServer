namespace MyServer.Services.SmartHome
{
    using System.Collections.Generic;

    using MyServer.Services.SmartHome.Models;

    public interface IHomeTemparatures
    {
        IEnumerable<Temperature> GetTemeratures();

        void Update();
    }
}