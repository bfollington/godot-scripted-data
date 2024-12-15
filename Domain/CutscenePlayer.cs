using Godot;
using MoonSharp.Interpreter;
using System.Threading.Tasks;

public partial class CutscenePlayer : Node
{
	[Signal]
	public delegate void CutsceneCompletedEventHandler();

	private MoonSharp.Interpreter.Script _luaScript;
	private DynValue _coroutine;
	private bool _waitingForInput = false;

	public override void _Ready()
	{
		InitializeLua();
	}

	public override void _Input(InputEvent @event)
	{
		if (_waitingForInput && @event.IsPressed())
		{
			_waitingForInput = false;
			ResumeCutscene();
		}
	}

	private void InitializeLua()
	{
		_luaScript = new MoonSharp.Interpreter.Script();

		// Register our API functions
		_luaScript.Globals["say"] = (System.Action<string>)(text =>
		{
			GD.Print($"Dialog: {text}");
		});

		_luaScript.Globals["waitForSeconds"] = (System.Func<double, DynValue>)(seconds =>
		{
			// Create a yielding point for time delay
			var tcs = new TaskCompletionSource<bool>();
			var timer = CreateTimer(seconds);
			timer.Connect("timeout", Callable.From(() =>
			{
				tcs.SetResult(true);
				timer.QueueFree();
				ResumeCutscene();
			}));

			return DynValue.NewYieldReq(new[] { DynValue.NewString("wait_time") });
		});

		_luaScript.Globals["waitForInput"] = (System.Func<DynValue>)(() =>
		{
			_waitingForInput = true;
			return DynValue.NewYieldReq(new[] { DynValue.NewString("wait_input") });
		});
	}

	private Timer CreateTimer(double seconds)
	{
		var timer = new Timer();
		AddChild(timer);
		timer.OneShot = true;
		timer.Start(seconds);
		return timer;
	}

	public void PlayCutscene(string scriptPath)
	{
		try
		{
			var script = FileAccess.GetFileAsString(scriptPath);
			GD.Print($"Loading cutscene: {scriptPath}");

			// Wrap the cutscene in a coroutine
			var wrappedScript = @"
				return function()
					" + script + @"
				end
			";

			// Get the coroutine function
			var func = _luaScript.DoString(wrappedScript);

			// Start the coroutine
			_coroutine = _luaScript.CreateCoroutine(func);
			ResumeCutscene();
		}
		catch (System.Exception e)
		{
			GD.PrintErr($"Error loading cutscene: {e.Message}");
		}
	}

	private void ResumeCutscene()
	{
		try
		{
			var result = _coroutine.Coroutine.Resume();

			if (_coroutine.Coroutine.State == CoroutineState.Dead)
			{
				GD.Print("Cutscene completed");
				EmitSignal(SignalName.CutsceneCompleted);
			}
		}
		catch (System.Exception e)
		{
			GD.PrintErr($"Error in cutscene: {e.Message}");
		}
	}
}
