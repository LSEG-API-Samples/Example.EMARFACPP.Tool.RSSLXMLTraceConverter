using System;
using System.Text.RegularExpressions;

namespace RsslTraceXmlFileUtil
{
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using RDMDictDecoder;
    using Newtonsoft.Json.Linq;
    using RSSLTraceDecoder.Utils;

    public class RsslTraceFileUtil
    {
        public static bool Convert(string sourceXmlFile, string destinationXmlFile, out string errorMessage,string rdmFieldDictFile="./Dict/RDMFieldDictionary",string rdmEnumTypeDefFile="./Dict/enumtype.def",bool verbose=false)
        {
            if(string.IsNullOrEmpty(sourceXmlFile) && string.IsNullOrEmpty(destinationXmlFile))
            {
                //Console.WriteLine($"{sourceXmlFile} {destinationXmlFile}");
                errorMessage = "Source and Destination file must not be empty path";
                return false;
            }

            if (!File.Exists(sourceXmlFile))
            {
                errorMessage = $"Source file {sourceXmlFile} does not exists";
                return false;
            }

            if (verbose) Console.WriteLine($"Loading RDMFieldDictionary from {rdmFieldDictFile}");
            errorMessage = string.Empty;
            if (!RdmUtils.LoadRdmDictionary(rdmFieldDictFile, out var fieldDictionary, out var errorMsg))
            {
                errorMessage = errorMsg;
                return false;
            }
            if(verbose) Console.WriteLine($"Loading RDMFieldDictionary completed.");

            if(verbose) Console.WriteLine($"\r\nLoading enumtype.def from {rdmEnumTypeDefFile}");
            if (!RdmUtils.LoadEnumTypeDef(rdmEnumTypeDefFile, out var enumTypeDef, out errorMsg))
            {
                errorMessage = errorMsg;
                return false;
            }
            if(verbose) Console.WriteLine($"Loading enumtype.def completed.");


            if(verbose) Console.WriteLine($"Start reading and processing XML data from {sourceXmlFile}");
            try
            {
                int numFragments = GetXmlFrgamentsCount(sourceXmlFile);

                var readerSettings =
                    new XmlReaderSettings() { ConformanceLevel = ConformanceLevel.Fragment, Async = true };
                var writerSetting =
                    new XmlWriterSettings() { ConformanceLevel = ConformanceLevel.Fragment, Async = true };
                using (var readerStream = File.OpenRead(sourceXmlFile))
                using (var writerStream = File.Open(destinationXmlFile, FileMode.OpenOrCreate))
                using (var xmlReader = XmlReader.Create(readerStream, readerSettings))
                using (var xmlWriter = XmlWriter.Create(writerStream, writerSetting))
                {

                    if (!DecodeAndCopy(xmlReader, xmlWriter, fieldDictionary, enumTypeDef, out errorMsg,numFragments,verbose))
                    {
                        errorMessage = errorMsg;
                        return false;
                    }
                }
            }
            catch (XmlException ex)
            {
                errorMessage=$"Error Detect!!: {ex.Message}";
                return false;
            }
            return true;
        }

        private static int GetXmlFrgamentsCount(string sourceXmlFile)
        {
            int numFragment = 0;
            try
            {
                var readerSettings =
                    new XmlReaderSettings() { ConformanceLevel = ConformanceLevel.Fragment, Async = true };
                
                using (var readerStream = File.OpenRead(sourceXmlFile))
                using (var xmlReader = XmlReader.Create(readerStream, readerSettings))
                {
                    while (xmlReader.Read())
                    {
                        switch (xmlReader.NodeType)
                        {
                            case XmlNodeType.Element:
                            {
                                numFragment++;
                                break;
                            }
                        }
                    }
                }
            }
            catch (XmlException ex)
            {
                Console.WriteLine(ex.Message);
                return numFragment;
            }
            return numFragment;
        }
        private static bool DecodeAndCopy(XmlReader reader, XmlWriter writer, RdmFieldDictionary fieldDictionary, RdmEnumTypeDef enumTypeDef,out string errorMsg,int numFragments,bool verbose)
        {

            errorMsg = string.Empty;
            bool bMrnDecoding = false;
            long totalSize = 0;
            long currentFragmentSize = 0;
            long guidSize = 0;
            var mrnStrings = new StringBuilder();
            var guidstr = string.Empty;
            JToken jsonToken=null;
            var currentElementName = string.Empty;
            mrnStrings.Clear();
            int currentFragmentNum = 0;
            int percent = 5;
            try
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                        {
                            currentFragmentNum++;
                            var percentRead = currentFragmentNum / (double) numFragments;
                            percentRead *= 100.0;
                            if ( percentRead>= percent )
                            {
                                Console.WriteLine($"Processing completed {percent}% {currentFragmentNum}/{numFragments} elements");
                                var r=new Random();
                                percent += r.Next(10,18) ;
                                if (percent > 100)
                                    percent = 100;
                            }
                                currentElementName = reader.Name;
                            writer.WriteStartElement(reader.Name);
                            if (reader.HasAttributes)
                            {
                                if (currentElementName != "fieldEntry")
                                {
                                    writer.WriteAttributes(reader, true);
                                }
                                else
                                {
                                    string fieldValue;
                                    reader.MoveToFirstAttribute();

                                    var fidId = 0;
                                    bMrnDecoding = false;
                                    do
                                    {

                                        // display attribute information
                                        var attributeName = reader.Name.Trim();
                                        var attributeVal = reader.Value.Trim();
                                        var fieldName = "Unknown";
                                        fieldValue = attributeVal;
                                        if (attributeName.ToLower() != "fieldid")
                                        {
                                            if (attributeName.ToLower() == "data")
                                            {
                                                if (FieldValueToString(
                                                    fidId,
                                                    attributeVal,
                                                    out var output,
                                                    out errorMsg,
                                                    fieldDictionary: fieldDictionary,
                                                    enumTypeDef: enumTypeDef))
                                                {
                                                    fieldValue = output;
                                                }

                                              
                                                writer.WriteAttributeString("decodedData",
                                                    fieldName == "Unknown" ? fieldValue : string.Empty);
                                                writer.WriteAttributeString(attributeName, attributeVal);

                                                    if (fidId == 32480) // Found total_size
                                                {
                                                    bMrnDecoding = false;
                                                    totalSize = 0;
                                                    currentFragmentSize = 0;
                                                    guidSize = 0;
                                                    mrnStrings = new StringBuilder();
                                                    guidstr = string.Empty;
                                                    if (fieldValue != "")
                                                    {

                                                        totalSize = long.Parse(fieldValue);

                                                    }
                                                }
                                                else if (fidId == 32641) //Found fragment
                                                {

                                                    mrnStrings.Append(fieldValue);
                                                    currentFragmentSize = RdmDataConverter.StringToByteArray(fieldValue)
                                                        .Length;
                                                    guidSize += currentFragmentSize;
                                                    if (totalSize > 0 && guidSize > 0 && (totalSize == guidSize))
                                                        bMrnDecoding = true;



                                                }
                                                else if (fidId == 4271 && fieldValue != string.Empty) // Found GUID
                                                {
                                                    guidstr = fieldValue;
                                                }


                                            }
                                        }
                                        else
                                        {
                                            fidId = int.Parse(attributeVal);
                                            if (fieldDictionary.GetDictEntryById(fidId, out var rdmEntry))
                                            {
                                                fieldName = rdmEntry.Acronym;
                                            }

                                            writer.WriteAttributeString(attributeName, attributeVal);
                                            writer.WriteAttributeString("fieldName", fieldName);
                                        }
                                    } while (reader.MoveToNextAttribute());
                                }
                            }


                            // Move reader pointer back to element
                            reader.MoveToElement();

                            if (reader.IsEmptyElement)
                            {
                                writer.WriteEndElement();
                            }


                            if (bMrnDecoding)
                            {
                                var pByteArray = RdmDataConverter.StringToByteArray(mrnStrings.ToString());

                                var jsonString = string.Empty;



                                using (var compressedStream = new MemoryStream(pByteArray))
                                using (var zipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
                                using (var resultStream = new MemoryStream())
                                {
                                    zipStream.CopyTo(resultStream);
                                    jsonString = Encoding.UTF8.GetString(resultStream.ToArray());
                                }

                                jsonToken = JToken.Parse(jsonString);
                                if(verbose) Console.WriteLine($"{jsonToken.ToString(Newtonsoft.Json.Formatting.Indented)}");

                                mrnStrings.Clear();
                                guidSize = 0;
                                totalSize = 0;
                                bMrnDecoding = false;


                            }
                              
                            }
                            break;

                        case XmlNodeType.Comment:
                            writer.WriteComment(reader.Value);
                            break;
                        case XmlNodeType.EndElement:
                            writer.WriteEndElement();
                            if (reader.Name == "updateMsg" && jsonToken!=null)
                            {
                                writer.WriteWhitespace("\r\n");
                                writer.WriteComment($"{jsonToken.ToString(Newtonsoft.Json.Formatting.Indented)}");
                                jsonToken = null;
                            }
                            break;
                        case XmlNodeType.Text:
                            writer.WriteString(reader.Value);
                            break;
                        case XmlNodeType.Whitespace:
                            writer.WriteWhitespace(reader.Value);
                            break;
                    }

                    writer.Flush();
                }
            }
            catch (Exception ex)
            {
                errorMsg = $"Error when decoding XML file\r\n{ex.Message}\r\n{ex.StackTrace}";
                return false;
            }

            return true;
        }

        // Implement Partial page decoder based on algorithm from
        // https://github.com/TR-API-Samples/Article.TRTH.NetSDK.PageRetrieval
        // Implement Partial page decoder based on algorithm from
        // https://github.com/TR-API-Samples/Article.TRTH.NetSDK.PageRetrieval
        private static string DecodePartialUpdate(int fidId, string buffer)
        {
            string pattern = @"(\x1B\x5B|\x9B|\x5B)([0-9]+)\x60([^\x1B^\x5B^\x9B]+)";

            Regex regEx = new Regex(pattern, RegexOptions.IgnoreCase);
            MatchCollection matches = regEx.Matches(buffer);
            var page = string.Empty;
            page = fidId <= 339 && fidId >= 315 ? string.Empty.PadRight(80) : string.Empty.PadRight(64);

            if (matches.Count <= 0) return buffer;

            for (var i = 0; i < matches.Count; i++)
            {
                var group = matches[i].Groups;
                var partialIndex = System.Convert.ToInt32(@group[2].ToString());
                var partialValue = @group[3].ToString();

                // replace updated value at the position 
                page = page.Remove(partialIndex, partialValue.Length);
                page = page.Insert(partialIndex, partialValue);
            }
            return page;
        }
        private static bool FieldValueToString(int fidId, string value, out string outputStr, out string errorMsg, RdmFieldDictionary fieldDictionary, RdmEnumTypeDef enumTypeDef)
        {
            outputStr = string.Empty;
            errorMsg = string.Empty;
            if (value.Trim() == string.Empty)
            {
                errorMsg = $"Fid {fidId} Data is null or empty";
                return false;
            }

            if (!fieldDictionary.GetDictEntryById(fidId, out var rdmEntry))
            {
                errorMsg = $"Unable to find fid {fidId} in data dictionary";
                return false;
            }

            var pBuffer = new string(value.ToCharArray()
                .Where(c => !char.IsWhiteSpace(c))
                .ToArray());
            try
            {
                switch (rdmEntry.RwfType)
                {
                    case RwfTypeEnum.Buffer:
                        outputStr = pBuffer;
                        break;
                    case RwfTypeEnum.Date:
                        var dt = RdmDataConverter.HexStringToDateTime(pBuffer);
                        outputStr =
                            $"{(dt.HasValue ? $"{dt.Value.Day}/{dt.Value.Month}/{dt.Value.Year}" : string.Empty)}";
                        break;
                    case RwfTypeEnum.Enum:
                        outputStr = enumTypeDef.GetEnumDisplayValue(fidId,
                            RdmDataConverter.HexStringToInt(pBuffer) ?? 0, out var display)
                            ? $"{display.EnumDisplay.Replace("\"", "")}({RdmDataConverter.HexStringToInt(pBuffer)})"
                            : $"Unknown Enum({int.Parse(pBuffer)})";
                        break;
                    case RwfTypeEnum.Int64:
                    case RwfTypeEnum.Uint64:
                        var intVal = RdmDataConverter.HexStringToInt(pBuffer) ?? 0;
                        outputStr = rdmEntry.Acronym.Contains("_MS") ? $"{RdmDataConverter.TimeMsToString(intVal)}" : $"{intVal}";
                        break;
                    case RwfTypeEnum.Real64:
                        outputStr = $"{RdmDataConverter.RealStringtoDouble(pBuffer).ToString()}";
                        break;
                    case RwfTypeEnum.RmtesString:
                        {
                        // Check if RMTES Header contains 0x1B 0x25 0x30 follow by UTF-8 string. Then remove the header 
                        if (pBuffer.StartsWith("1B2530"))
                        {
                            pBuffer = pBuffer.Remove(0, 6);
                            outputStr = Encoding.UTF8.GetString(RdmDataConverter.StringToByteArray(pBuffer.Trim()));
                        }
                        else if (pBuffer.StartsWith("1B5B"))
                        {
                            outputStr = DecodePartialUpdate(
                                fidId,
                                Encoding.UTF8.GetString(RdmDataConverter.StringToByteArray(pBuffer.Trim())));
                        }
                        else
                        {
                            outputStr = Encoding.UTF8.GetString(RdmDataConverter.StringToByteArray(pBuffer.Trim()));
                        }

                        var validXmlString = new StringBuilder();
                        validXmlString.Append(outputStr.Where(XmlConvert.IsXmlChar).ToArray());
                        outputStr = validXmlString.ToString();
                    }
                        break;
                    case RwfTypeEnum.AsciiString:

                        outputStr = Encoding.UTF8.GetString(
                            RdmDataConverter.StringToByteArray(pBuffer.Trim()));
                        break;
                    case RwfTypeEnum.Time:
                        outputStr = $"{RdmDataConverter.HexStringToTime(pBuffer.Trim())}";
                        break;
                    case RwfTypeEnum.Unhandled:
                        outputStr = $"{value}";
                        break;
                }
            }
            catch (Exception exception)
            {
                errorMsg = $"Fid {fidId} {exception.Message}";
                return false;
            }

            return true;
        }
    }

}
