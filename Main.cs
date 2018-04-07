using System;
using System.IO;
using System.Collections.Generic;
using SimpleScanner;
using ProgramTree;
using SimpleParser;
using SimpleLang.Visitors;
using ThreeAddr;
using SimpleLang.Optimizations;
using SimpleLang.Utility;
using System.Linq;

namespace SimpleCompiler
{
    public class CapitalizerVisitor : AutoVisitor
    {
        public override void VisitIdNode(IdNode id)
        {
            id.Name = id.Name[0].ToString().ToUpper() + id.Name.Substring(1);
        }
    }

    public class SimpleCompilerMain
    {

        public static void PrintOut(HashSet<int> s)
        {
            Console.WriteLine("Out defs:");
            foreach (int x in s)
                Console.Write($"{x} ");
            Console.WriteLine("\n-------");
        }



        public static void Optimize(List<BaseBlock> codeBlocks)
        {
            Console.WriteLine("Optimize");
            var bboptimizator = new BaseBlockOptimizator();
            bboptimizator.AddOptimization(new NopDeleteOptimization());
            bboptimizator.AddOptimization(new TemporaryExprPropagation());
            bboptimizator.AddOptimization(new ConstantsOptimization());
            bboptimizator.AddOptimization(new AlgebraIdentity());
            bboptimizator.AddOptimization(new ExprCanon());
            bboptimizator.AddOptimization(new IfGotoOptimization());
            bboptimizator.AddOptimization(new CopyPropagationOptimization());
            bboptimizator.AddOptimization(new DeadCodeOptimization());
            bboptimizator.AddOptimization(new CommonSubexpressionOptimization());


            var cboptimizator = new CrossBlocksOptimizator();
            cboptimizator.AddOptimization(new IsNotInitVariable());
            cboptimizator.AddOptimization(new AliveBlocksOptimization());
            cboptimizator.AddOptimization(new CrossBlocksDeadCodeOptimization());
            cboptimizator.AddOptimization(new CrossBlockConstantPropagation());

            while (bboptimizator.Optimize(codeBlocks) || cboptimizator.Optimize(codeBlocks) ) {};



        }

        public static void Compile(BlockNode prog)
        {
            var threeAddressGenerationVisitor = new ThreeAddressGenerationVisitor();
            prog.Visit(threeAddressGenerationVisitor);


            var code = threeAddressGenerationVisitor.Data;
            var codeSz = code.Count;

            var codeBlocks = BaseBlockHelper.GenBaseBlocks(threeAddressGenerationVisitor.Data);
                
            foreach (var block in codeBlocks)
                Console.Write(block);


            codeBlocks = BaseBlockHelper.GenBaseBlocks(code);
            Optimize(codeBlocks);


            foreach (var block in codeBlocks)
                Console.Write(block);
            
        }


        public static void Main(String[] arg)
        {   
            string FileName = "../../a.txt";

            if (arg.Length == 1) FileName = arg[0];

            var varRenamerVisitor = new VariableIdUnificationVisitor();    
                
            try
            {
                string Text = File.ReadAllText(FileName);

                Scanner scanner = new Scanner();
                scanner.SetSource(Text, 0);


                Parser parser = new Parser(scanner);


                var b = parser.Parse();
                if (!b)
                    Console.WriteLine("Ошибка");
                else
                {
                    parser.root.Visit(varRenamerVisitor);
                    Console.WriteLine("Программа распознана");
                    Compile(parser.root);
                }

            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Файл {0} не найден", FileName);
            }
            catch (LexException e)
            {
                Console.WriteLine("Лексическая ошибка. " + e.Message);
            }
            catch (SyntaxException e)
            {
                Console.WriteLine("Синтаксическая ошибка. " + e.Message);
            }
            catch (NotInitVariableException e)
            {
                foreach (var i in e.VarList)
                    Console.WriteLine("Переменная {0} -> {1} не инициализирована", i,  varRenamerVisitor.IDDict.First(a => i == "v" + a.Value).Key);
            }
        }
    }
}
