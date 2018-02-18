%{
    public BlockNode root; // Корневой узел синтаксического дерева 
    public Parser(AbstractScanner<ValueType, LexLocation> scanner) : base(scanner) { }
%}

%output = SimpleYacc.cs

%using ProgramTree;

%namespace SimpleParser



%union { 
		  public int intT;
		  public double doubleT;
		  public string stringT;
		  public StatementNode StatementN;
          public AssignNode AssignN;
          public CycleNode CycleN;
          public ExprNode ExprN;
          public IdNode IdN;
          public BinaryNumericOpNode BinaryNumericOpN;
          public BinaryCompareOpNode BinaryCompareOpN; 
          public BinaryBoolOpNode BinaryBoolOpN;
          public BlockNode BlockN; 
          public WriteNode WriteN;
          public IntNumNode IntNumN;
          public IfNode IfN;

       }


%token BEGIN END CYCLE ASSIGN SEMICOLON EQUALS IF ELSE AND OR NOT LE GE UNEQUALS PRINT


%token <intT> INUM 
%token <doubleT> RNUM 
%token <stringT> ID

%type <ExprN> ariphm ident mult term expr boolexpr
%type <StatementN> assign statement cycle empty if write
%type <BlockN> stlist block



%%

progr   : block {root = $1;}
		;

stlist	: statement { $$ = new BlockNode($1); }
		| stlist SEMICOLON statement { $1.Add($3); $$ = $1;} 
		;



statement:  block {$$ = $1;}
		| assign {$$ = $1;}
		| cycle {$$ = $1;}
		| if {$$ = $1;}
		| write {$$ = $1;}
		| empty {$$ = $1;}
		;


if  : IF expr statement { $$ = new IfNode($2, $3); }
	| IF expr statement ELSE statement { $$ = new IfNode($2, $3, $5); }
	;



empty  : {$$ = null; }
		;

ident 	: ID { $$ = new IdNode($1); }
		;
	
assign 	: ident ASSIGN expr { $$ = new AssignNode($1 as IdNode, $3); }
		;


expr    : boolexpr
		| NOT boolexpr { $$ = new BinaryBoolOpNode(null, BinaryBoolOpType.Not, $2); }
		| expr OR boolexpr  { $$ = new BinaryBoolOpNode($1, BinaryBoolOpType.Or, $3);  }
		| expr AND boolexpr { $$ = new BinaryBoolOpNode($1, BinaryBoolOpType.And, $3); }
		;

write   : PRINT expr { $$ = new WriteNode($2);}
		;

boolexpr : ariphm
		 |  boolexpr '<' ariphm { $$ = new BinaryCompareOpNode($1, BinaryCompareOpType.Less, $3);}
		 |  boolexpr '>' ariphm { $$ = new BinaryCompareOpNode($1, BinaryCompareOpType.Greater, $3);}
		 |  boolexpr EQUALS ariphm { $$ = new BinaryCompareOpNode($1, BinaryCompareOpType.Equals, $3);}
		 |  boolexpr UNEQUALS ariphm { $$ = new BinaryCompareOpNode($1, BinaryCompareOpType.UnEquals, $3);}
		 |  boolexpr GE ariphm { $$ = new BinaryCompareOpNode($1, BinaryCompareOpType.GreaterOrEquals, $3);}
		 | boolexpr  LE ariphm { $$ = new BinaryCompareOpNode($1, BinaryCompareOpType.LessOrEquals, $3);}
		 ;

ariphm  : mult
		| '+' mult {$$ = $2;}
		| '-' mult {$$ = new BinaryNumericOpNode(null, BinaryNumericOpType.Minus, $2);}
        | ariphm '+' mult { $$ = new BinaryNumericOpNode($1, BinaryNumericOpType.Plus, $3);}
        | ariphm '-' mult { $$ = new BinaryNumericOpNode($1, BinaryNumericOpType.Minus, $3); }
        ;

mult    : term
        | mult '*' term {$$ = new BinaryNumericOpNode($1, BinaryNumericOpType.Multiplies, $3);}
        | mult '/' term {$$ = new BinaryNumericOpNode($1, BinaryNumericOpType.Divides, $3);}
        ;

term    : ident
		| INUM  {$$ = new IntNumNode($1); }
		| '(' expr ')'  { $$ = $2; }
		;
		
block	: BEGIN stlist END { $$ = $2; }
		;

cycle	: CYCLE expr statement {$$ = new CycleNode($2, $3);}
		;
	
%%
