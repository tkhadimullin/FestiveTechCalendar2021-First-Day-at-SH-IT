using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;
using System.Net;

namespace test_santa_api
{
    public class GetChildren
    {
        private readonly Child theChild;
        private readonly SHDbContext context;

        public GetChildren(Child theChild, SHDbContext context) {
            this.theChild = theChild;
            this.context = context;
        }

        [FunctionName("getChildren")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })] 
        [OpenApiParameter(name: "city", In = ParameterLocation.Query, Required = true, Type = typeof(string))] 
        [OpenApiParameter(name: "isNaughty", In = ParameterLocation.Query, Required = true, Type = typeof(bool))] 
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/json", bodyType: typeof(List<Child>))] 
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "child/findByCity")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            
            //return new OkObjectResult(new List<Child> {theChild});
            //return new OkObjectResult(context.Clients.Take(5).ToList());

            string city = req.Query["city"];
            bool isNaughty = bool.Parse(req.Query["isNaughty"]);
            return new OkObjectResult(context.Clients.Where(c=> c.City== city && c.IsNaughty == isNaughty).Take(5).ToList());
        }
    }
}
