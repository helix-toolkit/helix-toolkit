namespace TextDemo
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media.Media3D;

    using ExampleBrowser;

    using HelixToolkit.Wpf;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [Example(null, "TextVisual3D and TextBillboardVisual3D.")]
    public partial class MainWindow : Window
    {
        private const string lorem = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit, sed diam nonummy nibh euismod tincidunt ut laoreet dolore magna aliquam erat volutpat. Ut wisi enim ad minim veniam, quis nostrud exerci tation ullamcorper suscipit lobortis nisl ut aliquip ex ea commodo consequat. Duis autem vel eum iriure dolor in hendrerit in vulputate velit esse molestie consequat, vel illum dolore eu feugiat nulla facilisis at vero eros et accumsan et iusto odio dignissim qui blandit praesent luptatum zzril delenit augue duis dolore te feugait nulla facilisi. Nam liber tempor cum soluta nobis eleifend option congue nihil imperdiet doming id quod mazim placerat facer possim assum. Typi non habent claritatem insitam; est usus legentis in iis qui facit eorum claritatem. Investigationes demonstraverunt lectores legere me lius quod ii legunt saepius. Claritas est etiam processus dynamicus, qui sequitur mutationem consuetudium lectorum. Mirum est notare quam littera gothica, quam nunc putamus parum claram, anteposuerit litterarum formas humanitatis per seacula quarta decima et quinta decima. Eodem modo typi, qui nunc nobis videntur parum clari, fiant sollemnes in futurum.";

        public IList<SpatialTextItem> TextItems { get; set; }

        public IList<BillboardTextItem> TextItems2 { get; set; }

        public IList<BillboardTextItem> TextItems3 { get; set; }

        public MainWindow()
        {
            this.InitializeComponent();
            this.CreateItems();
            this.DataContext = this;
        }

        private void CreateItems()
        {
            this.TextItems = new List<SpatialTextItem>();
            this.TextItems2 = new List<BillboardTextItem>();
            this.TextItems3 = new List<BillboardTextItem>();
            double x = 10;
            double y = 0;
            foreach (var word in lorem.Split(' '))
            {
                this.TextItems.Add(new SpatialTextItem { Text = word, Position = new Point3D(x, y, 0), TextDirection = new Vector3D(1, 0, 0), UpDirection = new Vector3D(0, 1, 0) });
                this.TextItems2.Add(new BillboardTextItem { Text = word, Position = new Point3D(x, y, 1), DepthOffset = 1e-6 });
                y += 1;
            }

            // a single item
            this.TextItems3.Add(new BillboardTextItem { Text = "This is a billboard with a 'pin'.", Position = new Point3D(0.5, 0, 0.5), DepthOffset = 0, WorldDepthOffset = 0.2 });
        }
    }
}
