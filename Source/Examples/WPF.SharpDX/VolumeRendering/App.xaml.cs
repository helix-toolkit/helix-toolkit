using HelixToolkit.Wpf.SharpDX.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace VolumeRendering
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private NVOptimusEnabler optEnabler = new NVOptimusEnabler();
    }
}
