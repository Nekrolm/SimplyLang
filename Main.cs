using System;
using System.IO;
using System.Collections.Generic;
using SimpleScanner;
using ProgramTree;
using SimpleParser;
using SimpleLang.Visitors;

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
        public static void Main()
        {
            string FileName = "../../a.txt";
            try
            {
                string Text = File.ReadAllText(FileName);

                Scanner scanner = new Scanner();
                scanner.SetSource(Text, 0);

                AssignCountVisitor avis = new AssignCountVisitor();
                PrettyPrintVisitor ppvis = new PrettyPrintVisitor();
                CapitalizerVisitor vc = new CapitalizerVisitor();

                Parser parser = new Parser(scanner);


                var b = parser.Parse();
                if (!b)
                    Console.WriteLine("Ошибка");
                else
                {
                    Console.WriteLine("Программа распознана");


                    Console.WriteLine(parser.root.StList.Count);

                    parser.root.Visit(avis);

                    Console.WriteLine(avis.Count);

                    parser.root.Visit(vc);

                    parser.root.Visit(ppvis);

                    Console.Write(ppvis.Text);
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