
using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using ScriptEditor.Core.SimpleArgs;

public interface ISimpleArgHandler
{
    void Load(object value);
    object GetValue();
    void AddTo(Control control);

    public static ISimpleArgHandler CreateHandler(SimpleArgConfig config, ScriptNodeDisplay display)
    {
        return config switch
        {
            BoolArgConfig boolArg => new BoolArgHandler(boolArg, display),
            EnumArgConfig enumArg => new EnumArgHandler(enumArg, display),
            IntegerArgConfig intArg => new IntegerArgHandler(intArg, display),

            StringArgConfig stringArg => stringArg.Multiline 
                ? new TextEditStringArgHandler(stringArg, display)
                : new LineEditStringArgHandler(stringArg, display),
            _ => throw new Exception($"No simple argument handler type found for {config.GetType()}"),
        };
    }

    public static ISimpleArgHandler Create(ScriptNodeSimpleArg arg, ScriptNodeDisplay display)
    {
        // TODO don't like this

        var el = arg switch
        {
            ScriptNodeArrayArg arrayArg => new ArrayArgHandler(arrayArg, display),
            _ => CreateHandler(arg.Config, display)
        };

        el.AddTo(display);
        return el;
    }
}

public class ArrayArgHandler : ISimpleArgHandler
{
    private readonly List<ISimpleArgHandler> _handlers;
    private readonly ScriptNodeDisplay _display;
    private readonly ScriptNodeArrayArg _arg;
    private readonly Container _container;
    private readonly PanelContainer _panel;
    private readonly Button _button;

    public ArrayArgHandler(ScriptNodeArrayArg arg, ScriptNodeDisplay display)
    {
        _display = display;
        _handlers = [];
        _arg = arg;

        _panel = new PanelContainer();

        _container = new VBoxContainer();
        _panel.AddChild(_container);

        _button = new Button()
        {
            Text = arg.AddButtonText
        };

        _button.Pressed += OnAddNewButtonPressed;
        _container.AddChild(_button);
    }

    public void AddTo(Control control)
    {
        control.AddChild(_panel);
    }

    public object GetValue()
    {
        return _handlers.Select(h => h.GetValue()).ToList();
    }

    public void Load(object value)
    {
        if (value is not List<object> list)
            throw new Exception($"Provided a non-list value for {nameof(ArrayArgHandler)} (actual type: {value.GetType()})");

        foreach (var item in list)
        {
            var el = CreateNewRow();
            el.Load(item);
        }
    }

    private ISimpleArgHandler CreateNewRow()
    {
        var newEl = ISimpleArgHandler.CreateHandler(_arg.Config, _display);

        var container = new HBoxContainer();
        newEl.AddTo(container);

        _container.AddChild(container);
        var removeBtn = new Button()
        {
            Text = " X "
        };
        container.AddChild(removeBtn);
        removeBtn.Pressed += () =>
        {
            _container.RemoveChild(container);
            container.QueueFree();
            _handlers.Remove(newEl);
            _display.Editor.UpdateState();
        };

        _container.MoveChild(_button, -1);
        
        _handlers.Add(newEl);
        return newEl;
    }

    private void OnAddNewButtonPressed()
    {
        CreateNewRow();
        _display.Editor.UpdateState();
    }
}

public class BoolArgHandler : ISimpleArgHandler
{
    private readonly CheckBox _checkbox;
    public BoolArgHandler(BoolArgConfig arg, ScriptNodeDisplay display)
    {
        _checkbox = new CheckBox()
        {
            Text = arg.Label,
            ButtonPressed = arg.Default,
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
        };
        _checkbox.Pressed += display.Editor.UpdateState;
    }

    public void Load(object value)
    {
        if (value is not bool v)
            throw new Exception($"Provided non-bool value for {nameof(BoolArgHandler)}");
        _checkbox.ButtonPressed = v;
    }

    public object GetValue() => _checkbox.ButtonPressed;

    public void AddTo(Control control)
    {
        control.AddChild(_checkbox);
    }
}

public class EnumArgHandler : ISimpleArgHandler
{
    private readonly OptionButton _optionButton;
    public EnumArgHandler(EnumArgConfig arg, ScriptNodeDisplay display)
    {
        _optionButton = new OptionButton()
        {
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
        };

        foreach (var (key, value) in arg.Values)
        {
            _optionButton.AddItem(key);
            _optionButton.SetItemMetadata(_optionButton.GetChildCount() - 1, value);
        }

        _optionButton.ItemSelected += (_) => display.Editor.UpdateState();
    }

    public void Load(object value)
    {
        if (value is not string label)
            throw new Exception($"Provided non-string value for {nameof(EnumArgHandler)}");

        for (int i = 0; i < _optionButton.ItemCount; ++i)
        {
            if (_optionButton.GetItemText(i) != label) continue;

            _optionButton.Select(i);
            return;
        }

        throw new Exception($"Provided unrecognizable value for enum arg: {label}");
    }

    public object GetValue() => _optionButton.GetItemText(_optionButton.Selected);

    public void AddTo(Control control)
    {
        control.AddChild(_optionButton);
    }
}

public class IntegerArgHandler : ISimpleArgHandler
{
    private readonly SpinBox _spinBox;
    public IntegerArgHandler(IntegerArgConfig arg, ScriptNodeDisplay display)
    {
        _spinBox = new SpinBox
        {
            Prefix = arg.Label,
            MinValue = int.MinValue,
            MaxValue = int.MaxValue,
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
        };

        if (arg.HasMin)
            _spinBox.MinValue = arg.Min;
        if (arg.HasMax)
            _spinBox.MaxValue = arg.Max;

        _spinBox.ValueChanged += (_) => display.Editor.UpdateState();
        _spinBox.SetValueNoSignal(arg.Default);

    }

    public void Load(object value)
    {
        if (value is not long v)
            throw new Exception($"Provided non-long value for {nameof(IntegerArgHandler)}");
        _spinBox.SetValueNoSignal(v);
    }

    public object GetValue() => Convert.ToInt64(_spinBox.Value);

    public void AddTo(Control control)
    {
        control.AddChild(_spinBox);
    }
}

public class TextEditStringArgHandler : ISimpleArgHandler
{
    private readonly TextEdit _textEdit;
    public TextEditStringArgHandler(StringArgConfig arg, ScriptNodeDisplay display)
    {
        _textEdit = new TextEdit()
        {
            Text = arg.Default,
            PlaceholderText = arg.Placeholder,
            SizeFlagsVertical = Control.SizeFlags.ExpandFill,
            WrapMode = TextEdit.LineWrappingMode.Boundary,
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
        };

        _textEdit.TextChanged += display.Editor.UpdateState;
        _textEdit.CustomMinimumSize = new(0, 100);
    }

    public void Load(object value)
    {
        if (value is not string text)
            throw new Exception($"Provided non-string value for {nameof(TextEditStringArgHandler)}");
        _textEdit.Text = text;
    }

    public object GetValue() => _textEdit.Text;

    public void AddTo(Control control)
    {
        control.AddChild(_textEdit);
    }
}

public class LineEditStringArgHandler : ISimpleArgHandler
{
    private readonly LineEdit _lineEdit;
    public LineEditStringArgHandler(StringArgConfig arg, ScriptNodeDisplay display)
    {
        _lineEdit = new LineEdit()
        {
            Text = arg.Default,
            PlaceholderText = arg.Placeholder,
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
        };

        _lineEdit.TextChanged += (_) => display.Editor.UpdateState();
    }

    public void Load(object value)
    {
        if (value is not string text)
            throw new Exception($"Provided non-string value for {nameof(LineEditStringArgHandler)}");
        _lineEdit.Text = text;
    }

    public object GetValue() => _lineEdit.Text;

    public void AddTo(Control control)
    {
        control.AddChild(_lineEdit);
    }
}

