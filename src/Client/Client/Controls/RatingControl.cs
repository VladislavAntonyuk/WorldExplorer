namespace Client.Controls;

using Resources.Fonts;

public class RatingControl : HorizontalStackLayout
{
	internal enum State
	{
		Empty,
		Half,
		Full
	}

	public static readonly BindableProperty AllowRatingProperty = BindableProperty.Create(nameof(AllowRating), typeof(bool), typeof(RatingControl), propertyChanged: UpdateLayout);
	public static readonly BindableProperty ValueProperty = BindableProperty.Create(nameof(Value), typeof(double), typeof(RatingControl), defaultBindingMode: BindingMode.TwoWay, propertyChanged: UpdateLayout);

	private static void UpdateLayout(BindableObject bindable, object oldvalue, object newvalue)
	{
		((RatingControl)bindable).UpdateLayout();
	}

	public double Value
	{
		get => (double)GetValue(ValueProperty);
		set => SetValue(ValueProperty, value);
	}

	public bool AllowRating
	{
		get => (bool)GetValue(AllowRatingProperty);
		set => SetValue(AllowRatingProperty, value);
	}

	public RatingControl()
	{
		HorizontalOptions = LayoutOptions.Center;
		VerticalOptions = LayoutOptions.Center;

		UpdateLayout();
	}

	private void UpdateLayout()
	{
		Children.Clear();
		const int maxValue = 5;
		var intValue = (int)Math.Clamp(Value, 0, maxValue);
		var decimalPart = Value - intValue;
		var isHalfStar = false;

		if (decimalPart > .25)
		{
			if (decimalPart is > 0.25 and <= .75)
			{
				isHalfStar = true;
			}
			else
			{
				intValue++;
			}
		}

		for (var i = 0; i < maxValue; i++)
		{
			if (intValue > i)
			{
				Add(CreateLabel(State.Full, i + 1));
			}
			else if (intValue <= i && isHalfStar)
			{
				Add(CreateLabel(State.Half, i + 1));
				isHalfStar = false;
			}
			else
			{
				Add(CreateLabel(State.Empty, i + 1));
			}
		}
	}

	private Label CreateLabel(State state, int i)
	{
		Label label = new()
		{
			FontSize = 20,
			FontFamily = state == State.Empty ? "FARegular" : "FASolid",
			TextColor = state == State.Empty ? Colors.White : Colors.Yellow,
			Text = state switch
			{
				State.Empty => FontAwesomeIcons.Star,
				State.Half => FontAwesomeIcons.StarHalf,
				_ => FontAwesomeIcons.Star
			}
		};

		if (AllowRating)
		{
			label.GestureRecognizers.Add(new TapGestureRecognizer()
			{
				NumberOfTapsRequired = 1,
				Command = new Command(() => Value = i)
			});
		}


		return label;
	}
}