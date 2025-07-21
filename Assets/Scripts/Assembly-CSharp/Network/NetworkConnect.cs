using System;
using System.Collections.Generic;
using UnityEngine;

namespace Network
{
	public class NetworkConnect
	{
		public enum RequestCommand
		{
			GetServerTime = 0,
			GetUniqueUid = 1,
			UploadStringData = 2,
			UploadJsonData = 3,
			GetStringData = 4,
			GetJsonData = 5,
			GetAllInnerJsonData = 6,
			UploadScoreFixedLength = 7,
			UploadScoreExpire = 8,
			GetDynamicRankList = 9,
			GetRankList = 10,
			GetUserScore = 11,
			GetUserRankIdx = 12,
			UploadFile = 13,
			GetFileUrl = 14,
			GetFriendRankList = 15,
			GetCountryCode = 16
		}

		private static NetworkConnect _instance;

		private const string ServerUrl = "http://subwaysnow.17taptap.com";

		private const string serverTimeUrl = "http://run.papermobi.com";

		private const string expireTime = "8640000";

		public static NetworkConnect Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new NetworkConnect();
				}
				return _instance;
			}
		}

		public string Url(RequestCommand cmd)
		{
			string text = "http://subwaysnow.17taptap.com";
			switch (cmd)
			{
			case RequestCommand.GetServerTime:
				text = "http://run.papermobi.com/index/gettime";
				break;
			case RequestCommand.GetUniqueUid:
				text += "/User/myid";
				break;
			case RequestCommand.UploadStringData:
				text += "/Appdata/commit";
				break;
			case RequestCommand.UploadJsonData:
				text += "/Appdata/hcommit";
				break;
			case RequestCommand.GetStringData:
				text += "/Appdata/get";
				break;
			case RequestCommand.GetJsonData:
				text += "/Appdata/hget";
				break;
			case RequestCommand.GetAllInnerJsonData:
				text += "/Appdata/hgetall";
				break;
			case RequestCommand.UploadScoreFixedLength:
				text += "/Appdata/setFixedLenRank";
				break;
			case RequestCommand.UploadScoreExpire:
				text += "/Appdata/setRankList";
				break;
			case RequestCommand.GetDynamicRankList:
				text += "/Appdata/getRankRange";
				break;
			case RequestCommand.GetRankList:
				text += "/Appdata/getRankList";
				break;
			case RequestCommand.GetUserScore:
				text += "/Appdata/getScore";
				break;
			case RequestCommand.GetUserRankIdx:
				text += "/Appdata/getLevel";
				break;
			case RequestCommand.UploadFile:
				text += "/Upload";
				break;
			case RequestCommand.GetFileUrl:
				text += "/Upload/getFile";
				break;
			case RequestCommand.GetFriendRankList:
				text += "/Appdata/getFriendsRank";
				break;
			}
			return text;
		}

		public string Uid()
		{
			string text = GetDeviceUid();
			if (text.Equals("tjb") || string.IsNullOrEmpty(text))
			{
				text = SystemInfo.deviceUniqueIdentifier;
				if (string.IsNullOrEmpty(text))
				{
					text = "uid" + DateTime.Now.Ticks + UnityEngine.Random.Range(1, 999);
				}
				text = text.ToLower();
				UpdateDeviceUid(text);
			}
			return text;
		}

		private string GetDeviceUid()
		{
			if (PlayerPrefs.HasKey("DeviceUid"))
			{
				return PlayerPrefs.GetString("DeviceUid");
			}
			PlayerPrefs.SetString("DeviceUid", "tjb");
			return "tjb";
		}

		private void UpdateDeviceUid(string uid)
		{
			PlayerPrefs.SetString("DeviceUid", uid);
		}

		public string AppId()
		{
			string empty = string.Empty;
			return "2213";
		}

		private string Stamp()
		{
			return ServeTimeUpdate.Instance.ServerTime.ToString();
		}

		private string Token(string userId)
		{
			return RiseSdk.CalculateMD5Hash(userId + "gamesr" + AppId() + Uid() + ServeTimeUpdate.Instance.ServerTime);
		}

		private string Version()
		{
			string empty = string.Empty;
			return "1.0";
		}

		public Dictionary<string, string> GetUserLoginDict(PlatFormType platForm = PlatFormType.guest, string platUid = null)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["app_id"] = AppId();
			dictionary["uuid"] = Uid();
			if (platForm != 0 && !string.IsNullOrEmpty(platUid))
			{
				dictionary["plat"] = platForm.ToString();
				dictionary["plat_id"] = platUid;
			}
			dictionary["version"] = Version();
			return dictionary;
		}

		public Dictionary<string, string> GetUploadStringDataDict(string userId, string key, string value, int expire = 0)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["app_id"] = AppId();
			dictionary["user_id"] = userId;
			dictionary["uuid"] = Uid();
			dictionary["stamp"] = Stamp();
			dictionary["token"] = Token(userId);
			dictionary["rk"] = key;
			dictionary["rv"] = value;
			dictionary["version"] = Version();
			if (expire > 0)
			{
				dictionary["expire"] = "8640000";
			}
			return dictionary;
		}

		public Dictionary<string, string> GetUploadJsonDataDict(string userId, string key, string json, int expire = 0)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["app_id"] = AppId();
			dictionary["user_id"] = userId;
			dictionary["uuid"] = Uid();
			dictionary["stamp"] = Stamp();
			dictionary["token"] = Token(userId);
			dictionary["rk"] = key;
			dictionary["json"] = json;
			dictionary["version"] = Version();
			if (expire > 0)
			{
				dictionary["expire"] = "8640000";
			}
			return dictionary;
		}

		public Dictionary<string, string> GetRequestStringDataDict(string userId, string key)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["app_id"] = AppId();
			dictionary["user_id"] = userId;
			dictionary["uuid"] = Uid();
			dictionary["stamp"] = Stamp();
			dictionary["token"] = Token(userId);
			dictionary["rk"] = key;
			dictionary["version"] = Version();
			return dictionary;
		}

		public Dictionary<string, string> GetRequestJsonDataDict(string userId, string key, string jsonKey)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["app_id"] = AppId();
			dictionary["user_id"] = userId;
			dictionary["uuid"] = Uid();
			dictionary["stamp"] = Stamp();
			dictionary["token"] = Token(userId);
			dictionary["rk"] = key;
			dictionary["sk"] = jsonKey;
			dictionary["version"] = Version();
			return dictionary;
		}

		public Dictionary<string, string> GetRequestAllJsonDataDict(string userId, string key)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["app_id"] = AppId();
			dictionary["user_id"] = userId;
			dictionary["uuid"] = Uid();
			dictionary["stamp"] = Stamp();
			dictionary["token"] = Token(userId);
			dictionary["rk"] = key;
			dictionary["version"] = Version();
			return dictionary;
		}

		public Dictionary<string, string> GetSubmitScoreFixedLengthDict(string userId, string rankTag, float score, int max_len = 0)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["app_id"] = AppId();
			dictionary["user_id"] = userId;
			dictionary["uuid"] = Uid();
			dictionary["stamp"] = Stamp();
			dictionary["token"] = Token(userId);
			dictionary["rank_key"] = rankTag;
			dictionary["score"] = score.ToString();
			if (max_len > 0)
			{
				dictionary["max_len"] = max_len.ToString();
			}
			dictionary["version"] = Version();
			return dictionary;
		}

		public Dictionary<string, string> GetSubmitScoreExpireDict(string userId, string rankTag, float score, int expire = 0)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["app_id"] = AppId();
			dictionary["user_id"] = userId;
			dictionary["uuid"] = Uid();
			dictionary["stamp"] = Stamp();
			dictionary["token"] = Token(userId);
			dictionary["rank_key"] = rankTag;
			dictionary["score"] = score.ToString();
			if (expire != 0)
			{
				dictionary["expire"] = "8640000";
			}
			dictionary["version"] = Version();
			return dictionary;
		}

		public Dictionary<string, string> GetRequestRankListAroundUserDict(string userId, string rankTag, int myRankIdxUpCount, int myRankIdxDownCount, RankOrder order)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["app_id"] = AppId();
			dictionary["user_id"] = userId;
			dictionary["uuid"] = Uid();
			dictionary["stamp"] = Stamp();
			dictionary["token"] = Token(userId);
			dictionary["rank_key"] = rankTag;
			dictionary["start"] = myRankIdxUpCount.ToString();
			dictionary["end"] = myRankIdxDownCount.ToString();
			int num = (int)order;
			dictionary["order"] = num.ToString();
			dictionary["version"] = Version();
			return dictionary;
		}

		public Dictionary<string, string> GetRequestRankListDict(string userId, string rankTag, RankOrder order, int rankCount = 100)
		{
			if (rankCount < 1)
			{
				rankCount = 1;
			}
			else if (rankCount > 1000)
			{
				rankCount = 1000;
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["app_id"] = AppId();
			dictionary["user_id"] = userId;
			dictionary["uuid"] = Uid();
			dictionary["stamp"] = Stamp();
			dictionary["token"] = Token(userId);
			dictionary["rank_key"] = rankTag;
			int num = (int)order;
			dictionary["order"] = num.ToString();
			if (rankCount != 100)
			{
				dictionary["num"] = rankCount.ToString();
			}
			dictionary["version"] = Version();
			return dictionary;
		}

		public Dictionary<string, string> GetRequestUserScoreDict(string userId, string rankTag)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["app_id"] = AppId();
			dictionary["user_id"] = userId;
			dictionary["uuid"] = Uid();
			dictionary["stamp"] = Stamp();
			dictionary["token"] = Token(userId);
			dictionary["rank_key"] = rankTag;
			dictionary["version"] = Version();
			return dictionary;
		}

		public Dictionary<string, string> GetRequestUserRankIdxDict(string userId, string rankTag)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["app_id"] = AppId();
			dictionary["user_id"] = userId;
			dictionary["uuid"] = Uid();
			dictionary["stamp"] = Stamp();
			dictionary["token"] = Token(userId);
			dictionary["rank_key"] = rankTag;
			dictionary["version"] = Version();
			return dictionary;
		}

		public Dictionary<string, string> GetUploadUserFileDict(string userId, string fileName, int expireSeconds, int expire = 0)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["app_id"] = AppId();
			dictionary["user_id"] = userId;
			dictionary["uuid"] = Uid();
			dictionary["stamp"] = Stamp();
			dictionary["token"] = Token(userId);
			dictionary["file_name"] = fileName;
			dictionary["expire"] = expireSeconds.ToString();
			dictionary["version"] = Version();
			if (expire > 0)
			{
				dictionary["expire"] = "8640000";
			}
			return dictionary;
		}

		public Dictionary<string, string> GetRequestUserFileUrlDict(string userId, string fileName)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["app_id"] = AppId();
			dictionary["user_id"] = userId;
			dictionary["uuid"] = Uid();
			dictionary["stamp"] = Stamp();
			dictionary["token"] = Token(userId);
			dictionary["file_name"] = fileName;
			dictionary["version"] = Version();
			return dictionary;
		}

		public Dictionary<string, string> GetRequestFriendRankListDict(string userId, string rankTag, RankOrder order, string platIds, PlatFormType platType)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["app_id"] = AppId();
			dictionary["user_id"] = userId;
			dictionary["uuid"] = Uid();
			dictionary["stamp"] = Stamp();
			dictionary["token"] = Token(userId);
			dictionary["rank_key"] = rankTag;
			int num = (int)order;
			dictionary["order"] = num.ToString();
			dictionary["fb_id"] = platIds;
			dictionary["plat"] = platType.ToString();
			dictionary["version"] = Version();
			return dictionary;
		}
	}
}
