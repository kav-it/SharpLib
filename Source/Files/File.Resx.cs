//*****************************************************************************
//
// Имя файла    : 'FileResx.cs'
// Заголовок    : Формат "Resx"
// Автор        : Крыцкий А.В.
// Контакты     : kav.it@mail.ru 
// Дата         : 05/04/2014
//
//*****************************************************************************

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;

namespace SharpLib.Resx
{

    #region Класс ResxFile

    public class ResxFile
    {
        #region Constructors

        public ResxFile(string filename) : this()
        {
            Filename = filename;
        }

        public ResxFile()
        {
            UseResxNodes = true;
            Items = new List<ResxRecord>();
        }

        #endregion

        #region Properties

        public bool UseResxNodes { get; set; }

        public string Filename { get; set; }

        public List<ResxRecord> Items { get; set; }

        #endregion

        #region Methods

        public string LoadTextInstance(string filename)
        {
            return "";
        }

        public void LoadFileInstance(string filename)
        {
            using (var reader = new ResXResourceReader(filename) { UseResXDataNodes = UseResxNodes })
            {
                var items = reader
                    .Cast<DictionaryEntry>()
                    .Select(entry => (ResXDataNode)entry.Value).ToList();
            }

            //using (var writer = new ResXResourceWriter(path))
            //{
            //    resources = resources.Where(str => !manualResources.ContainsKey(str.Key)).ToList();

            //    foreach (KeyValuePair<String, ResourceItem> resource in resources)
            //    {
            //        var resXDataNode = new ResXDataNode(resource.Key, resource.Value.Value)
            //        {
            //            Comment = CommonUtils.AUTO_GENERATED_RESOURCE_COMMENT
            //        };
            //        writer.AddResource(resXDataNode);
            //    }
            //    foreach (ResXDataNode manualResource in manualResources.Values)
            //    {
            //        writer.AddResource(manualResource);
            //    }
            //    writer.Generate();
            //}
            //foreach (KeyValuePair<String, String> project in includeInProjects)
            //{
            //    CommonUtils.AddResourceToProject(project.Key, project.Value, path);
            //}            
        }

        public void Load(string filename)
        {
            using (var reader = new ResXResourceReader(filename) { UseResXDataNodes = UseResxNodes })
            {
                var items = reader
                    .Cast<DictionaryEntry>()
                    .Select(entry => (ResXDataNode)entry.Value).ToList();

                Items.AddRange(
                    items.Select(item => new ResxRecord
                    {
                        Name = item.Name,
                        Comment = item.Comment,
                        Value = item.GetValue((ITypeResolutionService)null)
                    })
                    );
            }

            Filename = filename;
        }

        public void Save()
        {
            Save(Filename);
        }

        public void Save(string filename)
        {
            using (var writer = new ResXResourceWriter(filename))
            {
                Items.ForEach(item =>
                {
                    var res = new ResXDataNode(item.Name, item.Value) { Comment = item.Comment };
                    writer.AddResource(res);
                });

                writer.Generate();
            }
        }

        #endregion
    }

    #endregion Класс ResxFile

    #region Класс ResxRecord

    public class ResxRecord
    {
        #region Properties

        public string Name { get; set; }

        public object Value { get; set; }

        public string Comment { get; set; }

        #endregion
    }

    #endregion Класс ResxRecord

    #region Класс Resgen

    public class Resgen
    {
        #region Constants

        private const string BuildCultureKey = @"/culture";

        private const string BuildEmbedKey = @"/embed";

        private const string BuildOutputKey = @"/out";

        private const string BuildTemplateKey = @"/template";

        private const string PathAlVs2010 = @"c:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\Bin\al.exe";

        private const string PathAlVs2012 = @"c:\Program Files (x86)\Microsoft SDKs\Windows\v8.0A\bin\NETFX 4.0 Tools\al.exe";

        private const string PathResgenVs2010 = @"c:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\Bin\resgen.exe";

        private const string PathResgenVs2012 = @"c:\Program Files (x86)\Microsoft SDKs\Windows\v8.0A\bin\NETFX 4.0 Tools\resgen.exe";

        private const string ResgenCompileKey = @"/compile";

        private const string ResgenRelativeKey = @"/useSourcePath";

        #endregion

        #region Fields

        private static string _alPath;

        private static string _resgenPath;

        #endregion

        #region Properties

        public static string ResgenPath
        {
            get
            {
                if (_resgenPath.IsNotValid())
                {
                    if (Files.IsExists(PathResgenVs2012))
                    {
                        _resgenPath = PathResgenVs2012;
                    }
                    else if (Files.IsExists(PathResgenVs2010))
                    {
                        _resgenPath = PathResgenVs2010;
                    }
                }

                return _resgenPath;
            }
            set { _resgenPath = value; }
        }

        public static string AlPath
        {
            get
            {
                if (_alPath.IsNotValid())
                {
                    if (Files.IsExists(PathAlVs2012))
                    {
                        _alPath = PathResgenVs2012;
                    }
                    else if (Files.IsExists(PathAlVs2010))
                    {
                        _alPath = PathResgenVs2010;
                    }
                }

                return _alPath;
            }
            set { _alPath = value; }
        }

        #endregion

        #region Methods

        public static ResgenResult Compile(string inputfile)
        {
            return Compile(new List<string> { inputfile }, "");
        }

        public static ResgenResult Compile(string outputfile, List<ResxFile> resources)
        {
            var inputfiles = new List<string>();

            resources.ForEach(res =>
            {
                var filename = Path.GetFileNameWithoutExtension(res.Filename) + "_";
                filename = Files.GetTempFilename(filename);

                res.Save(filename);

                inputfiles.Add(filename);
            });

            return Compile(inputfiles, "");
        }

        public static ResgenResult Compile(List<string> inputfiles, string outputDir)
        {
            var outputFilenames = new List<string>();
            string options = "";

            foreach (var inputFilename in inputfiles)
            {
                string outputFilename = outputDir.IsValid()
                    ? Path.Combine(outputDir, Path.GetFileName(inputFilename))
                    : inputFilename;

                outputFilename = Path.ChangeExtension(outputFilename, ".resources");

                options += string.Format("{0},{1} ", inputFilename, outputFilename);

                outputFilenames.Add(outputFilename);
            }

            options = string.Format("{0} {1} {2}", ResgenRelativeKey, ResgenCompileKey, options);

            var processResult = Files.RunBat(ResgenPath, options);

            var result = new ResgenResult
            {
                Result = processResult,
                CompiledFilenames = outputFilenames
            };

            return result;
        }

        public static ResgenResult BuildSatellite(string outputFilename, CultureInfo culture, string template, List<string> inputfiles)
        {
            // Пример выполнения 
            // 
            // C:\Program Files (x86)\Microsoft SDKs\Windows\v8.0A\bin\NETFX 4.0 Tools\AL.exe 
            //    /culture:en-US 
            //    /out:obj\Debug\en-US\Resto.VideoUtils.resources.dll 
            //    /template:obj\Debug\Resto.VideoUtils.dll 
            //    /embed:obj\Debug\Resto.VideoUtils.Properties.Resources.en-US.resources 
            //    /embed:obj\Debug\Resto.VideoUtils.AxisCamera.en-US.resources 
            //    /embed:obj\Debug\Resto.VideoUtils.DLink2121.en-US.resources 
            //    /embed:obj\Debug\Resto.VideoUtils.DLink950.en-US.resources

            string options = "";

            foreach (var inputFilename in inputfiles)
            {
                options += string.Format("{0}:{1} ", BuildEmbedKey, inputFilename);
            }

            options = string.Format("{0}:{1} {2}:{3} {4}:{5} {6}", 
                BuildCultureKey, culture.Name,
                BuildOutputKey, outputFilename,
                BuildTemplateKey, template,
                options);

            var processResult = Files.RunBat(AlPath, options);

            var result = new ResgenResult
            {
                Result = processResult,
            };

            return result;
        }

        #endregion
    }

    #endregion Класс Resgen

    #region Класс ResgenResult

    public class ResgenResult
    {
        #region Properties

        public ProcessExecResult Result { get; set; }

        public List<string> CompiledFilenames { get; set; }

        #endregion
    }

    #endregion Класс ResgenResult
}