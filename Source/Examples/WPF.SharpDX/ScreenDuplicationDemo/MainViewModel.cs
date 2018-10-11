using DemoCore;
using HelixToolkit.Wpf.SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenDuplicationDemo
{
    public class MainViewModel :  BaseViewModel
    {
        public MainViewModel()
        {
            //Make sure to manually set device index to the default device(integrated graphics card) if using laptop with multiple graphics card.
            //Reference: https://social.msdn.microsoft.com/Forums/vstudio/en-US/9189da74-7b83-4a20-b0c1-7218ea38d633/does-desktop-duplication-api-work-only-on-default-graphics-adapter?forum=vcgeneral
            EffectsManager = new DefaultEffectsManager(0);
        }
    }
}
