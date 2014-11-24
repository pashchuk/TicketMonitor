using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace UzAPI
{
	public class MainAPI
	{
		private List<string> _fromPoint;
		private List<string> _toPoint;
		
	}
	[JsonObject]
	public class Station
	{
		[JsonProperty(PropertyName = "station_id")]
		public int StationID { get; private set; }
		[JsonProperty(PropertyName = "title")]
		public string Name { get; private set; }
	}
}
