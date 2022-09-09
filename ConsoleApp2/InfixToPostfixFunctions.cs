using System;
using System.Collections.Immutable;
using System.Reflection;
using System.Text;
using System.Linq;
namespace InfixToPostfix
{
    class ShuntingYard
    {
        public static string InvokeDynamic(string match, out string result, params dynamic[] arguments)
        {

            Type t = typeof(FunctionToken);
            MethodInfo[] mi = t.GetMethods();



            foreach (var method in mi)
            {
                if (method.Name.ToLower() == match.ToLower())
                {
                    result = Convert.ToString(method.Invoke(null,new object[] { arguments }));

                    return result;
                }
            }
            result = null;
            return result;
        }


        public static string InvokeConstant(string match, out string result, double arg1, double arg2)
        {

            Type t = typeof(FunctionToken);
            MethodInfo[] mi = t.GetMethods();



            foreach (var method in mi)
            {
                if (method.Name.ToLower() == match.ToLower())
                {
                    result = Convert.ToString(method.Invoke(null, new object[] { arg1,arg2 }));

                    return result;
                }
            }
            result = null;
            return result;
        }

        Stack<IToken> stack = new Stack<IToken>();
        Queue<IToken> queue = new Queue<IToken>();
        internal string ToPostfix(string infix, out string errors)
        {
            StringBuilder sb = new StringBuilder();
            int funcLength = 1;
            List<string> errors_ = new List<string>();
            foreach (var item in TokenlarıBul(infix))
            {
                if (item is ConstantToken)
                {
                    

                    queue.Enqueue(item);

                }

                else if (item is FunctionToken)
                {

                    stack.Push(item);

                }


                else if (item is CommaToken)
                {

                    funcLength++;
                }

                else if (item is ParanthesesToken)
                {


                    if (item.Text == "(")
                    {
                        stack.Push(item);
                    }
                    else if (item.Text == ")")
                    {

                        while (stack.Count > 0 && stack.Peek().Text != "(")
                        {

                            queue.Enqueue(stack.Pop());


                        }
                        if(stack.Count>0 && stack.Peek().Text == "(")
                        {
                            stack.Pop();

                            if (stack.Count > 0 && stack.Peek() is FunctionToken)
                            {
                                stack.Peek().ToPostfix = $"{stack.Peek().Text+"("+funcLength+")"}";
                                funcLength = 1;
                                queue.Enqueue(stack.Pop());
                            
                            }
                        }

                  


                    }
                }

                else if (item is OperatorToken)
                {
                    OperatorToken x = new OperatorToken(char.Parse(item.Text));


                    if (stack.Count == 0)
                        stack.Push(item);



                    else
                    {

                        if (char.TryParse(stack.Peek().Text, out char result))
                        {
                            OperatorToken sonItem = new OperatorToken(result);
                            while (stack.Count > 0 && stack.Peek().Text != "(" && (sonItem.Oncelik > x.Oncelik || (sonItem.Oncelik == x.Oncelik && x.LeftAssociavity == true)))
                            {
                                queue.Enqueue(stack.Pop());
                            }

                        }

                        //if (x.Text != ",")
                        //{
                            stack.Push(item);
                      //  }
                        //else
                       // {
                         

                          //  funcLength++;
                        //}
                    }


                }

                if (item is InvalidToken)
                    errors_.Add($"Invalid token found: {item.Text}");

            }
            while (stack.Count > 0)
            {
                queue.Enqueue(stack.Pop());



            }

            errors = string.Join(", ", errors_);
            
            while (queue.Count > 0)
            {
            
               
                if(queue.Peek() is FunctionToken)
                {
                    string fncc = queue.Dequeue().ToPostfix;
                    sb.Append(fncc + " ");
                }
                else
                {
                    string eleman = queue.Dequeue().Text;
                    sb.Append(eleman + " ");
                }
            
            }
            return sb.ToString();

        }

        internal string Calculate(string postfix)
        {
            Stack<string> calculationStack = new Stack<string>();

            foreach (var item in TokenlarıBul(postfix))
            {


                if (item is ConstantToken)
                {

                    calculationStack.Push(item.Text);

                }
                else if(item is WhitespaceToken)
                {
                  

                }
            

               else  if (item is OperatorToken)
                {
                   
                    OperatorToken token = new OperatorToken(char.Parse(item.Text));

                    double val1 = double.Parse(calculationStack.Pop());
                    double val2 = double.Parse(calculationStack.Pop());


                    switch (token.Text)
                    {
                        case "^":
                            calculationStack.Push(Convert.ToString(Math.Pow(val2, val1)));
                            break;
                        case "-":
                            calculationStack.Push(Convert.ToString(val2 - val1));
                            break;
                        case "+":
                            calculationStack.Push(Convert.ToString(val2 + val1));
                            break;
                        case "*":
                            calculationStack.Push(Convert.ToString(val2 * val1));
                            break;
                        case "/":
                            calculationStack.Push(Convert.ToString(val2 / val1));
                            break;
                        case "%":
                            calculationStack.Push(Convert.ToString(val2 % val1));
                            break;

                        default:
                            break;

                            
                    }
                }
                
               else if( item is FunctionToken)
                {
                    StringBuilder sb = new StringBuilder();
                    int numberOfArguments = 0;
                    foreach (var character in item.Text)
                    {
                        if (char.IsLetter(character))
                        {
                            sb.Append(character);
                        }
                        else if (char.IsDigit(character))
                        {

                            numberOfArguments = (int)char.GetNumericValue(character);
                        }
                    }


                    if (item.Text.Contains("sum"))
                    {
                       


                        dynamic[] primeNumbers = new dynamic[numberOfArguments];


                        for (int i = numberOfArguments - 1; i >= 0; i--)
                        {
                            primeNumbers[i] = double.Parse(calculationStack.Pop());
                        }
                        InvokeDynamic(sb.ToString(), out string res, primeNumbers);

                        calculationStack.Push(res);
                    }
                    else
                    {
                    
                       
                            double val1 = double.Parse(calculationStack.Pop());
                            double val2 = double.Parse(calculationStack.Pop());
                            InvokeConstant(sb.ToString(), out string res, val1, val2);
                            calculationStack.Push(res);
                       
                    }

                      
                    

                  





                }

            }
            string result = calculationStack.Pop();


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

                else if (S.TryReadNumber(out string numberText))
                    yield return new ConstantToken() { Text = numberText, Value = Convert.ToDouble(numberText) };
                else if (S.TryReadPostfixFunctions(out string postfixFunction))
                    yield return new FunctionToken(postfixFunction);
                else if (S.TryReadFunctions(out string function))
                    yield return new FunctionToken(function);
                else if (S.TryReadAny("()", out char parantheses))
                    yield return new ParanthesesToken() { Text = Convert.ToString(parantheses) };
                else if (S.TryRead(",", out string comma))
                    yield return new CommaToken() { Text =comma};
                else if (S.TryReadAny("+-/*^%", out char @operator))
                    yield return new OperatorToken(@operator);
                else
                    yield return new InvalidToken() { Text = S.Read().ToString() };
            }
        }
    }

    interface IToken
    {
        string Text { get; }
        string ToPostfix { get; set; }

        TokenType tokenType { get; set; }
    }

    enum TokenType
    {
        Constant,
        Operator,
        Function,
        Parantheses,
        WhiteSpace,
        Comma,
        Invalid,


    }

    class OperatorToken : IToken
    {
        public string Text { get; }
        public int Oncelik { get; set; }
        public bool LeftAssociavity { get; set; }
        public string ToPostfix { get;  set ; }

        public TokenType tokenType { get; set; } =TokenType.Operator;

        public OperatorToken(char @operator)
        {
            Text = @operator.ToString();
            ToPostfix= @operator.ToString();
            switch (@operator)
            {
                //case ',': Oncelik = 1; LeftAssociavity = true   ;      break;
                case '+': Oncelik = 2; LeftAssociavity = true   ;      break;
                case '-': Oncelik = 2; LeftAssociavity = true   ;      break;
                case '*': Oncelik = 3; LeftAssociavity = true   ;      break;
                case '/': Oncelik = 3; LeftAssociavity = true   ;      break;
                case '%': Oncelik = 3; LeftAssociavity = true   ;      break;
                case '^': Oncelik = 4; LeftAssociavity = false  ;      break;

            }
        }
        public override string ToString()
        {
            return $"Operator: {Text}, Oncelik: {Oncelik}, Left Associavity: {LeftAssociavity}";
        }
    }
    class FunctionToken : IToken
    {
        public string Text { get; set; }
        public int NumberOfArguments { get; set; }
        public string ToPostfix { get; set; }
        public TokenType tokenType { get; set; } = TokenType.Function;

        public FunctionToken(string @function)
        {
            Text = @function;
            ToPostfix = @function;
        }
        public override string ToString()
        {
            return $"Function Name : {Text}";
        }


        public static double Pow(double val1, double val2)
        {
            return Math.Pow(val1, val2);
        }

        public static dynamic Sum(params dynamic[] sayılar)
        {
            dynamic result = 0;
            foreach (var item in sayılar)
            {
                {
                    result += item;
                }
            }
            return result;
            
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

    class ParanthesesToken : IToken
    {
        public string Text { get; set; }
        public string ToPostfix { get; set; }
        public TokenType tokenType { get; set; } = TokenType.Parantheses;

        public override string ToString()
        {
            return $"Parantheses : {Text}";
        }

    }
    class ConstantToken : IToken
    {
        public string Text { get; set; }
        public double Value { get; set; }
        public string ToPostfix { get; set; }
        public TokenType tokenType { get; set; } = TokenType.Constant;

        public override string ToString()
        {
            return $"Constant: {Text}, Value: {Value}";
        }
    }

    class CommaToken : IToken
    {
        public string Text { get; set; }
        public string ToPostfix { get; set; }
        public TokenType tokenType { get; set; } = TokenType.Comma;

        public override string ToString()
        {
            return $"Comma : {Text}";
        }

    }
    class WhitespaceToken : IToken
    {
        public string Text { get; set; }
        public string ToPostfix { get; set; }
        public TokenType tokenType { get; set; } = TokenType.WhiteSpace;

        public override string ToString()
        {
            return $"Whitespace: {Text.Length}";
        }
    }
    class InvalidToken : IToken
    {
        public string Text { get; set; }
        public string ToPostfix { get; set; }
        public TokenType tokenType { get; set; } = TokenType.Invalid;

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
            Func<char, bool> karakerKontrolu = c => c == '.' || char.IsDigit(c);

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


        public bool TryReadFunctions(out string? function)
        {
            int currentIndex = Index;
            Type t = typeof(FunctionToken);
            MethodInfo[] mi = t.GetMethods();

            if (TryReadIdentifier(out string id))
            {

                foreach (var item in mi)
                {
                    if (item.Name.Equals(id, StringComparison.OrdinalIgnoreCase))
                    {
                        function = id;
                        return true;

                    }
                }
            
            }

            Index = currentIndex;
            function = "";
            return false;
        }


        public bool TryReadPostfixFunctions(out string? function)
        {
            int currentIndex = Index;
            Type t = typeof(FunctionToken);
            MethodInfo[] mi = t.GetMethods();

            if (TryReadIdentifier(out string id) && (TryRead("(", out string OP) && TryReadNumber(out string argumentCount) && TryRead(")", out string CP)))
            {

                foreach (var item in mi)
                {
                    if (item.Name.Equals(id, StringComparison.OrdinalIgnoreCase))
                    {
                        function = id+OP+argumentCount+CP;
                        return true;

                    }
                }

            }

            Index = currentIndex;
            function = "";
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
