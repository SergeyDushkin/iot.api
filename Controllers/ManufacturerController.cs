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
        public IEnumerable<object> Get(bool includeAll = false, string query = null)
        {
            var result = client.Search<IRDeviceSetting>(s => s.Index("ir"));

            if (includeAll)
            {
                return result.Documents
                    .Where(r => String.IsNullOrEmpty(query) ? true : r.Manufacturer.StartsWith(query, StringComparison.OrdinalIgnoreCase))
                    .Select(r => new { r.Manufacturer, r.Model })
                    .GroupBy(r => r.Manufacturer)
                    .Select(r => new { Manufacturer = r.Key, Models = r.Select(n => n.Model).OrderBy(n => n) })
                    .OrderBy(r => r.Manufacturer);
            }

            return result.Documents
                .Where(r => String.IsNullOrEmpty(query) ? true : r.Manufacturer.StartsWith(query, StringComparison.OrdinalIgnoreCase))
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
        public object Get(string manufacturer, string query = null)
        {
            var result = client.Search<IRDeviceSetting>(s => s.Index("ir"));

            return result.Documents
                .Where(r => r.Manufacturer == manufacturer)
                .Where(r => String.IsNullOrEmpty(query) ? true : r.Model.StartsWith(query, StringComparison.OrdinalIgnoreCase))
                .Select(r => new { r.Manufacturer, r.Model })
                .GroupBy(r => r.Manufacturer)
                .Select(r => new { Manufacturer = r.Key, Models = r.Select(n => n.Model).OrderBy(n => n) })
                .SingleOrDefault();
        }
    }
}
