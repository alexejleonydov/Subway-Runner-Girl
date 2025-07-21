using System;
using System.Collections;
using UnityEngine;

public class ImageDownloader
{
	private float _timeout;

	public object cookie { get; private set; }

	public Texture2D image { get; private set; }

	public string url { get; private set; }

	public event Action<bool, ImageDownloader> onDownloadComplete;

	public ImageDownloader(string url, Action<bool, ImageDownloader> onCompleteAction, float timeout, object obj)
	{
		this.url = url;
		_timeout = timeout;
		cookie = obj;
		this.onDownloadComplete = (Action<bool, ImageDownloader>)Delegate.Combine(this.onDownloadComplete, onCompleteAction);
	}

	public IEnumerator Download()
	{
		float _startedDownloading = Time.realtimeSinceStartup;
		if (url == null)
		{
			yield break;
		}
		WWW www = new WWW(url);
		while (!www.isDone)
		{
			if (_timeout > 0f && Time.realtimeSinceStartup - _startedDownloading >= _timeout)
			{
				DownloadComplete(false);
				yield break;
			}
			yield return null;
		}
		if (www.error != null)
		{
			DownloadComplete(false);
			yield break;
		}
		image = www.texture;
		if (image != null && (image.width != 8 || image.height != 8))
		{
			ImageManager.Instance.Add(url, image);
			DownloadComplete(true);
		}
		else
		{
			DownloadComplete(false);
		}
	}

	private void DownloadComplete(bool succes)
	{
		if (this.onDownloadComplete != null)
		{
			this.onDownloadComplete(succes, this);
		}
	}
}
