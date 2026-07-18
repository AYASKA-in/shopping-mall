using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

var app = new Application();
var window = new Window();

try
{
    var xaml = @"
<Window xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
        xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
    <Window.Resources>
        <BooleanToVisibilityConverter x:Key='BoolToVis' />
    </Window.Resources>
    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height='Auto'/>
            <RowDefinition Height='*'/>
        </Grid.RowDefinitions>
        <Border Grid.Row='0' Background='#1A56DB' Padding='10'>
            <TextBlock Text='Hello' />
        </Border>
    </Grid>
</Window>";
    var reader = new System.Xml.XmlReaderSettings { DtdProcessing = System.Xml.DtdProcessing.Prohibit };
    using var xmlReader = System.Xml.XmlReader.Create(new System.IO.StringReader(xaml), reader);
    var obj = XamlReader.Load(xmlReader);
    "XAML loaded OK".Dump();
}
catch (System.Exception ex)
{
    $"ERROR: {ex.GetType().Name}: {ex.Message}".Dump();
}

public static class Ext
{
    public static void Dump(this string s) => System.Console.WriteLine(s);
}
