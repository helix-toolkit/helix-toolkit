
namespace HelixToolkit.Wpf.Tests.Helpers
{
    using System.Windows.Controls;
    using System.IO;

    using NUnit.Framework;
    using NUnitHelpers;

    [TestFixture]
    public class Viewport3DHelperTests
    {
        [Test]
        public void Export_Obj_FilesExist()
        {
            var tempFile = Path.GetTempFileName();
            var tempObj = Path.ChangeExtension(tempFile, "obj");
            var tempMtl = Path.ChangeExtension(tempFile, "mtl");

            try
            {
                CrossThreadTestRunner.RunInSTAThrowException(() =>
                {
                    var view = new Viewport3D();
                    view.Export(tempObj);
                });

                FileAssert.Exists(tempObj);
                FileAssert.Exists(tempMtl);
            }
            finally
            {
                File.Delete(tempFile);

                if (File.Exists(tempObj))
                    File.Delete(tempObj);

                if (File.Exists(tempMtl))
                    File.Delete(tempMtl);
            }
        }
    }
}
