using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Parser
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            this.Width = 20;
            this.Height = 20;
        }
        // Medhat's CODE
        private void Form2_Paint(object sender, PaintEventArgs e)
        {
            int rectHeight = 40;
            int rectWidth = 80;
            int verticalSpace = 60;
            int horizontalSpace = 10;
            int tempX = 60;
            int tempY = 20;
            int ymax = 0;
            Font font1;
            StringFormat format = new StringFormat();
            format.LineAlignment = StringAlignment.Center;
            format.Alignment = StringAlignment.Center;

            Graphics g = e.Graphics;
            Pen myPen = new Pen(Color.Black);
            int P_ID = -1;
            int my_ID = 0;
            Rectangle rect;
           
            for (int i = 0; i < Global.Nodes.Count; i++)
            {
                if (i == 0)
                {
                    using (font1 = new Font("Arial", 9, FontStyle.Regular, GraphicsUnit.Point))
                    {
                        rect = new Rectangle(tempX, tempY, rectWidth, rectHeight);
                        g.DrawRectangle(myPen, tempX, tempY, rectWidth, rectHeight);
                        e.Graphics.DrawString(Global.Nodes[i].Text, font1, Brushes.Blue, rect, format);
                    }
                    Global.Nodes[0].localX = tempX + (int)(.5f * rectWidth);
                    Global.Nodes[0].localY = tempY + (int)(.5f * rectHeight);
                    tempX = Global.Nodes[0].localX + (int)(.5f * rectWidth) + horizontalSpace;
                    continue;
                }

                P_ID = Global.Nodes[i].Parent_ID;
                if (P_ID < 0) return;
                if (Global.Nodes[i].Level == Global.Nodes[P_ID].Level)
                {
                    horizontalSpace = 30;
                    using (font1 = new Font("Arial", 9, FontStyle.Regular, GraphicsUnit.Point))
                    {
                        rect = new Rectangle(tempX, Global.Nodes[P_ID].localY - (int)(.5f * rectHeight), rectWidth, rectHeight);
                        g.DrawRectangle(myPen, rect);
                        e.Graphics.DrawString(Global.Nodes[i].Text, font1, Brushes.Blue, rect, format);
                    }

                    Global.Nodes[i].localX = tempX + (int)(0.5f * rectWidth);
                    Global.Nodes[i].localY = Global.Nodes[P_ID].localY;
                    g.DrawLine(myPen, Global.Nodes[P_ID].localX + .5f * rectWidth, Global.Nodes[P_ID].localY, Global.Nodes[i].localX - 0.5f * rectWidth, Global.Nodes[i].localY);
                }
                else
                {
                    for (int K = 0; K < Global.Nodes.Count; K++)
                    {
                        if (Global.Nodes[K].Parent_ID == P_ID)
                        {
                            if (K == i) my_ID = 0;
                            else my_ID = 1;
                            break;
                        }
                    }
                    if (my_ID == 0)
                    {
                        rect = new Rectangle(Global.Nodes[P_ID].localX - (int)(0.5f * rectWidth),
                                             Global.Nodes[P_ID].localY + (int)(.5f * rectHeight) + verticalSpace, rectWidth, rectHeight);
                    }
                    else
                    {
                        rect = new Rectangle(tempX,
                                             Global.Nodes[P_ID].localY + (int)(.5f * rectHeight) + verticalSpace, rectWidth, rectHeight);
                    }

                    if (Global.Nodes[i].Circle)
                    {
                        using (font1 = new Font("Arial", 9, FontStyle.Regular, GraphicsUnit.Point))
                        {
                            g.DrawEllipse(myPen, rect);
                            e.Graphics.DrawString(Global.Nodes[i].Text, font1, Brushes.Blue, rect, format);
                        }
                    }
                    else
                    {
                        using (font1 = new Font("Arial", 9, FontStyle.Regular, GraphicsUnit.Point))
                        {
                            g.DrawRectangle(myPen, rect);
                            e.Graphics.DrawString(Global.Nodes[i].Text, font1, Brushes.Blue, rect, format);
                        }
                    }

                    if (my_ID == 0)
                        Global.Nodes[i].localX = Global.Nodes[P_ID].localX;
                    else
                        Global.Nodes[i].localX = tempX + (int)(0.5f * rectWidth);

                    tempX = Global.Nodes[i].localX + (int)(0.5f * rectWidth) + 10;

                    Global.Nodes[i].localY = Global.Nodes[P_ID].localY + verticalSpace + rectHeight;
                    g.DrawLine(myPen, Global.Nodes[P_ID].localX, Global.Nodes[P_ID].localY + .5f * rectHeight, Global.Nodes[i].localX, Global.Nodes[i].localY - .5f * rectHeight);
                }
                    
                tempX = Global.Nodes[i].localX + (int)(0.5f * rectWidth) + 10;
                if (Global.Nodes[i].localY > ymax) ymax = Global.Nodes[i].localY;
            }

            button1.Width = tempX;
            button2.Height = ymax;
            
        }

       
    }
}
