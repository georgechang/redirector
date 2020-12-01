using Microsoft.Azure.Cosmos.Table;

namespace BellBank
{
	public class RedirectEntity : TableEntity
	{
		public string Subdomain { get; set; }
	}
}