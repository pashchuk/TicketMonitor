using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace UzAPI
{
	public class MainAPI
	{
		private List<string> _fromPoint;
		private List<string> _toPoint;

		public void Search(int from, int to, string nameFrom, string nameTo, DateTime dateAndTime)
		{
			var data = new StringBuilder();
			//data.Append(HttpUtility.UrlEncode(string.Format("station_id_from={0}&", from)));
			//data.Append(HttpUtility.UrlEncode(string.Format("station_id_till={0}&", to)));
			//data.Append(HttpUtility.UrlEncode(string.Format("station_from={0}&", nameFrom)));
			//data.Append(HttpUtility.UrlEncode(string.Format("station_till={0}&", nameTo)));
			//data.Append(HttpUtility.UrlEncode(string.Format("date_dep={0}&", dateAndTime.ToShortDateString())));
			//data.Append(HttpUtility.UrlEncode(string.Format("time_dep={0}&", dateAndTime.ToShortTimeString())));
			//data.Append(HttpUtility.UrlEncode("another_ec=0&"));
			//data.Append(HttpUtility.UrlEncode("search="));
			data.Append(string.Format("station_id_from={0}&", from));
			data.Append(string.Format("station_id_till={0}&", to));
			data.Append(string.Format("station_from={0}&", HttpUtility.UrlEncode(nameFrom)));
			data.Append(string.Format("station_till={0}&", HttpUtility.UrlEncode(nameTo)));
			data.Append(string.Format("date_dep={0}&", HttpUtility.UrlEncode("30.11.2014")));
			data.Append(string.Format("time_dep={0}&", HttpUtility.UrlEncode("00:00")));
			data.Append("time_dep_till=&");
			data.Append("another_ec=0&");
			data.Append("search=");
			var resData = data.ToString();
			var uri = new Uri(@"http://booking.uz.gov.ua/purchase/search/");
			var request = WebRequest.CreateHttp(uri);
			request.Headers["Accept-Language"] = "en-US,en; q=0.8,ru; q=0.6,uk; q=0.4";
			//request.Headers["GV-Token"] = "5fd5eeed5c252ea14d5b96cb8c4bf393";
			request.Headers["Accept-Encoding"] = "gzip,deflate";
			request.Headers["GV-Unique-Host"] = "1";
			request.Headers["GV-Ajax"] = "1";
			//request.Headers["GV-Referer"] = @"http://booking.uz.gov.ua/";
			//request.Headers["GV-Referer-Src"] = @"http://uz.gov.ua/";
			//request.Headers["GV-Referer-Src-Jump"] = "1";
			//request.Headers["GV-Screen"] = "1280x1024";

			//GV - Ajax:1
			//GV - Referer:http://booking.uz.gov.ua/
			//GV - Referer - Src:http://uz.gov.ua/
			//GV - Referer - Src - Jump:1
			//GV - Screen:1280x1024
			//GV - Token:5fd5eeed5c252ea14d5b96cb8c4bf393
			//GV - Unique - Host:1
			//request.Headers["Cookie"] = "_gv_sessid=ea1fknb7lf404nkp02tsjqeu80; HTTPSERVERID=server2; _gv_lang=uk; __utmt=1; __utma=31515437.130482776.1414174285.1416865804.1416867645.17; __utmb=31515437.2.10.1416867645; __utmc=31515437; __utmz=31515437.1416867645.17.9.utmcsr=uz.gov.ua|utmccn=(referral)|utmcmd=referral|utmcct=/";

			request.Method = "POST";
			request.ContentType = "application/x-www-form-urlencoded";
			request.ContentLength = resData.Length;
            using (var writer = new StreamWriter(request.GetRequestStream()))
			{ writer.Write(resData.ToCharArray()); }
				var response = request.GetResponse();
			string result;
			using (var reader = new StreamReader(response.GetResponseStream()))
			{ result = reader.ReadToEnd(); }
			var obj = JsonConvert.DeserializeObject<StationResponse>(result);
			//if (obj.ErrorText != null || obj.Stations == null)
		}
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

	#region Json classes
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


	#endregion
}
