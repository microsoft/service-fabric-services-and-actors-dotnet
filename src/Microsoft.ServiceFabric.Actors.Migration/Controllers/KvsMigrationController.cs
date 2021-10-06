// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Migration.Controllers
{
#if DotNetCoreClr
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    [ApiController]
    [Route("api/[controller]")]
    internal class KvsMigrationController : ControllerBase
    {
        private readonly ILogger logger;

        public KvsMigrationController(ILogger<KvsMigrationController> logger)
        {
            this.logger = logger;
        }

        // GET api/KvsMigration/values
        [HttpGet("Values")]
        public IEnumerable<string> Values()
        {
            this.logger.LogInformation("inside KvsMigrationController.Values");
            return new string[] { "value1", "value2" };
        }

        // GET api/KvsMigration/values/5
        [HttpGet("Values/{id}")]
        public string Values(int id)
        {
            return "value";
        }

        // POST api/KvsMigration/values
        [HttpPost("Values/{value}")]
        public void Values([FromBody] string value)
        {
        }

        // PUT api/KvsMigration/values/5
        [HttpPut("Values/{id}/{value}")]
        public void Values(int id, [FromBody] string value)
        {
        }

        // DELETE api/KvsMigration/values/5
        [HttpDelete("Values/{id}")]
        public void DeleteValues(int id)
        {
        }
    }
#endif
}
