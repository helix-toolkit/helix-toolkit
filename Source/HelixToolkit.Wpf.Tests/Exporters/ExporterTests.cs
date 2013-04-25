// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExporterTests.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Threading;
using System.Windows.Controls;
using System.Xml;
using System.Xml.Schema;
using HelixToolkit.Wpf;

using NUnitHelpers;

namespace HelixToolkitTests
{
    public class ExporterTests
    {
        protected void ExportSimpleModel(Exporter e)
        {
            var runner = new CrossThreadTestRunner();
            runner.RunInSTA(
                delegate
                {
                    Console.WriteLine(Thread.CurrentThread.GetApartmentState());

                    var vp = new Viewport3D { Camera = CameraHelper.CreateDefaultCamera(), Width = 1280, Height = 720 };
                    vp.Children.Add(new DefaultLights());
                    var box = new BoxVisual3D();
                    box.UpdateModel();
                    vp.Children.Add(box);

                    e.Export(vp);
                });
        }

        /// <summary>
        /// Validates the specified XML file against a XSL schema.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="sc">The schema.</param>
        /// <returns>Number of errors and warnings, or null if the number of errors and warnings is zero.</returns>
        protected string Validate(string path, XmlSchemaSet sc)
        {
            // http://msdn.microsoft.com/en-us/library/as3tta56.aspx
            var settings = new XmlReaderSettings
            {
                ConformanceLevel = ConformanceLevel.Document,
                DtdProcessing = DtdProcessing.Parse,
                ValidationType = ValidationType.Schema,
                Schemas = sc,
                ValidationFlags = XmlSchemaValidationFlags.ProcessSchemaLocation | XmlSchemaValidationFlags.ProcessInlineSchema,
            };

            int warnings = 0;
            int errors = 0;

            settings.ValidationEventHandler += (sender, e) =>
            {
                Console.WriteLine(e.Message);
                if (e.Severity == XmlSeverityType.Warning)
                {
                    warnings++;
                }
                else
                {
                    errors++;
                }
            };

            using (var input = File.OpenRead(path))
            {
                using (var xvr = XmlReader.Create(input, settings))
                {
                    while (xvr.Read())
                    {
                        // do nothing
                    }

                    if (errors + warnings == 0)
                    {
                        return null;
                    }

                    return String.Format("Errors: {0}, Warnings: {1}", errors, warnings);
                    /*
                    catch (XmlSchemaException e)
                    {
                        Console.Error.WriteLine("Failed to read XML: {0}", e.Message);

                    }
                    catch (XmlException e)
                    {
                        Console.Error.WriteLine("XML Error: {0}", e.Message);

                    }
                    catch (IOException e)
                    {
                        Console.Error.WriteLine("IO error: {0}", e.Message);
                    }*/
                }
            }
        }
    }
}