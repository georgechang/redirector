using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace BellBank
{
	public static class Redirect
	{
		[FunctionName("Redirect")]
		public static async Task<IActionResult> Run(
			[HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
			ILogger log)
		{
			var host = new HostString(req.Headers["X-ORIGINAL-HOST"]).Host;
			log.LogInformation($"Host: {host}");

			var connectionString = Environment.GetEnvironmentVariable("StorageConnectionString", EnvironmentVariableTarget.Process);
			var tableName = Environment.GetEnvironmentVariable("TableName", EnvironmentVariableTarget.Process);
			var rowKey = Environment.GetEnvironmentVariable("RowKey", EnvironmentVariableTarget.Process) ?? host;
			var fallbackUrl = Environment.GetEnvironmentVariable("FallbackUrl", EnvironmentVariableTarget.Process);
			var hostnameFormat = Environment.GetEnvironmentVariable("RedirectedHostnameFormat", EnvironmentVariableTarget.Process);

			var storageAccount = CloudStorageAccount.Parse(connectionString);
			var tableClient = storageAccount.CreateCloudTableClient();
			var table = tableClient.GetTableReference(tableName);

			var operation = TableOperation.Retrieve<RedirectEntity>(host, rowKey);
			var result = await table.ExecuteAsync(operation);

			var url = result.Result is RedirectEntity redirectEntity ? string.Format(hostnameFormat, redirectEntity.Subdomain) : fallbackUrl;

			return new RedirectResult(url, false);
		}
	}
}
