
using System;
using System.IO;
using System.Runtime.Serialization;
using com.calitha.goldparser.lalr;
using com.calitha.commons;

namespace com.calitha.goldparser
{

    [Serializable()]
    public class SymbolException : System.Exception
    {
        public SymbolException(string message) : base(message)
        {
        }

        public SymbolException(string message,
            Exception inner) : base(message, inner)
        {
        }

        protected SymbolException(SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }

    }

    [Serializable()]
    public class RuleException : System.Exception
    {

        public RuleException(string message) : base(message)
        {
        }

        public RuleException(string message,
                             Exception inner) : base(message, inner)
        {
        }

        protected RuleException(SerializationInfo info,
                                StreamingContext context) : base(info, context)
        {
        }

    }

    enum SymbolConstants : int
    {
        SYMBOL_EOF                        =  0, // (EOF)
        SYMBOL_ERROR                      =  1, // (Error)
        SYMBOL_WHITESPACE                 =  2, // (Whitespace)
        SYMBOL_MINUS                      =  3, // '-'
        SYMBOL_LPARAN                     =  4, // '('
        SYMBOL_RPARAN                     =  5, // ')'
        SYMBOL_COMMA                      =  6, // ','
        SYMBOL_COLON                      =  7, // ':'
        SYMBOL_ATCMD                      =  8, // '@cmd'
        SYMBOL_LBRACKETCOMMANDRBRACKET    =  9, // '[Command]'
        SYMBOL_LBRACKETDEVICEINFORBRACKET = 10, // '[DeviceInfo]'
        SYMBOL_LBRACE                     = 11, // '{'
        SYMBOL_RBRACE                     = 12, // '}'
        SYMBOL_CLASSEQ                    = 13, // 'class='
        SYMBOL_CMD                        = 14, // Cmd
        SYMBOL_CMDEQ                      = 15, // 'cmd='
        SYMBOL_CMDCLASS                   = 16, // CmdClass
        SYMBOL_CMDTYPE                    = 17, // CmdType
        SYMBOL_DESCRIPTIONEQ              = 18, // 'description='
        SYMBOL_DEVICEID                   = 19, // DeviceID
        SYMBOL_DEVICEIDEQ                 = 20, // 'DeviceID='
        SYMBOL_DEVICETYPEEQ               = 21, // 'DeviceType='
        SYMBOL_FLOAT                      = 22, // Float
        SYMBOL_FUNC_NAMEEQ                = 23, // 'func_name='
        SYMBOL_HEXDIGITS                  = 24, // HexDigits
        SYMBOL_IDENTIFIER                 = 25, // Identifier
        SYMBOL_IP                         = 26, // IP
        SYMBOL_IPEQ                       = 27, // 'IP='
        SYMBOL_NUMBER                     = 28, // Number
        SYMBOL_PORTEQ                     = 29, // 'Port='
        SYMBOL_RETURNEQ                   = 30, // 'return='
        SYMBOL_SENDEQ                     = 31, // 'send='
        SYMBOL_STRINGLITERAL              = 32, // StringLiteral
        SYMBOL_TESTEQ                     = 33, // 'test='
        SYMBOL_TYPEEQ                     = 34, // 'type='
        SYMBOL_VERSIONEQ                  = 35, // 'Version='
        SYMBOL_BYTES                      = 36, // <Bytes>
        SYMBOL_CMD2                       = 37, // <Cmd>
        SYMBOL_CMDCLASS2                  = 38, // <CmdClass>
        SYMBOL_CMDTITLE                   = 39, // <CmdTitle>
        SYMBOL_CMDTYPE2                   = 40, // <CmdType>
        SYMBOL_COMMAND                    = 41, // <Command>
        SYMBOL_COMMANDLIST                = 42, // <CommandList>
        SYMBOL_DESCRIPTION                = 43, // <Description>
        SYMBOL_DEVICEID2                  = 44, // <DeviceId>
        SYMBOL_DEVICEINFO                 = 45, // <DeviceInfo>
        SYMBOL_DEVICETYPE                 = 46, // <DeviceType>
        SYMBOL_EXPRESSITEM                = 47, // <ExpressItem>
        SYMBOL_EXPRESSLIST                = 48, // <ExpressList>
        SYMBOL_FUNCNAME                   = 49, // <FuncName>
        SYMBOL_HVALUE                     = 50, // <HValue>
        SYMBOL_IP2                        = 51, // <IP>
        SYMBOL_LVALUE                     = 52, // <LValue>
        SYMBOL_NUMBERLIST                 = 53, // <NumberList>
        SYMBOL_PORT                       = 54, // <Port>
        SYMBOL_PROGRAM                    = 55, // <Program>
        SYMBOL_RANGEITEM                  = 56, // <RangeItem>
        SYMBOL_RANGEVALUE                 = 57, // <RangeValue>
        SYMBOL_REPEATEXPRESS              = 58, // <RepeatExpress>
        SYMBOL_RETURNEXPRESS              = 59, // <ReturnExpress>
        SYMBOL_SELECTITEM                 = 60, // <SelectItem>
        SYMBOL_SELECTVALUE                = 61, // <SelectValue>
        SYMBOL_SELECTVALUES               = 62, // <SelectValues>
        SYMBOL_SENDEXPRESS                = 63, // <SendExpress>
        SYMBOL_SUBCMD                     = 64, // <SubCmd>
        SYMBOL_TESTEXPRESS                = 65, // <TestExpress>
        SYMBOL_TESTEXPRESSLIST            = 66, // <TestExpressList>
        SYMBOL_TESTGROUPEXPRESS           = 67, // <TestGroupExpress>
        SYMBOL_TESTITEM                   = 68, // <TestItem>
        SYMBOL_TESTITEMLIST               = 69, // <TestItemList>
        SYMBOL_TESTRANGEITEM              = 70, // <TestRangeItem>
        SYMBOL_TESTREPEATITEMS            = 71, // <TestRepeatItems>
        SYMBOL_TESTSELECTITEM             = 72, // <TestSelectItem>
        SYMBOL_TESTSELECTVALUE            = 73, // <TestSelectValue>
        SYMBOL_TESTSELECTVALUES           = 74, // <TestSelectValues>
        SYMBOL_VALUEDESCRIPTION           = 75, // <ValueDescription>
        SYMBOL_VERSION                    = 76  // <Version>
    };

    enum RuleConstants : int
    {
        RULE_PROGRAM                                      =  0, // <Program> ::= <DeviceInfo> <CommandList>
        RULE_DEVICEINFO_LBRACKETDEVICEINFORBRACKET        =  1, // <DeviceInfo> ::= '[DeviceInfo]' <Version> <DeviceType> <IP> <Port> <DeviceId>
        RULE_VERSION_VERSIONEQ_FLOAT                      =  2, // <Version> ::= 'Version=' Float
        RULE_DEVICETYPE_DEVICETYPEEQ_IDENTIFIER           =  3, // <DeviceType> ::= 'DeviceType=' Identifier
        RULE_DEVICEID_DEVICEIDEQ_DEVICEID                 =  4, // <DeviceId> ::= 'DeviceID=' DeviceID
        RULE_IP_IPEQ_IP                                   =  5, // <IP> ::= 'IP=' IP
        RULE_CMDCLASS_CLASSEQ_CMDCLASS                    =  6, // <CmdClass> ::= 'class=' CmdClass
        RULE_PORT_PORTEQ_NUMBER                           =  7, // <Port> ::= 'Port=' Number
        RULE_CMD_CMDEQ_CMD                                =  8, // <Cmd> ::= 'cmd=' Cmd <SubCmd>
        RULE_SUBCMD                                       =  9, // <SubCmd> ::= 
        RULE_SUBCMD_CMD                                   = 10, // <SubCmd> ::= Cmd
        RULE_DESCRIPTION_DESCRIPTIONEQ_STRINGLITERAL      = 11, // <Description> ::= 'description=' StringLiteral
        RULE_CMDTYPE_TYPEEQ_CMDTYPE                       = 12, // <CmdType> ::= 'type=' CmdType
        RULE_CMDTITLE_LBRACKETCOMMANDRBRACKET             = 13, // <CmdTitle> ::= '[Command]'
        RULE_FUNCNAME_FUNC_NAMEEQ_STRINGLITERAL           = 14, // <FuncName> ::= 'func_name=' StringLiteral
        RULE_COMMANDLIST                                  = 15, // <CommandList> ::= 
        RULE_COMMANDLIST2                                 = 16, // <CommandList> ::= <Command> <CommandList>
        RULE_COMMAND                                      = 17, // <Command> ::= <CmdTitle> <Cmd> <Description> <CmdClass> <FuncName> <CmdType> <SendExpress> <ReturnExpress> <TestGroupExpress>
        RULE_SENDEXPRESS_SENDEQ                           = 18, // <SendExpress> ::= 'send='
        RULE_SENDEXPRESS_SENDEQ2                          = 19, // <SendExpress> ::= 'send=' <ExpressList>
        RULE_EXPRESSLIST                                  = 20, // <ExpressList> ::= <ExpressItem>
        RULE_EXPRESSLIST2                                 = 21, // <ExpressList> ::= <ExpressItem> <ExpressList>
        RULE_REPEATEXPRESS_LBRACE_RBRACE                  = 22, // <RepeatExpress> ::= <ExpressItem> '{' <ExpressList> '}'
        RULE_EXPRESSITEM                                  = 23, // <ExpressItem> ::= <RangeItem>
        RULE_EXPRESSITEM2                                 = 24, // <ExpressItem> ::= <SelectItem>
        RULE_EXPRESSITEM3                                 = 25, // <ExpressItem> ::= <RepeatExpress>
        RULE_RANGEITEM_IDENTIFIER_LPARAN_RPARAN           = 26, // <RangeItem> ::= Identifier '(' <RangeValue> ')'
        RULE_RANGEVALUE_COLON_MINUS                       = 27, // <RangeValue> ::= <Bytes> ':' <LValue> '-' <HValue>
        RULE_LVALUE_NUMBER                                = 28, // <LValue> ::= Number
        RULE_HVALUE_NUMBER                                = 29, // <HValue> ::= Number
        RULE_BYTES_NUMBER                                 = 30, // <Bytes> ::= Number
        RULE_SELECTITEM_IDENTIFIER_LPARAN_RPARAN          = 31, // <SelectItem> ::= Identifier '(' <SelectValues> ')'
        RULE_SELECTVALUES_COLON                           = 32, // <SelectValues> ::= <Bytes> ':' <NumberList>
        RULE_NUMBERLIST                                   = 33, // <NumberList> ::= <SelectValue>
        RULE_NUMBERLIST_COMMA                             = 34, // <NumberList> ::= <NumberList> ',' <SelectValue>
        RULE_SELECTVALUE_NUMBER                           = 35, // <SelectValue> ::= Number <ValueDescription>
        RULE_VALUEDESCRIPTION_STRINGLITERAL               = 36, // <ValueDescription> ::= StringLiteral
        RULE_RETURNEXPRESS_RETURNEQ                       = 37, // <ReturnExpress> ::= 'return='
        RULE_RETURNEXPRESS_RETURNEQ2                      = 38, // <ReturnExpress> ::= 'return=' <ExpressList>
        RULE_TESTGROUPEXPRESS_TESTEQ                      = 39, // <TestGroupExpress> ::= 'test='
        RULE_TESTGROUPEXPRESS_TESTEQ2                     = 40, // <TestGroupExpress> ::= 'test=' <TestExpressList>
        RULE_TESTEXPRESSLIST                              = 41, // <TestExpressList> ::= <TestExpress>
        RULE_TESTEXPRESSLIST_COMMA                        = 42, // <TestExpressList> ::= <TestExpressList> ',' <TestExpress>
        RULE_TESTEXPRESS_ATCMD                            = 43, // <TestExpress> ::= '@cmd' <TestItemList>
        RULE_TESTITEMLIST                                 = 44, // <TestItemList> ::= 
        RULE_TESTITEMLIST2                                = 45, // <TestItemList> ::= <TestItem> <TestItemList>
        RULE_TESTITEM                                     = 46, // <TestItem> ::= <TestRangeItem>
        RULE_TESTITEM2                                    = 47, // <TestItem> ::= <TestSelectItem>
        RULE_TESTITEM3                                    = 48, // <TestItem> ::= <TestRepeatItems>
        RULE_TESTREPEATITEMS_LBRACE_RBRACE                = 49, // <TestRepeatItems> ::= <TestItem> '{' <TestItemList> '}'
        RULE_TESTRANGEITEM_IDENTIFIER_LPARAN_MINUS_RPARAN = 50, // <TestRangeItem> ::= Identifier '(' <LValue> '-' <HValue> ')'
        RULE_TESTSELECTITEM_IDENTIFIER_LPARAN_RPARAN      = 51, // <TestSelectItem> ::= Identifier '(' <TestSelectValues> ')'
        RULE_TESTSELECTVALUES                             = 52, // <TestSelectValues> ::= <TestSelectValue>
        RULE_TESTSELECTVALUES_COMMA                       = 53, // <TestSelectValues> ::= <TestSelectValues> ',' <TestSelectValue>
        RULE_TESTSELECTVALUE_NUMBER                       = 54  // <TestSelectValue> ::= Number
    };

    public class MyParser
    {
        protected LALRParser parser;

        public MyParser(string filename)
        {
            FileStream stream = new FileStream(filename,
                                               FileMode.Open, 
                                               FileAccess.Read, 
                                               FileShare.Read);
            Init(stream);
            stream.Close();
        }

        public MyParser(string baseName, string resourceName)
        {
            byte[] buffer = ResourceUtil.GetByteArrayResource(
                System.Reflection.Assembly.GetExecutingAssembly(),
                baseName,
                resourceName);
            MemoryStream stream = new MemoryStream(buffer);
            Init(stream);
            stream.Close();
        }

        public MyParser(Stream stream)
        {
            Init(stream);
        }

        private void Init(Stream stream)
        {
            CGTReader reader = new CGTReader(stream);
            parser = reader.CreateNewParser();
            parser.TrimReductions = false;
            parser.StoreTokens = LALRParser.StoreTokensMode.NoUserObject;

            parser.OnReduce += new LALRParser.ReduceHandler(ReduceEvent);
            parser.OnTokenRead += new LALRParser.TokenReadHandler(TokenReadEvent);
            parser.OnAccept += new LALRParser.AcceptHandler(AcceptEvent);
            parser.OnTokenError += new LALRParser.TokenErrorHandler(TokenErrorEvent);
            parser.OnParseError += new LALRParser.ParseErrorHandler(ParseErrorEvent);
        }

        public void Parse(string source)
        {
            parser.Parse(source);

        }

        private void TokenReadEvent(LALRParser parser, TokenReadEventArgs args)
        {
            try
            {
                args.Token.UserObject = CreateObject(args.Token);
            }
            catch (Exception e)
            {
                args.Continue = false;
                Console.WriteLine(args.Token.ToString());
                //todo: Report message to UI?
            }
        }

        protected virtual Object CreateObject(TerminalToken token)
        {
            switch (token.Symbol.Id)
            {
                case (int)SymbolConstants.SYMBOL_EOF :
                //(EOF)
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_ERROR :
                //(Error)
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_WHITESPACE :
                //(Whitespace)
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_MINUS :
                //'-'
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_LPARAN :
                //'('
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_RPARAN :
                //')'
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_COMMA :
                //','
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_COLON :
                //':'
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_ATCMD :
                //'@cmd'
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_LBRACKETCOMMANDRBRACKET :
                //'[Command]'
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_LBRACKETDEVICEINFORBRACKET :
                //'[DeviceInfo]'
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_LBRACE :
                //'{'
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_RBRACE :
                //'}'
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_CLASSEQ :
                //'class='
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_CMD :
                //Cmd
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_CMDEQ :
                //'cmd='
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_CMDCLASS :
                //CmdClass
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_CMDTYPE :
                //CmdType
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_DESCRIPTIONEQ :
                //'description='
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_DEVICEID :
                //DeviceID
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_DEVICEIDEQ :
                //'DeviceID='
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_DEVICETYPEEQ :
                //'DeviceType='
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_FLOAT :
                //Float
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_FUNC_NAMEEQ :
                //'func_name='
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_HEXDIGITS :
                //HexDigits
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_IDENTIFIER :
                //Identifier
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_IP :
                //IP
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_IPEQ :
                //'IP='
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_NUMBER :
                //Number
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_PORTEQ :
                //'Port='
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_RETURNEQ :
                //'return='
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_SENDEQ :
                //'send='
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_STRINGLITERAL :
                //StringLiteral
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_TESTEQ :
                //'test='
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_TYPEEQ :
                //'type='
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_VERSIONEQ :
                //'Version='
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_BYTES :
                //<Bytes>
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_CMD2 :
                //<Cmd>
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_CMDCLASS2 :
                //<CmdClass>
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_CMDTITLE :
                //<CmdTitle>
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_CMDTYPE2 :
                //<CmdType>
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_COMMAND :
                //<Command>
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_COMMANDLIST :
                //<CommandList>
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_DESCRIPTION :
                //<Description>
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_DEVICEID2 :
                //<DeviceId>
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_DEVICEINFO :
                //<DeviceInfo>
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_DEVICETYPE :
                //<DeviceType>
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_EXPRESSITEM :
                //<ExpressItem>
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_EXPRESSLIST :
                //<ExpressList>
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_FUNCNAME :
                //<FuncName>
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_HVALUE :
                //<HValue>
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_IP2 :
                //<IP>
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_LVALUE :
                //<LValue>
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_NUMBERLIST :
                //<NumberList>
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_PORT :
                //<Port>
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_PROGRAM :
                //<Program>
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_RANGEITEM :
                //<RangeItem>
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_RANGEVALUE :
                //<RangeValue>
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_REPEATEXPRESS :
                //<RepeatExpress>
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_RETURNEXPRESS :
                //<ReturnExpress>
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_SELECTITEM :
                //<SelectItem>
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_SELECTVALUE :
                //<SelectValue>
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_SELECTVALUES :
                //<SelectValues>
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_SENDEXPRESS :
                //<SendExpress>
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_SUBCMD :
                //<SubCmd>
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_TESTEXPRESS :
                //<TestExpress>
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_TESTEXPRESSLIST :
                //<TestExpressList>
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_TESTGROUPEXPRESS :
                //<TestGroupExpress>
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_TESTITEM :
                //<TestItem>
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_TESTITEMLIST :
                //<TestItemList>
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_TESTRANGEITEM :
                //<TestRangeItem>
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_TESTREPEATITEMS :
                //<TestRepeatItems>
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_TESTSELECTITEM :
                //<TestSelectItem>
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_TESTSELECTVALUE :
                //<TestSelectValue>
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_TESTSELECTVALUES :
                //<TestSelectValues>
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_VALUEDESCRIPTION :
                //<ValueDescription>
                //todo: Create a new object that corresponds to the symbol
                return null;

                case (int)SymbolConstants.SYMBOL_VERSION :
                //<Version>
                //todo: Create a new object that corresponds to the symbol
                return null;

            }
            throw new SymbolException("Unknown symbol");
        }

        private void ReduceEvent(LALRParser parser, ReduceEventArgs args)
        {
            try
            {
                args.Token.UserObject = CreateObject(args.Token);
                //Console.WriteLine(args.Token);
            }
            catch (Exception e)
            {
                args.Continue = false;
                Console.WriteLine(args.Token.ToString());
                //todo: Report message to UI?
            }
        }

        protected virtual Object CreateObject(NonterminalToken token)
        {
            switch (token.Rule.Id)
            {
                case (int)RuleConstants.RULE_PROGRAM :
                //<Program> ::= <DeviceInfo> <CommandList>
                //todo: Create a new object using the stored user objects.
                return null;

                case (int)RuleConstants.RULE_DEVICEINFO_LBRACKETDEVICEINFORBRACKET :
                //<DeviceInfo> ::= '[DeviceInfo]' <Version> <DeviceType> <IP> <Port> <DeviceId>
                //todo: Create a new object using the stored user objects.
                return null;

                case (int)RuleConstants.RULE_VERSION_VERSIONEQ_FLOAT :
                //<Version> ::= 'Version=' Float
                //todo: Create a new object using the stored user objects.
                return null;

                case (int)RuleConstants.RULE_DEVICETYPE_DEVICETYPEEQ_IDENTIFIER :
                //<DeviceType> ::= 'DeviceType=' Identifier
                //todo: Create a new object using the stored user objects.
                return null;

                case (int)RuleConstants.RULE_DEVICEID_DEVICEIDEQ_DEVICEID :
                //<DeviceId> ::= 'DeviceID=' DeviceID
                //todo: Create a new object using the stored user objects.
                return null;

                case (int)RuleConstants.RULE_IP_IPEQ_IP :
                //<IP> ::= 'IP=' IP
                //todo: Create a new object using the stored user objects.
                return null;

                case (int)RuleConstants.RULE_CMDCLASS_CLASSEQ_CMDCLASS :
                //<CmdClass> ::= 'class=' CmdClass
                //todo: Create a new object using the stored user objects.
                return null;

                case (int)RuleConstants.RULE_PORT_PORTEQ_NUMBER :
                //<Port> ::= 'Port=' Number
                //todo: Create a new object using the stored user objects.
                return null;

                case (int)RuleConstants.RULE_CMD_CMDEQ_CMD :
                //<Cmd> ::= 'cmd=' Cmd <SubCmd>
                //todo: Create a new object using the stored user objects.
                return null;

                case (int)RuleConstants.RULE_SUBCMD :
                //<SubCmd> ::= 
                //todo: Create a new object using the stored user objects.
                return null;

                case (int)RuleConstants.RULE_SUBCMD_CMD :
                //<SubCmd> ::= Cmd
                //todo: Create a new object using the stored user objects.
                return null;

                case (int)RuleConstants.RULE_DESCRIPTION_DESCRIPTIONEQ_STRINGLITERAL :
                //<Description> ::= 'description=' StringLiteral
                //todo: Create a new object using the stored user objects.
                return null;

                case (int)RuleConstants.RULE_CMDTYPE_TYPEEQ_CMDTYPE :
                //<CmdType> ::= 'type=' CmdType
                //todo: Create a new object using the stored user objects.
                return null;

                case (int)RuleConstants.RULE_CMDTITLE_LBRACKETCOMMANDRBRACKET :
                //<CmdTitle> ::= '[Command]'
                //todo: Create a new object using the stored user objects.
                return null;

                case (int)RuleConstants.RULE_FUNCNAME_FUNC_NAMEEQ_STRINGLITERAL :
                //<FuncName> ::= 'func_name=' StringLiteral
                //todo: Create a new object using the stored user objects.
                return null;

                case (int)RuleConstants.RULE_COMMANDLIST :
                //<CommandList> ::= 
                //todo: Create a new object using the stored user objects.
                return null;

                case (int)RuleConstants.RULE_COMMANDLIST2 :
                //<CommandList> ::= <Command> <CommandList>
                //todo: Create a new object using the stored user objects.
                return null;

                case (int)RuleConstants.RULE_COMMAND :
                //<Command> ::= <CmdTitle> <Cmd> <Description> <CmdClass> <FuncName> <CmdType> <SendExpress> <ReturnExpress> <TestGroupExpress>
                //todo: Create a new object using the stored user objects.
                return null;

                case (int)RuleConstants.RULE_SENDEXPRESS_SENDEQ :
                //<SendExpress> ::= 'send='
                //todo: Create a new object using the stored user objects.
                return null;

                case (int)RuleConstants.RULE_SENDEXPRESS_SENDEQ2 :
                //<SendExpress> ::= 'send=' <ExpressList>
                //todo: Create a new object using the stored user objects.
                return null;

                case (int)RuleConstants.RULE_EXPRESSLIST :
                //<ExpressList> ::= <ExpressItem>
                //todo: Create a new object using the stored user objects.
                return null;

                case (int)RuleConstants.RULE_EXPRESSLIST2 :
                //<ExpressList> ::= <ExpressItem> <ExpressList>
                //todo: Create a new object using the stored user objects.
                return null;

                case (int)RuleConstants.RULE_REPEATEXPRESS_LBRACE_RBRACE :
                //<RepeatExpress> ::= <ExpressItem> '{' <ExpressList> '}'
                //todo: Create a new object using the stored user objects.
                return null;

                case (int)RuleConstants.RULE_EXPRESSITEM :
                //<ExpressItem> ::= <RangeItem>
                //todo: Create a new object using the stored user objects.
                return null;

                case (int)RuleConstants.RULE_EXPRESSITEM2 :
                //<ExpressItem> ::= <SelectItem>
                //todo: Create a new object using the stored user objects.
                return null;

                case (int)RuleConstants.RULE_EXPRESSITEM3 :
                //<ExpressItem> ::= <RepeatExpress>
                //todo: Create a new object using the stored user objects.
                return null;

                case (int)RuleConstants.RULE_RANGEITEM_IDENTIFIER_LPARAN_RPARAN :
                //<RangeItem> ::= Identifier '(' <RangeValue> ')'
                //todo: Create a new object using the stored user objects.
                return null;

                case (int)RuleConstants.RULE_RANGEVALUE_COLON_MINUS :
                //<RangeValue> ::= <Bytes> ':' <LValue> '-' <HValue>
                //todo: Create a new object using the stored user objects.
                return null;

                case (int)RuleConstants.RULE_LVALUE_NUMBER :
                //<LValue> ::= Number
                //todo: Create a new object using the stored user objects.
                return null;

                case (int)RuleConstants.RULE_HVALUE_NUMBER :
                //<HValue> ::= Number
                //todo: Create a new object using the stored user objects.
                return null;

                case (int)RuleConstants.RULE_BYTES_NUMBER :
                //<Bytes> ::= Number
                //todo: Create a new object using the stored user objects.
                return null;

                case (int)RuleConstants.RULE_SELECTITEM_IDENTIFIER_LPARAN_RPARAN :
                //<SelectItem> ::= Identifier '(' <SelectValues> ')'
                //todo: Create a new object using the stored user objects.
                return null;

                case (int)RuleConstants.RULE_SELECTVALUES_COLON :
                //<SelectValues> ::= <Bytes> ':' <NumberList>
                //todo: Create a new object using the stored user objects.
                return null;

                case (int)RuleConstants.RULE_NUMBERLIST :
                //<NumberList> ::= <SelectValue>
                //todo: Create a new object using the stored user objects.
                return null;

                case (int)RuleConstants.RULE_NUMBERLIST_COMMA :
                //<NumberList> ::= <NumberList> ',' <SelectValue>
                //todo: Create a new object using the stored user objects.
                return null;

                case (int)RuleConstants.RULE_SELECTVALUE_NUMBER :
                //<SelectValue> ::= Number <ValueDescription>
                //todo: Create a new object using the stored user objects.
                return null;

                case (int)RuleConstants.RULE_VALUEDESCRIPTION_STRINGLITERAL :
                //<ValueDescription> ::= StringLiteral
                //todo: Create a new object using the stored user objects.
                return null;

                case (int)RuleConstants.RULE_RETURNEXPRESS_RETURNEQ :
                //<ReturnExpress> ::= 'return='
                //todo: Create a new object using the stored user objects.
                return null;

                case (int)RuleConstants.RULE_RETURNEXPRESS_RETURNEQ2 :
                //<ReturnExpress> ::= 'return=' <ExpressList>
                //todo: Create a new object using the stored user objects.
                return null;

                case (int)RuleConstants.RULE_TESTGROUPEXPRESS_TESTEQ :
                //<TestGroupExpress> ::= 'test='
                //todo: Create a new object using the stored user objects.
                return null;

                case (int)RuleConstants.RULE_TESTGROUPEXPRESS_TESTEQ2 :
                //<TestGroupExpress> ::= 'test=' <TestExpressList>
                //todo: Create a new object using the stored user objects.
                return null;

                case (int)RuleConstants.RULE_TESTEXPRESSLIST :
                //<TestExpressList> ::= <TestExpress>
                //todo: Create a new object using the stored user objects.
                return null;

                case (int)RuleConstants.RULE_TESTEXPRESSLIST_COMMA :
                //<TestExpressList> ::= <TestExpressList> ',' <TestExpress>
                //todo: Create a new object using the stored user objects.
                return null;

                case (int)RuleConstants.RULE_TESTEXPRESS_ATCMD :
                //<TestExpress> ::= '@cmd' <TestItemList>
                //todo: Create a new object using the stored user objects.
                return null;

                case (int)RuleConstants.RULE_TESTITEMLIST :
                //<TestItemList> ::= 
                //todo: Create a new object using the stored user objects.
                return null;

                case (int)RuleConstants.RULE_TESTITEMLIST2 :
                //<TestItemList> ::= <TestItem> <TestItemList>
                //todo: Create a new object using the stored user objects.
                return null;

                case (int)RuleConstants.RULE_TESTITEM :
                //<TestItem> ::= <TestRangeItem>
                //todo: Create a new object using the stored user objects.
                return null;

                case (int)RuleConstants.RULE_TESTITEM2 :
                //<TestItem> ::= <TestSelectItem>
                //todo: Create a new object using the stored user objects.
                return null;

                case (int)RuleConstants.RULE_TESTITEM3 :
                //<TestItem> ::= <TestRepeatItems>
                //todo: Create a new object using the stored user objects.
                return null;

                case (int)RuleConstants.RULE_TESTREPEATITEMS_LBRACE_RBRACE :
                //<TestRepeatItems> ::= <TestItem> '{' <TestItemList> '}'
                //todo: Create a new object using the stored user objects.
                return null;

                case (int)RuleConstants.RULE_TESTRANGEITEM_IDENTIFIER_LPARAN_MINUS_RPARAN :
                //<TestRangeItem> ::= Identifier '(' <LValue> '-' <HValue> ')'
                //todo: Create a new object using the stored user objects.
                return null;

                case (int)RuleConstants.RULE_TESTSELECTITEM_IDENTIFIER_LPARAN_RPARAN :
                //<TestSelectItem> ::= Identifier '(' <TestSelectValues> ')'
                //todo: Create a new object using the stored user objects.
                return null;

                case (int)RuleConstants.RULE_TESTSELECTVALUES :
                //<TestSelectValues> ::= <TestSelectValue>
                //todo: Create a new object using the stored user objects.
                return null;

                case (int)RuleConstants.RULE_TESTSELECTVALUES_COMMA :
                //<TestSelectValues> ::= <TestSelectValues> ',' <TestSelectValue>
                //todo: Create a new object using the stored user objects.
                return null;

                case (int)RuleConstants.RULE_TESTSELECTVALUE_NUMBER :
                //<TestSelectValue> ::= Number
                //todo: Create a new object using the stored user objects.
                return null;

            }
            throw new RuleException("Unknown rule");
        }

        private void AcceptEvent(LALRParser parser, AcceptEventArgs args)
        {
            //todo: Use your fully reduced args.Token.UserObject
        }

        private void TokenErrorEvent(LALRParser parser, TokenErrorEventArgs args)
        {
            string message = "Token error with input: '"+args.Token.ToString()+"'";
            //todo: Report message to UI?
        }

        private void ParseErrorEvent(LALRParser parser, ParseErrorEventArgs args)
        {
            string message = "Parse error caused by token: '"+args.UnexpectedToken.ToString()+"'";
            //todo: Report message to UI?
        }


    }
}
