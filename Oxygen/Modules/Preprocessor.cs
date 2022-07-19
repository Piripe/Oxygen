using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Text.RegularExpressions;
using System.Drawing;

namespace Oxygen.Modules
{
    internal static class Preprocessor
    {
        /// <summary>
        /// Pre processor main function
        /// </summary>
        /// <param name="s"></param>
        /// <param name="JSEngine"></param>
        /// <returns></returns>
        internal static string PreProcess(string s, string file, Jint.Engine JSEngine)
        {
            string processedStr = ProcessIf(s, file, JSEngine);

            // Replace all ${x} with the evaluated result
            processedStr = Regex.Replace(processedStr, @"\${([^}]*)}",(Match x) =>
            {
                try
                {
                    object result = JSEngine.Evaluate(x.Groups[1].Value).ToObject();
                    if (result.GetType() == typeof(Color))
                    {
                        // Exception for the Color because Steam uses a weird color code and it's simpler for the skin developer
                        Color color = (Color)result;
                        return string.Format("{0} {1} {2} {3}", color.R, color.G, color.B, color.A);
                    }
                    else
                    {
                        return result.ToString()??"";
                    }
                }
                catch (Exception ex)
                {
                    ErrorManager.Error(ex.Message, file);
                    return "";
                }
            });

            return processedStr;
        }
        /// <summary>
        /// Process all <c>#IF</c> from a <see cref="string"/>
        /// </summary>
        /// <param name="s"></param>
        /// <param name="JSEngine"></param>
        /// <returns></returns>
        private static string ProcessIf(string s, string file, Jint.Engine JSEngine)
        {

            try
            {
                string result = "";

            // Get the first #IF/#ELSEIF/#ELSE/#ENDIF
            var ifSearch = Regex.Match(s, @"(?<!\\)#(IF\s*\(([^\)]*)\)|ELSEIF\s*\(([^\)]*)\)|ELSE|ENDIF)");

            if (ifSearch.Success)
            {
                result += s.Substring(0, ifSearch.Index);

                if (ifSearch.Groups[1].Value.StartsWith("IF"))
                {
                    // Evaluate the condition
                    string toDetect = s.Remove(0, ifSearch.Index + ifSearch.Length);
                    if (bool.Parse(JSEngine.Evaluate(ifSearch.Groups[2].Value).ToString()))
                    {
                        // If it's true, detect for the next else/end and process between the current condition position and the else/end position
                        Match? detectedElse = DetectIf("ELSEIFENDIF",toDetect);

                            if (detectedElse != null)
                            {
                                result += ProcessIf(toDetect.Substring(0, detectedElse.Index), file, JSEngine);

                                if (detectedElse.Groups[1].Value.StartsWith("ENDIF"))
                                {
                                    // If detectedElse is an end, process the string after the end
                                    result += ProcessIf(toDetect.Remove(0, detectedElse.Index + detectedElse.Length), file, JSEngine);
                                }
                                else
                                {
                                    // If detectedElse is an else, detect the next end and process after it
                                    Match? detectedEnd = DetectIf("ENDIF", toDetect);
                                    if (detectedEnd != null)
                                    result += ProcessIf(toDetect.Remove(0, detectedEnd.Index + detectedEnd.Length), file, JSEngine);
                                }
                            }

                    }
                    else
                    {
                        while (true)
                        {
                            // If it's false, detect the next else/elseif/end
                            Match? detectedElse = DetectIf("ELSEIFENDIF", toDetect);
                                if (detectedElse == null)
                                    break;
                            if (detectedElse.Groups[1].Value.StartsWith("ELSEIF"))
                            {
                                // If it's an elseif, evaluate its condition

                                toDetect = toDetect.Remove(0, detectedElse.Index + detectedElse.Length);
                                if (bool.Parse(JSEngine.Evaluate(ifSearch.Groups[2].Value).ToString()))
                                {
                                    // If it's true, detect for the next else/end and process between the current condition position and the else/end position
                                    detectedElse = DetectIf("ELSEIFENDIF", toDetect);
                                        if (detectedElse != null)
                                        {
                                            result += ProcessIf(toDetect.Substring(0, detectedElse.Index), file, JSEngine);

                                            if (detectedElse.Groups[1].Value.StartsWith("ENDIF"))
                                            {
                                                // If detectedElse is an end, process the string after the end
                                                result += ProcessIf(toDetect.Remove(0, detectedElse.Index + detectedElse.Length), file, JSEngine);
                                            }
                                            else
                                            {
                                                // If detectedElse is an else, detect the next end and process after it
                                                Match? detectedEnd = DetectIf("ENDIF", toDetect);
                                                if (detectedEnd!=null)
                                                result += ProcessIf(toDetect.Remove(0, detectedEnd.Index + detectedEnd.Length), file, JSEngine);
                                            }
                                        }
                                            // Exit the while
                                            break;
                                }
                            }
                            else if (detectedElse.Groups[1].Value.StartsWith("ELSE"))
                            {
                                // If it's an else, detect the next end and process between the else and the end
                                Match? detectedEnd = DetectIf("ENDIF", toDetect);
                                    if (detectedEnd != null)
                                    {
                                        result += ProcessIf(toDetect.Substring(detectedElse.Index + detectedElse.Length, detectedEnd.Index - detectedElse.Index - detectedElse.Length), file, JSEngine);

                                        result += ProcessIf(toDetect.Remove(0, detectedEnd.Index + detectedEnd.Length), file, JSEngine);
                                    }
                                // Exit the while
                                break;
                            }
                            else if (detectedElse.Groups[1].Value.StartsWith("ENDIF"))
                            {
                                // If it's an end, process after the end and exit the while
                                result += ProcessIf(toDetect.Remove(0, detectedElse.Index + detectedElse.Length), file, JSEngine);
                                break;
                            }
                        }
                    }
                }
                else if (ifSearch.Groups[1].Value.StartsWith("ELSEIF"))
                {
                    // Not supposed to be reached
                }
                else if (ifSearch.Groups[1].Value.StartsWith("ELSE"))
                {
                    // Not supposed to be reached
                }
                else if (ifSearch.Groups[1].Value.StartsWith("ENDIF"))
                {
                    // Not supposed to be reached
                }
                return result;
            }
            else
            {

                return s;
                }
            }
            catch (Exception ex)
            {
                ErrorManager.Error(ex.Message, file);
                return s;
            }
        }
        /// <summary>
        /// Detect the first ELSE/ELSEIF/ENDIF in a <see cref="string"/>
        /// </summary>
        /// <param name="findWhat"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        private static Match? DetectIf(string findWhat, string s)
        {
            // Detect all IF/ELSEIF/ELSE/ENDIF of the string
            MatchCollection ifSearches = Regex.Matches(s, @"(?<!\\)#(IF\s*\(([^\)]*)\)|ELSEIF\s*\(([^\)]*)\)|ELSE|ENDIF)");

            int depth = 0;

            foreach (Match ifSearch in ifSearches)
            {
                if (ifSearch.Groups[1].Value.StartsWith("IF"))
                {
                    depth++;
                }
                else if (ifSearch.Groups[1].Value.StartsWith("ELSEIF"))
                {
                    if (findWhat.Contains("ELSEIF") & depth == 0)
                    {
                        return ifSearch;
                    }
                }
                else if (ifSearch.Groups[1].Value.StartsWith("ELSE"))
                {

                    if (findWhat.Contains("ELSE") & depth == 0)
                    {
                        return ifSearch;
                    }
                }
                else if (ifSearch.Groups[1].Value.StartsWith("ENDIF"))
                {
                    if (findWhat.Contains("ENDIF") & depth == 0)
                    {
                        return ifSearch;
                    }
                    depth--;
                }
            }
            return null;
        }
    }
}
