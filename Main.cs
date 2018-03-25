using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using SimpleScanner;
using ProgramTree;
using SimpleParser;
using SimpleLang.Visitors;
using ThreeAddr;
using SimpleLang.Optimizations;


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


        public static void DefsOptimize(List<BaseBlock> codeBlocks)
        {
            var CFG = new ControlFlowGraph(codeBlocks);


            var (inp, outp) = CFG.GenerateInputOutputReachingDefs(codeBlocks);

            CFG.GenerateInputOutputAvaliableExpr(codeBlocks);
            CFG.GenerateInputOutputActiveDefs(codeBlocks);

            for (int i = 0; i < codeBlocks.Count; ++i)
            {
                Console.Write(codeBlocks[i]);
                //PrintOut(inp[i]);
                //PrintOut(outp[i]);
            }
        }


        public static void Optimize(List<BaseBlock> codeBlocks)
        {
            Console.WriteLine("Optimize");
            var optimizator = new BaseBlockOptimizator();
            optimizator.AddOptimization(new NopDeleteOptimization());
            optimizator.AddOptimization(new ConstantsOptimization());
            optimizator.AddOptimization(new AlgebraIdentity());
            optimizator.AddOptimization(new ExprCanon());
            optimizator.AddOptimization(new IfGotoOptimization());
            optimizator.AddOptimization(new CopyPropagationOptimization());
            optimizator.AddOptimization(new DeadCodeOptimization());
            optimizator.AddOptimization(new CommonSubexpressionOptimization());
            optimizator.Optimize(codeBlocks);
        }

       

        public static void Compile(BlockNode prog)
        {
            var threeAddressGenerationVisitor = new ThreeAddressGenerationVisitor();
            var varRenamerVisitor = new VariableIdUnificationVisitor();    

            prog.Visit(varRenamerVisitor);
            prog.Visit(threeAddressGenerationVisitor);


            var code = threeAddressGenerationVisitor.Data;
            var codeSz = code.Count;


            var codeBlocks = BaseBlockHelper.GenBaseBlocks(threeAddressGenerationVisitor.Data);
                
            foreach (var block in codeBlocks)
                Console.Write(block);


            while (true){
                codeBlocks = BaseBlockHelper.GenBaseBlocks(code);
                Optimize(codeBlocks);
                code = BaseBlockHelper.JoinBaseBlocks(codeBlocks);
                BaseBlockHelper.FixLabelsNumeration(code);
                codeBlocks = BaseBlockHelper.GenBaseBlocks(code);

                var CFG = new ControlFlowGraph(codeBlocks);
                codeBlocks = CFG.GetAliveBlocks();

                code = BaseBlockHelper.JoinBaseBlocks(codeBlocks);
                BaseBlockHelper.FixLabelsNumeration(code);
                codeBlocks = BaseBlockHelper.GenBaseBlocks(code);

                if (code.Count == codeSz) break;
                codeSz = code.Count;
            }


            foreach (var block in codeBlocks)
                Console.Write(block);



            DefsOptimize(codeBlocks);




        }


        public static void Main(String[] arg)
        {   
            string FileName = "../../a.txt";

            if (arg.Length == 1) FileName = arg[0];

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

        }
    }
}
