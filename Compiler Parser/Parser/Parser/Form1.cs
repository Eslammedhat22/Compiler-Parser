using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;


namespace Parser
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        OpenFileDialog ofd = new OpenFileDialog();
        private void Browse_Click(object sender, EventArgs e)
        {
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                filePath.Text = Global.path = ofd.FileName;

            }
        }
        
        private void Run_Click(object sender, EventArgs e)
        {
            if (!File.Exists(filePath.Text) | Path.GetExtension(filePath.Text) != ".txt") error.Visible = true;
            else
            {
                error.Visible = false;
                Global.path = filePath.Text;
                Global.Tokens.Clear();
                scanner.scanFile();
                while (!Global.done)
                {
                    scanner.getNextToken();
                    if (!Global.done)
                    {
                        Global.Tokens.Add(new Token(Global.tokenType, Global.tokenValue));
                    }
                }
                Global.done = false;
                Global.nextToken = 0;
                Global.code = "";
                Global.ID = 0;
                Global.Nodes.Clear();
                parser x = new parser();
                x.stmt_sequence(0, -1);
                x.correct_no_of_childerens();
                /*****mn awl hna a3ml ely y3gbk ya medhat *******/
                Form f2 = new Form2();
                f2.Show();
            }
        }
    }

    public class scanner
    {
        public static void getNextToken()
        {

            int currentState = 0;
            /////////////////////////////////////////////////////////////////////////////////////
            while (Global.code.Length > Global.nextToken)
            {
                char c = Global.code[Global.nextToken];
                if (currentState == 0) // start state // this function does not handle any error at the beginning of the Token //
                {
                    if (char.IsDigit(c)) { currentState = 2; Global.tokenValue = c.ToString(); Global.tokenType = "Const"; }
                    else if (char.IsLetter(c)) { currentState = 3; Global.tokenValue = c.ToString(); Global.tokenType = "Id"; }
                    else if (c == ':') { currentState = 4; Global.tokenValue = c.ToString(); Global.tokenType = "Special Symbol"; }
                    else if (c == '+' | c == '-' | c == '*' | c == '/' | c == '<' | c == ';' | c == '=' | c == '(' | c == ')') { Global.tokenValue = c.ToString(); Global.tokenType = "Special Symbol"; Global.nextToken++; return; }
                    else if (c == '{') { currentState = 1; }

                }

                else if (currentState == 1) // Comment
                {
                    if (c == '}') currentState = 0;
                }
                else if (currentState == 2) // integer number
                {
                    if (char.IsDigit(c)) Global.tokenValue += c.ToString();
                    else if (c == '.')
                    {
                        Global.tokenValue += c.ToString();
                        currentState = 5;
                    }
                    else return;
                }
                else if (currentState == 3) // Identifier
                {
                    if (char.IsDigit(c) | char.IsLetter(c) | c == '_') Global.tokenValue += c.ToString();
                    else
                    {
                        if (Global.tokenValue == "if" | Global.tokenValue == "then" | Global.tokenValue == "else" | Global.tokenValue == "end" | Global.tokenValue == "repeat" | Global.tokenValue == "until" | Global.tokenValue == "read" | Global.tokenValue == "write") Global.tokenType = "Reserved Words"; // IMPLEMENT THE RESERVED WORDS
                        return;
                    }
                }
                else if (currentState == 4) // Assignment
                {
                    if (c == '=') { Global.tokenValue += c.ToString(); Global.nextToken++; return; }
                    else currentState = 0; // when he finds ':'
                }
                else if (currentState == 5) // Decimal point
                {
                    if (char.IsDigit(c)) { Global.tokenValue += c.ToString(); currentState = 6; }
                    else currentState = 0;

                }
                else if (currentState == 6) // Decimal number
                {
                    if (char.IsDigit(c)) { Global.tokenValue += c.ToString(); }
                    else return;
                }

                Global.nextToken++;
            }

            if (Global.code.Length <= Global.nextToken)
            {
                Global.done = true;
            }
            else
            {
                Global.done = false;
            }


        }
        public static void scanFile()
        {
            Global.code = System.IO.File.ReadAllText(Global.path) + " ";
            Global.nextToken = 0;
            Global.done = false;
        }
    }
    static class Global
    {
        public static int nextToken = 0;// pointer to the next token
        public static string code = ""; // line read to scan
        public static string path = ""; //path of the file
        public static string tokenValue = ""; // contain the token value to be displayed
        public static string tokenType = ""; // contain the token type to be displayed
        public static bool done = false;
        public static List<Token> Tokens = new List<Token>(50);
        public static List<node> Nodes = new List<node>(50);

        public static int ID = 0;
    }
    public class Token
    {
        public string Type;
        public string Value;
        public Token(string type, string value)
        {
            Type = type;
            Value = value;
        }
    }
    public class parser
    {

        public void stmt_sequence(int level, int id)
        {
            int i = Global.Nodes.Count;
            int m;
            statement(level, id);
            while ((Global.nextToken < Global.Tokens.Count) && (Global.Tokens[Global.nextToken].Value == ";"))
            {
                m = Global.Nodes.Count;
                Global.nextToken++;
                statement(level, i);
                i = m;
            }
        }
        void statement(int level, int id)
        {
            if ((Global.nextToken < Global.Tokens.Count) && (Global.Tokens[Global.nextToken].Value == "if")) if_stmt(level, id);
            else if ((Global.nextToken < Global.Tokens.Count) && (Global.Tokens[Global.nextToken].Value == "repeat")) repeat_stmt(level, id);
            else if ((Global.nextToken < Global.Tokens.Count) && (Global.Tokens[Global.nextToken].Type == "Id")) assign_stmt(level, id);
            else if ((Global.nextToken < Global.Tokens.Count) && (Global.Tokens[Global.nextToken].Value == "read")) read_stmt(level, id);
            else if ((Global.nextToken < Global.Tokens.Count) && (Global.Tokens[Global.nextToken].Value == "write")) write_stmt(level, id);
            else
            { // error
            }

        }
        void if_stmt(int level, int id)
        {
            Global.Nodes.Add(new node(Global.Tokens[Global.nextToken].Value, level, id, false, 2));
            Global.nextToken++;
            int i = Global.ID - 1;
            exp(level + 1, i);
            Global.nextToken++;
            stmt_sequence(level + 1, i);
            if (Global.Tokens[Global.nextToken].Value == "else")
            {
                Global.Nodes[i].ChildernsNo++;
                Global.nextToken++;
                stmt_sequence(level + 1, i);
            }
            Global.nextToken++;

        }
        void repeat_stmt(int level, int id)
        {
            Global.Nodes.Add(new node(Global.Tokens[Global.nextToken].Value, level, id, false, 2));
            Global.nextToken++;
            int i = Global.ID - 1;
            stmt_sequence(level + 1, i);
            Global.nextToken++;
            exp(level + 1, i);
        }
        void assign_stmt(int level, int id)
        {
            Global.Nodes.Add(new node("assign<" + Global.Tokens[Global.nextToken].Value + ">", level, id, false, 1));
            Global.nextToken += 2;
            exp(level + 1, Global.ID - 1);
        }
        void read_stmt(int level, int id)
        {
            Global.nextToken++;
            Global.Nodes.Add(new node("read<" + Global.Tokens[Global.nextToken].Value + ">", level, id, false));
            Global.nextToken++;

        }
        void write_stmt(int level, int id)
        {
            Global.Nodes.Add(new node("write", level, id, false, 1));
            Global.nextToken++;
            exp(level + 1, Global.ID - 1);
        }
        public void exp(int level, int id)
        {
            node temp = new node("", true);
            temp = simple_exp();
            if ((Global.nextToken < Global.Tokens.Count) && ((Global.Tokens[Global.nextToken].Value == "<") || (Global.Tokens[Global.nextToken].Value == "=") || (Global.Tokens[Global.nextToken].Value == ">")))
            {
                Global.Nodes.Add(new node("Op(" + Global.Tokens[Global.nextToken].Value + ")", level, id, true, 2));
                int j = Global.ID - 1;
                expLevelHandle(temp, level + 1, j);
                Global.nextToken++;
                temp = simple_exp();
                expLevelHandle(temp, level + 1, j);
            }
            else
            {
                expLevelHandle(temp, level, id);
            }
        }
        void expLevelHandle(node T, int level, int id)
        {
            Global.Nodes.Add(new node(T.Text, level, id, T.Circle, T.childerns.Count));
            int i = Global.ID - 1;
            if (Global.Nodes[Global.Nodes.Count - 1].ChildernsNo != 0)
            {
                node left = new node("", true);
                node right = new node("", true);
                left = T.childerns[0];
                right = T.childerns[1];
                expLevelHandle(left, level + 1, i);
                expLevelHandle(right, level + 1, i);
            }

        }
        node simple_exp()
        {
            node temp = new node("", true);

            temp = term();
            while ((Global.nextToken < Global.Tokens.Count) && ((Global.Tokens[Global.nextToken].Value == "-") || (Global.Tokens[Global.nextToken].Value == "+")))
            {
                node newTemp = new node("", true);
                newTemp.Text = "Op(" + Global.Tokens[Global.nextToken].Value + ")";
                Global.nextToken++;
                newTemp.childerns.Add(temp);
                newTemp.childerns.Add(term());
                temp = newTemp;

            }
            return temp;
        }
        node term()
        {
            node temp = new node("", true);

            temp = factor();
            while ((Global.nextToken < Global.Tokens.Count) && ((Global.Tokens[Global.nextToken].Value == "*") || (Global.Tokens[Global.nextToken].Value == "/")))
            {
                node newTemp = new node("", true);
                newTemp.Text = "Op(" + Global.Tokens[Global.nextToken].Value + ")";
                Global.nextToken++;
                newTemp.childerns.Add(temp);
                newTemp.childerns.Add(factor());
                temp = newTemp;


            }
            return temp;

        }
        node factor()
        {
            node temp = new node("", true);
            if ((Global.nextToken < Global.Tokens.Count) && (Global.Tokens[Global.nextToken].Value == "("))
            {
                Global.nextToken++;
                temp = simple_exp();
                Global.nextToken++;
            }
            else
            {
                temp = new node(Global.Tokens[Global.nextToken].Type + "<" + Global.Tokens[Global.nextToken].Value + ">", true);
                Global.nextToken++;
            }
            return temp;

        }
        public void correct_no_of_childerens()
        {
            int n = Global.Nodes.Count;
            int count;
            for (int i = 0; i < n; i++)
            {
                count = 0;
                for (int j = 0; j < n; j++)
                {
                    if (Global.Nodes[j].Parent_ID == i) count++;
                }
                Global.Nodes[i].ChildernsNo = count;
            }
        }
    }
    public class node
    {
        public int ID;
        public string Text;
        public int Level;
        public int Parent_ID;
        public bool Circle;
        public List<node> childerns = new List<node>();
        public int ChildernsNo;
        public int localX;
        public int localY;
        public node(string text, int level, int parent_id, bool circle, int childsno = 0)
        {
            ID = Global.ID;
            Text = text;
            Level = level;
            Parent_ID = parent_id;
            Circle = circle;
            ChildernsNo = childsno;
            Global.ID++;
        }
        public node(string text, bool circle)
        {
            Text = text;
            Circle = circle;
            ChildernsNo = 0;
        }


    }
}
