using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace UzAPI
{
	public class MainAPI
	{
		private List<string> _fromPoint;
		private List<string> _toPoint;
		
	}

	public static class Station
	{
		public static List<StationInfo> GetStationsList(string phrase)
		{
			var uri = new Uri(string.Format(@"http://booking.uz.gov.ua/purchase/station/{0}/", phrase));
			var request = WebRequest.CreateHttp(uri);
			request.Method = "POST";
			request.ContentType = "application/x-www-form-urlencoded";
			var response = request.GetResponse();
			string result;
			using (var reader = new StreamReader(response.GetResponseStream()))
			{ result = System.Text.RegularExpressions.Regex.Unescape(reader.ReadToEnd().Replace("\"", "\\\"")); }
            var obj = JsonConvert.DeserializeObject<StationResponse>(result);
			if (obj.ErrorText != null || obj.Stations == null)
				return null;
			return obj.Stations;
		}
	}

	[JsonObject]
	public class StationInfo
	{
		[JsonProperty(PropertyName = "station_id")]
		public int StationID { get; set; }
		[JsonProperty(PropertyName = "title")]
		public string Name { get; set; }
	}
	[JsonObject]
	internal class StationResponse
	{
		[JsonProperty(PropertyName = "value")]
		public List<StationInfo> Stations { get; set; }
		[JsonProperty(PropertyName = "error")]
		public string ErrorText { get; set; }
		[JsonProperty(PropertyName = "data")]
		public Data AdditionalData { get; set; }
	}
	[JsonObject]
	internal class Data
	{
		[JsonProperty(PropertyName = "req_text")]
		public List<string> RequestText { get; set; }
	}
}
