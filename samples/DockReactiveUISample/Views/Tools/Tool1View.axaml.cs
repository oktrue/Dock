﻿using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace DockReactiveUISample.Views.Tools;

public partial class Tool1View : UserControl
{
    public Tool1View()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
