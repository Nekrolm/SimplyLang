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
        public static void Optimize(List<BaseBlock> codeBlocks)
        {
            Console.WriteLine("Optimize");
            var optimizator = new BaseBlockOptimizator();
            optimizator.AddOptimization(new ConstantsOptimization());
            optimizator.AddOptimization(new IfGotoOptimization());
            optimizator.AddOptimization(new CopyPropagationOptimization());

            optimizator.Optimize(codeBlocks);

        }

        public static void Compile(BlockNode prog)
        {
            var threeAddressGenerationVisitor = new ThreeAddressGenerationVisitor();
            var varRenamerVisitor = new VariableIdUnificationVisitor();    

            prog.Visit(varRenamerVisitor);
            prog.Visit(threeAddressGenerationVisitor);

            var codeBlocks = BaseBlockHelper.GenBaseBlocks(threeAddressGenerationVisitor.Data);



            foreach (var block in codeBlocks)
                Console.Write(block);

            Optimize(codeBlocks);


            foreach (var block in codeBlocks)
                Console.Write(block);


        }


        public static void Main()
        {   
            string FileName = "../../a.txt";
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

            Console.ReadLine();
        }
    }
}