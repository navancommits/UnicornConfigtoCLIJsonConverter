using static System.Windows.Forms.LinkLabel;

namespace UnicorntoCLIConverter
{

    public partial class frmConverterUtility : Form
    {
        private string ModuleNameLine = string.Empty;
        private string referencesLine = string.Empty;
        private string startingLine = "{";
        private string itemsEndLine = "\r\t}";
        private string endLine = "\r}";
        private bool includesPresent = false;
        private bool excludesPresent = false;
        private string includeModuleName = string.Empty;
        private string includeModulePath = string.Empty;
        private string includeModuleDB = string.Empty;
        private int predicateStartLineNum;
        private int predicateEndLineNum;
        private string strConvertedConcatIncludeLines;
        private string endArrayBracket;
        private string referenceName;
        private string[] lstConfig;
        private string ruleList;
        private int intLineNumTracker = 0;
        private int intlastInclude = 0;
        private bool RulesListed = false;
        private int intCurrentInclude = 0;
        private int intIncludeCount = 0;
        private int intRuleCount = 0;
        private List<Predicate> PredicateList;
        private List<Include> IncludeList;
        private List<Exclude> ExcludeList;
        private List<Except> ExceptList;
        private List<Configuration> ConfigurationList;
        private Configuration configuration;
        private Predicate predicate;
        private Configurations configurations;
        private Include include;
        private Exclude exclude;
        private Except except;
        private TagInfo tagInfo;
        private int configsTagCount;
        private int predicatesTagCount;
        private int includesTagCount;
        private int exceptsTagCount;
        private List<TagInfo> tags;
        private int predicateNumber = 0;
        private int includeNumber = 0;
        private int excludeNumber = 0;
        private int exceptNumber = 0;
        private int configurationNumber = 0;
        private int ConfigurationStartLine = 0;
        private int PredicateStartLine = 0;
        private List<string> FileNameList;
        private int intFileIndex = 0;
        private string FilePaths = string.Empty;
        private List<int> CommentedLines;
        public frmConverterUtility()
        {
            InitializeComponent();
        }

        //private void ExtractCommentedLines(string filePath)
        //{
        //    string configFileData = File.ReadAllText(filePath);
        //    string[] lstConfig = configFileData.Split(new Char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        //    int intLineNumTrackerIndex = 0;

        //    foreach (var line in lstConfig)
        //    {
        //        if (line.Trim().StartsWith("<!--"))
        //        {
        //            do
        //            {
        //                intLineNumTrackerIndex += 1;
        //                CommentedLines.Add(intLineNumTrackerIndex);
        //            } while (line.Trim().EndsWith("-->"));
        //        }
        //    }
        //}

        private void GetLineNumbers(string configData)
        {

            PredicateList = new List<Predicate>();
            IncludeList = new List<Include>();
            ExceptList = new List<Except>();
            ExcludeList = new List<Exclude>();
            ConfigurationList = new List<Configuration>();

            string strConfigText = configData;
            string[] lstConfig = strConfigText.Split(new Char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            int intLineNumTrackerIndex = 0;

            foreach (var line in lstConfig)
            {
                if (line.ToLowerInvariant().Contains("<configurations>"))
                {
                    configurations = new Configurations
                    {
                        StartLineIndex = intLineNumTrackerIndex
                    };

                }

                if (line.ToLowerInvariant().Contains("</configurations>"))
                {
                    configurations.EndLineIndex = intLineNumTrackerIndex;
                }

                if (line.ToLowerInvariant().Contains("<configuration ") && line.ToLowerInvariant().Contains("name"))
                {
                    configurationNumber += 1;
                    configuration = new Configuration
                    {
                        StartLineIndex = intLineNumTrackerIndex,
                        ModuleName= ExtractValueBetweenQuotes(line, "name=")
                };

                }

                if (line.ToLowerInvariant().Contains("</sitecore>")) break; //safety net for next line

                if (line.ToLowerInvariant().Contains("</configuration>"))
                {
                    configuration.EndLineIndex = intLineNumTrackerIndex;
                    configuration.ConfigurationNumber = configurationNumber;
                    ConfigurationList.Add(configuration);

                }

                if (line.ToLowerInvariant().Contains("<predicate"))
                {
                    predicateNumber += 1;
                    predicate = new Predicate
                    {
                        StartLineIndex = intLineNumTrackerIndex
                    };
                }

                if (line.ToLowerInvariant().Contains("</predicate>"))
                {
                    predicate.EndLineIndex = intLineNumTrackerIndex;
                    predicate.PredicateNumber = predicateNumber;
                    PredicateList.Add(predicate);
                }

                if (line.ToLowerInvariant().Contains("<include") && line.ToLowerInvariant().Contains("/>"))
                {
                    includeNumber += 1;
                    include = new Include
                    {
                        StartLineIndex = intLineNumTrackerIndex,
                        EndLineIndex = intLineNumTrackerIndex,
                        PredicateNumber = predicateNumber,
                        IncludeNumber = includeNumber

                    };
                    IncludeList.Add(include);
                }
                else if (line.ToLowerInvariant().Contains("<include") && !line.ToLowerInvariant().Contains("/>"))
                {
                    includeNumber += 1;
                    include = new Include
                    {
                        StartLineIndex = intLineNumTrackerIndex
                    };
                }

                if (line.ToLowerInvariant().Contains("</include>"))
                {
                    include.EndLineIndex = intLineNumTrackerIndex;
                    include.PredicateNumber = predicateNumber;
                    include.IncludeNumber = includeNumber;
                    IncludeList.Add(include);

                }

                if (line.ToLowerInvariant().Contains("<exclude") && line.ToLowerInvariant().Contains("/>"))
                {
                    excludeNumber += 1;
                    exclude = new Exclude
                    {
                        StartLineIndex = intLineNumTrackerIndex,
                        EndLineIndex = intLineNumTrackerIndex,
                        PredicateNumber = predicateNumber,
                        IncludeNumber = includeNumber,
                        ExcludeNumber = excludeNumber
                    };
                    ExcludeList.Add(exclude);
                }
                else if (line.ToLowerInvariant().Contains("<exclude") && !line.ToLowerInvariant().Contains("/>"))
                {
                    excludeNumber += 1;
                    exclude = new Exclude
                    {
                        StartLineIndex = intLineNumTrackerIndex
                    };
                }

                if (line.ToLowerInvariant().Contains("</exclude>"))
                {
                    exclude.EndLineIndex = intLineNumTrackerIndex;
                    exclude.PredicateNumber = predicateNumber;
                    exclude.IncludeNumber = includeNumber;
                    exclude.ExcludeNumber = excludeNumber;
                    ExcludeList.Add(exclude);

                }

                if (line.ToLowerInvariant().Contains("<except") && line.ToLowerInvariant().Contains("/>"))
                {
                    exceptNumber += 1;
                    except = new Except
                    {
                        StartLineIndex = intLineNumTrackerIndex,
                        EndLineIndex = intLineNumTrackerIndex,
                        PredicateNumber = predicateNumber,
                        IncludeNumber = includeNumber,
                        ExceptNumber = exceptNumber
                    };
                    ExceptList.Add(except);
                }

                intLineNumTrackerIndex += 1;
            }
        }

        private void GetPredicateLineNumbers(string configFileData)
        {
            string strConfigText = configFileData;
            string[] lstConfig = strConfigText.Split(new Char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            int intLineNumTrackerIndex = 0;

            foreach (var line in lstConfig)
            {
                if (line.ToLowerInvariant().Contains("<predicate>"))
                {
                    predicateStartLineNum = intLineNumTrackerIndex;
                }

                if (line.ToLowerInvariant().Contains("</predicate>"))
                {
                    predicateEndLineNum = intLineNumTrackerIndex;
                    break;
                }

                if (line.ToLowerInvariant().Contains("<include"))
                {
                    includesPresent = true;
                }

                intLineNumTrackerIndex += 1;
            }

        }

        private int IndexofnthOccurence(string mainString, char subString, int intOccurrence)
        {
            var str = mainString;
            var ch = subString;
            var n = intOccurrence;
            var result = str
                .Select((c, i) => new { c, i })
                .Where(x => x.c == ch)
                .Skip(n - 1)
                .FirstOrDefault();
            return result?.i ?? -1;
        }

        private void CountInclude(int predicateNumber)
        {
            int includeCount = 0;            

            for (int intCounter = PredicateList[predicateNumber - 1].StartLineIndex; intCounter <= PredicateList[predicateNumber - 1].EndLineIndex; intCounter++)
            {
                string currline = lstConfig[intCounter];

                if (currline.ToLowerInvariant().Contains("<include")) includeCount += 1;

            }

            intIncludeCount = includeCount;
        }

        private string GetInfoforInclude()
        {
            string convertedLine = string.Empty;
            string currline = lstConfig[intLineNumTracker];

            if (currline.ToLowerInvariant().Contains("include") && currline.ToLowerInvariant().Contains("name=") &&
                currline.ToLowerInvariant().Contains("database=") && currline.ToLowerInvariant().Contains("path="))
            {
                includeModuleName = ExtractValueBetweenQuotes(currline, "name=");
                includeModulePath = ExtractValueBetweenQuotes(currline, "path=");
                includeModuleDB = ExtractValueBetweenQuotes(currline, "database=");

                convertedLine += "\r\t\t\t{";

                convertedLine += "\r\t\t\t\t \"name\" : " + includeModuleName + ",";
                convertedLine += "\r\t\t\t\t \"path\" : " + includeModulePath + ",";
                convertedLine += "\r\t\t\t\t \"database\" : " + includeModuleDB;

                if (currline.Trim().ToLowerInvariant().Substring(currline.Trim().Length - 2, 2) != "/>")
                {
                    if (string.IsNullOrWhiteSpace(BuildRules(convertedLine)))
                    {
                        convertedLine += string.Empty;
                        excludesPresent = false;
                    }
                    else
                    {
                        convertedLine += BuildRules(convertedLine);
                        convertedLine += ",";
                        excludesPresent = true;
                    }
                }

                if (!string.IsNullOrWhiteSpace(ruleList)) convertedLine += ruleList;
                ruleList = string.Empty;

                convertedLine += "\r\t\t\t}";

                intCurrentInclude += 1;

                if (intCurrentInclude >= intIncludeCount)
                {
                    convertedLine += "";
                }
                else
                {
                    convertedLine += ",";
                }
            }

            return convertedLine;
        }

        private void ExtractCommentedLineNumbers()
        {
            for (int intTracker = configurations.StartLineIndex; intTracker <= configurations.EndLineIndex; intTracker++)
            {
                string currline = lstConfig[intTracker];

                if (currline.Trim().StartsWith("<!--") && currline.Trim().EndsWith("-->"))
                    CommentedLines.Add(intTracker);

                if (currline.Trim().StartsWith("<!--") && !currline.Trim().EndsWith("-->"))
                {
                    do
                    {
                        CommentedLines.Add(intTracker);
                        intTracker += 1;
                    } while (!lstConfig[intTracker].Trim().EndsWith("-->"));
                }


                if (currline.Trim().StartsWith("-->"))
                    CommentedLines.Add(intTracker);

            }

        }

        private string BuildRules(string convertedlines)
        {
            RulesListed = false;
            do
            {
                if (!IsBlankLine(intLineNumTracker))
                {
                    var currline = lstConfig[intLineNumTracker];

                    if (currline.ToLowerInvariant().Contains("exclude") && currline.ToLowerInvariant().Contains("children=\"true\""))
                    {
                        convertedlines = "\r\t\t\t\t \"scope\" : \"SingleItem\"";
                        var nextline = lstConfig[intLineNumTracker + 1];
                        //if (!nextline.ToLowerInvariant().Contains("except")) return convertedlines;
                        if (!CommentedLines.Contains(intLineNumTracker+1))
                        if (!nextline.ToLowerInvariant().Contains("except")) return convertedlines;//uncommented since without this scope is not added when exclude has only one except with children=true - xxxx.Feature.Serialization.config.

                    }

                    var prevline = lstConfig[intLineNumTracker - 1];
                    if (currline.ToLowerInvariant().Contains("except"))
                    {
                        //these must be serialized too
                        if (string.IsNullOrWhiteSpace(ruleList))
                        {
                            ruleList += "\r\t\t\t\t \"rules\": [";
                        }

                        do
                        {
                            if (!IsBlankLine(intLineNumTracker))  currline = lstConfig[intLineNumTracker];

                            if (currline.ToLowerInvariant().Contains("except ") && currline.ToLowerInvariant().Contains("name"))
                            {
                                var extractChildtoInclude = ExtractValueBetweenQuotes(currline, "name=", true);

                                ruleList += "\r\t\t\t\t\t\t {";

                                if (currline.ToLowerInvariant().Contains("except") && currline.ToLowerInvariant().Contains("includechildren=\"true\""))
                                { ruleList += "\r\t\t\t\t\t\t\t \"scope\" : \"ItemandDescendants\","; }
                                else
                                { ruleList += "\r\t\t\t\t\t\t\t \"scope\" : \"SingleItem\","; }

                                ruleList += "\r\t\t\t\t\t\t\t \"path\" : " + extractChildtoInclude;
                                ruleList += "\r\t\t\t\t\t\t }";

                                // if (lstConfig[intLineNumTracker + 1].Trim() != "</exclude>" && !RulesListed)
                                // {
                                ruleList += ",";
                            }
                            //}

                            intLineNumTracker += 1;

                        } while (lstConfig[intLineNumTracker].Trim() != "</exclude>");

                        if (lstConfig[intLineNumTracker].Trim() == "</exclude>")
                        {
                            ruleList += "\r\t\t\t\t\t\t {";
                            ruleList += "\r\t\t\t\t\t\t\t \"scope\" : \"ignored\",";
                            ruleList += "\r\t\t\t\t\t\t\t \"path\" : \"*\"";
                            ruleList += "\r\t\t\t\t\t\t }";

                            RulesListed = true;
                        }
                    }

                    intLineNumTracker++;
                }

            } while (lstConfig[intLineNumTracker].Trim() != "</include>");

            if (!string.IsNullOrWhiteSpace(ruleList) && lstConfig[intLineNumTracker].Trim() == "</include>")
            {
                ruleList += "\r\t\t\t\t ]";
            }

            return string.Empty;
        }

        private bool IsBlankLine(int currLine)
        {
            if (string.IsNullOrWhiteSpace(lstConfig[currLine])) return true;

            return false;
        }

        private string ExtractValueBetweenQuotes(string currline, string subStringStart, bool path = false)
        {
            int intIncludeModuleName = currline.LastIndexOf(subStringStart, StringComparison.Ordinal);
            if (intIncludeModuleName == -1) return string.Empty;
            string stringforIncludeName = currline.Substring(intIncludeModuleName);

            var intFirst = IndexofnthOccurence(stringforIncludeName, '"', 1);
            var intSecond = IndexofnthOccurence(stringforIncludeName, '"', 2);

            var intStringLen = intSecond - intFirst;

            if (path)
            {

                string value = stringforIncludeName.Substring(intFirst, intStringLen + 1);

                return ReplaceFirst(value, "\"", "\"/");

            }

            return stringforIncludeName.Substring(intFirst, intStringLen + 1);
        }

        public static string ReplaceFirst(string str, string term, string replace)
        {
            int position = str.IndexOf(term);
            if (position < 0)
            {
                return str;
            }
            str = str.Substring(0, position) + replace + str.Substring(position + term.Length);
            return str;
        }

        private string GetConfigurationLine(string currline, int occurrence)
        {
            string moduleLine = string.Empty;
            string refLine = string.Empty;

            if (currline.ToLowerInvariant().Contains("configuration") && currline.ToLowerInvariant().Contains("name"))
            {
                var intFirst = IndexofnthOccurence(currline, '"', 1);
                var intSecond = IndexofnthOccurence(currline, '"', 2);

                var intStringLen = intSecond - intFirst;
                string modulename = currline.Substring(intFirst, intStringLen + 1);

                moduleLine = "\"namespace\": " + modulename + ",";
            }

            if (currline.ToLowerInvariant().Contains("dependencies"))
                refLine = "\"references\": [" + ExtractValueBetweenQuotes(currline.Replace(",", "\",\""), "dependencies=") + "],";

            if (occurrence > 1) return "\r{" + "\r\t" + moduleLine + "\r\t" + refLine + "\r\t" + "\"items\": {";

            return "{" + "\r\t" + moduleLine + "\r\t" + refLine + "\r\t" + "\"items\": {";

            //if (currline.ToLowerInvariant().Contains("</include>")) intIncludeCount += 1;
        }

        private string ConverttoCLIModuleJson(string filePath)
        {
            string configFileData = File.ReadAllText(filePath);
            ruleList = string.Empty;

            if (!(configFileData.ToLowerInvariant().Contains("<predicate") && configFileData.ToLowerInvariant().Contains("<configuration ")))
                    return string.Empty;//not serialization config

            GetLineNumbers(configFileData);

            //GetPredicateLineNumbers(configFileData);
            string strConfigText = configFileData;//txtConfig.Text;
            lstConfig = strConfigText.Split(new Char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            ExtractCommentedLineNumbers();

            string convertedLine = string.Empty;
            int CurrentConfigNumber = 0;

            for (intLineNumTracker = configurations.StartLineIndex; intLineNumTracker <= configurations.EndLineIndex; intLineNumTracker++)
            {
                //if (!CommentedLines.Contains(intLineNumTracker))
                //{

                    foreach (var config in ConfigurationList)
                    {
                        if (intLineNumTracker == config.StartLineIndex)
                        {
                            var line = lstConfig[intLineNumTracker];

                            convertedLine = GetConfigurationLine(line, config.ConfigurationNumber);
                            CurrentConfigNumber = config.ConfigurationNumber;//same as predicate number
                        }

                    }

                    foreach (var predicate in PredicateList)
                    {
                        
                        if (intLineNumTracker >= predicate.StartLineIndex && intLineNumTracker <= predicate.EndLineIndex)
                        {
                            //intCurrentInclude = 0;

                            if (intLineNumTracker == predicate.StartLineIndex)
                            {
                                intCurrentInclude = 0;
                                CountInclude(CurrentConfigNumber);
                                convertedLine += "\r\t\t" + "\"includes\": [";
                            }

                            convertedLine += GetInfoforInclude();

                            if (intLineNumTracker == predicate.EndLineIndex) convertedLine += "\r\t\t" + "]";
                        }

                    }

                    //closing brackets
                    foreach (var config in ConfigurationList)
                    {
                        if (intLineNumTracker == config.EndLineIndex)
                        {

                            var line = lstConfig[intLineNumTracker];

                            if (intLineNumTracker == config.EndLineIndex) convertedLine += itemsEndLine + endLine;

                            SaveFile(convertedLine, config, filePath);//Save one file at a time
                                                                      //if (!string.IsNullOrWhiteSpace(convertedLine))
                                                                      //{
                                                                      //    //ok to save
                                                                      //    var fileName = Path.GetFileNameWithoutExtension(file);
                                                                      //    var jsonFileFullPath = Path.GetDirectoryName(file) + "\\" + Path.GetFileNameWithoutExtension(file) + ".module.json";

                            //    File.WriteAllText(jsonFileFullPath, convertedJsonString);
                            //    filePaths += jsonFileFullPath + "\r";
                            //    fileCount++;
                            //}
                        }
                    }
                //}
            }

            return convertedLine;
        }

        private string SaveFile(string concatenatedLines,Configuration config, string fileFullPath)
        {
            if (!string.IsNullOrWhiteSpace(concatenatedLines))
            {
                //ok to save
                string jsonFileFullPath = string.Empty;

                if (ConfigurationList.Count > 1)
                {
                    jsonFileFullPath = Path.GetDirectoryName(fileFullPath) + "\\" + config.ModuleName.Replace("\"",string.Empty) + ".module.json";
                }
                else
                {
                    var fileName = Path.GetFileNameWithoutExtension(fileFullPath);
                    jsonFileFullPath = Path.GetDirectoryName(fileFullPath) + "\\" + fileName + ".module.json";
                }

                File.WriteAllText(jsonFileFullPath, concatenatedLines);
                FilePaths += jsonFileFullPath + "\r";
            }

            return FilePaths;
        }
        private void btnPreview_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtConfig.Text)) return;

            txtJson.Text = ConverttoCLIModuleJson(txtConfig.Text);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void BrowseFolderButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDlg = new FolderBrowserDialog();
            folderDlg.ShowNewFolderButton = true;
            // Show the FolderBrowserDialog.  
            DialogResult result = folderDlg.ShowDialog();
            if (result == DialogResult.OK)
            {
                txtSelectedPath.Text = folderDlg.SelectedPath;
                Environment.SpecialFolder root = folderDlg.RootFolder;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSelectedPath.Text)) return;
            CommentedLines = new List<int>();

            var ext = new List<string> { "config" };
            var configFiles = Directory
                .EnumerateFiles(txtSelectedPath.Text, "*.config", SearchOption.AllDirectories)
                .Where(s => ext.Contains(Path.GetExtension(s).TrimStart('.').ToLowerInvariant()));

            string filePaths = string.Empty;
            int fileCount = 0;

            foreach (var file in configFiles)
            {
                //ExtractCommentedLines(file);
                configurationNumber = 0;
                var convertedJsonString=ConverttoCLIModuleJson(file);
                if (!string.IsNullOrWhiteSpace(convertedJsonString))
                {
                    filePaths += file + "\r";
                    fileCount++;
                }
            }

            var statusFileFullPath = ".\\unicorntocliconverter-statuslog-" + DateTime.Now.ToString("yyyymmdd Hmmss") + ".txt";
            File.WriteAllText(statusFileFullPath, filePaths);

            MessageBox.Show(fileCount + " file(s) converted");
        }

        internal class Predicate
        {
            internal int StartLineIndex { get; set; }
            internal int EndLineIndex { get; set; }
            internal int PredicateNumber { get; set; }
        }

        internal class FileDetails
        {
            internal string FileName { get; set; }
            internal string FileData { get; set;}
            internal int FileNumber { get; set; }
        }

        internal class Configuration
        {
            internal int StartLineIndex { get; set; }
            internal int EndLineIndex { get; set; }
            internal int ConfigurationNumber { get; set; }
            internal string ModuleName { get; set; }
        }

        internal class Include
        {
            internal int StartLineIndex { get; set; }
            internal int EndLineIndex { get; set; }
            internal int PredicateNumber { get; set; }
            internal int IncludeNumber { get; set; }
        }

        internal class Exclude
        {
            internal int StartLineIndex { get; set; }
            internal int EndLineIndex { get; set; }
            internal int PredicateNumber { get; set; }
            internal int IncludeNumber { get; set; }
            internal int ExcludeNumber { get; set; }
        }

        internal class Except
        {
            internal int StartLineIndex { get; set; }
            internal int EndLineIndex { get; set; }
            internal int PredicateNumber { get; set; }
            internal int IncludeNumber { get; set; }
            internal int ExcludeNumber { get; set; }
            internal int ExceptNumber { get; set; }
        }

        internal class Configurations
        {
            internal int StartLineIndex { get; set; }
            internal int EndLineIndex { get; set; }
        }

        internal class TagInfo
        {
            internal int StartLineIndex { get; set; }
            internal int EndLineIndex { get; set; }
            internal string TagType { get; set; }
            internal int TagTypeCount { get; set; }
            internal int ParentTagPosition { get; set; }
        }
    }
}
