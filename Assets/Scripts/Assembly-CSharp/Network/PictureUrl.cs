using System;
using UnityEngine;

namespace Network
{
	public class PictureUrl : StringKeyValue
	{
		public Action onDownloadImageSuccess;

		public Texture2D Image
		{
			get
			{
				return ImageManager.Instance.GetTexture(_value);
			}
		}

		public PictureUrl(string key)
		{
			_key = "pictureUrl";
			onValueChange = (Action)Delegate.Combine(onValueChange, new Action(DownloadImage));
			_playerPrefsKey = "Network_PictureUrl_" + key;
			_value = PlayerPrefs.GetString(_playerPrefsKey, string.Empty);
			GetStringValue();
		}

		public PictureUrl(string key, string value)
		{
			_key = "pictureUrl";
			onValueChange = (Action)Delegate.Combine(onValueChange, new Action(DownloadImage));
			_playerPrefsKey = "Network_PictureUrl_" + key;
			_value = PlayerPrefs.GetString(_playerPrefsKey, string.Empty);
			UploadKeyValue_Force(value);
		}

		protected override void UploadWithLocalValueByExpire()
		{
			UploadKeyValue_Force(_value);
		}

		protected void DownloadImage()
		{
			if (!ImageManager.Instance.ContainsKey(_value))
			{
				ImageDownloader imageDownloader = new ImageDownloader(_value, OnComplete, 60f, null);
				NetworkRequest.Instance.StartCoroutine(imageDownloader.Download());
			}
		}

		private void OnComplete(bool result, ImageDownloader loader)
		{
			Debug.Log("Download Result:" + result);
			if (result)
			{
				ServerManager.Instance.OnPictureURLChange();
			}
		}
	}
}
