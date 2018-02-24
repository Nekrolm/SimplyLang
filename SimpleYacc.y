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
          public BinaryOpNode BinaryOpN;
          public BlockNode BlockN; 
          public WriteNode WriteN;
          public IntNumNode IntNumN;
          public IfNode IfN;
          public ForCycleNode ForN;

       }


%token BEGIN END CYCLE ASSIGN SEMICOLON EQUALS IF ELSE AND OR NOT LE GE UNEQUALS PRINT FOR IN


%token <intT> INUM 
%token <doubleT> RNUM 
%token <stringT> ID

%type <ExprN> ariphm ident mult term expr boolexpr
%type <StatementN> assign statement cycle empty if write forcycle
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
		| forcycle {$$ = $1;}
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
		| NOT boolexpr { $$ = new BinaryOpNode(null, BinaryOpType.Not, $2); }
		| expr OR boolexpr  { $$ = new BinaryOpNode($1, BinaryOpType.Or, $3);  }
		| expr AND boolexpr { $$ = new BinaryOpNode($1, BinaryOpType.And, $3); }
		;

write   : PRINT expr { $$ = new WriteNode($2);}
		;

boolexpr : ariphm
		 |  boolexpr '<' ariphm { $$ = new BinaryOpNode($1, BinaryOpType.Less, $3);}
		 |  boolexpr '>' ariphm { $$ = new BinaryOpNode($1, BinaryOpType.Greater, $3);}
		 |  boolexpr EQUALS ariphm { $$ = new BinaryOpNode($1, BinaryOpType.Equals, $3);}
		 |  boolexpr UNEQUALS ariphm { $$ = new BinaryOpNode($1, BinaryOpType.UnEquals, $3);}
		 |  boolexpr GE ariphm { $$ = new BinaryOpNode($1, BinaryOpType.GreaterOrEquals, $3);}
		 | boolexpr  LE ariphm { $$ = new BinaryOpNode($1, BinaryOpType.LessOrEquals, $3);}
		 ;

ariphm  : mult
		| '+' mult {$$ = $2;}
		| '-' mult {$$ = new BinaryOpNode(null, BinaryOpType.Minus, $2);}
        | ariphm '+' mult { $$ = new BinaryOpNode($1, BinaryOpType.Plus, $3);}
        | ariphm '-' mult { $$ = new BinaryOpNode($1, BinaryOpType.Minus, $3); }
        ;

mult    : term
        | mult '*' term {$$ = new BinaryOpNode($1, BinaryOpType.Multiplies, $3);}
        | mult '/' term {$$ = new BinaryOpNode($1, BinaryOpType.Divides, $3);}
        ;

term    : ident
		| INUM  {$$ = new IntNumNode($1); }
		| '(' expr ')'  { $$ = $2; }
		;
		
block	: BEGIN stlist END { $$ = $2; }
		;

cycle	: CYCLE expr statement {$$ = new CycleNode($2, $3);}
		;
	
forcycle : FOR ident IN	'(' expr ',' expr ',' expr ')' statement { $$ = new ForCycleNode($2 as IdNode, $5, $7, $9, $11);}
		 ;
	
%%
