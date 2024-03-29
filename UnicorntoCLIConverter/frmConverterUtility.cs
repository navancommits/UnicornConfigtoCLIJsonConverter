using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.LinkLabel;

namespace UnicorntoCLIConverter
{

    public partial class frmConverterUtility : Form
    {
        private string ModuleNameLine = string.Empty;
        private string referencesLine = string.Empty;
        private string startingLine = "{";
        private string itemsEndLine = "\r\n\t}";
        private string endLine = "\r\n}";
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
        private int intCurrentRoleInclude = 0;
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
        private List<int> CreateOnlyDirectiveConfigNumber;
        private string Mode;
        private string ConfigFileType = string.Empty;
        private string includeDomainName = string.Empty;
        private string includePattern = string.Empty;
        private bool CommentsPresent;
        private int CurrentConfigNumber = 0;
        bool CreateOnlyDirective = false;
        int RunningConfigNumber = 0;
        string SectionNamePrefix = string.Empty;
        string ModuleNametoReplace = string.Empty;
        List<string> DataStorePath;
        string roleList= string.Empty;
        string userList = string.Empty;
        int intRoleIncludeCount = 0;
        int intRunningPredicateNumber = 0;
        List<string> lstFoundationBase = new List<string>();
        List<string> lstFeatureBase = new List<string>();
        List<string> lstProjectBase = new List<string>();
        int ExcludeLineNumberStart = 0;
        

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
            DataStorePath = new List<string>();

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

                if (line.ToLowerInvariant().Contains("<targetdatastore "))
                {
                    var physicalpath=ExtractValueBetweenQuotes(line, "physicalRootPath=");
                    int lastindex=physicalpath.ToLowerInvariant().LastIndexOf("\\serialization\\");

                    //var path=physicalpath.Substring(lastindex + 13, physicalpath.Length - lastindex);

                    if (lastindex > -1)
                    {
                        var path = (Right(physicalpath, physicalpath.Length - lastindex - 15).Replace("\"", string.Empty)).Replace("\\", "/");
                        DataStorePath.Add(path);
                    }
                }

                if (line.ToLowerInvariant().Contains("<configuration ") && line.ToLowerInvariant().Contains("name"))
                {
                    configurationNumber += 1;
                    predicateNumber = 0;
                    configuration = new Configuration
                    {
                        StartLineIndex = intLineNumTrackerIndex,
                        ModuleName = ExtractValueBetweenQuotes(line, "name=")
                    };


                    SectionNamePrefix = ExtractValueBetweenQuotes(line, "name=").Replace(".", "-").Replace("\"", string.Empty);
                    ModuleNametoReplace = SectionNamePrefix.Replace("-", ".");
                }

                if (line.ToLowerInvariant().Contains("newitemonlyevaluator")) CreateOnlyDirectiveConfigNumber.Add(configurationNumber);

                if (line.ToLowerInvariant().Contains("</sitecore>")) break; //safety net for next line

                if (line.ToLowerInvariant().Contains("</configuration>"))
                {
                    configuration.EndLineIndex = intLineNumTrackerIndex;
                    configuration.ConfigurationNumber = configurationNumber;
                    ConfigurationList.Add(configuration);
                    CreateOnlyDirective = false;

                }

                if (line.ToLowerInvariant().Contains("<predicate") || line.ToLowerInvariant().Contains("<rolepredicate") || line.ToLowerInvariant().Contains("<userpredicate"))
                {
                    predicateNumber += 1;
                    predicate = new Predicate
                    {
                        StartLineIndex = intLineNumTrackerIndex
                    };

                    if (line.ToLowerInvariant().Contains("<predicate"))
                    {
                        predicate.PredicateType = "include";
                    }
                    else if(line.ToLowerInvariant().Contains("<rolepredicate"))
                    {
                        predicate.PredicateType = "role";
                    }
                    else if (line.ToLowerInvariant().Contains("<userpredicate"))
                    {
                        predicate.PredicateType = "user";
                    }
                }

                if (line.ToLowerInvariant().Contains("</predicate>") || line.ToLowerInvariant().Contains("</rolepredicate") || line.ToLowerInvariant().Contains("</userpredicate"))
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

        private void CountRoleInclude(int predicateNumber)
        {
            int includeCount = 0;

            for (int intCounter = PredicateList[predicateNumber - 1].StartLineIndex; intCounter <= PredicateList[predicateNumber - 1].EndLineIndex; intCounter++)
            {
                string currline = lstConfig[intCounter];

                if (currline.ToLowerInvariant().Contains("<include")) includeCount += 1;

            }

            intRoleIncludeCount = includeCount;
        }

        private string GetInfoforRoleInclude(string endTag)
        {
            string convertedLine = string.Empty;            

            do
            {
                string currline = lstConfig[intLineNumTracker];
                if (currline.ToLowerInvariant().Contains("include") && currline.ToLowerInvariant().Contains("domain="))
                {
                    includeDomainName = ExtractValueBetweenQuotes(currline, "domain=");
                    includePattern = ExtractValueBetweenQuotes(currline, "pattern=");

                    convertedLine += "\r\n\t\t\t\t{";

                    convertedLine += "\r\n\t\t\t\t\t\t \"domain\" : " + includeDomainName + ",";
                    convertedLine += "\r\n\t\t\t\t\t\t \"pattern\" : " + includePattern;
                    convertedLine += "\r\n\t\t\t\t}";

                    intCurrentRoleInclude += 1;

                    if (intCurrentRoleInclude >= intRoleIncludeCount)
                    {
                        convertedLine += "";
                    }
                    else
                    {
                        convertedLine += ",";
                    }

                    if (CommentedLines.Contains(intLineNumTracker)) { convertedLine = "/*" + convertedLine + "*/"; }
                }

                intLineNumTracker += 1;

            } while (lstConfig[intLineNumTracker].ToLowerInvariant().Trim() != endTag);

            

            return convertedLine;
        }

        private string GetInfoforInclude()
        {
            string convertedLine = string.Empty;
            string currline = lstConfig[intLineNumTracker];

            if (currline.ToLowerInvariant().Contains("include") && currline.ToLowerInvariant().Contains("name=") &&
                currline.ToLowerInvariant().Contains("database=") && currline.ToLowerInvariant().Contains("path="))
            {
                if (DataStorePath.Any())
                {
                    includeModuleName = "\"" + DataStorePath[CurrentConfigNumber - 1] + "/" + ExtractValueBetweenQuotes(currline, "name=").Replace("\"", string.Empty) + "\"";
                }
                else
                {
                    includeModuleName = "\"" + ExtractValueBetweenQuotes(currline, "name=").Replace("\"", string.Empty) + "\"";
                }
                //includeModuleName = includeModuleName.Replace(includeModuleName.Replace("\"", string.Empty), ConfigurationList[CurrentConfigNumber - 1].ModuleName.Replace("\"", string.Empty) + "-" + includeModuleName.Replace("\"", string.Empty)).Replace(".", "-");
                includeModulePath = ExtractValueBetweenQuotes(currline, "path=");
                includeModuleDB = ExtractValueBetweenQuotes(currline, "database=");

                convertedLine += "\r\n\t\t\t{";

                convertedLine += "\r\n\t\t\t\t \"name\" : " + includeModuleName + ",";
                convertedLine += "\r\n\t\t\t\t \"path\" : " + includeModulePath + ",";
                convertedLine += "\r\n\t\t\t\t \"database\" : " + includeModuleDB;

                var rules = string.Empty;

                if (currline.Trim().ToLowerInvariant().Substring(currline.Trim().Length - 2, 2) != "/>")
                {
                    rules = BuildRules(convertedLine);
                    if (string.IsNullOrWhiteSpace(rules))
                    {
                        convertedLine += string.Empty;
                        //excludesPresent = false;
                        //if (Right(convertedLine, 1) != ",") convertedLine += ",";
                        if (CreateOnlyDirectiveConfigNumber.Contains(CurrentConfigNumber)) convertedLine += "\r\n\t\t\t\t \"allowedPushOperations\" : \"CreateOnly\"";
                    }
                    else
                    {
                        convertedLine += rules;
                    }
                }
                else
                {
                    //just include
                    //if (Right(convertedLine, 1) != ",") convertedLine += ",";
                    if (CreateOnlyDirectiveConfigNumber.Contains(CurrentConfigNumber)) convertedLine += ",\r\n\t\t\t\t \"allowedPushOperations\" : \"CreateOnly\"";
                }

                if (!string.IsNullOrWhiteSpace(ruleList)) if (Right(convertedLine, 1) != ",") convertedLine += "," + ruleList;
                ruleList = string.Empty;

                convertedLine += "\r\n\t\t\t}";

                intCurrentInclude += 1;

                if (intCurrentInclude >= intIncludeCount)
                {
                    convertedLine += "";
                }
                else
                {
                    if (Right(convertedLine, 1) != ",") convertedLine += ",";
                }
            }            

            return convertedLine;
        }

        public static string Right(string original, int numberCharacters)
        {
            return original.Substring(original.Length - numberCharacters);
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

        private bool IsSingleItem()
        {
            string currline = string.Empty;
            int tmpLineTracker = intLineNumTracker;
            bool istheLast = true;

            if (!IsBlankLine(tmpLineTracker)) currline = lstConfig[tmpLineTracker];

            if (currline.ToLowerInvariant().Contains("except ") && currline.ToLowerInvariant().Contains("name"))
            {
                var extractChildtoCompare = ExtractValueBetweenQuotes(currline, "name=");
                int SlashOccurences= extractChildtoCompare.Split('/').Length - 1;//int count = source.Split('/').Length - 1;
                var splitChildtoCompare = extractChildtoCompare.Split('/');
                string parenttoCompare = string.Empty;

                if (splitChildtoCompare.Length > 0)
                {
                    parenttoCompare = splitChildtoCompare[0];
                }
                else
                {
                    parenttoCompare = extractChildtoCompare;
                }

                parenttoCompare = parenttoCompare.Replace("\"",string.Empty);
                //take a child without slash in name and loop through all except within exclude list to find if there is a slash and then the path starts with this name, that means the parent has a child and so the parent must be marked with itemanddescendants then move to next child and do a similar loop until finding a child with slash
                int tmpnewLineTracker = ExcludeLineNumberStart;
                do
                {
                    if (tmpnewLineTracker != tmpLineTracker)
                    {
                        if (!IsBlankLine(tmpnewLineTracker))
                        {
                            string newcurrline = lstConfig[tmpnewLineTracker];
                            var extracttmpChildtoCompare = ExtractValueBetweenQuotes(newcurrline, "name=");
                            var tmpsplitChildtoCompare = extracttmpChildtoCompare.Replace("\"", string.Empty).Split('/');
                            if (tmpsplitChildtoCompare.Length > 0)
                            {
                                if (tmpsplitChildtoCompare[0].Replace("\"", string.Empty) == parenttoCompare)
                                {
                                    if (SlashOccurences < extracttmpChildtoCompare.Replace("\"", string.Empty).Split('/').Length - 1)
                                    {
                                        istheLast = false;
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    tmpnewLineTracker++;
                } while (lstConfig[tmpnewLineTracker].Trim() != "</exclude>");
            }
            //}


            return istheLast;
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
                        if (currline.ToLowerInvariant().Contains("/>"))
                        {
                            var tmprules= ",\r\n\t\t\t\t \"rules\": [";
                            tmprules += "\r\n\t\t\t\t\t\t {";
                            tmprules += "\r\n\t\t\t\t\t\t\t \"scope\" : \"ignored\",";
                            tmprules += "\r\n\t\t\t\t\t\t\t \"path\" : \"*\"";
                            tmprules += "\r\n\t\t\t\t\t\t }";
                            tmprules += "\r\n\t\t\t\t ]";

                            return tmprules;
                        }

                        var nextline = lstConfig[intLineNumTracker + 1];
                        if (!CommentsPresent)
                        { if (!nextline.ToLowerInvariant().Contains("except")) return convertedlines; }
                        else
                        { if (!CommentedLines.Contains(intLineNumTracker + 1)) if (!nextline.ToLowerInvariant().Contains("except")) return convertedlines; }//uncommented since without this scope is not added when exclude has only one except with children=true - xxxx.Feature.Serialization.config.

                    }

                    var prevline = lstConfig[intLineNumTracker - 1];
                    if (currline.ToLowerInvariant().Contains("except"))
                    {
                        //these must be serialized too
                        if (string.IsNullOrWhiteSpace(ruleList))
                        {
                            ruleList += "\r\n\t\t\t\t \"rules\": [";
                        }

                        do
                        {
                            if (!IsBlankLine(intLineNumTracker)) currline = lstConfig[intLineNumTracker];

                            if (currline.ToLowerInvariant().Contains("except ") && currline.ToLowerInvariant().Contains("name"))
                            {                                
                                var extractChildtoInclude = ExtractValueBetweenQuotes(currline, "name=", true);

                                ruleList += "\r\n\t\t\t\t\t\t {";

                                string rulestring = string.Empty;

                                if (currline.ToLowerInvariant().Contains("except"))
                                { rulestring = "\r\n\t\t\t\t\t\t\t \"scope\" : \"ItemandDescendants\","; }

                                if (currline.ToLowerInvariant().Contains("except") && currline.ToLowerInvariant().Contains("includechildren=\"false\""))
                                {
                                    if (ExcludeLineNumberStart == 0) ExcludeLineNumberStart = intLineNumTracker;
                                    bool isSingleItem = IsSingleItem();
                                    //invoke loop logic to find it except is singleitem
                                    if (!isSingleItem)
                                    {
                                        rulestring = "\r\n\t\t\t\t\t\t\t \"scope\" : \"ItemandDescendants\",";
                                    }
                                    else
                                    {
                                        rulestring = "\r\n\t\t\t\t\t\t\t \"scope\" : \"SingleItem\",";
                                    }
                                }

                                ruleList += rulestring + "\r\n\t\t\t\t\t\t\t \"path\" : " + extractChildtoInclude;

                                if (CreateOnlyDirectiveConfigNumber.Contains(CurrentConfigNumber)) ruleList += ",\r\n\t\t\t\t\t\t\t \"allowedPushOperations\" : \"CreateOnly\"";
                                ruleList += "\r\n\t\t\t\t\t\t }";

                                // if (lstConfig[intLineNumTracker + 1].Trim() != "</exclude>" && !RulesListed)
                                // {
                                ruleList += ",";
                            }
                            //}

                            intLineNumTracker += 1;

                        } while (lstConfig[intLineNumTracker].Trim() != "</exclude>");
                        ExcludeLineNumberStart = 0;

                        if (lstConfig[intLineNumTracker].Trim() == "</exclude>")
                        {
                            ruleList += "\r\n\t\t\t\t\t\t {";
                            ruleList += "\r\n\t\t\t\t\t\t\t \"scope\" : \"ignored\",";
                            ruleList += "\r\n\t\t\t\t\t\t\t \"path\" : \"*\"";
                            ruleList += "\r\n\t\t\t\t\t\t }";

                            RulesListed = true;
                        }
                    }

                    intLineNumTracker++;
                }

            } while (lstConfig[intLineNumTracker].Trim() != "</include>");

            if (!string.IsNullOrWhiteSpace(ruleList) && lstConfig[intLineNumTracker].Trim() == "</include>")
            {
                ruleList += "\r\n\t\t\t\t ]";
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

            if (occurrence > 1) return "\r\n{" + "\r\n\t" + moduleLine + "\r\n\t" + refLine + "\r\n\t" + "\"items\": {";

            return "{" + "\r\n\t" + moduleLine + "\r\n\t" + refLine + "\r\n\t" + "\"items\": {";

            //if (currline.ToLowerInvariant().Contains("</include>")) intIncludeCount += 1;
        }

        //one of the first activities
        private void LoadListsforBaseConfig()
        {
            //find helix.base.config in file system
            //traverse to fill diff layer lists
            var baseconfigfile = Directory
               .GetFiles(txtSelectedPath.Text, "unicorn.helix.config", SearchOption.AllDirectories);

            var tmpconfigFileData = File.ReadAllText(baseconfigfile.FirstOrDefault());

            GetLineNumbers(tmpconfigFileData);

            string[] lsttmpConfig = tmpconfigFileData.Split(new Char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var config in ConfigurationList)
            {
                if (config.ModuleName.ToLowerInvariant() == "\"helix.foundation\"" || config.ModuleName.ToLowerInvariant() == "\"helix.feature\"" || config.ModuleName.ToLowerInvariant() == "\"helix.project\"")
                {
                    for (int i = config.StartLineIndex; i <= config.EndLineIndex; i++)
                    {
                        
                        if (config.ModuleName.ToLowerInvariant() == "\"helix.foundation\"")
                        {
                            //start do loop and add the include name values to foundation list
                            string tmpcurrline = lsttmpConfig[i];

                            if (tmpcurrline.Contains("<include name="))
                            {
                                var intFirst = IndexofnthOccurence(tmpcurrline, '"', 1);
                                var intSecond = IndexofnthOccurence(tmpcurrline, '"', 2);

                                var intStringLen = intSecond - intFirst;
                                string modulename = tmpcurrline.Substring(intFirst, intStringLen + 1);
                                lstFoundationBase.Add(modulename.Replace("\"", string.Empty));
                            }

                        }
                        else if (config.ModuleName.ToLowerInvariant() == "\"helix.feature\"")
                        {
                            //start do loop and add the include name values to foundation list
                            string tmpcurrline = lsttmpConfig[i];

                            if (tmpcurrline.Contains("<include name="))
                            {
                                var intFirst = IndexofnthOccurence(tmpcurrline, '"', 1);
                                var intSecond = IndexofnthOccurence(tmpcurrline, '"', 2);

                                var intStringLen = intSecond - intFirst;
                                string modulename = tmpcurrline.Substring(intFirst, intStringLen + 1);
                                lstFeatureBase.Add(modulename.Replace("\"", string.Empty));
                            }

                        }
                        else if (config.ModuleName.ToLowerInvariant() == "\"helix.project\"")
                        {
                            //start do loop and add the include name values to foundation list
                            string tmpcurrline = lsttmpConfig[i];

                            if (tmpcurrline.Contains("<include name="))
                            {
                                var intFirst = IndexofnthOccurence(tmpcurrline, '"', 1);
                                var intSecond = IndexofnthOccurence(tmpcurrline, '"', 2);

                                var intStringLen = intSecond - intFirst;
                                string modulename = tmpcurrline.Substring(intFirst, intStringLen + 1);
                                lstProjectBase.Add(modulename.Replace("\"", string.Empty));
                            }

                        }
                    }
                }
            }
        }

        private string ConverttoCLIModuleJson(string filePath = "")
        {
            string configFileData;
            if (Mode == "P") configFileData = txtConfig.Text;
            else configFileData = File.ReadAllText(filePath);


            if (!filePath.ToLowerInvariant().Contains("\\foundation\\serialization"))
                SaveBaseItemFile(filePath);//one of the first activities since irrespective of the existence of a config file, base module json files must be saved 

            ruleList = string.Empty;

            if (configFileData.Contains("<!--")) CommentsPresent = true;

            if (!(configFileData.ToLowerInvariant().Contains("<predicate") && configFileData.ToLowerInvariant().Contains("<configuration ")))
                return string.Empty;//not serialization config

            GetLineNumbers(configFileData);            

            //GetPredicateLineNumbers(configFileData);
            string strConfigText = configFileData;//txtConfig.Text;
            lstConfig = strConfigText.Split(new Char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            if (CommentsPresent) ExtractCommentedLineNumbers();

            string convertedLine = string.Empty;
            CurrentConfigNumber = 0;
            string currline = string.Empty;

            for (intLineNumTracker = configurations.StartLineIndex; intLineNumTracker <= configurations.EndLineIndex; intLineNumTracker++)
            {
                //if (!CommentedLines.Contains(intLineNumTracker))
                //{
                RunningConfigNumber = 0;
                foreach (var config in ConfigurationList)
                {
                        RunningConfigNumber += 1;
                        if (intLineNumTracker == config.StartLineIndex)
                        {
                            var line = lstConfig[intLineNumTracker];

                            convertedLine = GetConfigurationLine(line, config.ConfigurationNumber);
                            CurrentConfigNumber = config.ConfigurationNumber;//same as predicate number
                            convertedLine = GetConfigurationLine(line, config.ConfigurationNumber);
                            CurrentConfigNumber = config.ConfigurationNumber;//same as predicate number
                        }

                }

                var commentedline = lstConfig[intLineNumTracker];
                if (commentedline.Trim().StartsWith("<!--")) convertedLine += "/*";
                if (commentedline.Trim().EndsWith("-->")) convertedLine += "*/";

                foreach (var predicate in PredicateList)
                {
                   
                    if (intLineNumTracker >= predicate.StartLineIndex && intLineNumTracker <= predicate.EndLineIndex)
                    {
                        //intCurrentInclude = 0;

                        
                        if (intLineNumTracker == predicate.StartLineIndex)
                        {
                            if (predicate.PredicateType == "include")
                            {
                                intCurrentInclude = 0;
                                CountInclude(predicate.PredicateNumber);
                            
                                convertedLine += "\r\n\t\t" + "\"includes\": [";
                            }   
                            else
                            {
                                intCurrentRoleInclude = 0;
                                CountRoleInclude(predicate.PredicateNumber);
                            }
                        }

                        if (predicate.PredicateType == "include")
                        {
                            convertedLine += GetInfoforInclude();
                        }
                        else if(predicate.PredicateType == "role")
                        {
                            roleList = "\r\n\t" + "\r\n\t\"roles\": \r\n\t\t [" + GetInfoforRoleInclude("</rolepredicate>") + "\r\n\t\t]";
                        }
                        else if (predicate.PredicateType == "user")
                        {
                            userList = "\r\n\t" + "\r\n\t\"users\": \r\n\t\t [" + GetInfoforRoleInclude("</userpredicate>") + "\r\n\t\t]";
                        }

                        if (predicate.PredicateType=="include") if (intLineNumTracker == predicate.EndLineIndex) convertedLine += "\r\n\t\t" + "]";
                    }

                }

                //closing brackets
                foreach (var config in ConfigurationList)
                {
                    if (intLineNumTracker == config.EndLineIndex)
                    {

                        var line = lstConfig[intLineNumTracker];

                        if (intLineNumTracker == config.EndLineIndex)
                        {
                            convertedLine += itemsEndLine;
                            if (!string.IsNullOrWhiteSpace(roleList)) convertedLine += "," + roleList;

                            if (!string.IsNullOrWhiteSpace(userList)) convertedLine += "," + userList;

                            //if (!string.IsNullOrWhiteSpace(roleList)) 
                            //    convertedLine += endLine;
                            //else
                            convertedLine += endLine;
                        }   

                        if (Mode == "B")
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

            PredicateList = new List<Predicate>();//reset
            roleList = string.Empty;
            userList = string.Empty;
            return convertedLine;
        }

        private string  GetBaseJsonFile(string layerName,string moduleName,string baseString)
        {
            var concatenatedString = string.Empty;

            concatenatedString += "{";
            concatenatedString+=  "\r\n\t\"namespace\": \"" + layerName + "." + moduleName + "." + baseString + "\",";

            concatenatedString += "\r\n\t\t\"items\": {";
            concatenatedString += "\r\n\t\t\t\"includes\": [";

            concatenatedString += "\r\n\t\t\t\t{";
            concatenatedString += "\r\n\t\t\t\t\t\"name\" : \"" + baseString + "\",";

            if (baseString.ToLowerInvariant() == "templates")
            {
                concatenatedString += "\r\n\t\t\t\t\t\"path\" : \"/sitecore/" + baseString + "/" + layerName + "/" + moduleName + "\",";
            }
            else if (baseString.ToLowerInvariant() == "renderings")
            {
                concatenatedString += "\r\n\t\t\t\t\t\"path\" : \"/sitecore/layout/" + baseString + "/" + layerName + "/" + moduleName + "\",";
            }
            else if (baseString.ToLowerInvariant() == "media")
            {
                concatenatedString += "\r\n\t\t\t\t\t\"path\" : \"/sitecore/media library/" + layerName + "/" + moduleName + "\",";
            }

            concatenatedString += "\r\n\t\t\t\t\t\"database\" : \"master\"";

            concatenatedString += "\r\n\t\t\t\t}";
		    concatenatedString += "\r\n\t\t\t]";
            concatenatedString += "\r\n\t\t}";
            concatenatedString += "\r\n}";

            return concatenatedString;
        }

        private void SaveBaseItemFile(string fileFullPath)
        {
            //in case of habitat, base files are specified in unicorn.helix.config since some projects might follow this pattern, better to check for unicorn.helix.config and do the needful
            var parentdir = Path.GetDirectoryName(fileFullPath);

            if (chkMoveJsontoserfolderloc.Checked)
            {
                int codeloc = parentdir.IndexOf("code");//assumption that there is a code dir
                if (codeloc > 0)
                {
                    if (parentdir.ToLowerInvariant().Contains("\\foundation\\"))
                    {
                        parentdir = parentdir.Substring(0, codeloc - 1);
                        if (Directory.Exists(parentdir + "\\serialization"))
                        {
                            //create the needed base module json files based on entries for each layer in unicorn.helix.config and save here in parentdir
                            //foundation
                            foreach (var item in lstFoundationBase)
                            {
                                int lastind = parentdir.LastIndexOf('\\');
                                string modName = Right(parentdir, parentdir.Length - lastind-1);
                                //Generate the file and save
                                var genString = GetBaseJsonFile("Foundation", modName, item);

                                var baseFilePath = parentdir + "\\Foundation." + modName + "." + item + ".Serialization.module.json";

                                File.WriteAllText(baseFilePath, genString);
                            }
                            
                        }
                    }
                    else if (parentdir.ToLowerInvariant().Contains("\\feature\\"))
                    {
                        //same as foundation base
                        parentdir = parentdir.Substring(0, codeloc - 1);
                        if (Directory.Exists(parentdir + "\\serialization"))
                        {
                            //create the needed base module json files based on entries for each layer in unicorn.helix.config and save here in parentdir
                            //foundation
                            foreach (var item in lstFeatureBase)
                            {
                                int lastind = parentdir.LastIndexOf('\\');
                                string modName = Right(parentdir, parentdir.Length - lastind - 1);
                                //Generate the file and save
                                var genString = GetBaseJsonFile("Feature", modName, item);

                                var baseFilePath = parentdir + "\\Feature." + modName + "." + item + ".Serialization.module.json";

                                File.WriteAllText(baseFilePath, genString);
                            }

                        }
                    }
                    else if (parentdir.ToLowerInvariant().Contains("\\project\\"))
                    {
                        //same as foundation base
                        parentdir = parentdir.Substring(0, codeloc - 1);
                        if (Directory.Exists(parentdir + "\\serialization"))
                        {
                            //create the needed base module json files based on entries for each layer in unicorn.helix.config and save here in parentdir
                            //foundation
                            foreach (var item in lstProjectBase)
                            {
                                int lastind = parentdir.LastIndexOf('\\');
                                string modName = Right(parentdir, parentdir.Length - lastind-1);
                                //Generate the file and save
                                var genString = GetBaseJsonFile("Project", modName, item);

                                var baseFilePath = parentdir + "\\Project." + modName + "." + item + ".Serialization.module.json";

                                File.WriteAllText(baseFilePath, genString);
                            }

                        }
                    }
                }
            }
        }

        private string SaveFile(string concatenatedLines, Configuration config, string fileFullPath)
        {
            if (!string.IsNullOrWhiteSpace(concatenatedLines))
            {
                //ok to save
                string jsonFileFullPath = string.Empty;

                if (ConfigurationList.Count > 1)
                {
                    var parentdir = Path.GetDirectoryName(fileFullPath);
                    if (chkMoveJsontoserfolderloc.Checked)
                    {
                        int codeloc = parentdir.IndexOf("code");//assumption that there is a code dir
                        if (codeloc > 0) parentdir = parentdir.Substring(0, codeloc - 1);
                    }
                    jsonFileFullPath = parentdir + "\\" + config.ModuleName.Replace("\"", string.Empty) + ".module.json";
                }
                else
                {
                    var fileName = Path.GetFileNameWithoutExtension(fileFullPath);
                    var parentdir = Path.GetDirectoryName(fileFullPath);
                    if (chkMoveJsontoserfolderloc.Checked)
                    {
                        int codeloc = parentdir.IndexOf("code");//assumption that there is a code dir
                        if (codeloc>0) parentdir = parentdir.Substring(0,codeloc-1);
                    }
                    jsonFileFullPath = parentdir + "\\" + fileName + ".module.json";
                }

                File.WriteAllText(jsonFileFullPath, concatenatedLines);
                FilePaths += jsonFileFullPath + "\r\n";
            }

            return FilePaths;
        }
        private void btnPreview_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtConfig.Text)) return;
            CommentedLines = new List<int>();
            CreateOnlyDirectiveConfigNumber = new List<int>();

            Mode = "P";
            txtJson.Text = ConverttoCLIModuleJson();
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
            CreateOnlyDirectiveConfigNumber = new List<int>();
            if (string.IsNullOrWhiteSpace(txtSelectedPath.Text)) return;
            CommentedLines = new List<int>();
            Mode = "B";
            LoadListsforBaseConfig();
            var ext = new List<string> { "config" };
            var configFiles = Directory
                .EnumerateFiles(txtSelectedPath.Text, "*.config", SearchOption.AllDirectories)
                .Where(s => ext.Contains(Path.GetExtension(s).TrimStart('.').ToLowerInvariant()));

            string filePaths = string.Empty;
            int fileCount = 0;

            foreach (var file in configFiles)
            {
                if (!file.ToLowerInvariant().StartsWith("unicorn."))    
                {
                    if (file.ToLowerInvariant().Contains(".serialization."))
                    {
                        //ExtractCommentedLines(file);
                        configurationNumber = 0;
                        //place to save corresponding base json file based on file path
                        var convertedJsonString = ConverttoCLIModuleJson(file);
                        if (!string.IsNullOrWhiteSpace(convertedJsonString))
                        {
                            filePaths += file + "\r\n";
                            fileCount++;
                        }
                    }
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
            internal string PredicateType { get; set; }
        }

        internal class FileDetails
        {
            internal string FileName { get; set; }
            internal string FileData { get; set; }
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

        private void txtJson_TextChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            var ext = new List<string> { "module.json" };
            var configFiles = Directory
                .EnumerateFiles(txtSelectedPath.Text, "*.module.json", SearchOption.AllDirectories)
                .Where(s => ext.Contains(Path.GetExtension(s).TrimStart('.').ToLowerInvariant()));
        }
    }
}
