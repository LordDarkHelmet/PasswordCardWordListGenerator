using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PasswordCardWordListGenerator
{
    
    public partial class FormPasswordCardWordListGenerator : Form
    {

        private const int NumOfRows_Horizontal = 8;
        private const int NumOfCols_Horizontal = 29;
        private const int NumOfRows_Vertical = NumOfCols_Horizontal;
        private const int NumOfCols_Vertical = NumOfRows_Horizontal;
        private int MinPasswordLength = 6;
        private int MaxPasswordLength = 12;


        public FormPasswordCardWordListGenerator()
        {
            InitializeComponent();
            foreach (TextBox mySelectedTextBox in this.Controls.OfType<TextBox>())
            {
                if (mySelectedTextBox != null)
                {
                    mySelectedTextBox.MaxLength = NumOfCols_Horizontal;
                }
            }
        }

        private void ButtonGernerateWordList_Click(object sender, EventArgs e)
        {

            MinPasswordLength = Convert.ToInt32(numericUpDownMinimumPasswordLength.Value);
            MaxPasswordLength = Convert.ToInt32(numericUpDownMaximumPasswordLength.Value);

            if (MinPasswordLength>MaxPasswordLength)
            {
                MessageBox.Show("Minimum password length must be less than or equal to maximum password length.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


            //validate
            foreach (TextBox mySelectedTextBox in this.Controls.OfType<TextBox>())
            {
                if (mySelectedTextBox.Text.Length != NumOfCols_Horizontal)
                {
                    MessageBox.Show("Error, text in each text box must be "+ NumOfCols_Horizontal + " characters", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

            }

            List<string> myResults = new List<string>(2000);

            // create array of arrays
            char[][] myJaggedArray = new char[NumOfRows_Horizontal][];
            // create arrays to put in the array of arrays

            myJaggedArray[0] = textBoxLine1.Text.ToCharArray();
            myJaggedArray[1] = textBoxLine2.Text.ToCharArray();
            myJaggedArray[2] = textBoxLine3.Text.ToCharArray();
            myJaggedArray[3] = textBoxLine4.Text.ToCharArray();
            myJaggedArray[4] = textBoxLine5.Text.ToCharArray();
            myJaggedArray[5] = textBoxLine6.Text.ToCharArray();
            myJaggedArray[6] = textBoxLine7.Text.ToCharArray();
            myJaggedArray[7] = textBoxLine8.Text.ToCharArray();

            string[] myHorizontalLeftToRightStringArray = new string[NumOfRows_Horizontal];
            for (int i = 0; i < myJaggedArray.Length; i++)
            {
                myHorizontalLeftToRightStringArray[i] = new string(myJaggedArray[i]);
            }


            string[] myVerticleTopToBottomStringArray = new string[NumOfCols_Horizontal];
            for (int CurrentCol = 0; CurrentCol < NumOfCols_Horizontal; CurrentCol++)
            {
                char[] tempArray = new char[NumOfRows_Horizontal];
                for (int CurrentRow = 0; CurrentRow < NumOfRows_Horizontal; CurrentRow++)
                {
                    tempArray[CurrentRow] = myJaggedArray[CurrentRow][CurrentCol];
                }
                myVerticleTopToBottomStringArray[CurrentCol] = new string(tempArray); 
            }


            //OK data loaded.

            //now lets create a new wordlist

            //ok, to solve this we should start at the top left and go left to right, then top to bottom. when we hit an edge we should go up and down/ right/left
            for (int CurrentRow = 0; CurrentRow < NumOfRows_Horizontal; CurrentRow++)
            {
                for (int CurrentCol = 0; CurrentCol < NumOfCols_Horizontal; CurrentCol++)
                {
                    //ok at the starting point of our first string. 
                    //we want to save each one at length
                    for (int numberOfChars = MinPasswordLength; numberOfChars <= MaxPasswordLength; numberOfChars++)
                    {
                        if ((numberOfChars + CurrentCol) <= NumOfCols_Horizontal)
                        {
                            if (checkBoxLeftToRight.Checked)
                            {
                                myResults.Add(myHorizontalLeftToRightStringArray[CurrentRow].Substring(CurrentCol, numberOfChars));
                                Console.WriteLine(myResults.Last());
                            }
                        }
                        else
                        {
                            int remainingCharsOnTheArray = NumOfCols_Horizontal - CurrentCol - 1;
                            string startOfString = myHorizontalLeftToRightStringArray[CurrentRow].Substring(CurrentCol, remainingCharsOnTheArray + 1);

                            //At this point we have reached an edge. Try to go up or down depending on the remaining chars
                            int maxRemainingSlots = MaxPasswordLength - startOfString.Length;
                            int minRemainingSlots = MinPasswordLength - startOfString.Length;
                            int remainingSlots = Math.Max(minRemainingSlots, 1);
                            while( remainingSlots <= maxRemainingSlots )
                            {

                                if (checkBoxLeftToRightThenDownAtEdge.Checked)
                                {
                                    //Take care of going down
                                    if (!((CurrentRow + remainingSlots) >= NumOfRows_Horizontal))
                                    {
                                        char[] myCharArray = myVerticleTopToBottomStringArray[CurrentCol + remainingCharsOnTheArray].ToCharArray(CurrentRow + 1, remainingSlots);
                                        string endOfString = new string(myCharArray);
                                        myResults.Add(startOfString + endOfString);
                                        Console.WriteLine(myResults.Last());
                                    }
                                }

                                if (checkBoxLeftToRightThenUpAtEdge.Checked)
                                {
                                    //Take care of going up
                                    if (!((CurrentRow - remainingSlots) < 0))
                                    {
                                        char[] myCharArray = myVerticleTopToBottomStringArray[CurrentCol + remainingCharsOnTheArray].ToCharArray(CurrentRow - remainingSlots, remainingSlots);
                                        Array.Reverse(myCharArray);
                                        string endOfString = new string(myCharArray);
                                        myResults.Add(startOfString + endOfString);
                                        Console.WriteLine(myResults.Last());
                                    }
                                }

                                remainingSlots++;
                            }

                            break;
                        }
                    }
                }
            }



            ////////////////////////////
            //ok, to solve this we should start at the top left and go left to right, then top to bottom. when we hit an edge we should go up and down/ right/left
            for (int CurrentRow = 0; CurrentRow < NumOfRows_Vertical; CurrentRow++)
            {
                for (int CurrentCol = 0; CurrentCol < NumOfCols_Vertical; CurrentCol++)
                {
                    //ok at the starting point of our first string. 
                    //we want to save each one at length
                    for (int numberOfChars = MinPasswordLength; numberOfChars <= MaxPasswordLength; numberOfChars++)
                    {
                        if ((numberOfChars + CurrentCol) <= NumOfCols_Vertical)
                        {
                            if (checkBoxTopToBottom.Checked)
                            {
                                myResults.Add(myVerticleTopToBottomStringArray[CurrentRow].Substring(CurrentCol, numberOfChars));
                                Console.WriteLine(myResults.Last());
                            }
                        }
                        else
                        {
                            int remainingCharsOnTheArray = NumOfCols_Vertical - CurrentCol - 1;
                            string startOfString = myVerticleTopToBottomStringArray[CurrentRow].Substring(CurrentCol, remainingCharsOnTheArray + 1);

                            //At this point we have reached an edge. Try to go up or down depending on the remaining chars
                            int maxRemainingSlots = MaxPasswordLength - startOfString.Length;
                            int minRemainingSlots = MinPasswordLength - startOfString.Length;
                            int remainingSlots = Math.Max(minRemainingSlots, 1);
                            if (remainingSlots < 1) { remainingSlots = 1; }
                            while (remainingSlots <= maxRemainingSlots)
                            {

                                if (checkBoxTopToBottomThenRightAtEdge.Checked)
                                {
                                    //Take care of going down
                                    if (!((CurrentRow + remainingSlots) >= NumOfRows_Vertical))
                                    {
                                        char[] myCharArray = myHorizontalLeftToRightStringArray[CurrentCol + remainingCharsOnTheArray].ToCharArray(CurrentRow + 1, remainingSlots);
                                        string endOfString = new string(myCharArray);
                                        myResults.Add(startOfString + endOfString);
                                        Console.WriteLine(myResults.Last());
                                    }
                                }

                                if (checkBoxTopToBottomThenLeftAtEdge.Checked)
                                {
                                    //Take care of going up
                                    if (!((CurrentRow - remainingSlots) < 0))
                                    {
                                        char[] myCharArray = myHorizontalLeftToRightStringArray[CurrentCol + remainingCharsOnTheArray].ToCharArray(CurrentRow - remainingSlots, remainingSlots);
                                        Array.Reverse(myCharArray);
                                        string endOfString = new string(myCharArray);
                                        myResults.Add(startOfString + endOfString);
                                        Console.WriteLine(myResults.Last());
                                    }
                                }

                                remainingSlots++;
                            }

                            break;
                        }
                    }
                }
            }


            //Yes extreamly lazy coding. I am just going to transpose the array then copy and paste the above code. 

            myHorizontalLeftToRightStringArray = new string[NumOfRows_Horizontal];
            for (int i = 0; i < myJaggedArray.Length; i++)
            {
                Array.Reverse(myJaggedArray[i]);
                myHorizontalLeftToRightStringArray[i] = new string(myJaggedArray[i]);
            }


            myVerticleTopToBottomStringArray = new string[NumOfCols_Horizontal];
            for (int CurrentCol = 0; CurrentCol < NumOfCols_Horizontal; CurrentCol++)
            {
                char[] tempArray = new char[NumOfRows_Horizontal];
                for (int CurrentRow = 0; CurrentRow < NumOfRows_Horizontal; CurrentRow++)
                {
                    tempArray[NumOfRows_Horizontal - CurrentRow -1] = myJaggedArray[CurrentRow][CurrentCol];
                }

                Array.Reverse(tempArray);
                myVerticleTopToBottomStringArray[CurrentCol] = new string(tempArray);
            }


            //ok, to solve this we should start at the top left and go left to right, then top to bottom. when we hit an edge we should go up and down/ right/left
            for (int CurrentRow = 0; CurrentRow < NumOfRows_Horizontal; CurrentRow++)
            {
                for (int CurrentCol = 0; CurrentCol < NumOfCols_Horizontal; CurrentCol++)
                {
                    //ok at the starting point of our first string. 
                    //we want to save each one at length
                    for (int numberOfChars = MinPasswordLength; numberOfChars <= MaxPasswordLength; numberOfChars++)
                    {
                        if ((numberOfChars + CurrentCol) <= NumOfCols_Horizontal)
                        {
                            if (checkBoxRightToLeft.Checked)
                            {
                                myResults.Add(myHorizontalLeftToRightStringArray[CurrentRow].Substring(CurrentCol, numberOfChars));
                                Console.WriteLine(myResults.Last());
                            }
                        }
                        else
                        {
                            int remainingCharsOnTheArray = NumOfCols_Horizontal - CurrentCol - 1;
                            string startOfString = myHorizontalLeftToRightStringArray[CurrentRow].Substring(CurrentCol, remainingCharsOnTheArray + 1);

                            //At this point we have reached an edge. Try to go up or down depending on the remaining chars
                            int maxRemainingSlots = MaxPasswordLength - startOfString.Length;
                            int minRemainingSlots = MinPasswordLength - startOfString.Length;
                            int remainingSlots = Math.Max(minRemainingSlots, 1);
                            while (remainingSlots <= maxRemainingSlots)
                            {

                                if (checkBoxRightToLeftThenDownAtEdge.Checked)
                                {
                                    //Take care of going down
                                    if (!((CurrentRow + remainingSlots) >= NumOfRows_Horizontal))
                                    {
                                        char[] myCharArray = myVerticleTopToBottomStringArray[CurrentCol + remainingCharsOnTheArray].ToCharArray(CurrentRow + 1, remainingSlots);
                                        string endOfString = new string(myCharArray);
                                        myResults.Add(startOfString + endOfString);
                                        Console.WriteLine(myResults.Last());
                                    }
                                }

                                if (checkBoxRightToLeftThenUpAtEdge.Checked)
                                {
                                    //Take care of going up
                                    if (!((CurrentRow - remainingSlots) < 0))
                                    {
                                        char[] myCharArray = myVerticleTopToBottomStringArray[CurrentCol + remainingCharsOnTheArray].ToCharArray(CurrentRow - remainingSlots, remainingSlots);
                                        Array.Reverse(myCharArray);
                                        string endOfString = new string(myCharArray);
                                        myResults.Add(startOfString + endOfString);
                                        Console.WriteLine(myResults.Last());
                                    }
                                }

                                remainingSlots++;
                            }

                            break;
                        }
                    }
                }
            }

            myVerticleTopToBottomStringArray = new string[NumOfCols_Horizontal];
            for (int CurrentCol = 0; CurrentCol < NumOfCols_Horizontal; CurrentCol++)
            {
                char[] tempArray = new char[NumOfRows_Horizontal];
                for (int CurrentRow = 0; CurrentRow < NumOfRows_Horizontal; CurrentRow++)
                {
                    tempArray[NumOfRows_Horizontal - CurrentRow - 1] = myJaggedArray[CurrentRow][CurrentCol];
                }

                //Array.Reverse(tempArray);
                myVerticleTopToBottomStringArray[CurrentCol] = new string(tempArray);
            }

            myHorizontalLeftToRightStringArray = new string[NumOfRows_Horizontal];
            for (int i = 0; i < myJaggedArray.Length; i++)
            {
                //Array.Reverse(myJaggedArray[i]);
                myHorizontalLeftToRightStringArray[myJaggedArray.Length - i -1] = new string(myJaggedArray[i]);
            }

            ////////////////////////////
            //ok, to solve this we should start at the top left and go left to right, then top to bottom. when we hit an edge we should go up and down/ right/left
            for (int CurrentRow = 0; CurrentRow < NumOfRows_Vertical; CurrentRow++)
            {
                for (int CurrentCol = 0; CurrentCol < NumOfCols_Vertical; CurrentCol++)
                {
                    //ok at the starting point of our first string. 
                    //we want to save each one at length
                    for (int numberOfChars = MinPasswordLength; numberOfChars <= MaxPasswordLength; numberOfChars++)
                    {
                        if ((numberOfChars + CurrentCol) <= NumOfCols_Vertical)
                        {
                            if (checkBoxBottomToTop.Checked)
                            {
                                myResults.Add(myVerticleTopToBottomStringArray[CurrentRow].Substring(CurrentCol, numberOfChars));
                                Console.WriteLine(myResults.Last());
                            }
                        }
                        else
                        {
                            int remainingCharsOnTheArray = NumOfCols_Vertical - CurrentCol - 1;
                            string startOfString = myVerticleTopToBottomStringArray[CurrentRow].Substring(CurrentCol, remainingCharsOnTheArray + 1);

                            //At this point we have reached an edge. Try to go up or down depending on the remaining chars
                            int maxRemainingSlots = MaxPasswordLength - startOfString.Length;
                            int minRemainingSlots = MinPasswordLength - startOfString.Length;
                            int remainingSlots = Math.Max(minRemainingSlots, 1);
                            if (remainingSlots < 1) { remainingSlots = 1; }
                            while (remainingSlots <= maxRemainingSlots)
                            {

                                if (checkBoxBottomToTopThenLeftAtEdge.Checked)
                                {
                                    //Take care of going down
                                    if (!((CurrentRow + remainingSlots) >= NumOfRows_Vertical))
                                    {
                                        char[] myCharArray = myHorizontalLeftToRightStringArray[CurrentCol + remainingCharsOnTheArray].ToCharArray(CurrentRow + 1, remainingSlots);
                                        string endOfString = new string(myCharArray);
                                        myResults.Add(startOfString + endOfString);
                                        Console.WriteLine(myResults.Last());

                                    }
                                }

                                if (checkBoxBottomToTopThenRightAtEdge.Checked)
                                {
                                    //Take care of going up
                                    if (!((CurrentRow - remainingSlots) < 0))
                                    {
                                        char[] myCharArray = myHorizontalLeftToRightStringArray[CurrentCol + remainingCharsOnTheArray].ToCharArray(CurrentRow - remainingSlots, remainingSlots);
                                        Array.Reverse(myCharArray);
                                        string endOfString = new string(myCharArray);
                                        myResults.Add(startOfString + endOfString);
                                        Console.WriteLine(myResults.Last());
                                    }
                                }

                                remainingSlots++;
                            }

                            break;
                        }
                    }
                }
            }


            Console.WriteLine("Total Number of strings on list = " + myResults.Count);
            //remove duplicates
            myResults = myResults.Distinct().ToList();
            Console.WriteLine("Total Number of strings on list after filter = " + myResults.Count);


            labelNumberOfPasswordsGenerated.Text = "Number of Passwords Generated: " + +myResults.Count;


            string myFileName = "PasswordCard_WordList_" + myResults.Count + "_words_" + DateTime.Now.ToString("MMMM_dd_yyyy_hhmmss") + ".txt";

            SaveFileDialog saveFileDialogWordFile = new SaveFileDialog();
            saveFileDialogWordFile.DefaultExt = "txt";

            saveFileDialogWordFile.FileName = myFileName;
            if (saveFileDialogWordFile.ShowDialog() == DialogResult.OK)
            {
                if (saveFileDialogWordFile.FileName != "")
                {
                    Console.WriteLine("Filename = " + saveFileDialogWordFile.FileName);
                    System.IO.File.WriteAllLines(saveFileDialogWordFile.FileName, myResults);
                }
            }

        }


        private void LinkLabelTelegram_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(@"https://t.me/xtzrecovery");
        }

        private void LinkLabelEmail_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("mailto:TezosHelp@outlook.com");
        }

        private void LinkLabelGitHub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(@"https://github.com/LordDarkHelmet/PasswordCardWordListGenerator");
        }

        private void PictureBoxSample_Click(object sender, EventArgs e)
        {
            Process.Start(@"https://www.passwordcard.org/");
        }

        private void PictureBoxTezosXTZRecoveryLogo_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Version " + Application.ProductVersion + "\nProduct Name " + Application.ProductName + "\nCompany Name " + Application.CompanyName, "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void LinkLabelDonationTezos_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //Thank you for donating. 
            Clipboard.SetText(linkLabelDonationTezos.Text);

            if (DialogResult.Yes == MessageBox.Show("Thank you for your support! Our work is donation based. \nThe Tezos address " + linkLabelDonationTezos.Text + " has been copied to your clipboard.\n\nWould you like to see the donation address?", "Thank you!", MessageBoxButtons.YesNo, MessageBoxIcon.Information))
            {
                Process.Start(@"https://tzscan.io/" + linkLabelDonationTezos.Text);
            }
        }

        private void LinkLabelDonationBTC_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //Thank you for donating. 
            Clipboard.SetText(linkLabelDonationBTC.Text);

            if (DialogResult.Yes == MessageBox.Show("Thank you for your support! Our work is donation based. \nThe Bitcoin address " + linkLabelDonationBTC.Text + " has been copied to your clipboard.\n\nWould you like to see the donation address?", "Thank you!", MessageBoxButtons.YesNo, MessageBoxIcon.Information))
            {
                Process.Start(@"https://www.blockchain.com/btc/address/" + linkLabelDonationBTC.Text);
            }
        }
    }
}
