﻿namespace DBDtimer
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


        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(12F, 30F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Name = "MainForm";
            Text = "DBD hook counter";
            TopMost = true;

            FormBorderStyle = FormBorderStyle.None;
            BackColor = Color.Magenta;         // Any unused color
            TransparencyKey = Color.Magenta;   // Makes that color fully transparent
            Load += MainForm_Load;
            ResumeLayout(false);
        }

        #endregion
    }
}