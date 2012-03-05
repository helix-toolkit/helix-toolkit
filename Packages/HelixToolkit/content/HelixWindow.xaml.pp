<Window x:Class="$rootnamespace$.HelixWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" xmlns:h="clr-namespace:HelixToolkit.Wpf;assembly=HelixToolkit.Wpf" Title="Helix 3D Toolkit" Height="500" Width="750">
    <Grid>
        <h:HelixViewport3D ZoomExtentsWhenLoaded="True">
            <h:DefaultLights/>
            <h:GridLinesVisual3D/>
            <h:BoxVisual3D Fill="Blue" Center="0,0,5" Width="10" Height="10" Length="10"/>
        </h:HelixViewport3D>
    </Grid>
</Window>
