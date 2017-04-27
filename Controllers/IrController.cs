using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nest;
using iot.api.Domain;

namespace iot.api.Controllers
{
    [Route("api/ir/")]
    public class IrController : Controller
    {
        private readonly IElasticClient client;

        public IrController(IElasticClient client)
        {
            this.client = client;
        }

        [HttpGet("{manufacturer=}")]
        public IEnumerable<IRDeviceSetting> Get(string manufacturer = null)
        {
            if (String.IsNullOrEmpty(manufacturer))
            {
                return client.Search<IRDeviceSetting>(s => s.Index("ir")).Documents;
            }

            return client.Search<IRDeviceSetting>(s => s.Index("ir"))
                .Documents
                .Where(r => r.Manufacturer == manufacturer);
        }
        
        [HttpGet("{manufacturer}/{model}")]
        public IRDeviceSetting Get(string manufacturer, string model)
        {
            return client.Search<IRDeviceSetting>(s => s.Index("ir"))
                .Documents
                .Where(r => r.Manufacturer == manufacturer && r.Model == model)
                .FirstOrDefault();
        }
        
        [HttpPost]
        public void Post([FromBody]IRDeviceSetting value)
        {
            if (String.IsNullOrEmpty(value.Manufacturer))
                throw new Exception("Manufacturer is not valid");
                
            if (String.IsNullOrEmpty(value.Model))
                throw new Exception("Model is not valid");
                
            var id = value.Manufacturer + ":" + value.Model;

            var response = client.Update<IRDeviceSetting, object>(id, u => u.Index("ir").Doc(value).DocAsUpsert());
        }
        
        [HttpDelete("{manufacturer}/{model}")]
        public void Delete(string manufacturer, string model)
        {
            var id = manufacturer + ":" + model;
            var response = client.Delete<IRDeviceSetting>(id, d => d.Index("ir"));
        }
    }
}
