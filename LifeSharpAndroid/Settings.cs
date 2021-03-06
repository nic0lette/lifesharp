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
using Android.Content;

namespace LifeSharp
{

public class Settings
{
	// These are the names of the pref items we'll be storing.
	class Prefs
	{
		public const string Name = "LifeSharpPrefs";
		public const string User = "username";
		public const string Pass = "password";
		public const string Auth = "authToken";
		public const string LastCheck = "lastCheck";
		public const string Enabled = "enabled";
		public const string LastImageTimestamp = "lastImageTimestamp";
	}

	// Default time, in seconds, to set the "last timestamp" if none exists. This will
	// cause us to auto-upload anything from one day ago.
	public const int DefaultDuration = 1 * 24 * 60 * 60;

	// Our base URL if nothing else is configured.
	public const string DefaultBaseUrl = "https://fia.isisview.org/lifestream/";

	Context _context;
	ISharedPreferences _settings;
	ISharedPreferencesEditor _editor;


	// This allows us to swap out URLs later if we want to.
	static public string BaseUrl
	{
		get
		{
			return DefaultBaseUrl;
		}
	}

	// Returns the Android ID of the device, if one exists; otherwise returns a new random GUID.
	static public string GetAndroidID(Context context)
	{
		string id = Android.Provider.Settings.Secure.GetString(context.ContentResolver,
			Android.Provider.Settings.Secure.AndroidId);
		if (string.IsNullOrEmpty(id))
			id = Guid.NewGuid().ToString("N");

		return id;
	}

	public Settings(Context context)
	{
		_context = context;
		_settings = _context.GetSharedPreferences(Prefs.Name, FileCreationMode.Private);
	}

	void edit()
	{
		if (_editor == null)
			_editor = _settings.Edit();
	}

	public void commit()
	{
		edit();
		_editor.Commit();
		_editor = null;
	}

	string getString(string id, string def)
	{
		return _settings.GetString(id, def);
	}
	void setString(string id, string val)
	{
		edit();
		_editor.PutString(id, val);
	}

	bool getBool(string id, bool def)
	{
		return _settings.GetBoolean(id, def);
	}
	void setBool(string id, bool val)
	{
		edit();
		_editor.PutBoolean(id, val);
	}

	long getLong(string id, long def)
	{
		return _settings.GetLong(id, def);
	}
	void setLong(string id, long val)
	{
		edit();
		_editor.PutLong(id, val);
	}

	public string userName
	{
		get
		{
			return getString(Prefs.Name, "");
		}
		set
		{
			setString(Prefs.Name, value);
		}
	}

	public string password
	{
		get
		{
			return getString(Prefs.Pass, "");
		}
		set
		{
			setString(Prefs.Pass, value);
		}
	}

	public bool enabled
	{
		get
		{
			return getBool(Prefs.Enabled, false);
		}
		set
		{
			setBool(Prefs.Enabled, value);
		}
	}

	public string authToken
	{
		get
		{
			return getString(Prefs.Auth, "");
		}

		set
		{
			setString(Prefs.Auth, value);
		}
	}

	static readonly DateTimeOffset Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

	public DateTimeOffset lastCheck
	{
		get
		{
			return Epoch.AddSeconds(getLong(Prefs.LastCheck, 0));
		}

		set
		{
			setLong(Prefs.LastCheck, (long)((value - Epoch).TotalSeconds));
		}
	}

	public long lastImageProcessedTimestamp
	{
		get
		{	
			return getLong(Prefs.LastImageTimestamp, (long)((DateTimeOffset.UtcNow.Subtract(new TimeSpan(0, 0, DefaultDuration)) - Epoch).TotalSeconds));
		}

		set
		{
			edit();
			setLong(Prefs.LastImageTimestamp, value);
		}
	}
}

}

