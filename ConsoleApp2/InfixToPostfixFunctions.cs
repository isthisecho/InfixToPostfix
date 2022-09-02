﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace InfixToPostfix
{
    class ShuntingYard
    {
        Stack<string> stack = new Stack<string>();
        internal string ToPostfix(string infix, out string errors)
        {


            Stack<string> stack = new Stack<string>();
            StringBuilder sb = new StringBuilder();

            List<string> errors_ = new List<string>();
            foreach (var item in TokenlarıBul(infix))
            {
                if (item is ConstantToken)
                {

                    sb.Append(item.Text);
                    sb.Append(' ');
                }

                if (item is OperatorToken)
                {
                    OperatorToken x = new OperatorToken(char.Parse(item.Text));
                    if (stack.Count == 0)
                        stack.Push(x.Text);
                    else if (x.Text == "(")
                    {
                        stack.Push(x.Text);
                    }
                    else if (x.Text == ")")
                    {
                      while(stack.Count>0 && (stack.Peek() != "("))
                        {
                            sb.Append(stack.Pop());
                            sb.Append(' ');
                        }
                        stack.Pop();
                    }
                    else
                    {
                        OperatorToken sonItem = new OperatorToken(char.Parse(stack.Peek()));

                        while (stack.Count > 0 && sonItem.Oncelik >= x.Oncelik)
                        {

                            sb.Append(stack.Pop());
                            sb.Append(' ');
                        }

                        stack.Push(x.Text);
                    }


                }


                if (item is InvalidToken)
                    errors_.Add($"Invalid token found: {item.Text}");

            }
            while (stack.Count > 0)
            {
                sb.Append(stack.Pop());
                sb.Append(' ');
            }

            errors = string.Join(", ", errors_);
            return sb.ToString();

        }
        internal string Calculate(string postfix)
        {

            foreach (var item in TokenlarıBul(postfix))
            {


                if (item is ConstantToken)
                {

                    stack.Push(item.Text);

                }
                if (item is OperatorToken)
                {
                   
                    OperatorToken token = new OperatorToken(char.Parse(item.Text));

                    double val1 = double.Parse(stack.Pop());
                    double val2 = double.Parse(stack.Pop());


                    switch (token.Text)
                    {
                        case "^":
                            stack.Push(Convert.ToString(Math.Pow(val2, val1)));
                            break;
                        case "-":
                            stack.Push(Convert.ToString(val2 - val1));
                            break;
                        case "+":
                            stack.Push(Convert.ToString(val2 + val1));
                            break;
                        case "*":
                            stack.Push(Convert.ToString(val2 * val1));
                            break;
                        case "/":
                            stack.Push(Convert.ToString(val2 / val1));
                            break;
                        case "%":
                            stack.Push(Convert.ToString(val2 % val1));
                            break;

                        default:
                            break;
                    }
                }

            }
            string result = stack.Pop();


            return result;
        }
        internal IEnumerable<IToken> TokenlarıBul(string infix)
        {
            StringReader S = new StringReader(infix);
            while (!S.IsEnd)
            {
                string ws = S.ReadWhileWhiteSpace();
                if (!string.IsNullOrEmpty(ws))
                    yield return new WhitespaceToken() { Text = ws };
                else if (S.TryReadMethod(out string result))
                {
                    yield return new MethodToken() { Text = result };
                    Console.WriteLine(result);
                }
                else if (S.TryReadNumber(out string numberText))
                    yield return new ConstantToken() { Text = numberText, Value = Convert.ToDouble(numberText) };
                else if (S.TryReadAny("+-/*^%()", out char @operator))
                    yield return new OperatorToken(@operator);
                else
                    yield return new InvalidToken() { Text = S.Read().ToString() };
            }
        }
    }

    interface IToken
    {
        string Text { get; }
    }
    class MethodToken : IToken
    {

        public double Result { get; set; }

        public string Text  {get; set;}

     

        public static double Pow(double val1, double val2)
        {
            return Math.Pow(val1, val2);
        }

        public static double Sum(double val1, double val2)
        {
            return val1 + val2;
        }

        public static double Multiply(double val1, double val2)
        {
            return val1 * val2;
        }

        public static double Divide(double val1, double val2)
        {
            return val1 / val2;
        }

        public static double Remainder(double val1, double val2)
        {
            return val1 % val2;
        }

    }
    class OperatorToken : IToken
    {
        public string Text { get; }
        public int Oncelik { get; set; }
        public OperatorToken(char @operator)
        {
            Text = @operator.ToString();
            switch (@operator)
            {
                case '+': Oncelik = 1; break;
                case '-': Oncelik = 1; break;
                case '*': Oncelik = 2; break;
                case '/': Oncelik = 2; break;
                case '%': Oncelik = 2; break;
                case '^': Oncelik = 3; break;
            }
        }
        public override string ToString()
        {
            return $"Operator: {Text}, Oncelik: {Oncelik}";
        }
    }
    class ConstantToken : IToken
    {
        public string Text { get; set; }
        public double Value { get; set; }
      

        public override string ToString()
        {
            return $"Constant: {Text}, Value: {Value}";
        }
    }
    class WhitespaceToken : IToken
    {
        public string Text { get; set; }
        public override string ToString()
        {
            return $"Whitespace: {Text.Length}";
        }
    }
    class InvalidToken : IToken
    {
        public string Text { get; set; }

        public override string ToString()
        {
            return $"Invalid: {Text}";
        }
    }
    class StringReader
    {
        public string Input { get; }
        public int Index { get; set; }
        public int Length { get; }
        public bool IsEnd => Index >= Length;

        public StringReader(string input)
        {
            Input = input;
            Index = 0;
            Length = input.Length;
        }
        public char Peek()
        {
            return Input[Index];
        }
        public char Peek(int offset)
        {
            return Input.ElementAtOrDefault(Index + offset);
        }
        public char Read()
        {
            if (IsEnd)
                return '\0';
            return Input[Index++];
        }
        public StringReader SkipWhileWhiteSpace()
        {
            while (!IsEnd)
                if (char.IsWhiteSpace(Peek()))
                    Read();
                else
                    break;
            return this;
        }
        public string ReadWhileWhiteSpace()
        {
            StringBuilder sb = new StringBuilder();
            while (!IsEnd)
                if (char.IsWhiteSpace(Peek()))
                    sb.Append(Read());
                else
                    break;
            return sb.ToString();
        }
        public bool ReadWhileTrue(Func<char, bool> fn, out string str)
        {
            StringBuilder sb = new StringBuilder();

            while (!IsEnd)
                if (fn(Peek()))
                    sb.Append(Read());
                else
                    break;

            str = sb.ToString();
            return str.Length > 0;
        }
        public bool TryReadIdentifier(out string identifier)
        {
            Func<char, bool> ilkKarakerKontrolu = c => char.IsLetter(c) || c == '_';
           // Func<char, bool> karakerKontrolu = c => char.IsLetterOrDigit(c) || c == '_';

            Func<char, bool> fn = ilkKarakerKontrolu;

            StringBuilder sb = new StringBuilder();

            while (!IsEnd)
            {
                if (fn(Peek()))
                    sb.Append(Read());
                else
                    break;
           //     fn = karakerKontrolu;
            }

            identifier = sb.ToString();

            return identifier.Length > 0;

        }
        public bool TryReadNumber(out string number)
        {

            Func<char, bool> ilkKarakerKontrolu = c => char.IsDigit(c);
             Func<char, bool> karakerKontrolu = c => c=='.' || char.IsDigit(c);

            Func<char, bool> fn = ilkKarakerKontrolu;

            int currentIndex = Index;
            StringBuilder sb = new StringBuilder();

            while (!IsEnd)
            {
                if (fn(Peek()))
                    sb.Append(Read());
                else
                    break;
                  fn = karakerKontrolu;
            }

            number = sb.ToString();
            return number.Length > 0;
        }
        public bool TryReadString(string quoteType, out string str)
        {
            StringBuilder sb = new StringBuilder();

            if (TryRead(quoteType, out string quoteSymbol))
            {
                sb.Append(quoteSymbol);

                while (!TryRead(quoteType, out string endStr))
                {
                    if (TryRead("\\", out string slash))
                    {
                        sb.Append(slash);
                    }
                    sb.Append(Read());

                }
                sb.Append(quoteSymbol);

            }


            str = sb.ToString();
            return str.Length > 0;
        }
        public bool TryReadComments(string start, string end, out string comment)
        {
            StringBuilder sb = new StringBuilder();

            if (TryRead(start, out string startStr))
            {
                while (!TryRead(end, out string endStr))
                    sb.Append(Read());
            }


            comment = sb.ToString();
            return comment.Length > 0;


        }
        public bool TryReadBooleans(string booleanCheck, out string? boolean)
        {
            int currentIndex = Index;


            if (TryReadIdentifier(out string id))
            {
                if (string.Equals(id, "true", StringComparison.OrdinalIgnoreCase))
                {
                    boolean = id;
                    return true;
                }
                else if (string.Equals(id, "false", StringComparison.OrdinalIgnoreCase))
                {
                    boolean = id;
                    return true;
                }
            }

            Index = currentIndex;
            boolean = null;
            return false;
        }
        public bool TryReadOperators(out string symbol)
        {
            Func<char, bool> operatorKontrol = c => c == '+' || c == '-' || c == '*' || c == '/' || c == '^';
            StringBuilder sb = new StringBuilder();

            while (!IsEnd)
                if (operatorKontrol(Peek()))
                    sb.Append(Read());
                else
                    break;

            symbol = sb.ToString();
            return symbol.Length > 0;
        }
        public bool TryReadAny(string set, out char matched)
        {
            char c = Peek();
            if (set.Contains(c))
            {
                Read();
                matched = c;
                return true;
            }

            matched = default;
            return false;
        }
        public bool TryReadMethod(out string result)
        {
            int currentIndex = Index;
            Type t = typeof(MethodToken);
            MethodInfo[] mi = t.GetMethods();
            if (TryReadIdentifier(out string identifier) && TryRead("(", out string _) &&TryReadNumber(out string val1) && TryRead(",", out string _)&& TryReadNumber(out string val2) && TryRead(")", out string _) && TryRead(";", out string _))
            {

              
              
               
                        foreach (var item in mi)
                        {

                            if (item.Name.ToLower() == identifier.ToLower())
                            {
                               
                                    result = Convert.ToString(item.Invoke(t, new object[] { double.Parse(val1), double.Parse(val2) }));
                                    return true;
                               
                              

                            }
                           
                        }
                        result = $"Hatalı Function Name Call ! {identifier}  ";
                        Index = currentIndex;
                      

            }

            result =null;
            return false;
        }
        public bool Try(char c)
        {
            return Peek() == c;
        }
        public bool Try(string str, out string r, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < str.Length; i++)
                sb.Append(Peek(i));

            r = sb.ToString();
            return string.Equals(str, r, comparison);
        }
        public bool Try(string str, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            return Try(str, out _, comparison);
        }
        public bool TryRead(string str, out string r, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            if (Try(str, out r))
            {
                Index += str.Length;
                return true;
            }
            return false;
        }


    }


}