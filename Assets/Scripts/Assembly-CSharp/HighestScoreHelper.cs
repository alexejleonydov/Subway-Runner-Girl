using System;
using Network;
using UnityEngine;

public class HighestScoreHelper : MonoBehaviour
{
	public enum AnimatingState
	{
		_notset = 0,
		OnScreen = 1,
		OffScreen = 2,
		AnimatingIn = 3,
		AnimatingOut = 4
	}

	[SerializeField]
	private UISprite background;

	[SerializeField]
	private UISprite iconBg;

	[SerializeField]
	private UISprite icon;

	[SerializeField]
	private UILabel points;

	[SerializeField]
	private UILabel scoreType;

	[SerializeField]
	private UITexture head;

	[SerializeField]
	private AnimatingState currentAnimationState;

	[SerializeField]
	private Vector3 _inPosition = new Vector3(0f, -240f, 0f);

	[SerializeField]
	private Vector3 _outPosition = new Vector3(0f, -425f, 0f);

	[SerializeField]
	private Vector3 _resetPosition = new Vector3(150f, -240f, 0f);

	private int _HighestScore;

	private string _playerName;

	private Texture _image;

	private bool _passedHighScore;

	private float _duration = 0.5f;

	private float _backgroundAlphaDefault;

	private float _iconBgAlphaDefault;

	private float _iconAlphaDefault;

	private float _pointsAlphaDefault;

	private float _scoreTypeAlphaDefault;

	private float _headAlphaDefault;

	private Transform _cachedTransform;

	private float _current;

	private bool _gameRunning;

	private bool _inited;

	private Vector3 _vector;

	private HighestScoreSystem _highestScore;

	private TopRun _topRun;

	private void Awake()
	{
		Init();
		NewGame();
	}

	public void AnimateIn()
	{
		if (!_passedHighScore)
		{
			_cachedTransform.localPosition = _resetPosition;
			currentAnimationState = AnimatingState.AnimatingIn;
		}
	}

	public void AnimateOut()
	{
		if (_gameRunning)
		{
			currentAnimationState = AnimatingState.AnimatingOut;
		}
	}

	public void GameOver()
	{
		_gameRunning = false;
		_cachedTransform.localPosition = _resetPosition;
		background.alpha = _backgroundAlphaDefault;
		iconBg.alpha = _iconBgAlphaDefault;
		icon.alpha = _iconAlphaDefault;
		head.alpha = _headAlphaDefault;
		points.alpha = _pointsAlphaDefault;
		scoreType.alpha = _scoreTypeAlphaDefault;
		currentAnimationState = AnimatingState.OffScreen;
	}

	private void Init()
	{
		_backgroundAlphaDefault = background.alpha;
		_iconBgAlphaDefault = iconBg.alpha;
		_iconAlphaDefault = icon.alpha;
		_headAlphaDefault = head.alpha;
		_pointsAlphaDefault = points.alpha;
		_scoreTypeAlphaDefault = scoreType.alpha;
		_cachedTransform = base.transform;
		_highestScore = HighestScoreSystem.Instance;
		_inited = true;
		Game.Instance.OnGameStarted = (Action)Delegate.Combine(Game.Instance.OnGameStarted, new Action(NewGame));
	}

	private int CalculateGapToFriendsScore(int scoreTo, int currentScore)
	{
		return scoreTo - currentScore;
	}

	private void SetNewHighScore()
	{
		if (_topRun == null)
		{
			_HighestScore = PlayerInfo.Instance.highestScore;
			_playerName = Strings.Get(LanguageKey.INGAME_UI_HIGHSCORE);
			_image = null;
			if (SecondManager.Instance.facebook)
			{
				PictureUrl pictureUrl = ServerManager.Instance.PictureUrl;
				if (pictureUrl != null)
				{
					_image = pictureUrl.Image;
				}
			}
		}
		else
		{
			_HighestScore = _topRun.highestScore;
			TopRunInfo topRunInfo = ((!_highestScore.isAI) ? ServerManager.Instance.GetTopRunInfo(_topRun.userId) : null);
			_playerName = ((topRunInfo != null) ? topRunInfo.playerName : _topRun.userId);
			head.mainTexture = null;
			if (topRunInfo != null && ImageManager.Instance.ContainsKey(topRunInfo.pictureUrl))
			{
				_image = ImageManager.Instance.GetTexture(topRunInfo.pictureUrl);
			}
		}
	}

	private void RefreshUI()
	{
		if (_gameRunning)
		{
			scoreType.text = _playerName;
			head.mainTexture = _image;
		}
	}

	public void NewGame()
	{
		GameStats.Instance.Reset();
		GameOver();
		_gameRunning = true;
		_highestScore.lastBeatenTopRun = null;
		_topRun = _highestScore.Init();
		if (PlayerInfo.Instance.tutorialStep > 0)
		{
			SetNewHighScore();
			points.text = _HighestScore.ToString();
			_passedHighScore = false;
			RefreshUI();
			if (CheckNewHighScore())
			{
				AnimateIn();
			}
		}
	}

	private bool CheckNewHighScore()
	{
		if (_HighestScore <= GameStats.Instance.score)
		{
			return false;
		}
		return true;
	}

	private void Update()
	{
		if (!_inited)
		{
			Init();
		}
		switch (currentAnimationState)
		{
		case AnimatingState.AnimatingIn:
			_current += Time.deltaTime;
			_vector = Vector3.Lerp(_resetPosition, _inPosition, _current / _duration);
			_cachedTransform.localPosition = _vector;
			if (_current >= _duration)
			{
				_current = 0f;
				currentAnimationState = AnimatingState.OnScreen;
			}
			break;
		case AnimatingState.AnimatingOut:
		{
			_current += Time.deltaTime;
			float t = _current / _duration;
			_vector = Vector3.Lerp(_inPosition, _outPosition, _current / _duration);
			background.alpha = Mathf.Lerp(_backgroundAlphaDefault, 0f, t);
			iconBg.alpha = Mathf.Lerp(_iconBgAlphaDefault, 0f, t);
			icon.alpha = Mathf.Lerp(_iconAlphaDefault, 0f, t);
			head.alpha = Mathf.Lerp(_headAlphaDefault, 0f, t);
			points.alpha = Mathf.Lerp(_pointsAlphaDefault, 0f, t);
			scoreType.alpha = Mathf.Lerp(_scoreTypeAlphaDefault, 0f, t);
			if (_current >= _duration)
			{
				_current = 0f;
				_cachedTransform.localPosition = _resetPosition;
				background.alpha = _backgroundAlphaDefault;
				iconBg.alpha = _iconBgAlphaDefault;
				icon.alpha = _iconAlphaDefault;
				head.alpha = _headAlphaDefault;
				points.alpha = _pointsAlphaDefault;
				scoreType.alpha = _scoreTypeAlphaDefault;
				currentAnimationState = AnimatingState.OffScreen;
				_topRun = _highestScore.Next();
				if (_topRun != null)
				{
					SetNewHighScore();
					points.text = CalculateGapToFriendsScore(_HighestScore, GameStats.Instance.score).ToString();
					RefreshUI();
					AnimateIn();
				}
				else
				{
					_passedHighScore = true;
					points.text = "0";
				}
			}
			else
			{
				_cachedTransform.localPosition = _vector;
			}
			break;
		}
		}
		if (currentAnimationState == AnimatingState.AnimatingOut || currentAnimationState == AnimatingState.OffScreen || currentAnimationState == AnimatingState._notset || !_gameRunning || _passedHighScore || PlayerInfo.Instance.tutorialStep <= 0 || _passedHighScore)
		{
			return;
		}
		if (_HighestScore >= GameStats.Instance.score)
		{
			points.text = CalculateGapToFriendsScore(_HighestScore, GameStats.Instance.score).ToString();
			return;
		}
		AnimateOut();
		if (_topRun == null)
		{
			_passedHighScore = true;
			return;
		}
		points.text = "0";
		_highestScore.lastBeatenTopRun = _topRun;
	}
}
