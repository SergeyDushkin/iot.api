using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nest;
using iot.api.Domain;

namespace iot.api.Controllers
{
    [Route("api/manufacturer/")]
    public class ManufacturerController : Controller
    {
        private readonly IElasticClient client;

        public ManufacturerController(IElasticClient client)
        {
            this.client = client;
        }

        [HttpGet()]
        public IEnumerable<object> Get(bool includeAll = false)
        {
            var query = client.Search<IRDeviceSetting>(s => s.Index("ir"));

            if (includeAll)
            {
                return query.Documents
                    .Select(r => new { r.Manufacturer, r.Model })
                    .GroupBy(r => r.Manufacturer)
                    .Select(r => new { Manufacturer = r.Key, Models = r.Select(n => n.Model) })
                    .OrderBy(r => r.Manufacturer);
            }

            return query.Documents
                .Select(r => new { r.Manufacturer })
                .GroupBy(r => r.Manufacturer)
                .Select(r => new { Manufacturer = r.Key })
                .OrderBy(r => r.Manufacturer);

            /*
            var result = query.Documents
                .Select(r => r.Manufacturer)
                .Distinct()
                .OrderBy(r => r);
                */
            /*
            var query1 = client.Search<IRDeviceSetting>(s => s
                .Index("ir")
                .Aggregations(a => a
                    .Terms("manufacturer", st => st.Field(o => o.Manufacturer))
                )
            );

            var query = client.Search<IRDeviceSetting>(s => s
                .Index("ir")
                .Aggregations(a => a
                    .Terms("manufacturers", st => st
                        .Field(o => o.Manufacturer)
                        .Aggregations(aa => aa
                            .Terms("models", m => m
                                .Field(o => o.Model)
                            )
                        )
                    )
                )
            );

            var manufacturers = query.Aggs.Terms("manufacturers").Buckets;
            
            var result = manufacturers.Select(r => new {
                Manufacturer = r.Key,
                Models = r.Terms("models").Buckets.Select(m => m.Key)
            }).ToList();
            */
        }
        
        [HttpGet("{manufacturer}")]
        public object Get(string manufacturer)
        {
            var query = client.Search<IRDeviceSetting>(s => s.Index("ir"));

            var result = query.Documents
                .Select(r => new { r.Manufacturer, r.Model })
                .GroupBy(r => r.Manufacturer)
                .Select(r => new { Manufacturer = r.Key, Models = r.Select(n => n.Model) })
                .SingleOrDefault(r => r.Manufacturer == manufacturer);

            return result;
        }
    }
}
