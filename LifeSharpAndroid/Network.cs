﻿/*
    LifeStream - Instant Photo Sharing
    Copyright (C) 2014-2016 Kayateia

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.

 */

using System;
using System.Json;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Util;
using System.Net.Http;
using System.Net.Http.Headers;

namespace LifeSharp
{
static public class Network
{
	const string LogTag = "LifeSharp/Network";

	// Downloads the specified URL into the specified file.
	static public async Task HttpDownloadAsync(string url, string token, string outputFilename)
	{
		Log.Info(LogTag, "Performing HttpDownloadAsync, URL {0}", url);

		using (HttpClient client = new HttpClient())
		{
			var request = new HttpRequestMessage()
			{
				RequestUri = new System.Uri(url),
				Method = HttpMethod.Get
			};
			if (token != null)
				request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

			using (HttpResponseMessage response = await client.SendAsync(request))
			using (HttpContent responseContent = response.Content)
			{
				// Copy the results out to the specified file.
				using (FileStream output = File.OpenWrite(outputFilename))
				{
					await responseContent.CopyToAsync(output);
				}
			}
		}
	}

	// Downloads the specified URL into a JSON output.
	static public async Task<JsonValue> HttpPostToJsonAsync(string url, string token, IDictionary<string, string> param)
	{
		Log.Info(LogTag, "Performing HttpPostToJsonAsync, URL {0}", url);

		using (HttpContent content = new FormUrlEncodedContent(param.ToArray()))
		using (HttpClient client = new HttpClient())
		{
			var request = new HttpRequestMessage()
			{
				RequestUri = new System.Uri(url),
				Method = HttpMethod.Post,
				Content = content
			};
			if (token != null)
				request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

			using (HttpResponseMessage response = await client.SendAsync(request))
			using (HttpContent responseContent = response.Content)
			{
				// Use this stream to build a JSON document object.
				JsonValue jsonDoc = JsonObject.Load(await responseContent.ReadAsStreamAsync());
				Log.Info(LogTag, "Response: {0}", jsonDoc.ToString());

				return jsonDoc;
			}
		}
	}

	// Downloads the specified URL into a JSON output.
	static public async Task<JsonValue> HttpGetToJsonAsync(string url, string token)
	{
		Log.Info(LogTag, "Performing HttpGetToJsonAsync, URL {0}", url);

		using (HttpClient client = new HttpClient())
		{
			var request = new HttpRequestMessage()
			{
				RequestUri = new System.Uri(url),
				Method = HttpMethod.Get
			};
			if (token != null)
				request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

			using (HttpResponseMessage response = await client.SendAsync(request))
			using (HttpContent responseContent = response.Content)
			{
				// Use this stream to build a JSON document object.
				JsonValue jsonDoc = JsonObject.Load(await responseContent.ReadAsStreamAsync());
				Log.Info(LogTag, "Response: {0}", jsonDoc.ToString());

				return jsonDoc;
			}
		}
	}

	// Logs into the LifeStream server and gets the JSON results of that request. Returns a token as a string
	// or null if the login failed.
	static public async Task<string> Login(Settings settings)
	{
		var param = new Dictionary<string, string>()
		{
			{ "login", settings.userName },
			{ "password", settings.password }
		};
		JsonValue results = await HttpPostToJsonAsync(Settings.BaseUrl + "api/user/login", null, param);

		bool success = results["success"];
		if (success)
		{
			string token = results["token"];
			Log.Info(LogTag, "Login results: {0}", token);

			return token;
		}
		else
		{
			Log.Info(LogTag, "Login failed: {0}", results["error"]);
			return null;
		}
	}
}
}

