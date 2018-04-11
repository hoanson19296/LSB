using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Form1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        
        public void buttonOpen_Click(object sender, EventArgs e)
        {            
            OpenFileDialog openDialog = new OpenFileDialog();            
            openDialog.Filter = "Image Files (*.png, *.jpg) | *.png; *.jpg";        
            openDialog.InitialDirectory = @"C:\";
            //Ghi File Path v�o Textbox
            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                textBoxFilePath.Text = openDialog.FileName.ToString();
                pictureBox1.ImageLocation = textBoxFilePath.Text;
            }
        }
        public string padding(int b)
        {
            string bin = Convert.ToString(b, 2);
            while (bin.Length < 8)
            {
                bin = "0" + bin;
            }
            return bin;
        }
        //Chuy?n Chu?i th�nh Binary
        public string MessageToBinary(string msg)
        {
            string result = "";

            for (int i = 0; i < msg.ToCharArray().Length; i++)
            {
                char c = msg.ToCharArray()[i];
                result += padding(c);
            }
            return result;
        }
        //Chuy?n chu?i Binary th�nh Byte - d? k?t h?p v?i H�m Encode,ASCII
        public static Byte[] GetBytesFromBinaryString(string binary)
        {
            Console.WriteLine("byte" + binary);
            var list = new List<Byte>();

            for (int i = 0; i < binary.Length - 1; i += 8)
            {
                string t = binary.Substring(i, 8);

                list.Add(Convert.ToByte(t, 2));
            }

            return list.ToArray();
        }

        //Button Encode
        public void buttonEncode_Click(object sender, EventArgs e)
        {
            int count = 0;
            //Chuy?n Message th�nh Bin d?ng String
            string msgInBin = MessageToBinary(textBoxMessage.Text);
            //Th�m d? d�i message (H? bin) v�o d?u Chu?i Bin
            string hiddenString = padding(textBoxMessage.Text.Length) + msgInBin;
            //Th�m v�i s? 0 dang sau d? d? d�i chu?i chia h?t cho 8
            while (hiddenString.Length % 3 != 0)
            {
                hiddenString += "0";
            }
            //Chuy?n th�nh M?ng Char
            char[] binHidden = hiddenString.ToCharArray();
            //L?y ?nh
            Bitmap img = new Bitmap(textBoxFilePath.Text);
            //L?y t?ng Pixel
            for (int row = 0; row < img.Width; row++)
            {
                for (int column = 0; column < img.Height; column++)
                {
                    if (count == binHidden.Length)
                        break;
                    Color pixel = img.GetPixel(row, column);
                    int newPixelR = (binHidden[count++] == '0') ? (pixel.R & 254) : (pixel.R | 1);
                    int newPixelG = (binHidden[count++] == '0') ? (pixel.G & 254) : (pixel.G | 1);
                    int newPixelB = (binHidden[count++] == '0') ? (pixel.B & 254) : (pixel.B | 1);
                    img.SetPixel(row, column, Color.FromArgb(newPixelR, newPixelG, newPixelB));
                }
                if (count == binHidden.Length)
                    break;
            }

            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Filter = "Image Files (*.png, *.jpg) | *.png; *.jpg";
            saveFile.InitialDirectory = @"C:\";

            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                textBoxFilePath.Text = saveFile.FileName.ToString();
                pictureBox1.ImageLocation = textBoxFilePath.Text;

                img.Save(textBoxFilePath.Text);
                textBoxMessage.Clear();         //X�a Message
            }
        }

        //L?y LSB trong Pixel
        public static string getLSBBits(Color pixel)
        {
            string result = "";
            string redInPixel = Convert.ToString(pixel.R, 2);
            string greenInPixel = Convert.ToString(pixel.G, 2);
            string blueInPixel = Convert.ToString(pixel.B, 2);
            result += redInPixel[redInPixel.Length - 1];
            result += greenInPixel[greenInPixel.Length - 1];
            result += blueInPixel[blueInPixel.Length - 1];

            return result;
        }
        //Button Decode
        public void buttonDecode_Click(object sender, EventArgs e)
        {
            Bitmap img = new Bitmap(textBoxFilePath.Text);
            var message = "";
            int count = 0;
            int length = 0;

            for (int row = 0; row < img.Width; row++)
            {
                for (int column = 0; column < img.Height; column++)
                {
                    Color pixel = img.GetPixel(row, column);
                    message += getLSBBits(pixel);
                    count += 3;        //V� l?y 1 pixel th� ta s? du?c 3 LSB (R - G - B)
                    if (count == 9)
                    {
                        length = Convert.ToInt32(message.Substring(0, 8), 2); //Chuy?n t? String sang Bin
                        message = message.Substring(8); //Message s? t? v? tr� th? 8 tr? di (8 bit d?u l� d? d�i chu?i)
                    }
                    if (count >= ((length + 1) * 8)) //Length + 1 v� t�nh lu�n 8 bit d?u c?a length
                    {
                        int du = count % ((length + 1) * 8);  //Ph?n du, v� count + m?t l�c 3 don v? => s? c� l�c vu?t hon length
                                                              
                        message = message.Substring(0, message.Length - du);
                        break;
                    }

                }
                if (count >= ((length + 1) * 8))
                    break;
            }
            var data2 = GetBytesFromBinaryString(message);
            message = Encoding.ASCII.GetString(data2);
            textBoxMessage.Text = message;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}

