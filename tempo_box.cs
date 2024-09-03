using Godot;
using System;

public partial class tempo_box : Node
{
	[Export] Button[] TimeButtons;
	[Export] Button ResetButton;
	[Export] Button PauseButton;
	[Export] Button QuitButton;
	[Export] Label TimeLabel;
	[Export] ProgressBar progressBar;
	[Export] AudioStream jingleTimeUp;
	[Export] AudioStream jingleTimeAdd;
	[Export] AudioStream jingleTimeAction;
	[Export] AudioStreamPlayer AudioStreamP;

	protected double Time = 0f;
	protected double MaxTime = 0f;
	protected bool Paused = false;
	protected Window win;
	protected bool Done = true;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		foreach (var btn in TimeButtons)
		{
			btn.Pressed += () => { OnButtonPressed(btn); };
		}
		win = GetViewport().GetWindow();
		PauseButton.Pressed += TogglePause;
		ResetButton.Pressed += OnReset;
		GetTree().AutoAcceptQuit = false;
		QuitButton.Pressed += () => GetTree().Quit();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!Paused)
		{
			if (!Done)
			{
				Time -= delta;
				if (Time <= 0)
				{
					MaxTime = 0f;
					Time = 0f;
					// Play sound
					AudioStreamP.Stream = jingleTimeUp;
					AudioStreamP.Play();
					Done = true;
					progressBar.Value = 100f;
					TimeLabel.Text = "DONE";
				}
				if (win.Visible)
				{
					progressBar.Value = (1f - (Time / MaxTime)) * 100f;
					var mins = (Time / 60f);
					var hour = mins / 60f;
					var min = mins % 60;
					TimeLabel.Text = $"{Mathf.FloorToInt(hour).ToString().PadZeros(2)}:{min:0.00}";
				}
				win.Title = $"{(1f - (Time / MaxTime)) * 100f:0}%";
			}
		}
	}

	void OnButtonPressed(Button btn)
	{
		if (Done)
		{
			AudioStreamP.Stream = jingleTimeAdd;
			AudioStreamP.Play();
		}
		var val = Convert.ToDouble(btn.Name) * 60d;
		MaxTime += val;
		Time += val;
		Done = false;
	}

	void TogglePause()
	{
		Paused = !Paused;
		PauseButton.ToggleMode = Paused;
		PauseButton.Text = Paused ? "PUASED" : "Pause";
	}

	void OnReset()
	{
		MaxTime = 0f;
		Time = 0f;
		Done = true;
		progressBar.Value = 0f;
		TimeLabel.Text = "-";
	}

	public override void _Notification(int what)
	{
		if (what == NotificationWMCloseRequest)
		{
			win.Mode = Window.ModeEnum.Minimized;
			// GetTree().Quit(); // default behavior
		}
	}

}
