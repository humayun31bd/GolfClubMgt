using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Transactions;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.Web;
using System.Web.Caching;
using System.Web.Configuration;
using System.Web.Security;
using System.Globalization;

namespace MyCompany.Data
{
	public partial class DataControllerBase
    {
        
        private string _viewFilter;
        
        private BusinessObjectParameters _parameters;
        
        private bool _hasWhere;
        
        private DbCommand _currentCommand;
        
        private SelectClauseDictionary _currentExpressions;
        
        public static Regex FilterExpressionRegex = new Regex("(?\'Alias\'[\\w\\,\\.]+):(?\'Values\'[\\s\\S]*)");
        
        public static Regex MatchingModeRegex = new Regex("^(?\'Match\'_match_|_donotmatch_)\\:(?\'Scope\'\\$all\\$|\\$any\\$)$");
        
        public static Regex FilterValueRegex = new Regex("(?\'Operation\'\\*|\\$\\w+\\$|=|~|<(=|>){0,1}|>={0,1})(?\'Value\'[\\s\\S]*?)(\\0|$)");
        
        public static string[] NegativeFilterOperations = new string[] {
                "$notin$",
                "$doesnotequal$",
                "<>",
                "$doesnotbegingwith$>",
                "$doesnotcontain$>",
                "$doesnotendwith$>"};
        
        public static string[] ReverseNegativeFilterOperations = new string[] {
                "$in$",
                "$equals$",
                "=",
                "$beginswith$>",
                "$contains$>",
                "$endswith$>"};
        
        private void AppendWhereExpressions(StringBuilder sb, DbCommand command, ViewPage page, SelectClauseDictionary expressions, FieldValue[] values)
        {
            sb.AppendLine();
            sb.Append("where");
            bool firstField = true;
            foreach (FieldValue v in values)
            {
                DataField field = page.FindField(v.Name);
                if ((field != null) && field.IsPrimaryKey)
                {
                    sb.AppendLine();
                    if (firstField)
                    	firstField = false;
                    else
                    	sb.Append("and ");
                    sb.AppendFormat(RemoveTableAliasFromExpression(expressions[v.Name]));
                    sb.AppendFormat("={0}p{1}", _parameterMarker, command.Parameters.Count);
                    DbParameter parameter = command.CreateParameter();
                    parameter.ParameterName = String.Format("{0}p{1}", _parameterMarker, command.Parameters.Count);
                    AssignParameterValue(parameter, field.Type, v.OldValue);
                    command.Parameters.Add(parameter);
                }
            }
            bool ignorePrimaryKeyInWhere = false;
            if (firstField)
            {
                foreach (FieldValue fv in values)
                	if (fv.Name == "_IgnorePrimaryKeyInWhere")
                    {
                        ignorePrimaryKeyInWhere = true;
                        break;
                    }
                // if the first field has not been processed then a primary key has not been provided
                if (!(ignorePrimaryKeyInWhere))
                	throw new Exception("A primary key field value is not provided.");
            }
            if (ignorePrimaryKeyInWhere || _config.ConflictDetectionEnabled)
            	foreach (FieldValue v in values)
                {
                    DataField field = page.FindField(v.Name);
                    if ((field != null) && (!((field.IsPrimaryKey || field.OnDemand)) && !(v.ReadOnly)))
                    {
                        sb.AppendLine();
                        if (firstField)
                        	firstField = false;
                        else
                        	sb.Append("and ");
                        sb.Append(RemoveTableAliasFromExpression(expressions[v.Name]));
                        if (v.OldValue == null)
                        	sb.Append(" is null");
                        else
                        {
                            sb.AppendFormat("={0}p{1}", _parameterMarker, command.Parameters.Count);
                            DbParameter parameter = command.CreateParameter();
                            parameter.ParameterName = String.Format("{0}p{1}", _parameterMarker, command.Parameters.Count);
                            AssignParameterValue(parameter, field.Type, v.OldValue);
                            command.Parameters.Add(parameter);
                        }
                    }
                }
            sb.AppendLine();
        }
        
        private void EnsureWhereKeyword(StringBuilder sb)
        {
            if (!(_hasWhere))
            {
                _hasWhere = true;
                sb.AppendLine("where");
            }
        }
        
        private string ProcessViewFilter(ViewPage page, DbCommand command, SelectClauseDictionary expressions)
        {
            _currentCommand = command;
            _currentExpressions = expressions;
            string filter = Regex.Replace(_viewFilter, "/\\*Sql\\*/(?\'Sql\'[\\s\\S]+)/\\*Sql\\*/|(?\'Parameter\'(@|:)\\w+)|(?\'Other\'(\"|\'|\\[|`)\\s*\\w" +
                    "+)|(?\'Function\'\\$\\w+\\s*\\((?\'Arguments\'[\\S\\s]*?)\\))|(?\'Name\'\\w+)", DoReplaceKnownNames);
            return filter;
        }
        
        private string DoReplaceKnownNames(Match m)
        {
            string sql = m.Groups["Sql"].Value;
            if (!(String.IsNullOrEmpty(sql)))
            	return sql;
            if (!(String.IsNullOrEmpty(m.Groups["Other"].Value)))
            	return m.Value;
            if (!(String.IsNullOrEmpty(m.Groups["Parameter"].Value)))
            	return AssignFilterParameterValue(m.Groups["Parameter"].Value);
            else
            	if (!(String.IsNullOrEmpty(m.Groups["Function"].Value)))
                	return FilterFunctions.Replace(_currentCommand, _currentExpressions, m.Groups["Function"].Value);
                else
                {
                    string s = null;
                    if (_currentExpressions.TryGetValue(m.Groups["Name"].Value, out s))
                    	return s;
                }
            return m.Value;
        }
        
        private string AssignFilterParameterValue(string qualifiedName)
        {
            char prefix = qualifiedName[0];
            string name = qualifiedName.Substring(1);
            if ((prefix.Equals('@') || prefix.Equals(':')) && !(_currentCommand.Parameters.Contains(qualifiedName)))
            {
                object result = null;
                if ((_parameters != null) && _parameters.ContainsKey(qualifiedName))
                	result = _parameters[qualifiedName];
                else
                {
                    BusinessRules rules = _serverRules;
                    if (rules == null)
                    	rules = CreateBusinessRules();
                    result = rules.GetProperty(name);
                }
                IEnumerable<object> enumerable = null;
                if (typeof(IEnumerable<object>).IsInstanceOfType(result))
                	enumerable = ((IEnumerable<object>)(result));
                if (enumerable != null)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("(");
                    int parameterIndex = 0;
                    foreach (object o in enumerable)
                    {
                        DbParameter p = _currentCommand.CreateParameter();
                        _currentCommand.Parameters.Add(p);
                        p.ParameterName = (qualifiedName + parameterIndex.ToString());
                        p.Value = o;
                        if (parameterIndex > 0)
                        	sb.Append(",");
                        sb.Append(p.ParameterName);
                        parameterIndex++;
                    }
                    sb.Append(")");
                    return sb.ToString();
                }
                else
                {
                    DbParameter p = _currentCommand.CreateParameter();
                    _currentCommand.Parameters.Add(p);
                    p.ParameterName = qualifiedName;
                    if (result == null)
                    	result = DBNull.Value;
                    p.Value = result;
                }
            }
            return qualifiedName;
        }
        
        protected virtual void AppendFilterExpressionsToWhere(StringBuilder sb, ViewPage page, DbCommand command, SelectClauseDictionary expressions, string whereClause)
        {
            string quickFindHint = page.QuickFindHint;
            bool firstCriteria = String.IsNullOrEmpty(_viewFilter);
            if (!(firstCriteria))
            {
                EnsureWhereKeyword(sb);
                sb.AppendLine("(");
                sb.Append(ProcessViewFilter(page, command, expressions));
            }
            int matchListCount = 0;
            bool firstDoNotMatch = true;
            string logicalConcat = "and ";
            bool useExclusiveQuickFind = false;
            foreach (DataField f in page.Fields)
            	if (!(String.IsNullOrEmpty(f.SearchOptions)) && Regex.IsMatch(f.SearchOptions, "\\$quickfind(?!disabled)"))
                {
                    useExclusiveQuickFind = true;
                    break;
                }
            if (page.Filter != null)
            	foreach (string filterExpression in page.Filter)
                {
                    Match matchingMode = MatchingModeRegex.Match(filterExpression);
                    if (matchingMode.Success)
                    {
                        bool doNotMatch = (matchingMode.Groups["Match"].Value == "_donotmatch_");
                        if (doNotMatch)
                        {
                            if (firstDoNotMatch)
                            {
                                firstDoNotMatch = false;
                                EnsureWhereKeyword(sb);
                                if (!(firstCriteria))
                                	sb.AppendLine(")");
                                if (matchListCount > 0)
                                	sb.AppendLine(")");
                                if (!(firstCriteria) || (matchListCount > 0))
                                	sb.AppendLine("and");
                                matchListCount = 0;
                                sb.AppendLine(" not");
                                firstCriteria = true;
                            }
                        }
                        if (matchListCount == 0)
                        {
                            EnsureWhereKeyword(sb);
                            if (!(firstCriteria))
                            	sb.Append(") and");
                            // the list of matches begins
                            sb.AppendLine("(");
                        }
                        else
                        {
                            sb.AppendLine(")");
                            sb.AppendLine("or");
                        }
                        // begin a list of conditions for the next match
                        if (matchingMode.Groups["Scope"].Value == "$all$")
                        	logicalConcat = " and ";
                        else
                        	logicalConcat = " or ";
                        matchListCount++;
                        firstCriteria = true;
                    }
                    Match filterMatch = FilterExpressionRegex.Match(filterExpression);
                    if (filterMatch.Success)
                    {
                        // "ProductName:?g", "CategoryCategoryName:=Condiments\x00=Seafood"
                        bool firstValue = true;
                        string fieldOperator = " or ";
                        if (Regex.IsMatch(filterMatch.Groups["Values"].Value, ">|<"))
                        	fieldOperator = " and ";
                        Match valueMatch = FilterValueRegex.Match(filterMatch.Groups["Values"].Value);
                        while (valueMatch.Success)
                        {
                            string alias = filterMatch.Groups["Alias"].Value;
                            string operation = valueMatch.Groups["Operation"].Value;
                            string paramValue = valueMatch.Groups["Value"].Value;
                            if ((operation == "~") && (alias == "_quickfind_"))
                            	alias = page.Fields[0].Name;
                            bool deepSearching = alias.Contains(",");
                            DataField field = page.FindField(alias);
                            if (((((field != null) && field.AllowQBE) || (operation == "~")) && (((page.DistinctValueFieldName != field.Name || (matchListCount > 0)) || (operation == "~")) || (page.AllowDistinctFieldInFilter || page.CustomFilteredBy(field.Name)))) || deepSearching)
                            {
                                if (firstValue)
                                {
                                    if (firstCriteria)
                                    {
                                        EnsureWhereKeyword(sb);
                                        sb.AppendLine("(");
                                        firstCriteria = false;
                                    }
                                    else
                                    	sb.Append(logicalConcat);
                                    sb.Append("(");
                                    firstValue = false;
                                }
                                else
                                	sb.Append(fieldOperator);
                                if (deepSearching)
                                {
                                    string deepSearchFieldName = alias.Substring(0, alias.IndexOf(','));
                                    string hint = alias.Substring((deepSearchFieldName.Length + 1));
                                    string deepFilterExpression = (deepSearchFieldName + filterExpression.Substring(filterExpression.IndexOf(':')));
                                    AppendDeepFilter(hint, page, command, sb, deepFilterExpression);
                                }
                                else
                                	if (operation == "~")
                                    {
                                        paramValue = Convert.ToString(StringToValue(paramValue));
                                        List<string> words = new List<string>();
                                        List<List<string>> phrases = new List<List<string>>();
                                        phrases.Add(words);
                                        CultureInfo currentCulture = CultureInfo.CurrentCulture;
                                        string textDateNumber = ("\\p{L}\\d" + Regex.Escape((currentCulture.DateTimeFormat.DateSeparator 
                                                        + (currentCulture.DateTimeFormat.TimeSeparator + currentCulture.NumberFormat.NumberDecimalSeparator))));
                                        string[] removableNumericCharacters = new string[] {
                                                currentCulture.NumberFormat.NumberGroupSeparator,
                                                currentCulture.NumberFormat.CurrencyGroupSeparator,
                                                currentCulture.NumberFormat.CurrencySymbol};
                                        Match m = Regex.Match(paramValue, String.Format("\\s*(?\'Token\'((?\'Quote\'\")(?\'Value\'.+?)\")|((?\'Quote\'\\\')(?\'Value\'.+?)\\\')|(,|;|(^|\\s+" +
                                                    ")-)|(?\'Value\'[{0}]+))", textDateNumber));
                                        bool negativeSample = false;
                                        while (m.Success)
                                        {
                                            string token = m.Groups["Token"].Value.Trim();
                                            if ((token == ",") || (token == ";"))
                                            {
                                                words = new List<string>();
                                                phrases.Add(words);
                                                negativeSample = false;
                                            }
                                            else
                                            	if (token == "-")
                                                	negativeSample = true;
                                                else
                                                {
                                                    string exactFlag = "=";
                                                    if (String.IsNullOrEmpty(m.Groups["Quote"].Value))
                                                    	exactFlag = " ";
                                                    string negativeFlag = " ";
                                                    if (negativeSample)
                                                    {
                                                        negativeFlag = "-";
                                                        negativeSample = false;
                                                    }
                                                    words.Add(String.Format("{0}{1}{2}", negativeFlag, exactFlag, m.Groups["Value"].Value));
                                                }
                                            m = m.NextMatch();
                                        }
                                        bool firstPhrase = true;
                                        foreach (List<String> phrase in phrases)
                                        	if (phrase.Count > 0)
                                            {
                                                if (firstPhrase)
                                                	firstPhrase = false;
                                                else
                                                	sb.AppendLine("or");
                                                sb.AppendLine("(");
                                                bool firstWord = true;
                                                System.DateTime paramValueAsDate;
                                                foreach (string paramValueWord in phrase)
                                                {
                                                    bool negativeFlag = (paramValueWord[0] == '-');
                                                    bool exactFlag = (paramValueWord[1] == '=');
                                                    string comparisonOperator = "like";
                                                    if (exactFlag)
                                                    	comparisonOperator = "=";
                                                    string pv = paramValueWord.Substring(2);
                                                    string fieldNameFilter = null;
                                                    Match complexParam = Regex.Match(pv, "^(.+)\\:(.+)$");
                                                    if (complexParam.Success)
                                                    {
                                                        fieldNameFilter = complexParam.Groups[1].Value;
                                                        bool fieldIsMatched = false;
                                                        foreach (DataField tf in page.Fields)
                                                        	if ((tf.AllowQBE && String.IsNullOrEmpty(tf.AliasName)) && (!((tf.IsPrimaryKey && tf.Hidden)) && tf.IsMatchedByName(fieldNameFilter)))
                                                            {
                                                                fieldIsMatched = true;
                                                                break;
                                                            }
                                                        if (fieldIsMatched)
                                                        	pv = complexParam.Groups[2].Value;
                                                        else
                                                        	fieldNameFilter = null;
                                                    }
                                                    bool paramValueIsDate = SqlStatement.TryParseDate(command.GetType(), pv, out paramValueAsDate);
                                                    bool firstTry = true;
                                                    DbParameter parameter = null;
                                                    if (!(paramValueIsDate))
                                                    	pv = SqlStatement.EscapePattern(command, pv);
                                                    double paramValueAsNumber;
                                                    string testNumber = pv;
                                                    foreach (string s in removableNumericCharacters)
                                                    	testNumber = testNumber.Replace(s, string.Empty);
                                                    bool paramValueIsNumber = double.TryParse(testNumber, out paramValueAsNumber);
                                                    if (!(exactFlag) && !(pv.Contains("%")))
                                                    	pv = String.Format("%{0}%", pv);
                                                    if (firstWord)
                                                    	firstWord = false;
                                                    else
                                                    	sb.Append("and");
                                                    if (negativeFlag)
                                                    	sb.Append(" not");
                                                    sb.Append("(");
                                                    bool hasTests = false;
                                                    DbParameter originalParameter = null;
                                                    if (String.IsNullOrEmpty(quickFindHint) || !(quickFindHint.StartsWith(";")))
                                                    	foreach (DataField tf in page.Fields)
                                                        	if ((tf.AllowQBE && String.IsNullOrEmpty(tf.AliasName)) && (!((tf.IsPrimaryKey && tf.Hidden)) && (!(tf.Type.StartsWith("Date")) || paramValueIsDate)))
                                                            {
                                                                if (String.IsNullOrEmpty(fieldNameFilter) || tf.IsMatchedByName(fieldNameFilter))
                                                                {
                                                                    if ((!(useExclusiveQuickFind) && (String.IsNullOrEmpty(tf.SearchOptions) || !(tf.SearchOptions.Contains("$quickfinddisabled")))) || (useExclusiveQuickFind && (!(String.IsNullOrEmpty(tf.SearchOptions)) && tf.SearchOptions.Contains("$quickfind"))))
                                                                    {
                                                                        hasTests = true;
                                                                        if ((parameter == null) || command.GetType().FullName.Contains("ManagedDataAccess"))
                                                                        {
                                                                            parameter = command.CreateParameter();
                                                                            parameter.ParameterName = String.Format("{0}p{1}", _parameterMarker, command.Parameters.Count);
                                                                            parameter.DbType = DbType.String;
                                                                            command.Parameters.Add(parameter);
                                                                            parameter.Value = pv;
                                                                            if (exactFlag && paramValueIsNumber)
                                                                            {
                                                                                parameter.DbType = DbType.Double;
                                                                                parameter.Value = paramValueAsNumber;
                                                                            }
                                                                        }
                                                                        if (!((exactFlag && ((!(tf.Type.Contains("String")) && !(paramValueIsNumber)) || (tf.Type.Contains("String") && paramValueIsNumber)))))
                                                                        {
                                                                            if (firstTry)
                                                                            	firstTry = false;
                                                                            else
                                                                            	sb.Append(" or ");
                                                                            if (tf.Type.StartsWith("Date"))
                                                                            {
                                                                                DbParameter dateParameter = command.CreateParameter();
                                                                                dateParameter.ParameterName = String.Format("{0}p{1}", _parameterMarker, command.Parameters.Count);
                                                                                dateParameter.DbType = DbType.DateTime;
                                                                                command.Parameters.Add(dateParameter);
                                                                                dateParameter.Value = paramValueAsDate;
                                                                                if (negativeFlag)
                                                                                	sb.AppendFormat("({0} is not null)and", expressions[tf.ExpressionName()]);
                                                                                sb.AppendFormat("({0} = {1})", expressions[tf.ExpressionName()], dateParameter.ParameterName);
                                                                            }
                                                                            else
                                                                            {
                                                                                bool skipLike = false;
                                                                                if (!((comparisonOperator == "=")) && ((tf.Type == "String") && ((tf.Len > 0) && (tf.Len < pv.Length))))
                                                                                {
                                                                                    string pv2 = pv;
                                                                                    pv2 = pv2.Substring(1);
                                                                                    if (tf.Len < pv2.Length)
                                                                                    	pv2 = pv2.Substring(0, (pv2.Length - 1));
                                                                                    if (pv2.Length > tf.Len)
                                                                                    	skipLike = true;
                                                                                    else
                                                                                    {
                                                                                        originalParameter = parameter;
                                                                                        parameter = command.CreateParameter();
                                                                                        parameter.ParameterName = String.Format("{0}p{1}", _parameterMarker, command.Parameters.Count);
                                                                                        parameter.DbType = DbType.String;
                                                                                        command.Parameters.Add(parameter);
                                                                                        parameter.Value = pv2;
                                                                                    }
                                                                                }
                                                                                if (_serverRules.EnableResultSet)
                                                                                {
                                                                                    string fieldNameExpression = expressions[tf.ExpressionName()];
                                                                                    if ((tf.Type != "String" && !(exactFlag)) || (tf.Type == "Boolean"))
                                                                                    	fieldNameExpression = String.Format("convert({0}, \'System.String\')", fieldNameExpression);
                                                                                    if (negativeFlag)
                                                                                    	sb.AppendFormat("({0} is not null)and", fieldNameExpression);
                                                                                    sb.AppendFormat("({0} {2} {1})", fieldNameExpression, parameter.ParameterName, comparisonOperator);
                                                                                }
                                                                                else
                                                                                	if (skipLike)
                                                                                    	sb.Append("1=0");
                                                                                    else
                                                                                    {
                                                                                        if (negativeFlag)
                                                                                        	sb.AppendFormat("({0} is not null)and", expressions[tf.ExpressionName()]);
                                                                                        if (DatabaseEngineIs(command, "Oracle", "DB2", "Firebird"))
                                                                                        {
                                                                                            sb.AppendFormat("(upper({0}) {2} {1})", expressions[tf.ExpressionName()], parameter.ParameterName, comparisonOperator);
                                                                                            parameter.Value = Convert.ToString(parameter.Value).ToUpper();
                                                                                        }
                                                                                        else
                                                                                        	sb.AppendFormat("({0} {2} {1})", expressions[tf.ExpressionName()], parameter.ParameterName, comparisonOperator);
                                                                                    }
                                                                            }
                                                                        }
                                                                        if (originalParameter != null)
                                                                        {
                                                                            parameter = originalParameter;
                                                                            originalParameter = null;
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                    if (!(String.IsNullOrEmpty(quickFindHint)) && quickFindHint.Contains(";"))
                                                    {
                                                        sb.AppendLine();
                                                        if (hasTests)
                                                        	sb.AppendLine("or");
                                                        else
                                                        	hasTests = true;
                                                        sb.AppendLine("(");
                                                        bool firstHint = true;
                                                        foreach (string hint in quickFindHint.Substring((quickFindHint.IndexOf(';') + 1)).Split(new char[] {
                                                                    ';'}))
                                                        {
                                                            if (firstHint)
                                                            	firstHint = false;
                                                            else
                                                            {
                                                                sb.AppendLine();
                                                                sb.AppendLine("or");
                                                            }
                                                            sb.AppendLine("(");
                                                            string newFilterExpression = filterExpression;
                                                            StringBuilder reversedFilterExpression = new StringBuilder();
                                                            if (negativeFlag)
                                                            {
                                                                bool firstExpressionPhrase = true;
                                                                foreach (List<string> ph in phrases)
                                                                {
                                                                    if (firstExpressionPhrase)
                                                                    	firstExpressionPhrase = false;
                                                                    else
                                                                    	reversedFilterExpression.Append(",");
                                                                    bool firstExpressionWord = true;
                                                                    foreach (string w in ph)
                                                                    {
                                                                        if (firstExpressionWord)
                                                                        	firstExpressionWord = false;
                                                                        else
                                                                        	reversedFilterExpression.Append(" ");
                                                                        if (!((w[0] == '-')))
                                                                        	reversedFilterExpression.Append("-");
                                                                        if (w[1] == '=')
                                                                        	reversedFilterExpression.Append("\"");
                                                                        reversedFilterExpression.Append(w.Substring(2));
                                                                        if (w[1] == '=')
                                                                        	reversedFilterExpression.Append("\"");
                                                                    }
                                                                }
                                                                newFilterExpression = ("_quickfind_:~" + ValueToString(reversedFilterExpression.ToString()));
                                                            }
                                                            AppendDeepFilter(hint, page, command, sb, newFilterExpression);
                                                            sb.AppendLine(")");
                                                        }
                                                        sb.AppendLine(")");
                                                    }
                                                    if (!(hasTests))
                                                    	if (negativeFlag && quickFindHint.StartsWith(";"))
                                                        	sb.Append("1=1");
                                                        else
                                                        	sb.Append("1=0");
                                                    sb.Append(")");
                                                }
                                                sb.AppendLine(")");
                                            }
                                        if (firstPhrase)
                                        	sb.Append("1=1");
                                    }
                                    else
                                    	if (operation.StartsWith("$"))
                                        	sb.Append(FilterFunctions.Replace(command, expressions, String.Format("{0}({1}$comma${2})", operation.TrimEnd('$'), alias, Convert.ToBase64String(Encoding.UTF8.GetBytes(paramValue)))));
                                        else
                                        {
                                            DbParameter parameter = command.CreateParameter();
                                            parameter.ParameterName = String.Format("{0}p{1}", _parameterMarker, command.Parameters.Count);
                                            AssignParameterDbType(parameter, field.Type);
                                            sb.Append(expressions[field.ExpressionName()]);
                                            bool requiresRangeAdjustment = ((operation == "=") && (field.Type.StartsWith("DateTime") && !(StringIsNull(paramValue))));
                                            if ((operation == "<>") && StringIsNull(paramValue))
                                            	sb.Append(" is not null ");
                                            else
                                            	if ((operation == "=") && StringIsNull(paramValue))
                                                	sb.Append(" is null ");
                                                else
                                                {
                                                    if (operation == "*")
                                                    {
                                                        sb.Append(" like ");
                                                        parameter.DbType = DbType.String;
                                                        if (!(paramValue.Contains("%")))
                                                        	paramValue = (SqlStatement.EscapePattern(command, paramValue) + "%");
                                                    }
                                                    else
                                                    	if (requiresRangeAdjustment)
                                                        	sb.Append(">=");
                                                        else
                                                        	sb.Append(operation);
                                                    try
                                                    {
                                                        parameter.Value = StringToValue(field, paramValue);
                                                        if ((parameter.DbType == DbType.Binary) && (parameter.Value is Guid))
                                                        	parameter.Value = ((Guid)(parameter.Value)).ToByteArray();
                                                    }
                                                    catch (Exception )
                                                    {
                                                        parameter.Value = DBNull.Value;
                                                    }
                                                    sb.Append(parameter.ParameterName);
                                                    command.Parameters.Add(parameter);
                                                    if (requiresRangeAdjustment)
                                                    {
                                                        DbParameter rangeParameter = command.CreateParameter();
                                                        AssignParameterDbType(rangeParameter, field.Type);
                                                        rangeParameter.ParameterName = String.Format("{0}p{1}", _parameterMarker, command.Parameters.Count);
                                                        sb.Append(String.Format(" and {0} < {1}", expressions[field.ExpressionName()], rangeParameter.ParameterName));
                                                        if (field.Type == "DateTimeOffset")
                                                        {
                                                            DateTime dt = Convert.ToDateTime(parameter.Value);
                                                            parameter.Value = new DateTimeOffset(dt).AddHours(-14);
                                                            rangeParameter.Value = new DateTimeOffset(dt).AddDays(1).AddHours(14);
                                                        }
                                                        else
                                                        	rangeParameter.Value = Convert.ToDateTime(parameter.Value).AddDays(1);
                                                        command.Parameters.Add(rangeParameter);
                                                    }
                                                }
                                        }
                            }
                            valueMatch = valueMatch.NextMatch();
                        }
                        if (!(firstValue))
                        	sb.AppendLine(")");
                    }
                }
            if (matchListCount > 0)
            {
                sb.AppendLine(")");
                // the end of the "match" list
                sb.AppendLine(")");
                firstCriteria = true;
            }
            if (!(firstCriteria))
            {
                sb.AppendLine(")");
                if (!(String.IsNullOrEmpty(whereClause)))
                	sb.Append("and ");
            }
            if (!(String.IsNullOrEmpty(whereClause)))
            {
                sb.AppendLine("(");
                sb.AppendLine(whereClause);
                sb.AppendLine(")");
            }
        }
        
        protected virtual void AppendDeepFilter(string hint, ViewPage page, DbCommand command, StringBuilder sb, string filter)
        {
            string[] hintInfo = hint.Split(new char[] {
                        '.'});
            int index = filter.IndexOf(":");
            string fieldData = filter.Substring((index + 1));
            for (int i = 0; (i < NegativeFilterOperations.Length); i++)
            {
                string negativeOperation = NegativeFilterOperations[i];
                if (fieldData.StartsWith(negativeOperation))
                {
                    sb.Append("not ");
                    filter = (filter.Substring(0, (index + 1)) 
                                + (ReverseNegativeFilterOperations[i] + filter.Substring((index 
                                    + (1 + negativeOperation.Length)))));
                    break;
                }
            }
            sb.Append("exists(");
            PageRequest r = new PageRequest();
            r.Controller = hintInfo[0];
            r.View = hintInfo[1];
            r.Filter = new string[] {
                    filter};
            DataControllerBase controller = ((DataControllerBase)(ControllerFactory.CreateDataController()));
            foreach (DataField field in page.Fields)
            	if (field.IsPrimaryKey)
                {
                    r.InnerJoinPrimaryKey = ("resultset__." + field.Name);
                    r.InnerJoinForeignKey = hintInfo[2];
                    break;
                }
            controller.ConfigureSelectExistingCommand(r, command, sb);
            sb.Append(")");
        }
        
        protected virtual void ConfigureSelectExistingCommand(PageRequest request, DbCommand command, StringBuilder sb)
        {
            string controller = request.Controller;
            string view = request.View;
            SelectView(controller, view);
            request.AssignContext(controller, this._viewId, _config);
            ViewPage page = new ViewPage(request);
            if (_config.PlugIn != null)
            	_config.PlugIn.PreProcessPageRequest(request, page);
            _config.AssignDynamicExpressions(page);
            InitBusinessRules(request, page);
            using (DataConnection connection = CreateConnection(this))
            {
                DbCommand selectCommand = CreateCommand(connection);
                if ((selectCommand == null) && _serverRules.EnableResultSet)
                {
                    // it is not possible to "deep" search in this controller
                    sb.AppendLine("select 1");
                    return;
                }
                ConfigureCommand(selectCommand, page, CommandConfigurationType.SelectExisting, null);
            }
            string commandText = _currentCommand.CommandText;
            int parameterIndex = (_currentCommand.Parameters.Count - 1);
            while (parameterIndex >= 0)
            {
                DbParameter p = _currentCommand.Parameters[parameterIndex];
                string newParameterName = (_parameterMarker 
                            + ("cp" + command.Parameters.Count.ToString()));
                commandText = commandText.Replace(p.ParameterName, newParameterName);
                p.ParameterName = newParameterName;
                _currentCommand.Parameters.RemoveAt(parameterIndex);
                command.Parameters.Add(p);
                parameterIndex = (parameterIndex - 1);
            }
            int resultSetIndex = commandText.IndexOf("resultset__");
            int resultSetLastIndex = commandText.LastIndexOf("resultset__");
            if (resultSetIndex < resultSetLastIndex)
            	commandText = (commandText.Substring(0, (resultSetIndex + 9)) 
                            + ("2" + commandText.Substring((resultSetIndex + 9))));
            sb.AppendLine(commandText);
        }
        
        protected virtual void AppendSystemFilter(DbCommand command, ViewPage page, SelectClauseDictionary expressions)
        {
            string[] systemFilter = page.SystemFilter;
            if (!(RequiresHierarchy(page)) || ((systemFilter == null) || (systemFilter.Length < 2)))
            	return;
            if (!(String.IsNullOrEmpty(_viewFilter)))
            	_viewFilter = String.Format("({0})and", _viewFilter);
            StringBuilder sb = new StringBuilder(_viewFilter);
            sb.Append("(");
            bool collapse = (systemFilter[0] == "collapse-nodes");
            DataField parentField = null;
            foreach (DataField field in page.Fields)
            	if (field.IsTagged("hierarchy-parent"))
                {
                    parentField = field;
                    break;
                }
            string parentFieldExpression = expressions[parentField.Name];
            sb.AppendFormat("{0} is null or ", parentFieldExpression);
            if (collapse)
            	sb.Append("not(");
            sb.AppendFormat("{0} in (", parentFieldExpression);
            bool first = true;
            for (int i = 1; (i < systemFilter.Length); i++)
            {
                object v = StringToValue(systemFilter[i]);
                DbParameter p = command.CreateParameter();
                p.ParameterName = String.Format("{0}p{1}", _parameterMarker, command.Parameters.Count);
                p.Value = v;
                command.Parameters.Add(p);
                if (first)
                	first = false;
                else
                	sb.Append(",");
                sb.Append(p.ParameterName);
            }
            if (collapse)
            	sb.Append(")");
            sb.Append("))");
            _viewFilter = sb.ToString();
        }
        
        private void AppendAccessControlRules(DbCommand command, ViewPage page, SelectClauseDictionary expressions)
        {
            object handler = _config.CreateActionHandler();
            if (!((handler is BusinessRules)))
            	return;
            BusinessRules rules = _serverRules;
            if ((rules == null) && (handler != null))
            	rules = ((BusinessRules)(handler));
            if (rules == null)
            	rules = CreateBusinessRules();
            string accessControlFilter = rules.EnumerateAccessControlRules(command, _config.ControllerName, _parameterMarker, page, expressions);
            if (String.IsNullOrEmpty(accessControlFilter))
            	return;
            if (!(String.IsNullOrEmpty(_viewFilter)))
            	_viewFilter = (_viewFilter + " and ");
            _viewFilter = String.Format("{0}/*Sql*/{1}/*Sql*/", _viewFilter, accessControlFilter);
        }
    }
}
