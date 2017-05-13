using DemoCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParticleSystemDemo
{
    public class MainViewModel : BaseViewModel
    {
        public Stream ParticleTexture { set; get; }

        public MainViewModel()
        {
            ParticleTexture = new FileStream(new System.Uri(@"Snowflake.png", System.UriKind.RelativeOrAbsolute).ToString(), FileMode.Open);
        }
    }
}
