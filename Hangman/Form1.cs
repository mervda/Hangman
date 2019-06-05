using MetroFramework.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;

namespace Hangman
{
    //TODO: make user choose from different pools of random words
    //TODO: comment project properly
    //TODO: localization ? maybe even localized keyboard ?
    //TODO: refactor
    //REDO: gallows graphics

    public partial class Form1 : MetroFramework.Forms.MetroForm
    {
        MetroFramework.Forms.MetroForm gallowsForm = new MetroFramework.Forms.MetroForm();
        PictureBox pictureBox = new PictureBox();
        int health = 9;
        int lastHealth = 0;
        bool ready = false;
        string text = null;
        string hashText = null;
        bool found = false;



        public Form1()
        {
            InitializeComponent();            
        }

        private void button_Click(object sender, EventArgs e)
        {
            string senderBtn = (sender as Button).Text;

            int gfPositionX = this.Location.X + this.Width-10;
            int gfPositonY = this.Location.Y;
            int gfHeight = this.Height;

            if (ready)
            {
                for (int i = 0; i < text.Length; i++)
                {
                    if (text[i].ToString() == senderBtn)
                    {
                        hashText = new StringBuilder(hashText).Remove(i, 1).Insert(i, senderBtn).ToString();
                        mtbWord.Text = makeSpaces(hashText);
                        found = true;
                    }
                }

                if (mtbWord.Text.Replace(" ","") == text)
                {
                    MessageBox.Show("You won !!");
                    mbtnReady.PerformClick();
                    mbtnReady.Text = "Play again";
                }

                if (!found)
                {
                    if (mtgglSounds.Checked)
                    {
                        System.Media.SoundPlayer buzzer = new System.Media.SoundPlayer(Properties.Resources.buzzer1);
                        buzzer.PlaySync();
                    }
                    lastHealth = health;
                    health--;
                }

                found = false;

                if (health != lastHealth && health != 9)
                    makeNewForm(gfPositionX, gfPositonY, gfHeight, health, gallowsForm, pictureBox);

                if (health == 0)
                {
                    MessageBox.Show("YOU LOST !!!" + Environment.NewLine + "Word: " + text);
                    mbtnReady.PerformClick();
                    mbtnReady.Text = "Try again";
                }
            }
        }

        //make a new form and set its picture
        public static void makeNewForm(int gfPositionX, int gfPositonY, int gfHeight, int health, Form gf, PictureBox pictureBox)
        {
            Image myImage = null;
            switch (health)
            {
                case 8:
                    myImage = new Bitmap(Hangman.Properties.Resources.firststage);
                    break;
                case 7:
                    myImage = new Bitmap(Hangman.Properties.Resources.secondstage);
                    break;
                case 6:
                    myImage = new Bitmap(Hangman.Properties.Resources.thirdstage);
                    break;
                case 5:
                    myImage = new Bitmap(Hangman.Properties.Resources.fourthstage);
                    break;
                case 4:
                    myImage = new Bitmap(Hangman.Properties.Resources.fifthstage);
                    break;
                case 3:
                    myImage = new Bitmap(Hangman.Properties.Resources.sixthstage);
                    break;
                case 2:
                    myImage = new Bitmap(Hangman.Properties.Resources.seventhstage);
                    break;
                case 1:
                    myImage = new Bitmap(Hangman.Properties.Resources.eighthstage);
                    break;
                case 0:
                    myImage = new Bitmap(Hangman.Properties.Resources.ninethstage);
                    break;
                default:
                    myImage = new Bitmap(Hangman.Properties.Resources.canvas);
                    break;
            }

            pictureBox.Image = myImage;
            pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox.Dock = DockStyle.Fill;

            if (pictureBox.Image != null && pictureBox.Image != myImage)
            {
                pictureBox.Image.Dispose();
                pictureBox.Image = myImage;
            }

            gf.StartPosition = FormStartPosition.Manual;
            gf.Location = new Point(gfPositionX, gfPositonY);
            gf.Size = new Size(300, gfHeight);
            gf.ControlBox = false;
            gf.Controls.Add(pictureBox);
            gf.Show();
        }

        //theme changing
        private void themeChange_Click(object sender, EventArgs e)
        {
            string senderBtn = (sender as Button).Text;

            if (senderBtn == "Dark")
                MetroStyleManager.Default.Theme = MetroFramework.MetroThemeStyle.Dark;
            else
                MetroStyleManager.Default.Theme = MetroFramework.MetroThemeStyle.Light;
        }

        //handles start and end of the game
        private void mbtnReady_Click(object sender, EventArgs e)
        {
            if (ready)
            {
                if (gallowsForm != null)
                    gallowsForm.Hide();

                text = null;
                hashText = null;
                mtbWord.Text = null;
                mbtnReady.Text = "ready";
                health = 9;
                ready = false;
            }
            else if (!ready && mtgglChoice.Checked)
            {
                if (gallowsForm != null)
                    gallowsForm.Hide();

                if (mtbWord.Text == "")
                {
                    MessageBox.Show("Write your word, please.");
                    mtbWord.ReadOnly = false;
                    mbtnReady.Text = "save";
                    ready = false;

                    return;
                }

                hashWord(hashText, mtbWord.Text);
                mbtnReady.Text = "stop";
                ready = true;
                mtbWord.ReadOnly = true;
            }
            else if (!ready && mtgglRandom.Checked)
            {
                if (gallowsForm != null)
                    gallowsForm.Hide();

                generateWord();
                hashWord(hashText, mtbWord.Text.ToString());
                mbtnReady.Text = "stop";
                ready = true;
            }
            else
                MessageBox.Show("Something went horribly wrong.");
        }

        //takes a word, tears it apart, makes it more spacious 
        public string makeSpaces(string text)
        {
            int j = 0;
            string outputText = null;
            for (j = 0; j <= text.Length - 1; j++)
            {
                if (j == 0)
                    outputText += text[j];

                if (j > 0)
                {
                    outputText += " ";
                    outputText += text[j];
                }
            }
            return outputText;
        }

        //making sure only one option is toggled
        private void mtgglRandom_CheckedChanged(object sender, EventArgs e)
        {
            if (mtgglRandom.Checked)
            {
                mtgglChoice.Checked = false;
                mcbChoice.Visible = true;
                mlblToBeAdded.Visible = true;
                mtbWord.ReadOnly = true;
            }
            else
                mtgglChoice.Checked = true;

        }

        //making sure only one option is toggled
        private void mtgglChoice_CheckedChanged(object sender, EventArgs e)
        {
            if (mtgglChoice.Checked)
            {
                mtgglRandom.Checked = false;
                mcbChoice.Visible = false;
                mlblToBeAdded.Visible = false;
                mtbWord.ReadOnly = false;
            }

        }

        public int randomizeID ()
        {
            Random rnd = new Random();
            int randomCislo = 0;
            SQLiteConnection m_dbConnection;

            m_dbConnection = new SQLiteConnection("Data Source=MyDatabase.sqlite;Version=3;");
            m_dbConnection.Open();
            string sql = "select count(ID) from Word";
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            randomCislo = Convert.ToInt32(command.ExecuteScalar());
            m_dbConnection.Close();

            int randomID = rnd.Next(1, randomCislo);

            return randomID;
        }

        public void hashWord(string _hashText, string _text)
        {
            int i = 0;
            _text = _text.ToUpper();
            for (i = 0; i <= _text.Length - 1; i++)
            {
                if (i == 0)
                    _hashText += _text[i];

                if (i > 0)
                {
                    _hashText += "_";
                }
            }
            hashText = _hashText;
            text = _text;
            mtbWord.Text = hashText;
            mtbWord.Text = makeSpaces(mtbWord.Text);
        }

        public void generateWord()
        {
            Random rnd = new Random();
            SQLiteConnection m_dbConnection;
            m_dbConnection =
            new SQLiteConnection("Data Source=MyDatabase.sqlite;Version=3;");
            m_dbConnection.Open();

            string sql = "select * from Word WHERE ID = @randomID";
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            command.Parameters.Add(new SQLiteParameter("@randomID", randomizeID()));
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                MessageBox.Show("ID: " + reader["ID"] + " Slovo: " + reader["Word"] + " Typ: " + reader["Type"]);
                mtbWord.Text = reader["Word"].ToString();
            }
        }

        private void Form1_LocationChanged(object sender, EventArgs e)
        {
            int gfPositionX = this.Location.X + this.Width-10;
            int gfPositonY = this.Location.Y;
            int gfHeight = this.Height;

            gallowsForm.Location = new Point(gfPositionX, gfPositonY);
        }
    }
}