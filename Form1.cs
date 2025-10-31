using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;
using System.Media;
using System.IO;

namespace PildiYlesanne
{
    public partial class Form1 : Form
    {
        TableLayoutPanel mainLayout;
        TreeView treeView;
        Panel contentPanel;

        // --- PictureViewer ---
        TableLayoutPanel imageLayout;
        PictureBox pictureBox;
        FlowLayoutPanel flowPanel;
        Button btnShowImage, btnClearImage, btnChangeColor, btnClose, btnFlipHorizontal, btnFlipVertical, btnInvertColors;
        CheckBox chkStretch;
        ComboBox recentFilesBox;
        OpenFileDialog openFileDialog;
        ColorDialog colorDialog;
        Point mouseDownLocation;
        System.Windows.Forms.Timer slideshowTimer;
        string[] imageFiles;
        int currentImageIndex = 0;

        // --- MathQuiz ---
        Panel mathQuizPanel;
        Label timeLabel;
        Button startButton;
        ProgressBar progressBar;
        Label scoreLabel;
        Button hintButton;
        ComboBox difficultyBox;
        CheckBox practiceModeCheckBox;

        // --- Matching Game ---
        Panel matchingGamePanel;
        TableLayoutPanel gameTableLayout;
        System.Windows.Forms.Timer gameTimer;
        Label firstClicked = null;
        Label secondClicked = null;
        Random random = new Random();
        List<string> icons = new List<string>()
        {
            "!", "!", "N", "N", ",", ",", "k", "k",
            "b", "b", "v", "v", "w", "w", "z", "z"
        };
        System.Windows.Forms.Timer countdownTimer;
        Label timeLeftLabel;
        int gameTimeLeft = 60;
        Button startGameButton;
        ComboBox cardColorBox;
        ComboBox themeBox;

        // --- RANDOM GENERATION FOR QUIZ ---
        Random randomizer = new Random();
        int addend1, addend2;
        int minuend, subtrahend;
        int multiplicand, multiplier;
        int dividend, divisor;
        System.Windows.Forms.Timer timer1;
        int timeLeft;

        // --- Quiz Scores / Records ---
        int quizScore = 0;
        string resultsFile = "results.txt";

        public Form1()
        {
            this.Text = "Pildi Ülesanne TreeView versioon";
            this.Size = new Size(1000, 700);
            this.MinimumSize = new Size(900, 600);

            mainLayout = new TableLayoutPanel();
            mainLayout.Dock = DockStyle.Fill;
            mainLayout.ColumnCount = 2;
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 200));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            treeView = new TreeView();
            treeView.Dock = DockStyle.Fill;
            TreeNode root = new TreeNode("Menu");
            root.Nodes.Add("Pildiülesanne");
            root.Nodes.Add("Matemaatiline Quiz");
            root.Nodes.Add("Matching Game");
            treeView.Nodes.Add(root);
            treeView.ExpandAll();
            treeView.AfterSelect += TreeView_AfterSelect;

            contentPanel = new Panel();
            contentPanel.Dock = DockStyle.Fill;
            contentPanel.BackColor = Color.WhiteSmoke;

            mainLayout.Controls.Add(treeView, 0, 0);
            mainLayout.Controls.Add(contentPanel, 1, 0);
            this.Controls.Add(mainLayout);

            openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Vali pilt";
            openFileDialog.Filter = "JPEG (*.jpg)|*.jpg|PNG (*.png)|*.png|BMP (*.bmp)|*.bmp|Kõik failid (*.*)|*.*";
            colorDialog = new ColorDialog();

            CreateImagePanel();
            CreateMathQuizPanel();
            CreateMatchingGamePanel();

            timer1 = new System.Windows.Forms.Timer();
            timer1.Interval = 1000;
            timer1.Tick += Timer1_Tick;
        }

        // --------------------- Pildiülesanne ---------------------
        private void CreateImagePanel()
        {
            imageLayout = new TableLayoutPanel();
            imageLayout.Dock = DockStyle.Fill;
            imageLayout.RowCount = 2;
            imageLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 90));
            imageLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 10));

            pictureBox = new PictureBox();
            pictureBox.Dock = DockStyle.Fill;
            pictureBox.BorderStyle = BorderStyle.Fixed3D;
            pictureBox.SizeMode = PictureBoxSizeMode.Normal;

            EnablePictureDrag();

            flowPanel = new FlowLayoutPanel();
            flowPanel.Dock = DockStyle.Fill;
            flowPanel.FlowDirection = FlowDirection.RightToLeft;

            btnShowImage = new Button() { Text = "Näita pilti", AutoSize = true };
            btnClearImage = new Button() { Text = "Kustuta pilt", AutoSize = true };
            btnChangeColor = new Button() { Text = "Muuda tausta värv", AutoSize = true };
            btnClose = new Button() { Text = "Sulge", AutoSize = true };
            chkStretch = new CheckBox() { Text = "Venita", AutoSize = true };
            btnFlipHorizontal = new Button() { Text = "Pööra horisontaalselt", AutoSize = true };
            btnFlipVertical = new Button() { Text = "Pööra vertikaalselt", AutoSize = true };
            btnInvertColors = new Button() { Text = "Inverteeri värvid", AutoSize = true };

            // Новые улучшения

            btnShowImage.Click += ShowImage_Click;
            btnClearImage.Click += ClearImage_Click;
            btnChangeColor.Click += ChangeColor_Click;
            btnClose.Click += Close_Click;
            chkStretch.CheckedChanged += Stretch_CheckedChanged;
            btnFlipHorizontal.Click += FlipHorizontal_Click;
            btnFlipVertical.Click += FlipVertical_Click;
            btnInvertColors.Click += InvertColors_Click;

            flowPanel.Controls.AddRange(new Control[] { btnClose, btnChangeColor, btnClearImage, btnShowImage,
                btnInvertColors, btnFlipVertical, btnFlipHorizontal, chkStretch, recentFilesBox });

            imageLayout.Controls.Add(pictureBox, 0, 0);
            imageLayout.Controls.Add(flowPanel, 0, 1);
        }

        private void EnablePictureDrag()
        {
            pictureBox.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Left) mouseDownLocation = e.Location;
            };
            pictureBox.MouseMove += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    pictureBox.Left = e.X + pictureBox.Left - mouseDownLocation.X;
                    pictureBox.Top = e.Y + pictureBox.Top - mouseDownLocation.Y;
                }
            };
        }

        private void ShowImage_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                pictureBox.Image = Image.FromFile(openFileDialog.FileName);
            }
        }


        private void ClearImage_Click(object sender, EventArgs e) => pictureBox.Image = null;
        private void ChangeColor_Click(object sender, EventArgs e)
        {
            if (colorDialog.ShowDialog() == DialogResult.OK)
                pictureBox.BackColor = colorDialog.Color;
        }
        private void Close_Click(object sender, EventArgs e) => this.Close();
        private void Stretch_CheckedChanged(object sender, EventArgs e) =>
            pictureBox.SizeMode = chkStretch.Checked ? PictureBoxSizeMode.StretchImage : PictureBoxSizeMode.Normal;
        private void FlipHorizontal_Click(object sender, EventArgs e)
        {
            if (pictureBox.Image != null)
            {
                pictureBox.Image.RotateFlip(RotateFlipType.RotateNoneFlipX);
                pictureBox.Refresh();
            }
        }
        private void FlipVertical_Click(object sender, EventArgs e)
        {
            if (pictureBox.Image != null)
            {
                pictureBox.Image.RotateFlip(RotateFlipType.RotateNoneFlipY);
                pictureBox.Refresh();
            }
        }
        private void InvertColors_Click(object sender, EventArgs e)
        {
            if (pictureBox.Image != null)
            {
                Bitmap bitmap = new Bitmap(pictureBox.Image);
                for (int x = 0; x < bitmap.Width; x++)
                    for (int y = 0; y < bitmap.Height; y++)
                    {
                        Color c = bitmap.GetPixel(x, y);
                        bitmap.SetPixel(x, y, Color.FromArgb(255 - c.R, 255 - c.G, 255 - c.B));
                    }
                pictureBox.Image = bitmap;
            }
        }

        private void SaveAs_Click(object sender, EventArgs e)
        {
            if (pictureBox.Image != null)
            {
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "PNG (*.png)|*.png|JPEG (*.jpg)|*.jpg|BMP (*.bmp)|*.bmp";
                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    var format = System.Drawing.Imaging.ImageFormat.Png;
                    if (saveDialog.FileName.EndsWith(".jpg")) format = System.Drawing.Imaging.ImageFormat.Jpeg;
                    else if (saveDialog.FileName.EndsWith(".bmp")) format = System.Drawing.Imaging.ImageFormat.Bmp;
                    pictureBox.Image.Save(saveDialog.FileName, format);
                }
            }
        }

        private void Slideshow_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string folder = Path.GetDirectoryName(openFileDialog.FileName);
                imageFiles = Directory.GetFiles(folder).Where(f => f.EndsWith(".jpg") || f.EndsWith(".png") || f.EndsWith(".bmp")).ToArray();
                currentImageIndex = 0;
                slideshowTimer = new System.Windows.Forms.Timer();
                slideshowTimer.Interval = 2000;
                slideshowTimer.Tick += (s, ev) =>
                {
                    if (imageFiles.Length == 0) return;
                    pictureBox.Image = Image.FromFile(imageFiles[currentImageIndex]);
                    currentImageIndex = (currentImageIndex + 1) % imageFiles.Length;
                };
                slideshowTimer.Start();
            }
        }

        private void ZoomImage(float factor)
        {
            if (pictureBox.Image == null) return;
            pictureBox.Width = (int)(pictureBox.Width * factor);
            pictureBox.Height = (int)(pictureBox.Height * factor);
        }

        // --------------------- Matemaatiline Quiz ---------------------
        private void CreateMathQuizPanel()
        {
            mathQuizPanel = new Panel();
            mathQuizPanel.Dock = DockStyle.Fill;
            mathQuizPanel.BackColor = Color.LightBlue;
            mathQuizPanel.Padding = new Padding(20);

            TableLayoutPanel mainTable = new TableLayoutPanel();
            mainTable.Dock = DockStyle.Fill;
            mainTable.ColumnCount = 1;
            mainTable.RowCount = 6;
            mainTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));
            mainTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            mainTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
            mainTable.RowStyles.Add(new RowStyle(SizeType.Percent, 60));
            mainTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));
            // increased bottom row to make progress controls and time visible
            mainTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 140));

            Label titleLabel = new Label();
            titleLabel.Text = "Matemaatiline Quiz";
            titleLabel.Font = new Font("Segoe UI", 24, FontStyle.Bold);
            titleLabel.ForeColor = Color.DarkBlue;
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            titleLabel.Dock = DockStyle.Fill;
            mainTable.Controls.Add(titleLabel, 0, 0);

            // ComboBox raskusaste
            difficultyBox = new ComboBox();
            difficultyBox.Items.AddRange(new string[] { "Lihtne", "Keskmine", "Raske" });
            difficultyBox.SelectedIndex = 0;
            difficultyBox.Dock = DockStyle.Fill;
            mainTable.Controls.Add(difficultyBox, 0, 1);

            // Чекбокс тренировки
            practiceModeCheckBox = new CheckBox();
            practiceModeCheckBox.Text = "Harjutusrežiim";
            practiceModeCheckBox.Dock = DockStyle.Fill;
            mainTable.Controls.Add(practiceModeCheckBox, 0, 2);

            Panel examplesPanel = new Panel();
            examplesPanel.Dock = DockStyle.Fill;
            examplesPanel.BackColor = Color.White;
            mainTable.Controls.Add(examplesPanel, 0, 3);

            void CreateExamples()
            {
                examplesPanel.Controls.Clear();
                int spacing = 60;
                int startY = 20;

                void CreateMathProblem(int y, string leftName, string op, string rightName, string numericName)
                {
                    int centerX = Math.Max(0, examplesPanel.Width / 2 - 100);
                    Label left = new Label() { Name = leftName, Text = "?", Size = new Size(50, 40), Location = new Point(centerX, y), Font = new Font("Segoe UI", 16, FontStyle.Bold), TextAlign = ContentAlignment.MiddleCenter, BackColor = Color.LightGray, BorderStyle = BorderStyle.FixedSingle };
                    Label operation = new Label() { Text = op, Size = new Size(30, 40), Location = new Point(left.Right + 5, y), Font = new Font("Segoe UI", 16), TextAlign = ContentAlignment.MiddleCenter };
                    Label right = new Label() { Name = rightName, Text = "?", Size = new Size(50, 40), Location = new Point(operation.Right + 5, y), Font = new Font("Segoe UI", 16, FontStyle.Bold), TextAlign = ContentAlignment.MiddleCenter, BackColor = Color.LightGray, BorderStyle = BorderStyle.FixedSingle };
                    Label equals = new Label() { Text = "=", Size = new Size(20, 40), Location = new Point(right.Right + 5, y), Font = new Font("Segoe UI", 16), TextAlign = ContentAlignment.MiddleCenter };
                    NumericUpDown answer = new NumericUpDown() { Name = numericName, Size = new Size(80, 30), Location = new Point(equals.Right + 10, y + 5), Font = new Font("Segoe UI", 12), Maximum = 1000 };
                    answer.Enter += Answer_Enter;
                    examplesPanel.Controls.AddRange(new Control[] { left, operation, right, equals, answer });
                }

                CreateMathProblem(startY, "plusLeftLabel", "+", "plusRightLabel", "sum");
                CreateMathProblem(startY + spacing, "minusLeftLabel", "-", "minusRightLabel", "difference");
                CreateMathProblem(startY + spacing * 2, "timesLeftLabel", "×", "timesRightLabel", "product");
                CreateMathProblem(startY + spacing * 3, "dividedLeftLabel", "÷", "dividedRightLabel", "quotient");
            }

            examplesPanel.SizeChanged += (s, e) => CreateExamples();
            startButton = new Button();
            startButton.Name = "startButton";
            startButton.Text = "ALUSTA QUIZI";
            startButton.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            startButton.BackColor = Color.Green;
            startButton.ForeColor = Color.White;
            startButton.Dock = DockStyle.Fill;
            startButton.Margin = new Padding(100, 10, 100, 10);
            startButton.Click += StartButton_Click;
            mainTable.Controls.Add(startButton, 0, 4);

            Panel progressPanel = new Panel();
            progressPanel.Dock = DockStyle.Fill;
            progressPanel.BackColor = Color.Transparent;
            // add padding so controls inside are not clipped
            progressPanel.Padding = new Padding(10);
            mainTable.Controls.Add(progressPanel, 0, 5);

            progressBar = new ProgressBar() { Name = "progressBar", Size = new Size(300, 20), Visible = false };
            progressPanel.Controls.Add(progressBar);
            scoreLabel = new Label() { Name = "scoreLabel", Text = "Õigeid vastuseid: 0/4", AutoSize = true, Font = new Font("Segoe UI", 12, FontStyle.Bold), ForeColor = Color.DarkGreen, Visible = false };
            progressPanel.Controls.Add(scoreLabel);
            hintButton = new Button() { Text = "❓ Vihje", Size = new Size(100, 30), Font = new Font("Segoe UI", 10), BackColor = Color.Gold, Visible = false };
            hintButton.Click += HintButton_Click;
            progressPanel.Controls.Add(hintButton);

            timeLabel = new Label {
                Name = "timeLabel",
                Text = "30 sekundit",
                AutoSize = true,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.DarkRed,
                Visible = false
            };
            progressPanel.Controls.Add(timeLabel);

            // improved layout positions so controls are not hidden
            progressPanel.SizeChanged += (s, e) =>
            {
                int centerX = progressPanel.Width / 2;
                progressBar.Location = new Point(centerX - progressBar.Width / 2, 8);
                scoreLabel.Location = new Point(centerX - scoreLabel.Width / 2, 36);
                timeLabel.Location = new Point(centerX - timeLabel.Width / 2, 64);
                hintButton.Location = new Point(centerX - hintButton.Width / 2, 96);
            };
            mathQuizPanel.Controls.Add(mainTable);
            CreateExamples();
        }

        // --------------------- Matching Game ---------------------
        private void CreateMatchingGamePanel()
        {
            matchingGamePanel = new Panel();
            matchingGamePanel.Dock = DockStyle.Fill;
            matchingGamePanel.BackColor = Color.CornflowerBlue;

            Panel headerPanel = new Panel() { Dock = DockStyle.Top, Height = 70, BackColor = Color.Navy };
            Label titleLabel = new Label() { Text = "Matching Game", Font = new Font("Arial", 24, FontStyle.Bold), ForeColor = Color.White, AutoSize = true, Location = new Point(20, 15) };
            timeLeftLabel = new Label() { Text = "Aega jäänud: 60 sek", Font = new Font("Arial", 14, FontStyle.Bold), ForeColor = Color.Yellow, AutoSize = true, Location = new Point(400, 25) };
            headerPanel.Controls.Add(titleLabel); headerPanel.Controls.Add(timeLeftLabel);

            gameTableLayout = new TableLayoutPanel() { Dock = DockStyle.Fill, BackColor = Color.CornflowerBlue, Padding = new Padding(20), ColumnCount = 4, RowCount = 4 };
            for (int i = 0; i < 4; i++) { gameTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F)); gameTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 25F)); }

            for (int i = 0; i < 16; i++)
            {
                Label cardLabel = new Label() { Dock = DockStyle.Fill, Margin = new Padding(8), BackColor = Color.RoyalBlue, ForeColor = Color.RoyalBlue, Font = new Font("Webdings", 48, FontStyle.Bold), TextAlign = ContentAlignment.MiddleCenter, Cursor = Cursors.Hand };
                cardLabel.Click += CardLabel_Click;
                gameTableLayout.Controls.Add(cardLabel);
            }

            Panel footerPanel = new Panel() { Dock = DockStyle.Bottom, Height = 80, BackColor = Color.Navy };
            startGameButton = new Button() { Text = "ALUSTA MÄNGU", Font = new Font("Arial", 16, FontStyle.Bold), BackColor = Color.Green, ForeColor = Color.White, Size = new Size(200, 50) };
            footerPanel.Controls.Add(startGameButton);
            footerPanel.Resize += (s, e) =>
            {
                startGameButton.Location = new Point((footerPanel.Width - startGameButton.Width) / 2, (footerPanel.Height - startGameButton.Height) / 2);
            };
            startGameButton.Click += StartGameButton_Click;

            // Новые элементы
            cardColorBox = new ComboBox() { Items = { "Red", "Green", "Blue", "Yellow" }, SelectedIndex = 0, Dock = DockStyle.Top };
            themeBox = new ComboBox() { Items = { "Sinine", "Roheline", "Punane" }, SelectedIndex = 0, Dock = DockStyle.Top };
            themeBox.SelectedIndexChanged += (s, e) =>
            {
                switch (themeBox.SelectedItem.ToString())
                {
                    case "Sinine": gameTableLayout.BackColor = Color.CornflowerBlue; break;
                    case "Roheline": gameTableLayout.BackColor = Color.LightGreen; break;
                    case "Punane": gameTableLayout.BackColor = Color.IndianRed; break;
                }
            };
            matchingGamePanel.Controls.Add(cardColorBox);
            matchingGamePanel.Controls.Add(themeBox);

            matchingGamePanel.Controls.Add(gameTableLayout);
            matchingGamePanel.Controls.Add(headerPanel);
            matchingGamePanel.Controls.Add(footerPanel);

            gameTimer = new System.Windows.Forms.Timer() { Interval = 1000 };
            gameTimer.Tick += GameTimer_Tick;
            countdownTimer = new System.Windows.Forms.Timer() { Interval = 1000 };
            countdownTimer.Tick += CountdownTimer_Tick;
        }

        private void CardLabel_Click(object sender, EventArgs e)
        {
            if (gameTimer.Enabled || !countdownTimer.Enabled) return;
            Label clickedLabel = sender as Label;
            if (clickedLabel == null || clickedLabel.ForeColor == Color.Black) return;

            Color selectedColor = Color.Red;
            switch (cardColorBox.SelectedItem.ToString())
            {
                case "Red": selectedColor = Color.Red; break;
                case "Green": selectedColor = Color.Green; break;
                case "Blue": selectedColor = Color.Blue; break;
                case "Yellow": selectedColor = Color.Yellow; break;
            }
            clickedLabel.ForeColor = selectedColor;

            if (firstClicked == null) { firstClicked = clickedLabel; return; }
            secondClicked = clickedLabel;

            if (firstClicked.Text == secondClicked.Text)
            {
                firstClicked = null; secondClicked = null;
                CheckForWinner();
            }
            else
            {
                gameTimer.Start();
            }
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            gameTimer.Stop();
            if (firstClicked != null && secondClicked != null)
            {
                firstClicked.ForeColor = firstClicked.BackColor;
                secondClicked.ForeColor = secondClicked.BackColor;
            }
            firstClicked = null; secondClicked = null;
        }

        private void CountdownTimer_Tick(object sender, EventArgs e)
        {
            gameTimeLeft--;
            timeLeftLabel.Text = $"Aega jäänud: {gameTimeLeft} sek";
            if (gameTimeLeft <= 0) { countdownTimer.Stop(); gameTimer.Stop(); MessageBox.Show("Aeg läbi! Proovi uuesti."); ResetGame(); }
        }

        private void StartGameButton_Click(object sender, EventArgs e) => StartNewGame();
        private void StartNewGame()
        {
            gameTimeLeft = 60; timeLeftLabel.Text = $"Aega jäänud: {gameTimeLeft} sek"; firstClicked = null; secondClicked = null; AssignIconsToSquares(); countdownTimer.Start();
        }
        private void ResetGame()
        {
            firstClicked = null; secondClicked = null; gameTimeLeft = 60; timeLeftLabel.Text = $"Aega jäänud: {gameTimeLeft} sek"; countdownTimer.Stop(); gameTimer.Stop();
            foreach (Control control in gameTableLayout.Controls) { if (control is Label l) l.ForeColor = l.BackColor; }
        }
        private void AssignIconsToSquares()
        {
            List<string> iconsCopy = new List<string>(icons);
            foreach (Control control in gameTableLayout.Controls)
            {
                if (control is Label l && iconsCopy.Count > 0)
                {
                    int idx = random.Next(iconsCopy.Count);
                    l.Text = iconsCopy[idx];
                    l.ForeColor = l.BackColor;
                    iconsCopy.RemoveAt(idx);
                }
            }
        }

        // --------------------- TreeView ---------------------
        private void TreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            contentPanel.Controls.Clear();
            if (e.Node.Text == "Pildiülesanne") contentPanel.Controls.Add(imageLayout);
            else if (e.Node.Text == "Matemaatiline Quiz") contentPanel.Controls.Add(mathQuizPanel);
            else if (e.Node.Text == "Matching Game") contentPanel.Controls.Add(matchingGamePanel);
        }

        // --------------------- Quiz Utility ---------------------
        private void Answer_Enter(object sender, EventArgs e) { if (sender is NumericUpDown n) n.Select(0, n.Value.ToString().Length); }
        private Label GetLabel(string name) { var mainTable = mathQuizPanel.Controls[0] as TableLayoutPanel; var examplesPanel = mainTable?.Controls[3] as Panel; return examplesPanel?.Controls[name] as Label; }
        private NumericUpDown GetNumeric(string name) { var mainTable = mathQuizPanel.Controls[0] as TableLayoutPanel; var examplesPanel = mainTable?.Controls[3] as Panel; return examplesPanel?.Controls[name] as NumericUpDown; }
        private void StartButton_Click(object sender, EventArgs e) { StartTheQuiz(); startButton.Enabled = false; }
        public void StartTheQuiz()
        {
            int maxVal = difficultyBox.SelectedItem.ToString() == "Lihtne" ? 10 : difficultyBox.SelectedItem.ToString() == "Keskmine" ? 50 : 100;
            addend1 = randomizer.Next(maxVal); addend2 = randomizer.Next(maxVal); GetLabel("plusLeftLabel").Text = addend1.ToString(); GetLabel("plusRightLabel").Text = addend2.ToString(); GetNumeric("sum").Value = 0;
            minuend = randomizer.Next(maxVal); subtrahend = randomizer.Next(1, minuend); GetLabel("minusLeftLabel").Text = minuend.ToString(); GetLabel("minusRightLabel").Text = subtrahend.ToString(); GetNumeric("difference").Value = 0;
            multiplicand = randomizer.Next(2, 11); multiplier = randomizer.Next(2, 11); GetLabel("timesLeftLabel").Text = multiplicand.ToString(); GetLabel("timesRightLabel").Text = multiplier.ToString(); GetNumeric("product").Value = 0;
            divisor = randomizer.Next(2, 11); int tempQuotient = randomizer.Next(2, 11); dividend = divisor * tempQuotient; GetLabel("dividedLeftLabel").Text = dividend.ToString(); GetLabel("dividedRightLabel").Text = divisor.ToString(); GetNumeric("quotient").Value = 0;

            timeLeft = practiceModeCheckBox.Checked ? 9999 : 30;
            timeLabel.Text = timeLeft + " sekundit";
            timer1.Start();
            ShowProgressControls(true); progressBar.Value = 0; scoreLabel.Text = "Õigeid vastuseid: 0/4";
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            int correct = 0;
            if (GetNumeric("sum").Value == addend1 + addend2) correct++;
            if (GetNumeric("difference").Value == minuend - subtrahend) correct++;
            if (GetNumeric("product").Value == multiplicand * multiplier) correct++;
            if (GetNumeric("quotient").Value == dividend / divisor) correct++;
            progressBar.Value = correct * 25; scoreLabel.Text = $"Õigeid vastuseid: {correct}/4";

            if (correct == 4) { timer1.Stop(); MessageBox.Show("Kõik õiged!"); startButton.Enabled = true; ShowProgressControls(false); }
            else if (timeLeft > 0) { timeLeft--; timeLabel.Text = timeLeft + " sekundit"; }
            else { timer1.Stop(); MessageBox.Show("Aeg läbi!"); startButton.Enabled = true; ShowProgressControls(false); }
        }

        private void ShowProgressControls(bool show)
        {
            progressBar.Visible = show;
            scoreLabel.Visible = show;
            hintButton.Visible = show;
            timeLabel.Visible = show;
        }
        private void HintButton_Click(object sender, EventArgs e)
        {
            // Collect problems that are not yet correct
            var incomplete = new List<string>();
            var sumControl = GetNumeric("sum");
            var diffControl = GetNumeric("difference");
            var prodControl = GetNumeric("product");
            var quotControl = GetNumeric("quotient");

            if (sumControl != null && sumControl.Value != addend1 + addend2) incomplete.Add("sum");
            if (diffControl != null && diffControl.Value != minuend - subtrahend) incomplete.Add("difference");
            if (prodControl != null && prodControl.Value != multiplicand * multiplier) incomplete.Add("product");
            if (quotControl != null && quotControl.Value != dividend / divisor) incomplete.Add("quotient");

            if (incomplete.Count == 0)
            {
                // All answers already correct — nothing to hint.
                MessageBox.Show("Kõik vastused on juba õiged.", "Vihje", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Reveal exactly one random incomplete answer
            string pick = incomplete[randomizer.Next(incomplete.Count)];
            NumericUpDown target = GetNumeric(pick);
            if (target == null) return;

            int answer = pick switch
            {
                "sum" => addend1 + addend2,
                "difference" => minuend - subtrahend,
                "product" => multiplicand * multiplier,
                "quotient" => dividend / divisor,
                _ => 0
            };

            target.Value = Math.Max(target.Minimum, Math.Min(target.Maximum, answer));
        }

        private void CheckForWinner()
        {
            foreach (Label l in gameTableLayout.Controls.OfType<Label>())
            {
                if (l.ForeColor == l.BackColor) return;
            }
            countdownTimer.Stop();
            MessageBox.Show("Võitsid! Palju õnne!");
            ResetGame();
        }
    }
}
