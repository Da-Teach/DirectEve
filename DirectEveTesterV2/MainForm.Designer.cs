namespace DirectEveTesterV2
{
    partial class MainForm
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
            this.ExecuteTest = new System.Windows.Forms.Button();
            this.TestStatesComboBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // ExecuteTest
            // 
            this.ExecuteTest.Location = new System.Drawing.Point(248, 12);
            this.ExecuteTest.Name = "ExecuteTest";
            this.ExecuteTest.Size = new System.Drawing.Size(75, 23);
            this.ExecuteTest.TabIndex = 0;
            this.ExecuteTest.Text = "Execute";
            this.ExecuteTest.UseVisualStyleBackColor = true;
            this.ExecuteTest.Click += new System.EventHandler(this.ExecuteTest_Click);
            // 
            // TestStatesComboBox
            // 
            this.TestStatesComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.TestStatesComboBox.FormattingEnabled = true;
            this.TestStatesComboBox.Location = new System.Drawing.Point(12, 12);
            this.TestStatesComboBox.Name = "TestStatesComboBox";
            this.TestStatesComboBox.Size = new System.Drawing.Size(230, 21);
            this.TestStatesComboBox.TabIndex = 1;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(335, 45);
            this.Controls.Add(this.TestStatesComboBox);
            this.Controls.Add(this.ExecuteTest);
            this.Name = "MainForm";
            this.Text = "DirectEve Tester";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button ExecuteTest;
        private System.Windows.Forms.ComboBox TestStatesComboBox;
    }
}

