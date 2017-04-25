using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using iot.api.Domain;
using Nest;

namespace iot.api.Controllers
{
    [Route("api/ir/{manufacturer}")]
    public class IrController : Controller
    {
        private readonly IElasticClient client;

        public IrController(IElasticClient client)
        {
            this.client = client;
        }

        [HttpGet]
        public IEnumerable<IRDeviceSetting> Get(string manufacturer)
        {
            return client.Search<IRDeviceSetting>(s => s.Index("ir"))
                .Documents
                .Where(r => r.Manufacturer == manufacturer);
        }
        
        [HttpGet("{model}")]
        public IRDeviceSetting Get(string manufacturer = null, string model = null)
        {
            return client.Search<IRDeviceSetting>(s => s.Index("ir"))
                .Documents
                .Where(r => r.Manufacturer == manufacturer && r.Model == model)
                .FirstOrDefault();
        }
        
        [HttpPost]
        public void Post([FromBody]IRDeviceSetting value)
        {
            var id = value.Manufacturer + ":" + value.Model;

            var response = client.Update<IRDeviceSetting, object>(id, u => u.Index("ir").Doc(value).DocAsUpsert());
        }
        
        [HttpDelete("{model}")]
        public void Delete(string manufacturer, string model)
        {
            var id = manufacturer + ":" + model;
            var response = client.Delete<IRDeviceSetting>(id, d => d.Index("ir"));
        }
    }
}
