using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;
using System.Net;

namespace test_santa_api
{
    public class AddChild
    {
        private readonly SHDbContext context;

        public AddChild(SHDbContext context) 
        {
            this.context = context;
        }
        [FunctionName("addChild")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })] 
        //[OpenApiRequestBody("text/json", typeof(Child))]
        [OpenApiRequestBody("text/json", typeof(CreateChildDto))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Created, contentType: "text/json", bodyType: typeof(Child))] 
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "child")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");            
            //Child entity = null;

            //return new CreatedResult("child", entity);

            string requestBody = String.Empty;
            using (StreamReader streamReader =  new  StreamReader(req.Body))
            {
                requestBody = await streamReader.ReadToEndAsync();
            }
            //var newChild = JsonConvert.DeserializeObject<Child>(requestBody);
            //if(newChild.Id != int.MinValue ) {throw new Exception("new items only");}
            
            //context.Clients.Add(newChild);
            //var result = await context.SaveChangesAsync();
            //return new CreatedResult("child", newChild);

            // now use DTO
            var newChild = JsonConvert.DeserializeObject<CreateChildDto>(requestBody);
            var entity = newChild.ToEntity();
            context.Clients.Add(entity);
            var result = await context.SaveChangesAsync();
            return new CreatedResult("child", entity);
        }
    }
}
