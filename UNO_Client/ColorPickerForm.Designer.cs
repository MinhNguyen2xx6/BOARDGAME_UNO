namespace UNO_Client
{
    partial class ColorPickerForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnRed = new Button();
            btnBlue = new Button();
            btnGreen = new Button();
            btnYellow = new Button();
            SuspendLayout();
            // 
            // btnRed
            // 
            btnRed.Location = new Point(34, 30);
            btnRed.Name = "btnRed";
            btnRed.Size = new Size(94, 29);
            btnRed.TabIndex = 0;
            btnRed.Text = "red";
            btnRed.UseVisualStyleBackColor = true;
            btnRed.Click += btnRed_Click;
            // 
            // btnBlue
            // 
            btnBlue.Location = new Point(43, 113);
            btnBlue.Name = "btnBlue";
            btnBlue.Size = new Size(94, 29);
            btnBlue.TabIndex = 1;
            btnBlue.Text = "blue";
            btnBlue.UseVisualStyleBackColor = true;
            btnBlue.Click += btnBlue_Click;
            // 
            // btnGreen
            // 
            btnGreen.Location = new Point(223, 30);
            btnGreen.Name = "btnGreen";
            btnGreen.Size = new Size(94, 29);
            btnGreen.TabIndex = 2;
            btnGreen.Text = "green";
            btnGreen.UseVisualStyleBackColor = true;
            btnGreen.Click += btnGreen_Click;
            // 
            // btnYellow
            // 
            btnYellow.ForeColor = Color.CornflowerBlue;
            btnYellow.Location = new Point(261, 113);
            btnYellow.Name = "btnYellow";
            btnYellow.Size = new Size(94, 29);
            btnYellow.TabIndex = 3;
            btnYellow.Text = "Yellow";
            btnYellow.UseVisualStyleBackColor = true;
            btnYellow.Click += btnYellow_Click;
            // 
            // ColorPickerForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(btnYellow);
            Controls.Add(btnGreen);
            Controls.Add(btnBlue);
            Controls.Add(btnRed);
            Name = "ColorPickerForm";
            Text = "ColorPickerForm";
            ResumeLayout(false);
        }

        #endregion

        private Button btnRed;
        private Button btnBlue;
        private Button btnGreen;
        private Button btnYellow;
    }
}