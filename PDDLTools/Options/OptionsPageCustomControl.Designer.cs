namespace PDDLTools.Options
{
    partial class OptionsPageCustomControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.FastDownwardPathTextbox = new System.Windows.Forms.TextBox();
            this.SelectFastDownwardPathButton = new System.Windows.Forms.Button();
            this.FolderDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.label2 = new System.Windows.Forms.Label();
            this.PythonPrefixTextbox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.FastDownwardTimeoutNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.FastDownwardSearchOptionsTextbox = new System.Windows.Forms.TextBox();
            this.OpenFDResultsOnSuccessCheckbox = new System.Windows.Forms.CheckBox();
            this.OpenSASResultOnSuccess = new System.Windows.Forms.CheckBox();
            this.VALPathButton = new System.Windows.Forms.Button();
            this.VALPathTextbox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.EnableEditorMarginCheckbox = new System.Windows.Forms.CheckBox();
            this.EnableSyntaxHighlightingCheckbox = new System.Windows.Forms.CheckBox();
            this.EnableAutoCompletementofStatementsCheckbox = new System.Windows.Forms.CheckBox();
            this.EnableErrorCheckingCheckbox = new System.Windows.Forms.CheckBox();
            this.EnableBraceMatchingCheckbox = new System.Windows.Forms.CheckBox();
            this.EnableWordHighlightingCheckbox = new System.Windows.Forms.CheckBox();
            this.IntermediateOutputPathTextbox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.OutputPlanPathTextbox = new System.Windows.Forms.TextBox();
            this.OutputPlanPathLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.FastDownwardTimeoutNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(106, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Fast Downward Path";
            // 
            // FastDownwardPathTextbox
            // 
            this.FastDownwardPathTextbox.AccessibleName = "";
            this.FastDownwardPathTextbox.Enabled = false;
            this.FastDownwardPathTextbox.Location = new System.Drawing.Point(140, 7);
            this.FastDownwardPathTextbox.Name = "FastDownwardPathTextbox";
            this.FastDownwardPathTextbox.Size = new System.Drawing.Size(189, 20);
            this.FastDownwardPathTextbox.TabIndex = 1;
            // 
            // SelectFastDownwardPathButton
            // 
            this.SelectFastDownwardPathButton.AccessibleName = "";
            this.SelectFastDownwardPathButton.Location = new System.Drawing.Point(335, 7);
            this.SelectFastDownwardPathButton.Name = "SelectFastDownwardPathButton";
            this.SelectFastDownwardPathButton.Size = new System.Drawing.Size(59, 23);
            this.SelectFastDownwardPathButton.TabIndex = 2;
            this.SelectFastDownwardPathButton.Text = "Select";
            this.SelectFastDownwardPathButton.UseVisualStyleBackColor = true;
            this.SelectFastDownwardPathButton.Click += new System.EventHandler(this.SelectFastDownwardPathButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Python Prefix";
            // 
            // PythonPrefixTextbox
            // 
            this.PythonPrefixTextbox.AccessibleName = "";
            this.PythonPrefixTextbox.Location = new System.Drawing.Point(140, 32);
            this.PythonPrefixTextbox.Name = "PythonPrefixTextbox";
            this.PythonPrefixTextbox.Size = new System.Drawing.Size(189, 20);
            this.PythonPrefixTextbox.TabIndex = 4;
            this.PythonPrefixTextbox.Leave += new System.EventHandler(this.PythonPrefixTextbox_Leave);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 61);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(122, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Fast Downward Timeout";
            // 
            // FastDownwardTimeoutNumericUpDown
            // 
            this.FastDownwardTimeoutNumericUpDown.Location = new System.Drawing.Point(140, 59);
            this.FastDownwardTimeoutNumericUpDown.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.FastDownwardTimeoutNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.FastDownwardTimeoutNumericUpDown.Name = "FastDownwardTimeoutNumericUpDown";
            this.FastDownwardTimeoutNumericUpDown.Size = new System.Drawing.Size(189, 20);
            this.FastDownwardTimeoutNumericUpDown.TabIndex = 6;
            this.FastDownwardTimeoutNumericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.FastDownwardTimeoutNumericUpDown.ValueChanged += new System.EventHandler(this.FastDownwardTimeoutNumericUpDown_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 90);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(80, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Search Options";
            // 
            // FastDownwardSearchOptionsTextbox
            // 
            this.FastDownwardSearchOptionsTextbox.AccessibleName = "";
            this.FastDownwardSearchOptionsTextbox.Location = new System.Drawing.Point(140, 87);
            this.FastDownwardSearchOptionsTextbox.Name = "FastDownwardSearchOptionsTextbox";
            this.FastDownwardSearchOptionsTextbox.Size = new System.Drawing.Size(189, 20);
            this.FastDownwardSearchOptionsTextbox.TabIndex = 8;
            this.FastDownwardSearchOptionsTextbox.Leave += new System.EventHandler(this.FastDownwardSearchOptionsTextbox_Leave);
            // 
            // OpenFDResultsOnSuccessCheckbox
            // 
            this.OpenFDResultsOnSuccessCheckbox.AutoSize = true;
            this.OpenFDResultsOnSuccessCheckbox.Location = new System.Drawing.Point(140, 113);
            this.OpenFDResultsOnSuccessCheckbox.Name = "OpenFDResultsOnSuccessCheckbox";
            this.OpenFDResultsOnSuccessCheckbox.Size = new System.Drawing.Size(161, 17);
            this.OpenFDResultsOnSuccessCheckbox.TabIndex = 9;
            this.OpenFDResultsOnSuccessCheckbox.Text = "Open FD Result on Success";
            this.OpenFDResultsOnSuccessCheckbox.UseVisualStyleBackColor = true;
            this.OpenFDResultsOnSuccessCheckbox.CheckedChanged += new System.EventHandler(this.OpenFDResultsOnSuccessCheckbox_CheckedChanged);
            // 
            // OpenSASResultOnSuccess
            // 
            this.OpenSASResultOnSuccess.AutoSize = true;
            this.OpenSASResultOnSuccess.Location = new System.Drawing.Point(140, 136);
            this.OpenSASResultOnSuccess.Name = "OpenSASResultOnSuccess";
            this.OpenSASResultOnSuccess.Size = new System.Drawing.Size(163, 17);
            this.OpenSASResultOnSuccess.TabIndex = 10;
            this.OpenSASResultOnSuccess.Text = "Open SAS result on Success";
            this.OpenSASResultOnSuccess.UseVisualStyleBackColor = true;
            this.OpenSASResultOnSuccess.CheckedChanged += new System.EventHandler(this.OpenSASResultOnSuccess_CheckedChanged);
            // 
            // VALPathButton
            // 
            this.VALPathButton.AccessibleName = "";
            this.VALPathButton.Location = new System.Drawing.Point(335, 154);
            this.VALPathButton.Name = "VALPathButton";
            this.VALPathButton.Size = new System.Drawing.Size(59, 23);
            this.VALPathButton.TabIndex = 13;
            this.VALPathButton.Text = "Select";
            this.VALPathButton.UseVisualStyleBackColor = true;
            this.VALPathButton.Click += new System.EventHandler(this.VALPathButton_Click);
            // 
            // VALPathTextbox
            // 
            this.VALPathTextbox.AccessibleName = "";
            this.VALPathTextbox.Enabled = false;
            this.VALPathTextbox.Location = new System.Drawing.Point(140, 154);
            this.VALPathTextbox.Name = "VALPathTextbox";
            this.VALPathTextbox.Size = new System.Drawing.Size(189, 20);
            this.VALPathTextbox.TabIndex = 12;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 157);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(52, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "VAL Path";
            // 
            // EnableEditorMarginCheckbox
            // 
            this.EnableEditorMarginCheckbox.AutoSize = true;
            this.EnableEditorMarginCheckbox.Location = new System.Drawing.Point(140, 180);
            this.EnableEditorMarginCheckbox.Name = "EnableEditorMarginCheckbox";
            this.EnableEditorMarginCheckbox.Size = new System.Drawing.Size(122, 17);
            this.EnableEditorMarginCheckbox.TabIndex = 14;
            this.EnableEditorMarginCheckbox.Text = "Enable editor margin";
            this.EnableEditorMarginCheckbox.UseVisualStyleBackColor = true;
            this.EnableEditorMarginCheckbox.CheckedChanged += new System.EventHandler(this.EnableEditorMarginCheckbox_CheckedChanged);
            // 
            // EnableSyntaxHighlightingCheckbox
            // 
            this.EnableSyntaxHighlightingCheckbox.AutoSize = true;
            this.EnableSyntaxHighlightingCheckbox.Location = new System.Drawing.Point(140, 203);
            this.EnableSyntaxHighlightingCheckbox.Name = "EnableSyntaxHighlightingCheckbox";
            this.EnableSyntaxHighlightingCheckbox.Size = new System.Drawing.Size(148, 17);
            this.EnableSyntaxHighlightingCheckbox.TabIndex = 15;
            this.EnableSyntaxHighlightingCheckbox.Text = "Enable syntax highlighting";
            this.EnableSyntaxHighlightingCheckbox.UseVisualStyleBackColor = true;
            this.EnableSyntaxHighlightingCheckbox.CheckedChanged += new System.EventHandler(this.EnableSyntaxHighlightingCheckbox_CheckedChanged);
            // 
            // EnableAutoCompletementofStatementsCheckbox
            // 
            this.EnableAutoCompletementofStatementsCheckbox.AutoSize = true;
            this.EnableAutoCompletementofStatementsCheckbox.Location = new System.Drawing.Point(140, 226);
            this.EnableAutoCompletementofStatementsCheckbox.Name = "EnableAutoCompletementofStatementsCheckbox";
            this.EnableAutoCompletementofStatementsCheckbox.Size = new System.Drawing.Size(218, 17);
            this.EnableAutoCompletementofStatementsCheckbox.TabIndex = 16;
            this.EnableAutoCompletementofStatementsCheckbox.Text = "Enable auto completement of statements";
            this.EnableAutoCompletementofStatementsCheckbox.UseVisualStyleBackColor = true;
            this.EnableAutoCompletementofStatementsCheckbox.CheckedChanged += new System.EventHandler(this.EnableAutoCompletementofStatementsCheckbox_CheckedChanged);
            // 
            // EnableErrorCheckingCheckbox
            // 
            this.EnableErrorCheckingCheckbox.AutoSize = true;
            this.EnableErrorCheckingCheckbox.Location = new System.Drawing.Point(140, 249);
            this.EnableErrorCheckingCheckbox.Name = "EnableErrorCheckingCheckbox";
            this.EnableErrorCheckingCheckbox.Size = new System.Drawing.Size(236, 17);
            this.EnableErrorCheckingCheckbox.TabIndex = 17;
            this.EnableErrorCheckingCheckbox.Text = "Enable error checking on saving a PDDL file";
            this.EnableErrorCheckingCheckbox.UseVisualStyleBackColor = true;
            this.EnableErrorCheckingCheckbox.CheckedChanged += new System.EventHandler(this.EnableErrorCheckingCheckbox_CheckedChanged);
            // 
            // EnableBraceMatchingCheckbox
            // 
            this.EnableBraceMatchingCheckbox.AutoSize = true;
            this.EnableBraceMatchingCheckbox.Location = new System.Drawing.Point(140, 295);
            this.EnableBraceMatchingCheckbox.Name = "EnableBraceMatchingCheckbox";
            this.EnableBraceMatchingCheckbox.Size = new System.Drawing.Size(137, 17);
            this.EnableBraceMatchingCheckbox.TabIndex = 19;
            this.EnableBraceMatchingCheckbox.Text = "Enable Brace Matching";
            this.EnableBraceMatchingCheckbox.UseVisualStyleBackColor = true;
            this.EnableBraceMatchingCheckbox.CheckedChanged += new System.EventHandler(this.EnableBraceMatchingCheckbox_CheckedChanged);
            // 
            // EnableWordHighlightingCheckbox
            // 
            this.EnableWordHighlightingCheckbox.AutoSize = true;
            this.EnableWordHighlightingCheckbox.Location = new System.Drawing.Point(140, 272);
            this.EnableWordHighlightingCheckbox.Name = "EnableWordHighlightingCheckbox";
            this.EnableWordHighlightingCheckbox.Size = new System.Drawing.Size(163, 17);
            this.EnableWordHighlightingCheckbox.TabIndex = 18;
            this.EnableWordHighlightingCheckbox.Text = "Enable Highlighting of Words";
            this.EnableWordHighlightingCheckbox.UseVisualStyleBackColor = true;
            this.EnableWordHighlightingCheckbox.CheckedChanged += new System.EventHandler(this.EnableWordHighlightingCheckbox_CheckedChanged);
            // 
            // IntermediateOutputPathTextbox
            // 
            this.IntermediateOutputPathTextbox.AccessibleName = "";
            this.IntermediateOutputPathTextbox.Location = new System.Drawing.Point(140, 318);
            this.IntermediateOutputPathTextbox.Name = "IntermediateOutputPathTextbox";
            this.IntermediateOutputPathTextbox.Size = new System.Drawing.Size(189, 20);
            this.IntermediateOutputPathTextbox.TabIndex = 21;
            this.IntermediateOutputPathTextbox.Leave += new System.EventHandler(this.IntermediateOutputPathTextbox_Leave);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 321);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(125, 13);
            this.label6.TabIndex = 20;
            this.label6.Text = "Intermediate Output Path";
            // 
            // OutputPlanPathTextbox
            // 
            this.OutputPlanPathTextbox.AccessibleName = "";
            this.OutputPlanPathTextbox.Location = new System.Drawing.Point(140, 344);
            this.OutputPlanPathTextbox.Name = "OutputPlanPathTextbox";
            this.OutputPlanPathTextbox.Size = new System.Drawing.Size(189, 20);
            this.OutputPlanPathTextbox.TabIndex = 23;
            this.OutputPlanPathTextbox.Leave += new System.EventHandler(this.OutputPlanPathTextbox_Leave);
            // 
            // OutputPlanPathLabel
            // 
            this.OutputPlanPathLabel.AutoSize = true;
            this.OutputPlanPathLabel.Location = new System.Drawing.Point(12, 347);
            this.OutputPlanPathLabel.Name = "OutputPlanPathLabel";
            this.OutputPlanPathLabel.Size = new System.Drawing.Size(88, 13);
            this.OutputPlanPathLabel.TabIndex = 22;
            this.OutputPlanPathLabel.Text = "Output Plan Path";
            // 
            // OptionsPageCustomControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.OutputPlanPathTextbox);
            this.Controls.Add(this.OutputPlanPathLabel);
            this.Controls.Add(this.IntermediateOutputPathTextbox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.EnableBraceMatchingCheckbox);
            this.Controls.Add(this.EnableWordHighlightingCheckbox);
            this.Controls.Add(this.EnableErrorCheckingCheckbox);
            this.Controls.Add(this.EnableAutoCompletementofStatementsCheckbox);
            this.Controls.Add(this.EnableSyntaxHighlightingCheckbox);
            this.Controls.Add(this.EnableEditorMarginCheckbox);
            this.Controls.Add(this.VALPathButton);
            this.Controls.Add(this.VALPathTextbox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.OpenSASResultOnSuccess);
            this.Controls.Add(this.OpenFDResultsOnSuccessCheckbox);
            this.Controls.Add(this.FastDownwardSearchOptionsTextbox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.FastDownwardTimeoutNumericUpDown);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.PythonPrefixTextbox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.SelectFastDownwardPathButton);
            this.Controls.Add(this.FastDownwardPathTextbox);
            this.Controls.Add(this.label1);
            this.Name = "OptionsPageCustomControl";
            this.Size = new System.Drawing.Size(397, 367);
            this.Load += new System.EventHandler(this.OptionsPageCustomControl_Load);
            ((System.ComponentModel.ISupportInitialize)(this.FastDownwardTimeoutNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox FastDownwardPathTextbox;
        private System.Windows.Forms.Button SelectFastDownwardPathButton;
        private System.Windows.Forms.FolderBrowserDialog FolderDialog;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox PythonPrefixTextbox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown FastDownwardTimeoutNumericUpDown;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox FastDownwardSearchOptionsTextbox;
        private System.Windows.Forms.CheckBox OpenFDResultsOnSuccessCheckbox;
        private System.Windows.Forms.CheckBox OpenSASResultOnSuccess;
        private System.Windows.Forms.Button VALPathButton;
        private System.Windows.Forms.TextBox VALPathTextbox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox EnableEditorMarginCheckbox;
        private System.Windows.Forms.CheckBox EnableSyntaxHighlightingCheckbox;
        private System.Windows.Forms.CheckBox EnableAutoCompletementofStatementsCheckbox;
        private System.Windows.Forms.CheckBox EnableErrorCheckingCheckbox;
        private System.Windows.Forms.CheckBox EnableBraceMatchingCheckbox;
        private System.Windows.Forms.CheckBox EnableWordHighlightingCheckbox;
        private System.Windows.Forms.TextBox IntermediateOutputPathTextbox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox OutputPlanPathTextbox;
        private System.Windows.Forms.Label OutputPlanPathLabel;
    }
}
