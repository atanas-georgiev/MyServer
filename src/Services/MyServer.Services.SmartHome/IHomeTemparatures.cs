using MyServer.Services.SmartHome.Models;
using System.Collections.Generic;

namespace MyServer.Services.SmartHome
{
    public interface IHomeTemparatures
    {
        IEnumerable<Temperature> GetTemeratures();

        void Update();
    }
}
