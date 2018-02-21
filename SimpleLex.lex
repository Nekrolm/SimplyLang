%using SimpleParser;
%using QUT.Gppg;
%using System.Linq;

%namespace SimpleScanner

Alpha 	[a-zA-Z_]
Digit   [0-9] 
AlphaDigit {Alpha}|{Digit}
INTNUM  {Digit}+
REALNUM {INTNUM}\.{INTNUM}
ID {Alpha}{AlphaDigit}* 

%%

{INTNUM} { 
  yylval.intT = int.Parse(yytext);
  return (int)Tokens.INUM; 
}

{REALNUM} { 
  yylval.doubleT = double.Parse(yytext);
  return (int)Tokens.RNUM;
}

{ID}  { 
  int res = ScannerHelper.GetIDToken(yytext);
  if (res == (int)Tokens.ID){
  	yylval.stringT = yytext; 
  }
  return res;
}

":=" { return (int)Tokens.ASSIGN; }
";"  { return (int)Tokens.SEMICOLON; }
"+"  { return '+'; }
"-"  { return '-'; }
"*"  { return '*'; }
"/"  { return '/'; }
"("  { return '('; }
")"  { return ')'; }
"==" { return (int)Tokens.EQUALS; }
"<"  { return '<';}
">"  { return '>';}
">=" { return (int)Tokens.GE; }
"<=" { return (int)Tokens.LE; }
","  { return ','; }
"!=" { return (int)Tokens.UNEQUALS; }
 
[^ \r\n] {
	LexError();
	return (int)Tokens.EOF; // конец разбора
}

%{
  yylloc = new LexLocation(tokLin, tokCol, tokELin, tokECol); // позиция символа (терминального или нетерминального), возвращаемая @1 @2 и т.д.
%}

%%

public override void yyerror(string format, params object[] args) // обработка синтаксических ошибок
{
  var ww = args.Skip(1).Cast<string>().ToArray();
  string errorMsg = string.Format("({0},{1}): found {2}, expected {3}", yyline, yycol, args[0], string.Join(" or ", ww));
  throw new SyntaxException(errorMsg);
}

public void LexError()
{
	string errorMsg = string.Format("({0},{1}): unknown symbol {2}", yyline, yycol, yytext);
    throw new LexException(errorMsg);
}

class ScannerHelper 
{
  private static Dictionary<string,int> keywords;

  static ScannerHelper() 
  {
    keywords = new Dictionary<string,int>();
    keywords.Add("begin",(int)Tokens.BEGIN);
    keywords.Add("end",(int)Tokens.END);
    keywords.Add("while",(int)Tokens.CYCLE);
    keywords.Add("if", (int)Tokens.IF);
    keywords.Add("else", (int)Tokens.ELSE); 
    keywords.Add("or", (int)Tokens.OR);
    keywords.Add("and", (int)Tokens.AND);
    keywords.Add("not", (int)Tokens.NOT);
    keywords.Add("print", (int)Tokens.PRINT);
    keywords.Add("for", (int)Tokens.FOR);
    keywords.Add("in", (int)Tokens.IN);
    
  }

  public static int GetIDToken(string s)
  {
    if (keywords.ContainsKey(s.ToLower())) // язык нечувствителен к регистру
      return keywords[s];
    else
      return (int)Tokens.ID;
  }
}
