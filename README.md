# RSSL XML Trace Data Converter Tool

The utility is console application which was designed for decoding and converting XML Fragments inside RSSL Tracing log generated by [RFA C++/.NET](https://developers.thomsonreuters.com/thomson-reuters-enterprise-platform/apis-in-this-family) and [EMA/ETA C/C++](https://developers.thomsonreuters.com/elektron/elektron-sdk-cc). It can convert a Hex string inside the field entry to the actual value according to data dictionary (RDMFieldDictionary and enumtype.def). Please note that RSSL Tracing log and the utility was provided for troubleshooting purpose only.

## Prerequisites

Required software components:

* [.NET Core Framework](https://www.microsoft.com/net/download/dotnet-core/2.1)-.NET Framework version 2.1 or later version
* [Visual Studio 2017](https://visualstudio.microsoft.com/downloads/)- We use Visual Studio 2017 to develop the application on Windows.

Optional software components:

* [Visual Studio Code](https://code.visualstudio.com/) - IDE for .NET Core development on Windows and mac OS. 
* Other IDE's such as: [Rider](https://www.jetbrains.com/rider/) - IDE From Jetbrain for .NET Core projects on mac OS.

## Getting Started

The application was implemented using .NET standard library and .NET Core 2.1 framework so that user can rebuild or republish the application on .NET Core supported platforms such as Windows, Linux, and Mac OS. Please find more details about the build and publish command from [MSDN](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-build?tabs=netcore2x). 

The console application requires RDMFieldDictionary and enumtype.def to decode the raw data in a Hex string format to the actual value. The user can update the data dictionary in the folder "Dict" to the latest version. In case that the application unable to find field definition of the fid or enum value, it will instead write original Hex string in the tracing log. Note that you can download the latest version of data dictionary from the [Software Download page](https://customers.thomsonreuters.com/a/support/technical/softwaredownload/default.aspx) and then search for product name "TREP Template service pack".

# Overview

The utility is console application which was designed for decoding and converting XML Fragments inside RSSL Tracing log generated by RFA C++/.NET and EMA/ETA C/C++. It can decode Hex string inside fieldEntry XML element to the actual value according to data dictionary from RDMFieldDictionary and enumtype.def. The utility is command line base console application which was developed by using .NET Core Framework so that user can build and run the console application on Windows, Linux and Mac OS according to information from  [.NET Core](https://www.microsoft.com/net) support platforms.

Typically, when the user using RFA or EMA application requesting data for Market Price and Level 2 Market By Price or Market by Order, the response message usually contains a field entry with a field value in Hex string format. The utility can be used to read the XML fragment inside the XML file and then converting the Hex string in the field entry to actual data according to the data type from the data dictionary. Moreover, it can be used to decode the new [MRN Real-Time News](https://developers.thomsonreuters.com/article/introduction-machine-readable-news-mrn-elektron-message-api-ema) which is the data from RIC MRN_STORY. The utility also has application's logic to scan XML fragments and consolidate a series of MRN compressed data and then unpack the compressed buffer to a raw JSON data. Then it will write decoded data along with MRN JSON data to a new XML file. The user can open the new XML file with an XML editor to search data. The utility should be able to help a developer to investigate a data relate issue or verify the actual data API sent or receive from a Provider server.

## How to collect RSSL Tracing log

The RSSL tracing log was provided for troubleshooting purpose and it's not turn on by default. A user may use the following instruction to turn on the log in RFA C++/.NET and EMA C++ application. Please note that currently, this utility does not support the trace from RFA and EMA java because it can't provide a valid XML trace format. There are some garbage or unexpected text inside the XML fragment and there are mixing between simple logger message and XML data inside the trace so that it causes the XML parsing error. If you want to use the trace log from EMA java with the application, the workaround is to manually remove invalid text or string from XML fragment and save it to a valid XML file.


### RFA C++ and .NET application

A user has to add the following configuration to turn on RSSL tracing log. <Connection_RSSL> is a connection name which the user is using in the configuration file.

```
\Connections\<Connection_RSSL>\connectionType    = "RSSL"
\Connections\<Connection_RSSL>\rsslPort     = "<RSSL Port>"
\Connections\<Connection_RSSL>\hostName        = "<ADS/Provider Hostname>"
\Connections\<Connection_RSSL>\traceMsg = false
\Connections\<Connection_RSSL>\traceMsgToFile = true
\Connections\<Connection_RSSL>\tracePing = true
\Connections\<Connection_RSSL>\traceMsgFileName   = "RSSLConsumerTrace"
\Connections\<Connection_RSSL>\traceMsgDomains = "all"
\Connections\<Connection_RSSL>\traceRespMsg = true
\Connections\<Connection_RSSL>\traceReqMsg = true
\Connections\<Connection_RSSL>\traceMsgHex = false 
\Connections\<Connection_RSSL>\traceMsgMaxMsgSize = 200000000
\Connections\<Connection_RSSL>\traceMsgMultipleFiles = true

```

Note that traceMsgFileName is an output of the XML file used by RFA to generate the log. From the example provided above, the output will be RSSLConsumerTrace_<pid>.xml where <pid> is the process id of the application.

### EMA C++ application

For EMA C++ application, to turn on the RSSL Trace log, a user has to copy EmaConfig.xml which provided in EMA Examples folder to project directory or running directory and then add below XML element to EMA configuration file under section Consumer.Note that you have to set __XmlTraceToFile__ to __1__ to turn on the log and set it to __0__ to turn off the log.

```
<DefaultConsumer value="Consumer_1"/>
<Consumer>
    <Name value="Consumer_1"/>
    ...
    <XmlTraceToFile value="1"/>
    <XmlTraceToStdout value="0"/>
</Consumer>
		
```

Though you are setting server name/IP address and the RSSL port in application codes and not using the Channel config from the configuration file, you can also copy the configuration file to the running directory and set the config under DefaultConsumer or DefaultProvider section. 

From the above sample, we set DefaultConsumer to Consumer_1 then EMA will check the value inside the configuration file and then verify option to turn on trace file from Consumer_1. EMA will generate the RSSL Tracing log file name EmaTrace_<id>.xml under the running directory.

## Utility download

The project and README are available for Download within from [GitHub](https://github.com/TR-API-Samples/Example.EMARFACPP.Tool.RSSLTraceViewer).

## Running the Utility

To run the utility from the command line, a user can run "__dotnet build__" and "__dotnet run__" command to build and run the application. An alternative option is using "__dotnet publish__" command to create a native executable file and then run the executable file directly.

### Launching the tool from the console:

Start a command prompt and change the folder to "<GitHub Repo>\Example.EMARFACPP.Tool.RSSLTraceViewer\rsslxmltracedataconverter", you should see rsslxmltracedataconverter.csproj in the folder. Then run the following command

```
dotnet build
```
To build the project then just type __dotnet run__ to run the application. It will automatically compile and build the project and run the application. It shows the following console output after running __dotnet run__.

```
rsslxmltracedataconverter 1.0.0
Copyright (C) 2018 Refinitiv
ERROR(S):
Required option 's, source' is missing.

  -s, --source      Required. Absolution path to RSSL XML Trace file

  -o, --output      (Default: output.xml) Absolute path to Xml output file, default is output.xml
                    generate under running directory.

  -r, --rdmdict     (Default: ./Dict/RDMFieldDictionary) Absolution path to RDMFieldDictionary
                    file. Default reading dictionary form /Dict/RDMFieldDictionary

  -e, --enumdict    (Default: ./Dict/enumtype.def) Absolution path to enumtype.def file. Default
                    reading dictionary form /Dict/enumtype.def

  --verbose         (Default: false) Prints all messages to standard output.

  --help            Display this help screen.

  --version         Display version information.

```
#### Command-line Options

From the previous section, to run the utility, it requires command line argument "-s" following by name or path to RSSL Trace file you want to convert. You can also specify the name of XML output by using -o following by output file name. If the user does not set output file name, the application will set it to "output.xml" instead. There are "-r" and "-e" that is an optional command line argument to specify a path to your own data dictionary. By default it will read the dictionary from folder "<RunningDirectory>\Dict". Please make sure that you have folder "Dict" in your running directory.

The following sample is an output after running the application with RSSL Tracing log.

```
$ dotnet run -s MRNViewerTrace_3160.xml
Using launch settings from launchSettings.json...
Start loading and processing Xml file MRNViewerTrace_3160.xml please wait
Processing completed 5% 20555/411087 elements
Processing completed 18% 73996/411087 elements
Processing completed 33% 135659/411087 elements
Processing completed 47% 193211/411087 elements
Processing completed 63% 258985/411087 elements
Processing completed 74% 304205/411087 elements
Processing completed 88% 361757/411087 elements
Processing completed 100% 411087/411087 elements
Finished reading and processing Xml data, please find output from output.xml
```

And below outputs is a sample of RSSL trace messages for original message and decoded data from MRN Real-Time News RIC, the utility will write MRN JSON data in XML comment tag.

Original XML Fragment.

```XML
<!-- Incoming Message from '192.168.27.46:14002' on 'localhost' interface -->
<!-- Time: 16:23:08:201 -->
<!-- rwfMajorVer="14" rwfMinorVer="0" -->
<updateMsg domainType="RSSL_DMT_NEWS_TEXT_ANALYTICS" streamId="3" containerType="RSSL_DT_FIELD_LIST" flags="0x1D2 (RSSL_UPMF_HAS_PERM_DATA|RSSL_UPMF_HAS_SEQ_NUM|RSSL_UPMF_DO_NOT_CACHE|RSSL_UPMF_DO_NOT_CONFLATE|RSSL_UPMF_DO_NOT_RIPPLE)" updateType="0 (RDM_UPD_EVENT_TYPE_UNSPECIFIED)" seqNum="54878" permData="0308 4310 153C" dataSize="1917">
    <dataBody>
        <fieldList flags="0x8 (RSSL_FLF_HAS_STANDARD_DATA)">
            <fieldEntry fieldId="4148" data="0203 95A0"/>
            <fieldEntry fieldId="17" data="0A0A 07E2"/>
            <fieldEntry fieldId="8593" data="5354 4F52 59"/>
            <fieldEntry fieldId="8506" data="32"/>
            <fieldEntry fieldId="11787" data="3130"/>
            <fieldEntry fieldId="32480" data="0708"/>
            <fieldEntry fieldId="32479" data="01"/>
            <fieldEntry fieldId="4271" data="4E44 4C33 3644 5173 735F 3138 3130 3130 3234 5275 7543 6163 2F49 6C5A 542B 4F6B
                6769 5A6F 7258 6E4A 787A 744F 3166 672F 565A 746A 6D57"/>
            <fieldEntry fieldId="12215" data="4844 435F 5052 445F 41"/>
            <fieldEntry fieldId="32641" data="1F8B 0800 0000 0000 02FF 8D57 6B73 DBB8 15FD DE5F 81D1 1727 2DF5 B493 78F5 A563
                CBF2 6337 B6B4 B1B3 6953 753A 1079 2922 0601 2E00 4ACB 76FA DF7B 2E48 2A4E B7D3
                ...
                387B D74F CEFB C965 3F76 A26E 960F EDE4 7679 D14F 16DD EC7D BFF7 70F1 B4EA 043C
                2C3F 7D68 67AB E5E2 AA5B 5CDF DE77 8BEB D5FB 76F2 B85A DCB5 B34F CB8F FC6B 8829
                FC91 7EAD F9A7 D260 3E4D 065C 054C 8A5F 48A7 C900 B5C0 2363 BE03 85FF FEC3 7F00
                5350 DB8A 870D 0000"/>
        </fieldList>
    </dataBody>
</updateMsg>
```

Decoded data inside fieldEntry element containing additional attributes in the fieldEntry that are __fieldName__ and __decodedData__. For the updateMsg type for domainType __RSSL_DMT_NEWS_TEXT_ANALYTICS__, it has additional XML comments after the last MRN fragment containing the same GUID.

```XML
<!-- Incoming Message from '192.168.27.46:14002' on 'localhost' interface -->
<!-- Time: 16:23:08:831 -->
<!-- rwfMajorVer="14" rwfMinorVer="0" -->
<updateMsg domainType="RSSL_DMT_NEWS_TEXT_ANALYTICS" streamId="3" containerType="RSSL_DT_FIELD_LIST" flags="0x1D2 (RSSL_UPMF_HAS_PERM_DATA|RSSL_UPMF_HAS_SEQ_NUM|RSSL_UPMF_DO_NOT_CACHE|RSSL_UPMF_DO_NOT_CONFLATE|RSSL_UPMF_DO_NOT_RIPPLE)" updateType="0 (RDM_UPD_EVENT_TYPE_UNSPECIFIED)" seqNum="54910" permData="0308 4310 127B 1221 4B12 287C" dataSize="698">
    <dataBody>
        <fieldList flags="0x8 (RSSL_FLF_HAS_STANDARD_DATA)">
            <fieldEntry fieldId="4148" fieldName="TIMACT_MS" data="0203 979F" decodedData="9:23:9:855" />
            <fieldEntry fieldId="17" fieldName="ACTIV_DATE" data="0A0A 07E2" decodedData="10/10/2018" />
            <fieldEntry fieldId="8593" fieldName="MRN_TYPE" data="5354 4F52 59" decodedData="STORY" />
            <fieldEntry fieldId="8506" fieldName="MRN_V_MAJ" data="32" decodedData="2" />
            <fieldEntry fieldId="11787" fieldName="MRN_V_MIN" data="3130" decodedData="10" />
            <fieldEntry fieldId="32480" fieldName="TOT_SIZE" data="0245" decodedData="581" />
            <fieldEntry fieldId="32479" fieldName="FRAG_NUM" data="01" decodedData="1" />
            <fieldEntry fieldId="4271" fieldName="GUID" data="4253 4562 7050 5177 745F 3138 3130 3130 3259 4D56 3451 386A 4974 744D 4E4C 6767                 4C53 5231 6B36 6D41 4E53 7656 592F 6636 4F4A 6572 7132" decodedData="BSEbpPQwt_1810102YMV4Q8jIttMNLggLSR1k6mANSvVY/f6OJerq2" />
            <fieldEntry fieldId="12215" fieldName="MRN_SRC" data="4844 435F 5052 445F 41" decodedData="HDC_PRD_A" />
            <fieldEntry fieldId="32641" fieldName="FRAGMENT" data="1F8B 0800 0000 0000 02FF 8D53 DB4E DB40 107D EF57 ACF6 A14F 21BE 0542 5642 1524                 8106 2506 620A A24D 55AD ED71 B2E0 ACCD EE2C 1455 FDF7 CEC6 2D6A 5F2A 244B 73D9                 9933 B7E3 1F5C D638 2BB9 E0FA 249B E6ED E5D5 33F2 1E97 AE54 A00B B05C ...
            0364 F0E8 3C7B B888 7ADC 9935 E9F4 3B24 3DFE 44D7 548D 7EC3 B17E BEFB 050F 719E                 4B90 0300 00" decodedData="1F8B08000000000002FF8D53DB4EDB40107DEF57ACF6A14F21BE05425642152481062506620AA24D55ADED71B2E0ACCDEE2C1455FDF7CEC62D6A5F2A244B73D99933B7E31F5CD6382BB9E0FA249BE6EDE5D533F21E97AE54A00BB05C7CE1E9A5A0B76372775ADA69E374F9AA1CF3AF3D9E37E50B01ADF41D587622F5039B63C99E54094CA16535208261A54428D945814D4E5638EAB1388C0E7B6C232D53BA6ACC969EA90A9379E370A533A4842D68644DC5269023CBA07046A1021BA4E3896594C37003AC05A39A92812E09218396D27C891DFC4AB7CE582709061B964D4F666CAC4CE16A69580ED228BD66062A307E6AA69B3E1BCF96C16C310926A77B5170300C0866D835BFD2E74E034BC25DEBC3FE4AAFF406B11541D09655DF80A3396DBF68B6DED6F06CFFC8BEB4ED07753448C2FD5191E45508C9707F10BD7747CE68E123B6B5F82B5FF8DEA3300AC5EE3A6787B7E7035F8DF65E2963716CC037444BF7817B5148DF7538127122C2517F38187EA6C80DC8B2561A28EA9FC3ECB1B7ADF63F6B2574E5ABBF52E75BD76E7CB7B8195C1DDECF1017E97CBD9E67CBE8E1607B9C664F3777417570710EE631F6D9DAA2F43CBBA88869C4A15AEAB5936BDF2CF831B7602D99D72F2DB962B2D5F6B7C111BE63D0D652F9B8D6349E6886FC69E649EA7D2EF7133A2231A732289C95790DF4625D7E0F05EED8BD10D1D5847C0B91C49DF8B813E927CFEE58CC3BBEC762717D3939EDD4747ABBECB4E5F4CC531FE50364F0E83C7BB8887ADC9935E9F43B243DFE44D7548D7EC3B17EBEFB050F719E4B90030000" />
        </fieldList>
    </dataBody>
</updateMsg>
<!--{
  "altId": "nBSEbpPQwt",
  "audiences": [
    "NP:BSEA",
    "NP:BSEN",
    "NP:CNR",
    "NP:CNRA"
  ],
  "body": "\nYes Bank Ltd vide its letter dated October 09, 2018, has informed BSE about\nStatement of Debt Securities/NCDs for the period ended September 2018,\npursuant to SEBI Circular bearing reference no. CIR/IMD/DF-1/67/2017 dated\nJune 30, 2017.\n\nhttp://pdf.reuters.com/pdfnews/pdfnews.asp?i=43059c3bf0e37541&u=urn:newsml:reuters.com:20181010:nBSEbG8WJ4\n\n",
  "firstCreated": "2018-10-10T09:23:09.747Z",
  "headline": "Yes Bank Ltd - Statement of Debt Securities/NCDs for period ended September 2018",
  "id": "BSEbpPQwt_1810102YMV4Q8jIttMNLggLSR1k6mANSvVY/f6OJerq2",
  "instancesOf": [],
  "language": "en",
  "messageType": 2,
  "mimeType": "text/plain",
  "provider": "NS:BSE",
  "pubStatus": "stat:usable",
  "subjects": [
    "M:1QD",
    "M:32",
    "M:3H",
    "M:NU",
    "N2:LEN",
    "N2:MTPDF",
    "N2:NEWR",
    "N2:REG"
  ],
  "takeSequence": 1,
  "urgency": 3,
  "versionCreated": "2018-10-10T09:23:09.747Z"
}-->
```

Note that there is command line option "--verbose" which can print JSON output to console while the utility processing the data.

In addition to using the utility to decode MRN data, as described in the overview section, the application can be used to decoded data for the Level 2 Market Price data and below is a sample output for the Market By Order domain when using the utility convert the data.

```XML
<!-- Incoming Message from '192.168.27.46:14002' on 'localhost' interface -->
<!-- Time: 13:05:16:687 -->
<!-- rwfMajorVer="14" rwfMinorVer="0" -->
<refreshMsg domainType="RSSL_DMT_MARKET_BY_ORDER" streamId="4" containerType="RSSL_DT_MAP" flags="0xFA (RSSL_RFMF_HAS_PERM_DATA|RSSL_RFMF_HAS_MSG_KEY|RSSL_RFMF_HAS_SEQ_NUM|RSSL_RFMF_SOLICITED|RSSL_RFMF_REFRESH_COMPLETE|RSSL_RFMF_HAS_QOS)" groupId="2" seqNum="15472" permData="0308 4249 50C0" qosDynamic="0" qosRate="1" qosTimeliness="1" dataState="RSSL_DATA_OK" streamState="RSSL_STREAM_OPEN" code="RSSL_SC_NONE" text="All is well" dataSize="937">
    <key flags="0x7 (RSSL_MKF_HAS_SERVICE_ID|RSSL_MKF_HAS_NAME|RSSL_MKF_HAS_NAME_TYPE)" serviceId="2114" name="0005.HK" nameType="1" />
    <dataBody>
        <map flags="0x0" countHint="0" keyPrimitiveType="RSSL_DT_BUFFER" containerType="RSSL_DT_FIELD_LIST">
            <mapEntry flags="0x0" action="RSSL_MPEA_ADD_ENTRY" key="5@13F9EC03">
                <fieldList flags="0x9 (RSSL_FLF_HAS_FIELD_LIST_INFO|RSSL_FLF_HAS_STANDARD_DATA)" fieldListNum="32767" dictionaryId="1">
                    <fieldEntry fieldId="3426" fieldName="ORDER_ID" data="3333 3531 3435 3938 37" decodedData="335145987" />
                    <fieldEntry fieldId="3427" fieldName="ORDER_PRC" data="0B01 0D24" decodedData="68.9" />
                    <fieldEntry fieldId="3428" fieldName="ORDER_SIDE" data="02" decodedData="ASK(2)" />
                    <fieldEntry fieldId="3429" fieldName="ORDER_SIZE" data="0E01 90" decodedData="400" />
                    <fieldEntry fieldId="6520" fieldName="PR_TIM_MS" data="6984 67" decodedData="1:55:15:175" />
                    <fieldEntry fieldId="6522" fieldName="PR_DATE" data="040A 07E2" decodedData="4/10/2018" />
                </fieldList>
            </mapEntry>
            <mapEntry flags="0x0" action="RSSL_MPEA_ADD_ENTRY" key="5@4353803">
                <fieldList flags="0x9 (RSSL_FLF_HAS_FIELD_LIST_INFO|RSSL_FLF_HAS_STANDARD_DATA)" fieldListNum="32767" dictionaryId="1">
                    <fieldEntry fieldId="3426" fieldName="ORDER_ID" data="3730 3539 3636 3131" decodedData="70596611" />
                    <fieldEntry fieldId="3427" fieldName="ORDER_PRC" data="0B01 03C4" decodedData="66.5" />
                    <fieldEntry fieldId="3428" fieldName="ORDER_SIDE" data="01" decodedData="BID(1)" />
                    <fieldEntry fieldId="3429" fieldName="ORDER_SIZE" data="0E03 20" decodedData="800" />
                    <fieldEntry fieldId="6520" fieldName="PR_TIM_MS" data="55AF 73" decodedData="1:33:35:475" />
                    <fieldEntry fieldId="6522" fieldName="PR_DATE" data="040A 07E2" decodedData="4/10/2018" />
                </fieldList>
            </mapEntry>
            <mapEntry flags="0x0" action="RSSL_MPEA_ADD_ENTRY" key="5@3F036903">
                <fieldList flags="0x9 (RSSL_FLF_HAS_FIELD_LIST_INFO|RSSL_FLF_HAS_STANDARD_DATA)" fieldListNum="32767" dictionaryId="1">
                    <fieldEntry fieldId="3426" fieldName="ORDER_ID" data="3130 3537 3138 3830 3939" decodedData="1057188099" />
                    <fieldEntry fieldId="3427" fieldName="ORDER_PRC" data="0B01 0842" decodedData="67.65" />
                    <fieldEntry fieldId="3428" fieldName="ORDER_SIDE" data="01" decodedData="BID(1)" />
                    <fieldEntry fieldId="3429" fieldName="ORDER_SIZE" data="0E06 40" decodedData="1600" />
                    <fieldEntry fieldId="6520" fieldName="PR_TIM_MS" data="0141 6B02" decodedData="5:51:4:450" />
                    <fieldEntry fieldId="6522" fieldName="PR_DATE" data="040A 07E2" decodedData="4/10/2018" />
                </fieldList>
            </mapEntry>
            <mapEntry flags="0x0" action="RSSL_MPEA_ADD_ENTRY" key="5@676503">
                <fieldList flags="0x9 (RSSL_FLF_HAS_FIELD_LIST_INFO|RSSL_FLF_HAS_STANDARD_DATA)" fieldListNum="32767" dictionaryId="1">
                    <fieldEntry fieldId="3426" fieldName="ORDER_ID" data="3637 3736 3036 37" decodedData="6776067" />
                    <fieldEntry fieldId="3427" fieldName="ORDER_PRC" data="0B01 0360" decodedData="66.4" />
                    <fieldEntry fieldId="3428" fieldName="ORDER_SIDE" data="01" decodedData="BID(1)" />
                    <fieldEntry fieldId="3429" fieldName="ORDER_SIZE" data="0E03 20" decodedData="800" />
                    <fieldEntry fieldId="6520" fieldName="PR_TIM_MS" data="493E C8" decodedData="1:20:0:200" />
                    <fieldEntry fieldId="6522" fieldName="PR_DATE" data="040A 07E2" decodedData="4/10/2018" />
                </fieldList>
            </mapEntry>
            <mapEntry flags="0x0" action="RSSL_MPEA_ADD_ENTRY" key="5@2928D703">
                <fieldList flags="0x9 (RSSL_FLF_HAS_FIELD_LIST_INFO|RSSL_FLF_HAS_STANDARD_DATA)" fieldListNum="32767" dictionaryId="1">
                    <fieldEntry fieldId="3426" fieldName="ORDER_ID" data="3639 3035 3432 3333 39" decodedData="690542339" />
                    <fieldEntry fieldId="3427" fieldName="ORDER_PRC" data="0B01 07AC" decodedData="67.5" />
                    <fieldEntry fieldId="3428" fieldName="ORDER_SIDE" data="01" decodedData="BID(1)" />
                    <fieldEntry fieldId="3429" fieldName="ORDER_SIZE" data="0E04 B0" decodedData="1200" />
                    <fieldEntry fieldId="6520" fieldName="PR_TIM_MS" data="9C82 68" decodedData="2:50:57:0" />
                    <fieldEntry fieldId="6522" fieldName="PR_DATE" data="040A 07E2" decodedData="4/10/2018" />
                </fieldList>
            </mapEntry>
            <mapEntry flags="0x0" action="RSSL_MPEA_ADD_ENTRY" key="5@133EF03">
                <fieldList flags="0x9 (RSSL_FLF_HAS_FIELD_LIST_INFO|RSSL_FLF_HAS_STANDARD_DATA)" fieldListNum="32767" dictionaryId="1">
                    <fieldEntry fieldId="3426" fieldName="ORDER_ID" data="3230 3138 3037 3339" decodedData="20180739" />
                    <fieldEntry fieldId="3427" fieldName="ORDER_PRC" data="0B01 04BE" decodedData="66.75" />
                    <fieldEntry fieldId="3428" fieldName="ORDER_SIDE" data="01" decodedData="BID(1)" />
                    <fieldEntry fieldId="3429" fieldName="ORDER_SIZE" data="0E0C 80" decodedData="3200" />
                    <fieldEntry fieldId="6520" fieldName="PR_TIM_MS" data="52CB 82" decodedData="1:30:26:50" />
                    <fieldEntry fieldId="6522" fieldName="PR_DATE" data="040A 07E2" decodedData="4/10/2018" />
                </fieldList>
            </mapEntry>
            <mapEntry flags="0x0" action="RSSL_MPEA_ADD_ENTRY" key="5@40064F03">
                <fieldList flags="0x9 (RSSL_FLF_HAS_FIELD_LIST_INFO|RSSL_FLF_HAS_STANDARD_DATA)" fieldListNum="32767" dictionaryId="1">
                    <fieldEntry fieldId="3426" fieldName="ORDER_ID" data="3130 3734 3135 3532 3637" decodedData="1074155267" />
                    <fieldEntry fieldId="3427" fieldName="ORDER_PRC" data="0B01 07DE" decodedData="67.55" />
                    <fieldEntry fieldId="3428" fieldName="ORDER_SIDE" data="01" decodedData="BID(1)" />
                    <fieldEntry fieldId="3429" fieldName="ORDER_SIZE" data="0E5D C0" decodedData="24000" />
                    <fieldEntry fieldId="6520" fieldName="PR_TIM_MS" data="0148 795B" decodedData="5:58:46:875" />
                    <fieldEntry fieldId="6522" fieldName="PR_DATE" data="040A 07E2" decodedData="4/10/2018" />
                </fieldList>
            </mapEntry>
            <mapEntry flags="0x0" action="RSSL_MPEA_ADD_ENTRY" key="5@1C4E1E03">
                <fieldList flags="0x9 (RSSL_FLF_HAS_FIELD_LIST_INFO|RSSL_FLF_HAS_STANDARD_DATA)" fieldListNum="32767" dictionaryId="1">
                    <fieldEntry fieldId="3426" fieldName="ORDER_ID" data="3437 3438 3831 3533 39" decodedData="474881539" />
                    <fieldEntry fieldId="3427" fieldName="ORDER_PRC" data="0B01 07AC" decodedData="67.5" />
                    <fieldEntry fieldId="3428" fieldName="ORDER_SIDE" data="01" decodedData="BID(1)" />
                    <fieldEntry fieldId="3429" fieldName="ORDER_SIZE" data="0E03 20" decodedData="800" />
                    <fieldEntry fieldId="6520" fieldName="PR_TIM_MS" data="79B5 6C" decodedData="2:12:56:300" />
                    <fieldEntry fieldId="6522" fieldName="PR_DATE" data="040A 07E2" decodedData="4/10/2018" />
                </fieldList>
            </mapEntry>
            <mapEntry flags="0x0" action="RSSL_MPEA_ADD_ENTRY" key="5@FB103">
                <fieldList flags="0x9 (RSSL_FLF_HAS_FIELD_LIST_INFO|RSSL_FLF_HAS_STANDARD_DATA)" fieldListNum="32767" dictionaryId="1">
                    <fieldEntry fieldId="3426" fieldName="ORDER_ID" data="3130 3238 3335 35" decodedData="1028355" />
                    <fieldEntry fieldId="3427" fieldName="ORDER_PRC" data="0B01 1940" decodedData="72" />
                    <fieldEntry fieldId="3428" fieldName="ORDER_SIDE" data="02" decodedData="ASK(2)" />
                    <fieldEntry fieldId="3429" fieldName="ORDER_SIZE" data="0E51 40" decodedData="20800" />
                    <fieldEntry fieldId="6520" fieldName="PR_TIM_MS" data="493E AF" decodedData="1:20:0:175" />
                    <fieldEntry fieldId="6522" fieldName="PR_DATE" data="040A 07E2" decodedData="4/10/2018" />
                </fieldList>
            </mapEntry>
            <mapEntry flags="0x0" action="RSSL_MPEA_ADD_ENTRY" key="5@19C503">
                <fieldList flags="0x9 (RSSL_FLF_HAS_FIELD_LIST_INFO|RSSL_FLF_HAS_STANDARD_DATA)" fieldListNum="32767" dictionaryId="1">
                    <fieldEntry fieldId="3426" fieldName="ORDER_ID" data="3136 3838 3833 35" decodedData="1688835" />
                    <fieldEntry fieldId="3427" fieldName="ORDER_PRC" data="0B01 10A8" decodedData="69.8" />
                    <fieldEntry fieldId="3428" fieldName="ORDER_SIDE" data="02" decodedData="ASK(2)" />
                    <fieldEntry fieldId="3429" fieldName="ORDER_SIZE" data="0E03 20" decodedData="800" />
                    <fieldEntry fieldId="6520" fieldName="PR_TIM_MS" data="493E AF" decodedData="1:20:0:175" />
                    <fieldEntry fieldId="6522" fieldName="PR_DATE" data="040A 07E2" decodedData="4/10/2018" />
                </fieldList>
            </mapEntry>
            <mapEntry flags="0x0" action="RSSL_MPEA_ADD_ENTRY" key="5@5FDF003">
                <fieldList flags="0x9 (RSSL_FLF_HAS_FIELD_LIST_INFO|RSSL_FLF_HAS_STANDARD_DATA)" fieldListNum="32767" dictionaryId="1">
                    <fieldEntry fieldId="3426" fieldName="ORDER_ID" data="3130 3035 3238 3133 31" decodedData="100528131" />
                    <fieldEntry fieldId="3427" fieldName="ORDER_PRC" data="0B01 0716" decodedData="67.35" />
                    <fieldEntry fieldId="3428" fieldName="ORDER_SIDE" data="01" decodedData="BID(1)" />
                    <fieldEntry fieldId="3429" fieldName="ORDER_SIZE" data="0E09 60" decodedData="2400" />
                    <fieldEntry fieldId="6520" fieldName="PR_TIM_MS" data="570A 9E" decodedData="1:35:4:350" />
                    <fieldEntry fieldId="6522" fieldName="PR_DATE" data="040A 07E2" decodedData="4/10/2018" />
                </fieldList>
            </mapEntry>
            <mapEntry flags="0x0" action="RSSL_MPEA_ADD_ENTRY" key="5@FFE903">
                <fieldList flags="0x9 (RSSL_FLF_HAS_FIELD_LIST_INFO|RSSL_FLF_HAS_STANDARD_DATA)" fieldListNum="32767" dictionaryId="1">
                    <fieldEntry fieldId="3426" fieldName="ORDER_ID" data="3136 3737 3133 3331" decodedData="16771331" />
                    <fieldEntry fieldId="3427" fieldName="ORDER_PRC" data="0B01 05B8" decodedData="67" />
                    <fieldEntry fieldId="3428" fieldName="ORDER_SIDE" data="01" decodedData="BID(1)" />
                    <fieldEntry fieldId="3429" fieldName="ORDER_SIZE" data="0E07 D0" decodedData="2000" />
                    <fieldEntry fieldId="6520" fieldName="PR_TIM_MS" data="52A4 72" decodedData="1:30:16:50" />
                    <fieldEntry fieldId="6522" fieldName="PR_DATE" data="040A 07E2" decodedData="4/10/2018" />
                </fieldList>
            </mapEntry>
            <mapEntry flags="0x0" action="RSSL_MPEA_ADD_ENTRY" key="5@D68203">
                <fieldList flags="0x9 (RSSL_FLF_HAS_FIELD_LIST_INFO|RSSL_FLF_HAS_STANDARD_DATA)" fieldListNum="32767" dictionaryId="1">
                    <fieldEntry fieldId="3426" fieldName="ORDER_ID" data="3134 3035 3739 3837" decodedData="14057987" />
                    <fieldEntry fieldId="3427" fieldName="ORDER_PRC" data="0B01 07AC" decodedData="67.5" />
                    <fieldEntry fieldId="3428" fieldName="ORDER_SIDE" data="01" decodedData="BID(1)" />
                    <fieldEntry fieldId="3429" fieldName="ORDER_SIZE" data="0E03 20" decodedData="800" />
                    <fieldEntry fieldId="6520" fieldName="PR_TIM_MS" data="5289 1A" decodedData="1:30:9:50" />
                    <fieldEntry fieldId="6522" fieldName="PR_DATE" data="040A 07E2" decodedData="4/10/2018" />
                </fieldList>
            </mapEntry>
            <mapEntry flags="0x0" action="RSSL_MPEA_ADD_ENTRY" key="5@47D003">
                <fieldList flags="0x9 (RSSL_FLF_HAS_FIELD_LIST_INFO|RSSL_FLF_HAS_STANDARD_DATA)" fieldListNum="32767" dictionaryId="1">
                    <fieldEntry fieldId="3426" fieldName="ORDER_ID" data="3437 3036 3330 37" decodedData="4706307" />
                    <fieldEntry fieldId="3427" fieldName="ORDER_PRC" data="0B01 0360" decodedData="66.4" />
                    <fieldEntry fieldId="3428" fieldName="ORDER_SIDE" data="01" decodedData="BID(1)" />
                    <fieldEntry fieldId="3429" fieldName="ORDER_SIZE" data="0E27 10" decodedData="10000" />
                    <fieldEntry fieldId="6520" fieldName="PR_TIM_MS" data="493E C8" decodedData="1:20:0:200" />
                    <fieldEntry fieldId="6522" fieldName="PR_DATE" data="040A 07E2" decodedData="4/10/2018" />
                </fieldList>
            </mapEntry>
            <mapEntry flags="0x0" action="RSSL_MPEA_ADD_ENTRY" key="5@250003">
                <fieldList flags="0x9 (RSSL_FLF_HAS_FIELD_LIST_INFO|RSSL_FLF_HAS_STANDARD_DATA)" fieldListNum="32767" dictionaryId="1">
                    <fieldEntry fieldId="3426" fieldName="ORDER_ID" data="3234 3234 3833 35" decodedData="2424835" />
                    <fieldEntry fieldId="3427" fieldName="ORDER_PRC" data="0B01 2CC8" decodedData="77" />
                    <fieldEntry fieldId="3428" fieldName="ORDER_SIDE" data="02" decodedData="ASK(2)" />
                    <fieldEntry fieldId="3429" fieldName="ORDER_SIZE" data="0E15 E0" decodedData="5600" />
                    <fieldEntry fieldId="6520" fieldName="PR_TIM_MS" data="493E AF" decodedData="1:20:0:175" />
                    <fieldEntry fieldId="6522" fieldName="PR_DATE" data="040A 07E2" decodedData="4/10/2018" />
                </fieldList>
            </mapEntry>
        </map>
    </dataBody>
</refreshMsg>
``` 
 
### Launching the tool from executable file

There is an option for .NET Core application to publish the project to native executable file and then you can run executable file directly without using __dotnet__ command on windows, mac os, and Linux. You can refer to [MSDN Document](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-publish?tabs=netcore21) for __dotnet__ command line option to publish the project. 

To generate a native executable file on Windows, you can run
```
dotnet publish -c release -r win-x64
```

You should see the following output when executing __dotnet publish__ command.

```
C:\gitrepo\Example.EMARFACPP.Tool.RSSLTraceViewer\rsslxmltracedataconverter>dotnet publish -c release -r win-x64
Microsoft (R) Build Engine version 15.7.179.6572 for .NET Core
Copyright (C) Microsoft Corporation. All rights reserved.

  Restoring packages for C:\gitrepo\Example.EMARFACPP.Tool.RSSLTraceViewer\rsslxmltracedataconverter\rsslxmltracedataconverter.csproj...
  Generating MSBuild file C:\gitrepo\Example.EMARFACPP.Tool.RSSLTraceViewer\rsslxmltracedataconverter\obj\rsslxmltracedataconverter.csproj.nuget.g.props.
  Generating MSBuild file C:\gitrepo\Example.EMARFACPP.Tool.RSSLTraceViewer\rsslxmltracedataconverter\obj\rsslxmltracedataconverter.csproj.nuget.g.targets.
  Restore completed in 1.58 sec for C:\gitrepo\Example.EMARFACPP.Tool.RSSLTraceViewer\rsslxmltracedataconverter\rsslxmltracedataconverter.csproj.
  rsslxmltracedataconverter -> C:\gitrepo\Example.EMARFACPP.Tool.RSSLTraceViewer\rsslxmltracedataconverter\bin\release\netcoreapp2.1\win-x64\rsslxmltracedataconverter.dll
          1 file(s) copied.
          1 file(s) copied.
  rsslxmltracedataconverter -> C:\gitrepo\Example.EMARFACPP.Tool.RSSLTraceViewer\rsslxmltracedataconverter\bin\release\netcoreapp2.1\win-x64\publish\

```
After executing the command, you can find a native executable file __rsslxmltracedataconverter.exe__ , it should be located under the folder "<Project Dir>\bin\release\netcoreapp2.1\win-x64" and then you can copy or share the folder in order to run the utility on other Windows.

To use the utility then you can just executing the following command.

```
c:\<Project Publish Folder>\rsslxmltracedataconverter.exe -s <XML file path>
```

For macOS and Linux you can just change the Runtime Identifier from __win-x64__ to __osx-x64__ and __linux-x64__ according to the list from[rid-catalog page](https://docs.microsoft.com/en-us/dotnet/core/rid-catalog).

Then you can run the utility from a publishing folder by executing command
```
./rsslxmltracedataconverter -s <XML file path>
```
Just like executing another native application on macOS or Linux.

## Contributing

Please read [CONTRIBUTING.md](https://gist.github.com/PurpleBooth/b24679402957c63ec426) for details on our code of conduct, and the process for submitting pull requests to us.

## Authors

* **Moragodkrit Chumsri** - Release 1.0.  *Initial work*


## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details
