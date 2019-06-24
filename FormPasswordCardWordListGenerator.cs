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
        private const int TotalNumberOfCharacters = NumOfRows_Horizontal * NumOfCols_Horizontal;
        private const int NumOfRows_Vertical = NumOfCols_Horizontal;
        private const int NumOfCols_Vertical = NumOfRows_Horizontal;
        private const string FileExtension = ".txt";
        private int MinPasswordLength = 6;
        private int MaxPasswordLength = 12;

        List<string> myResults;


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

            numericUpDownMaximumPasswordLength.Maximum = TotalNumberOfCharacters;
            numericUpDownMinimumPasswordLength.Maximum = TotalNumberOfCharacters;
        }

        private void ButtonGernerateWordList_Click(object sender, EventArgs e)
        {

            MinPasswordLength = Convert.ToInt32(numericUpDownMinimumPasswordLength.Value);
            MaxPasswordLength = Convert.ToInt32(numericUpDownMaximumPasswordLength.Value);

            if (MinPasswordLength > MaxPasswordLength)
            {
                MessageBox.Show("Minimum password length must be less than or equal to maximum password length.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (MinPasswordLength < 1)
            {
                MessageBox.Show("Minimum password length must be at least 1.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            //validate
            foreach (TextBox mySelectedTextBox in this.Controls.OfType<TextBox>())
            {
                if (mySelectedTextBox.Text.Length != NumOfCols_Horizontal)
                {
                    MessageBox.Show("Error, text in each text box must be " + NumOfCols_Horizontal + " characters", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

            }

            myResults = new List<string>(2000);

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

            char[][] myOriginalJaggedArray = CopyArrayLinq(myJaggedArray);

            if (checkBoxZigZagLeftToRightThenDown.Checked)
            {
                GenerateZigZagLeftToRightToDownToLeft(myOriginalJaggedArray);
            }
            if (checkBoxZigZagRightToLeftThenDown.Checked)
            {
                GenerateZigZagRightToLeftToDownToRight(myOriginalJaggedArray);
            }
            if (checkBoxZigZagLeftToRightThenUp.Checked)
            {
                GenerateZigZagLeftToRightToUpToLeft(myOriginalJaggedArray);
            }
            if (checkBoxZigZagRightToLeftThenUp.Checked)
            {
                GenerateZigZagRightToLeftToUpToRight(myOriginalJaggedArray);
            }

            //Diagonals
            if (checkBoxDiagonalTopDownToTheRight.Checked)
            {
                GenerateDiagonalTopDownToTheRight(myOriginalJaggedArray);
            }
            if (checkBoxDiagonalTopDownToTheLeft.Checked)
            {
                GenerateDiagonalTopDownToTheLeft(myOriginalJaggedArray);
            }
            if (checkBoxDiagonalBottomUpToTheRight.Checked)
            {
                GenerateDiagonalBottomUpToTheRight(myOriginalJaggedArray);
            }
            if (checkBoxDiagonalBottomUpToTheLeft.Checked)
            {
                GenerateDiagonalBottomUpToTheLeft(myOriginalJaggedArray);
            }

            //Spirals
            if (checkBoxSpiralClockwiseStartRight.Checked)
            {
                GenerateSpiralClockwise(myOriginalJaggedArray);
            }


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
                            while (remainingSlots <= maxRemainingSlots)
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
                    tempArray[NumOfRows_Horizontal - CurrentRow - 1] = myJaggedArray[CurrentRow][CurrentCol];
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
                myHorizontalLeftToRightStringArray[myJaggedArray.Length - i - 1] = new string(myJaggedArray[i]);
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


            string myFileName = "PasswordCard_WordList_" + myResults.Count + "_words_" + DateTime.Now.ToString("MMMM_dd_yyyy_hhmmss") + FileExtension;

            using (SaveFileDialog saveFileDialogWordFile = new SaveFileDialog())
            {
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


        private void ButtonGeneratePasswordCardFromSeed_Click(object sender, EventArgs e)
        {
            GeneratePasswordCardFromSeed();
        }

        private void GeneratePasswordCardFromSeed()
        {
            try
            {
                PasswordCard pc = new PasswordCard(textBoxSeed.Text);
                textBoxHeader.Text = pc.MyHeader;
                textBoxLine1.Text = new string(pc.myPasswordCardGrid[1]);
                textBoxLine2.Text = new string(pc.myPasswordCardGrid[2]);
                textBoxLine3.Text = new string(pc.myPasswordCardGrid[3]);
                textBoxLine4.Text = new string(pc.myPasswordCardGrid[4]);
                textBoxLine5.Text = new string(pc.myPasswordCardGrid[5]);
                textBoxLine6.Text = new string(pc.myPasswordCardGrid[6]);
                textBoxLine7.Text = new string(pc.myPasswordCardGrid[7]);
                textBoxLine8.Text = new string(pc.myPasswordCardGrid[8]);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Generating Password Card:\n\n" + ex.Message, "Error Generating Password Card", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }


        private void AddPasswordsToWordListFrom1dArray(char[] my1dArray)
        {
            string myString = new string(my1dArray);

            for (int StringStartLocation = 0; StringStartLocation < myString.Length; StringStartLocation++)
            {
                for (int passwordLength = MinPasswordLength; passwordLength <= MaxPasswordLength; passwordLength++)
                {
                    if (StringStartLocation + passwordLength <= myString.Length)
                    {
                        myResults.Add(myString.Substring(StringStartLocation, passwordLength));
                        Console.WriteLine(myResults.Last());
                    }
                }
            }
            
        }

        /// <summary>
        /// Copy a jaggad array
        /// 
        /// <see cref="https://stackoverflow.com/questions/4670720/extremely-fast-way-to-clone-the-values-of-a-jagged-array-into-a-second-array"/>
        /// </summary>
        /// <param name="source"> The array you want to copy</param>
        /// <returns>A copy of that array</returns>
        static char[][] CopyArrayLinq(char[][] source)
        {
            return source.Select(s => s.ToArray()).ToArray();
        }

        private  void GenerateZigZagLeftToRightToDownToLeft(char[][] myArray)
        {
            IEnumerable<char> myFlatArray;

            //In this case we are going to reverse every other row. This fits when you start at an odd row, then go right, down, left, down, ... 
            char[][] myZigZagEvenArray = CopyArrayLinq(myArray);
            for (int row = 0; row < myZigZagEvenArray.Length; row++)
            {
                if (row %2 != 0)
                {
                    myZigZagEvenArray[row] = myZigZagEvenArray[row].Reverse().ToArray();
                }
            }

            myFlatArray = myZigZagEvenArray.SelectMany(x => x);
            AddPasswordsToWordListFrom1dArray(myFlatArray.ToArray());

            //Now if you start with Row 2, the inverse above does not apply, you need to reverse the even rows.
            char[][] myZigZagOddArray = CopyArrayLinq(myArray); ;
            //eliminate the first row because it does not make sense in this action
            myZigZagOddArray[0] = "".ToCharArray();
            for (int row = 0; row < myZigZagOddArray.Length; row++)
            {
                if (row % 2 == 0)
                {
                    myZigZagOddArray[row] = myZigZagOddArray[row].Reverse().ToArray();
                }
            }

            myFlatArray = myZigZagOddArray.SelectMany(x => x);
            AddPasswordsToWordListFrom1dArray(myFlatArray.ToArray());

            //Yes there will be lots of overlap with "right to left to down to right" but users may not see it that way when the select one and start on an odd line.
            //Duplicates are cleaned up later. 
        }

        private void GenerateZigZagRightToLeftToDownToRight(char[][] myArray)
        {
            GenerateZigZagLeftToRightToDownToLeft(HorizontalArraySwap(myArray));
        }

        private void GenerateZigZagLeftToRightToUpToLeft(char[][] myArray)
        {
            GenerateZigZagLeftToRightToDownToLeft(VerticalArraySwap(myArray));
        }

        private void GenerateZigZagRightToLeftToUpToRight(char[][] myArray)
        {
            GenerateZigZagLeftToRightToDownToLeft(HorizontalArraySwap(VerticalArraySwap(myArray)));
        }

        private void GenerateDiagonalTopDownToTheRight(char[][] myArray)
        {
            char[][] myDiagonalArray = CopyArrayLinq(myArray);

            for (int startingCol = 0; startingCol < myArray[0].Length; startingCol++)
            {
                for (int startingRow = 0; startingRow < myArray.Length; startingRow++)
                {
                    int currentRow = startingRow;
                    string currentPassword = "";
                    for (int currentCol = startingCol; (currentCol < myArray[0].Length) && (currentRow < myDiagonalArray.Length); currentCol++)
                    {
                        if (currentCol < myDiagonalArray[currentRow].Length)
                        {
                            currentPassword += myDiagonalArray[currentRow][currentCol];
                            currentRow++;

                            if ((currentPassword.Length <= MaxPasswordLength) && (currentPassword.Length >= MinPasswordLength))
                            {
                                myResults.Add(currentPassword);
                                Console.WriteLine(myResults.Last());
                            }
                        }
                    }
                }
            }
        }

        private void GenerateDiagonalTopDownToTheLeft(char[][] myArray)
        {
            GenerateDiagonalTopDownToTheRight(HorizontalArraySwap(myArray)); 
        }

        private void GenerateDiagonalBottomUpToTheRight(char[][] myArray)
        {
            GenerateDiagonalTopDownToTheRight(VerticalArraySwap(myArray));
        }

        private void GenerateDiagonalBottomUpToTheLeft(char[][] myArray)
        {
            GenerateDiagonalTopDownToTheRight(VerticalArraySwap(HorizontalArraySwap(myArray)));
        }

        private char[][] HorizontalArraySwap(char[][] myArray)
        {
            char[][] mySwappedArray = CopyArrayLinq(myArray);
            for (int row = 0; row < myArray.Length; row++)
            {
                mySwappedArray[row] = myArray[row].Reverse().ToArray();
            }
            return mySwappedArray;
        }

        private char[][] VerticalArraySwap(char[][] myArray)
        {
            char[][] mySwappedArray = CopyArrayLinq(myArray);
            for (int row = 0; row < myArray.Length; row++)
            {
                myArray[row].CopyTo(mySwappedArray[myArray.Length - 1 - row], 0);
            }
            return mySwappedArray;
        }

        private void GenerateSpiralClockwise(char[][] myArray)
        {
            GenerateSpiral(myArray, SpiralDirection.Clockwise, SpiralStartDirection.Right);
        }

        enum SpiralStartDirection
        {
            Right,
            Left,
            Up,
            Down
        }

        enum SpiralDirection
        {
            Clockwise,
            Counterclockwise
        }

        private void GenerateSpiral(char[][] myArray, SpiralDirection myDirection = SpiralDirection.Clockwise, SpiralStartDirection myStartDirection = SpiralStartDirection.Right)
        {
            for (int rowStartPosition = 0; rowStartPosition < myArray.Length; rowStartPosition++)
            {
                for (int colStartPosition = 0; colStartPosition < myArray[0].Length; colStartPosition++)
                {
                    int nextRowMove;
                    int nextColMove;
                    bool startWithCol;

                    if (myDirection == SpiralDirection.Clockwise)
                    {
                        switch (myStartDirection)
                        {
                            case SpiralStartDirection.Right:
                                startWithCol = true;
                                nextRowMove = 1;
                                nextColMove = 1;
                                break;
                            case SpiralStartDirection.Left:
                                startWithCol = true;
                                nextRowMove = -1;
                                nextColMove = -1;
                                break;
                            case SpiralStartDirection.Up:
                                startWithCol = false;
                                nextRowMove = 1;
                                nextColMove = 1;
                                break;
                            case SpiralStartDirection.Down:
                                startWithCol = false;
                                nextRowMove = -1;
                                nextColMove = -1;
                                break;
                            default:
                                startWithCol = true;
                                nextRowMove = 1;
                                nextColMove = 1;
                                break;
                        }

                    }
                    else
                    {

                        switch (myStartDirection)
                        {
                            case SpiralStartDirection.Right:
                                startWithCol = true;
                                nextRowMove = -1;
                                nextColMove = 1;
                                break;
                            case SpiralStartDirection.Left:
                                startWithCol = true;
                                nextRowMove = 1;
                                nextColMove = -1;
                                break;
                            case SpiralStartDirection.Up:
                                startWithCol = false;
                                nextRowMove = 1;
                                nextColMove = -1;
                                break;
                            case SpiralStartDirection.Down:
                                startWithCol = false;
                                nextRowMove = -1;
                                nextColMove = 1;
                                break;
                            default:
                                startWithCol = true;
                                nextRowMove = -1;
                                nextColMove = 1;
                                break;
                        }
                    }

                    int currentRow = rowStartPosition;
                    int currentCol = colStartPosition;

                    int rowTarget = currentRow + nextRowMove;
                    int colTarget = currentCol + nextColMove;

                    bool timeToQuit = false; 

                    string currentPassword = myArray[currentRow][currentCol].ToString();

                    AddPotentialPassword(currentPassword);

                    while (!timeToQuit)
                    {
                        

                        //First move is to the right.
                        if (startWithCol)
                        {
                            do
                            {
                                if (nextColMove > 0) { currentCol++; } else { currentCol--; }
                                if ((currentCol < 0) || (currentCol >= myArray[0].Length)) { timeToQuit = true; break; }

                                currentPassword += myArray[currentRow][currentCol].ToString();
                                AddPotentialPassword(currentPassword);

                            } while (currentCol != colTarget);

                            if (timeToQuit)
                            {
                                break;
                            }

                            if (nextColMove > 0) { nextColMove = -1 * (nextColMove + 1); } else { nextColMove = -1 * (nextColMove - 1); }

                            colTarget = colTarget + nextColMove;
                        }
                        startWithCol = true;

                        if (timeToQuit)
                        {
                            break;
                        }

                        //First move is down.
                        do
                        {
                            if (nextRowMove > 0) { currentRow++; } else { currentRow--; }
                            if ((currentRow < 0) || (currentRow >= myArray.Length)) { timeToQuit = true; break; }

                            currentPassword += myArray[currentRow][currentCol].ToString();
                            AddPotentialPassword(currentPassword);

                        } while (currentRow != rowTarget);

                        if (timeToQuit)
                        {
                            break;
                        }

                        if (nextRowMove > 0) { nextRowMove = -1 * (nextRowMove + 1); } else { nextRowMove = -1 * (nextRowMove - 1); }

                        rowTarget = rowTarget + nextRowMove;
                    }

                }
            }
        }

        private void AddPotentialPassword(string potentialPassword)
        {
            if ((potentialPassword.Length <= MaxPasswordLength) && (potentialPassword.Length >= MinPasswordLength))
            {
                myResults.Add(potentialPassword);
                Console.WriteLine(myResults.Last());
            }
        }

        private void SpiralCounterClockwise(char[][] myArray)
        {

        }

    }

    /// <summary>
    /// 
    /// This is derived from https://www.passwordcard.org
    /// I did not port the whole thing. Only the ability to import. 
    /// 
    ///* This file is part of PasswordCard.
    ///*
    ///* PasswordCard is free software: you can redistribute it and/or modify
    ///* it under the terms of the GNU General Public License as published by
    ///* the Free Software Foundation, either version 3 of the License, or
    ///* (at your option) any later version.
    ///*
    ///* PasswordCard is distributed in the hope that it will be useful,
    ///* but WITHOUT ANY WARRANTY; without even the implied warranty of
    ///* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
    ///* GNU General Public License for more details.
    ///*
    ///* You should have received a copy of the GNU General Public License
    ///* along with PasswordCard.If not, see<http://www.gnu.org/licenses/>.
    ///*
    ///* Copyright © 2010 pepsoft.org.
    /// </summary>
    public class PasswordCard
    {
        public PasswordCard(string seed, bool digitArea = false, bool includeSymbols = false, int height = HEIGHT, int width = WIDTH)
        {
            GeneratePasswordCardFromSeed(seed, digitArea, includeSymbols, height, width);
        }

        public PasswordCard(UInt64 seed, bool digitArea = false, bool includeSymbols = false, int height = HEIGHT, int width = WIDTH)
        {
            GeneratePasswordCardFromSeed(seed, digitArea, includeSymbols, height, width);
        }



        //public char[,] myPasswordCardGrid;

        public const int WIDTH = 29, HEIGHT = 9, BODY_HEIGHT = HEIGHT - 1;
        public const string HEADER_CHARS = "■□▲△○●★☂☀☁☹☺♠♣♥♦♫€¥£$!?¡¿⊙◐◩�";
        public const string DIGITS = "0123456789";
        public const string DIGITS_AND_LETTERS = "23456789abcdefghjkmnpqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ";
        public const string DIGITS_LETTERS_AND_SYMBOLS = "23456789abcdefghjkmnpqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ@#$%&*<>?€+{}[]()/\\";


        public char[][] myPasswordCardGrid;

        /// <summary>
        /// The header chars in the top line of the array. 
        /// </summary>
        public string MyHeader { get { return new string(myPasswordCardGrid[0]); }  }



        private bool GeneratePasswordCardFromSeed(string seed, bool digitArea = false, bool includeSymbols = false, int height = HEIGHT, int width = WIDTH)
        {

            if (seed == null) { throw new ArgumentException("The string cannot be null", "seed"); }
            seed = seed.Trim();
            if (seed == "") { throw new ArgumentException("The string cannot be empty", "seed"); }
            if (seed.Length > 16) { throw new ArgumentException("The string length must be no more than 16 characters", "seed"); }

            string precursor = "";
            for (int i = seed.Length; i < 16; i++)
            {
                precursor += '0';
            }
            seed = precursor + seed;

            UInt64 mySeed; 

            try
            {
                //return Long.parseLong(paddedStr.substring(0, 8), 16) << 32 | Long.parseLong(paddedStr.substring(8), 16);
                mySeed = UInt64.Parse(seed, System.Globalization.NumberStyles.HexNumber);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Could not parse Hex Number String. " + ex.Message, "seed", ex.InnerException);
            }

            return GeneratePasswordCardFromSeed(mySeed, digitArea, includeSymbols, height, width);
        }

        private bool GeneratePasswordCardFromSeed(UInt64 seed, bool digitArea = false, bool includeSymbols = false, int height = HEIGHT, int width = WIDTH)
        {

            //Check for minimums here. 
            if (height < 2) { throw new ArgumentException("Parameter height must be greater than 2"); }
            if (width < 2) { throw new ArgumentException("Parameter width must be greater than 2"); }
                       
            myPasswordCardGrid = new char[height][];
            for (int i = 0; i < myPasswordCardGrid.Length; i++)
            {
                myPasswordCardGrid[i] = new char[width];
            }

            Random random = new Random(seed);

            char[]  headerChars = HEADER_CHARS.ToCharArray();

            try
            {

            Shuffle(headerChars, random);

            if (headerChars.Length > width)
            {
                //Array.Copy(headerChars, 0, headerChars, 0, width);
                Array.Resize(ref headerChars, width);
            }

            myPasswordCardGrid[0] = headerChars;

            if (digitArea)
            {
                int halfHeight = 1 + ((height - 1) / 2);
                for (int y = 1; y < halfHeight; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        if (includeSymbols && ((x % 2) == 0))
                        {
                            myPasswordCardGrid[y][x] = DIGITS_LETTERS_AND_SYMBOLS.ToCharArray()[random.NextInt(DIGITS_LETTERS_AND_SYMBOLS.Length)];
                        }
                        else
                        {
                            myPasswordCardGrid[y][x] = DIGITS_AND_LETTERS.ToCharArray()[random.NextInt(DIGITS_AND_LETTERS.Length)];
                        }
                    }
                }
                for (int y = halfHeight; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        myPasswordCardGrid[y][x] = DIGITS.ToCharArray()[random.NextInt(10)];
                    }
                }
            }
            else
            {
                for (int y = 1; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        if (includeSymbols && ((x % 2) == 0))
                        {
                            myPasswordCardGrid[y][x] = DIGITS_LETTERS_AND_SYMBOLS.ToCharArray()[random.NextInt(DIGITS_LETTERS_AND_SYMBOLS.Length)];
                        }
                        else
                        {
                            myPasswordCardGrid[y][x] = DIGITS_AND_LETTERS.ToCharArray()[random.NextInt(DIGITS_AND_LETTERS.Length)];
                        }
                    }
                }
            }

            }
            catch (Exception)
            {
                throw;
            }

            return true;
        }

        private void Shuffle(char[] list, Random rnd)
        {
            int size = list.Length;
            for (int i = size; i > 1; i--)
                Swap(list, i - 1, rnd.NextInt(i));
        }

        private void Swap(char[] list, int i, int j)
        {
            char tmp = list[i];
            list[i] = list[j];
            list[j] = tmp;
        }

    }

    /// <summary>
    /// from https://stackoverflow.com/questions/2147524/c-java-number-randomization
    /// credit to https://stackoverflow.com/users/12048/finnw 
    /// </summary>
    [Serializable]
    public class Random
    {
        public Random(UInt64 seed)
        {
            this.seed = (seed ^ 0x5DEECE66DUL) & ((1UL << 48) - 1);
        }



        public int NextInt(int n)
        {
            if (n <= 0) throw new ArgumentException("n must be positive");

            if ((n & -n) == n)  // i.e., n is a power of 2
                return (int)((n * (long)Next(31)) >> 31);

            long bits, val;
            do
            {
                bits = Next(31);
                val = bits % (UInt32)n;
            }
            while (bits - val + (n - 1) < 0);

            return (int)val;
        }

        protected UInt32 Next(int bits)
        {
            seed = (seed * 0x5DEECE66DL + 0xBL) & ((1L << 48) - 1);

            return (UInt32)(seed >> (48 - bits));
        }

        private UInt64 seed;
    }

}
