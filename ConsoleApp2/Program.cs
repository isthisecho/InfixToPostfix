using System.Reflection;
namespace InfixToPostfix
{
    internal class Program
    {

        static void Main(string[] args)
        {
            bool run = true;        
            while (run)
            {
                Console.Write(" > ");


                string infix = Console.ReadLine();
             

                switch (infix.ToLowerInvariant())
                {
                    case "":break;
                    case "exit": run = false; break;
                    case "cls" : Console.Clear(); break;
                    default:
                        if (infix.Length > 0)
                        {
                            ShuntingYard shuntingYard = new ShuntingYard();
                                string postfix = shuntingYard.ToPostfix(infix, out string errors);
                                string  result = shuntingYard.Calculate(postfix);
                                        if (!string.IsNullOrEmpty(errors))
                                        {
                                            Console.WriteLine("Hata:");
                                            Console.WriteLine(errors);
                                        }
                                        else
                                        {
                                            if (postfix != null)
                                            {
                                                Console.Write("Postfix :  ");
                                                Console.WriteLine(postfix);
                                            }
                                            if (result != null)
                                            {
                                                Console.Write("Result  :  ");
                                                Console.WriteLine(result);

                                            }


                                        }
                            


                          
                        }
                        else
                        {
                            Console.WriteLine("Infix giriniz!");
                        }
                        break;
                }
            }
        }
    }
}