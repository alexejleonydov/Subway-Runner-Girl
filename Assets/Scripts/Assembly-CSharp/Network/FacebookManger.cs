using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Network
{
	public class FacebookManger : MonoBehaviour
	{
		private static FacebookManger _instance;

		public bool facebookHasLogin;

		public Action<bool> OnFacebookLoginResult;

		public List<FaceBookPerson> friendsList;

		public FaceBookPerson me;

		public static FacebookManger Instance
		{
			get
			{
				if (_instance == null)
				{
					GameObject gameObject = new GameObject("FacebookManger");
					_instance = gameObject.AddComponent<FacebookManger>();
				}
				return _instance;
			}
		}

		public void URL()
		{
			Application.OpenURL("https://www.facebook.com/Subway-Princess-Runner-226987164727227/");
		}

		public void LoginFacebook()
		{
			if (!RiseSdk.Instance.IsLogin())
			{
				RiseSdkListener.OnSNSEvent -= OnLoginFacebook;
				RiseSdkListener.OnSNSEvent += OnLoginFacebook;
				RiseSdk.Instance.Login();
			}
			else
			{
				OnLoginFacebook(RiseSdk.SnsEventType.LoginSuccess, 0);
			}
		}

		private void OnLoginFacebook(RiseSdk.SnsEventType type, int id)
		{
			RiseSdkListener.OnSNSEvent -= OnLoginFacebook;
			facebookHasLogin = type == RiseSdk.SnsEventType.LoginSuccess;
			if (OnFacebookLoginResult != null)
			{
				OnFacebookLoginResult(facebookHasLogin);
			}
			Debug.LogError("Has Facebook Login:" + facebookHasLogin);
			if (facebookHasLogin)
			{
				PlayerInfo.Instance.hasFacebookLogin = true;
				GetFriends();
				Me();
				if (me != null && !string.IsNullOrEmpty(me.id))
				{
					SecondManager.Instance.RequestFacebookUserID(me.id);
				}
			}
			else
			{
				SecondManager.Instance.RequestGuestUserID();
			}
		}

		public void GetFriends()
		{
			if (!RiseSdk.Instance.IsLogin())
			{
				return;
			}
			string friends = RiseSdk.Instance.GetFriends();
			if (string.IsNullOrEmpty(friends))
			{
				return;
			}
			Debug.Log(friends);
			List<object> list = RiseJson.Deserialize(friends) as List<object>;
			if (list == null || list.Count == 0)
			{
				return;
			}
			Dictionary<string, object> dictionary = null;
			friendsList = new List<FaceBookPerson>(list.Count);
			int i = 0;
			for (int count = list.Count; i < count; i++)
			{
				dictionary = list[i] as Dictionary<string, object>;
				FaceBookPerson faceBookPerson = new FaceBookPerson();
				try
				{
					object value;
					if (dictionary.TryGetValue("id", out value))
					{
						faceBookPerson.id = (string)value;
					}
					if (dictionary.TryGetValue("name", out value))
					{
						faceBookPerson.name = (string)value;
					}
					if (dictionary.TryGetValue("picture", out value))
					{
						faceBookPerson.picture = (string)value;
					}
					friendsList.Add(faceBookPerson);
				}
				catch
				{
					faceBookPerson = null;
				}
			}
		}

		public void Me()
		{
			string text = RiseSdk.Instance.Me();
			Debug.LogError("Facebook Me:" + text);
			if (!string.IsNullOrEmpty(text) && text.Length > 5)
			{
				Dictionary<string, object> dictionary = (Dictionary<string, object>)RiseJson.Deserialize(text);
				me = new FaceBookPerson();
				if (dictionary.ContainsKey("id"))
				{
					me.id = dictionary["id"].ToString();
				}
				if (dictionary.ContainsKey("name"))
				{
					me.name = dictionary["name"].ToString();
				}
				if (dictionary.ContainsKey("picture"))
				{
					me.picture = dictionary["picture"].ToString();
				}
			}
			if (!ImageManager.Instance.ContainsKey(me.picture))
			{
				ImageDownloader imageDownloader = new ImageDownloader(me.picture, OnMeComplete, 60f, null);
				StartCoroutine(imageDownloader.Download());
			}
		}

		private void OnMeComplete(bool result, ImageDownloader loader)
		{
		}

		public string FriendsIds()
		{
			if (friendsList == null || friendsList.Count <= 0)
			{
				return null;
			}
			StringBuilder stringBuilder = new StringBuilder();
			if (me != null)
			{
				stringBuilder.Append(me.id);
				stringBuilder.Append(",");
			}
			int i = 0;
			for (int count = friendsList.Count; i < count; i++)
			{
				stringBuilder.Append(friendsList[i].id);
				if (i != count - 1)
				{
					stringBuilder.Append(",");
				}
			}
			return stringBuilder.ToString();
		}

		public string GetNameAccrodingID(string id)
		{
			if (friendsList == null || friendsList.Count <= 0)
			{
				return null;
			}
			int i = 0;
			for (int count = friendsList.Count; i < count; i++)
			{
				if (friendsList[i].id.Equals(id))
				{
					return friendsList[i].name;
				}
			}
			return null;
		}

		public string GetPictureAccrodingID(string id)
		{
			if (friendsList == null || friendsList.Count <= 0)
			{
				return null;
			}
			int i = 0;
			for (int count = friendsList.Count; i < count; i++)
			{
				if (friendsList[i].id.Equals(id))
				{
					return friendsList[i].picture;
				}
			}
			return null;
		}
	}
}
