using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

		private CookieContainer _cookie;
		public MainAPI()
		{
			var uri = new Uri(@"http://booking.uz.gov.ua/");
			_cookie = new CookieContainer();
			var request = WebRequest.CreateHttp(uri);
			request.Method = "GET";
			var response = request.GetResponse();
			var cookies = response.Headers["Set-Cookie"].Split(',');
			for (int i = 0; i < cookies.Length; i++)
			{
				if (cookies[i].Contains("expires"))
					cookies[i+1] = cookies[i] + "," + cookies[++i];
				var cook = ParseCookie(cookies[i]);
				if (cook.Domain == string.Empty || cook.Domain == null)
					cook.Domain = uri.Host;
				_cookie.Add(cook);
			}
		}
		private static Cookie ParseCookie(string cookie)
		{
			var result = new Cookie();
			var values = cookie.Split(';');
			result.Name = values[0].Split('=')[0];
			result.Value = values[0].Split('=')[1];
			for(int i = 1; i < values.Length; i++)
			{
				var elements = values[i].Split('=');
				if (values[i].Contains("expires"))
					result.Expires = DateTime.Parse(values[i].Split('=')[1]);
				if (values[i].Contains("path"))
					result.Path = values[i].Split('=')[1];
				if(values[i].Contains("domain"))
					result.Domain = values[i].Split('=')[1];
			}
			return result;
		}
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
			//request.CookieContainer = _cookie;
			request.Headers["Cookie"] = @"_gv_sessid = ip304rpdsuv1hfkds221ao4is2; _gv_lang = uk; HTTPSERVERID = server2; __utmt = 1; __utma = 31515437.1324051733.1416924431.1416924431.1416938968.2; __utmb = 31515437.1.10.1416938968; __utmc = 31515437; __utmz = 31515437.1416938968.2.2.utmcsr = uz.gov.ua | utmccn = (referral) | utmcmd = referral | utmcct =/";
            request.Headers["Accept-Encoding"] = "gzip,deflate";
			request.Headers["Accept-Language"] = "en-US,en; q=0.8,ru; q=0.6,uk; q=0.4";
			request.ContentType = "application/x-www-form-urlencoded";
			request.Headers["GV-Ajax"] = "1";
			request.Headers["GV-Referer"] = @"http://booking.uz.gov.ua/";
			request.Headers["GV-Referer-Src"] = @"http://uz.gov.ua/";
			request.Headers["GV-Referer-Src-Jump"] = "1";
			request.Headers["GV-Screen"] = "1280x1024";
			request.Headers["GV-Token"] = "3363087e5361a17577e412689814cffc";
			request.Headers["GV-Unique-Host"] = "1";
			request.Host = "booking.uz.gov.ua";
			request.Headers["Origin"] = @"http://booking.uz.gov.ua";
			request.Referer = @"http://booking.uz.gov.ua/";
			request.UserAgent = "Mozilla / 5.0(Windows NT 6.3; WOW64) AppleWebKit / 537.36(KHTML, like Gecko) Chrome / 38.0.2125.111 Safari / 537.36";

			request.Method = "POST";
			request.ContentLength = resData.Length;
            using (var writer = new StreamWriter(request.GetRequestStream()))
			{ writer.Write(resData.ToCharArray()); }
				var response = request.GetResponse();
			string result;
			using (var reader = new StreamReader(response.GetResponseStream()))
			{ result = reader.ReadToEnd(); }
			var obj = JsonConvert.DeserializeObject<StationResponse>(result);
			//if (obj.ErrorText != null || obj.Stations == null)
/*
Accept - Encoding:gzip,deflate
Accept - Language:en - US,en; q = 0.8,ru; q = 0.6,uk; q = 0.4
Connection:	keep - alive
Content - Length:208
Content - Type:application / x - www - form - urlencoded
Cookie: _gv_sessid = ip304rpdsuv1hfkds221ao4is2; _gv_lang = uk; HTTPSERVERID = server2; __utmt = 1; __utma = 31515437.1324051733.1416924431.1416924431.1416938968.2; __utmb = 31515437.1.10.1416938968; __utmc = 31515437; __utmz = 31515437.1416938968.2.2.utmcsr = uz.gov.ua | utmccn = (referral) | utmcmd = referral | utmcct =/
GV - Ajax:1
GV - Referer:http://booking.uz.gov.ua/
GV - Referer - Src:http://uz.gov.ua/
GV - Referer - Src - Jump:1
GV - Screen:1280x1024
GV - Token:3363087e5361a17577e412689814cffc
GV - Unique - Host:1
Host: booking.uz.gov.ua
Origin: http://booking.uz.gov.ua
Referer: http://booking.uz.gov.ua/
User - Agent:Mozilla / 5.0(Windows NT 6.3; WOW64) AppleWebKit / 537.36(KHTML, like Gecko) Chrome / 38.0.2125.111 Safari / 537.36
*/
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
