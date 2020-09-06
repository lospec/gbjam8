using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
	private enum MainMenuButton
	{
		Play,
		Controls,
		Credits
	}

	[SerializeField] private Button _playButton;
	[SerializeField] private Button _controlsButton;
	[SerializeField] private Button _creditsButton;

	private Dictionary<MainMenuButton, Button> _buttons;

	private MainMenuButton _selectedButton;

	private KeyControl[] _rightButtonKeyControls;
	private KeyControl[] _leftButtonKeyControls;
	private KeyControl[] _pressButtonKeyControls;

	private const int RIGHT_KEY_INCREMENT_AMOUNT = 1;
	private const int LEFT_KEY_INCREMENT_AMOUNT = -1;

	private void Awake()
	{
		_rightButtonKeyControls = new[]
		{
			Keyboard.current.rightArrowKey
		};

		_leftButtonKeyControls = new[]
		{
			Keyboard.current.leftArrowKey
		};

		_pressButtonKeyControls = new[]
		{
			Keyboard.current.enterKey,
			Keyboard.current.xKey,
			Keyboard.current.cKey
		};

		_buttons = new Dictionary<MainMenuButton, Button>()
		{
			{ MainMenuButton.Play, _playButton },
			{ MainMenuButton.Controls, _controlsButton },
			{ MainMenuButton.Credits, _creditsButton }
		};
	}

	private void Update()
	{
		if (AreAnyKeysPressed(_rightButtonKeyControls))
		{
			_selectedButton = GetNextButton(_selectedButton, RIGHT_KEY_INCREMENT_AMOUNT);
		}

		if (AreAnyKeysPressed(_leftButtonKeyControls))
		{
			_selectedButton = GetNextButton(_selectedButton, LEFT_KEY_INCREMENT_AMOUNT);
		}

		Button button = _buttons[_selectedButton];
		button.Select();

		if (AreAnyKeysPressed(_pressButtonKeyControls))
		{
			button.onClick?.Invoke();
		}
	}

	private bool AreAnyKeysPressed(KeyControl[] keyControls)
	{
		return keyControls.Any(control => control.wasPressedThisFrame);
	}

	private MainMenuButton GetNextButton(MainMenuButton currentButton, int incrementAmount)
	{
		return (MainMenuButton)Mathf.Abs(Mod((int)currentButton + incrementAmount, Enum.GetNames(typeof(MainMenuButton)).Length));
	}

	private int Mod(int x, int m)
	{
		return (x % m + m) % m;
	}

	public void GoToMapScene()
	{
        TransitionManager.Instance.SingleTransition(false);
        StartCoroutine(WaitAndLoadScene("MapScene"));
	}

	public void GoToControlsScene()
	{
        TransitionManager.Instance.SingleTransition(false);
        StartCoroutine(WaitAndLoadScene("MainMenuControls"));
	}

	public void GoToCreditsScene()
	{
        TransitionManager.Instance.SingleTransition(false);
        StartCoroutine(WaitAndLoadScene("MainMenuCredits"));
	}

    private IEnumerator WaitAndLoadScene(string name)
    {
        yield return new WaitForSeconds(0.5f);

        SceneManager.LoadScene(name);
    }
}
